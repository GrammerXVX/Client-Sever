using Client.Model.Entity;
using Client.Model.Service.ApiRepository;
using Client.View.Dialog;
using Client.View.UpdateWindow;
using Client.ViewModel.DialogViewModel;
using DocumentFormat.OpenXml.Wordprocessing;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModel.GeneralWindow
{
    class MainWindowVM : ViewModelBase
    {
        private bool isError;
        private int pageSize = 3;
        private bool LastPage;

        private WebApiRepository apiRepository;
        public ICommand InsertCommand { get; }
        public ICommand AddRoomsCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveFileCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand CloseAppCommand { get; }
        public ICommand ReConnectAppCommand { get; }
        private bool _isLoading=true;
        private int _pageNumber=1;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        public int PageNumber
        {
            get => _pageNumber <= 0 ? 1: _pageNumber;
            set
            {
                SetProperty(ref _pageNumber, value);
                LoadData();
            }
        }
        private Hotel? _selectedItem;
        public Hotel? SelectedItem
        {
            get { return _selectedItem; }
            set 
            {
                SetProperty(ref _selectedItem, value ?? (Hotel.Count!=0?Hotel.Last():null)); 
                SetProperty(ref _rooms, (isError ? null : (_selectedItem!=null?new ObservableCollection<Room>(apiRepository.GetDataAsync(_selectedItem.HotelName).Result.ToList()):null)), nameof(Rooms));
            }
        }

        private ObservableCollection<Room>? _rooms;
        public ObservableCollection<Room> Rooms
        {
            get { return _rooms; }
            set { SetProperty(ref _rooms, value); }
        }

        private ObservableCollection<Hotel>? _hotel;
        public ObservableCollection<Hotel>? Hotel
        {
            get => _hotel;
            set => SetProperty(ref _hotel, (isError ? null : value), "Hotel");
        }

        public MainWindowVM()
        {
            apiRepository = new WebApiRepository(new HttpClient());
            try
            {
                
                Hotel = new ObservableCollection<Hotel>(collection: apiRepository.GetDataAsync(pageSize,1).Result.ToList());
                
                GetRooms();
            } 
            catch (Exception ex) 
            {
                MessageBox.Show($"Occured error connection.\nDetails: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                _hotel = null;
                _rooms = null;
                isError = true;
            }
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            InsertCommand = new RelayCommand<object>(InsertItem);
            UpdateCommand = new RelayCommand<object>(UpdateItem);
            AddRoomsCommand = new RelayCommand<object>(AddRooms);
            DeleteCommand = new RelayCommand<ObservableCollection<object>>(DeleteItem);
            SaveFileCommand = new RelayCommand(OpenSaveFileDialog);
            CloseAppCommand = new RelayCommand(CloseWindow);
            ReConnectAppCommand = new RelayCommand(TryConnect);
    }
        private void NextPage()
        {
            if(IsLoading)
            PageNumber++;
        }

        private void PreviousPage()
        {
            if (PageNumber-1>0)
            PageNumber--;
        }
        private async void LoadData()
        {
            if (isError)
                return;

            var response = await apiRepository.GetDataAsync(pageSize,PageNumber);

            if (response.Count!=0)
            {
                var data = response.ToList();
                Hotel = new ObservableCollection<Hotel>(data);
                IsLoading = true;
            }
            else
            {
                IsLoading = false;
            }
        }
        private void OpenSaveFileDialog()
        {
            if (Hotel != null)
            {
                SaveFile saveFile = new();
                ((SaveFileVM)saveFile.DataContext).Data = apiRepository.GetDataAsync().Result.ToList();
                saveFile.Show();
            }
            else
                MessageBox.Show("No selected data to export", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private async void GetRooms()
        {
            if(Hotel!=null)
            foreach (var x in Hotel)
            {
                x.Rooms = new List<Room>(await apiRepository.GetDataAsync(x.HotelName));
            }
        }
        private async void TryConnect()
        {
            try
            {
                apiRepository = new WebApiRepository(new HttpClient());
                Hotel = new ObservableCollection<Hotel>(collection: apiRepository.GetDataAsync(pageSize, 1).Result.ToList());
                GetRooms();
                isError= false;
                MessageBox.Show($"Connection successful.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Occured error connection.\nDetails: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                _hotel = null;
                _rooms = null;
                isError = true;
            }
        }
        private void InsertItem(object item)
        {
            if (isError)
                return;
            View.AddWindow.AddHotel newHotel = new()
            {
                DataContext = new AddHotelVM(apiRepository)
            };
            newHotel.Show();
        }
        private async void AddRooms(object item)
        {
            if (isError)
                return;
            GetRooms();
            if (SelectedItem != null)
            {
                View.AddWindow.AddRoomsForHotel saveFile = new()
                {
                    DataContext = new AddRoomVM(apiRepository)
                };
                saveFile.Show();
            }
        }
        private void UpdateItem(object item)
        {
            if (isError)
                return;
            if (SelectedItem != null)
            {
                UpdateHotel newHotel = new()
                {
                    DataContext = new UpdateVM.UpdateHotelVM(apiRepository, SelectedItem)
                };
                newHotel.Show();
            }
        }
        private async void DeleteItem(ObservableCollection<object> items)
        {
            if (isError)
                return;
            if (items != null && items.Count > 0)
            {
                if (MessageBox.Show($"Are you sure you want to delete {items.Count} objects?", "Delete objects?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        foreach (var hotel in items.ToList())
                        {
                            var existingHotel = apiRepository.GetDataAsync((hotel as Hotel).Id).Result;
                            if (existingHotel != null)
                            {
                                await apiRepository.DeleteDataAsync(existingHotel.Id);
                                Hotel?.Remove(hotel as Hotel);
                            }
                        }

                        if (Hotel != null)
                        {
                            Hotel = new ObservableCollection<Hotel>(apiRepository.GetDataAsync(pageSize,PageNumber).Result.ToList());
                        }

                        MessageBox.Show($"Selected objects were deleted", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Occured error delete objects.\nDetails: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
                MessageBox.Show("No hotel selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        private static void CloseWindow()
            => Application.Current.Windows.OfType<MainWindow>().SingleOrDefault(x => x.IsActive)?.Close();
    }
}

