using Client.Model.Entity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Client.Model.Service.ApiRepository;
using System;
using Client.View.AddWindow;

namespace Client.ViewModel.GeneralWindow
{
    public class AddRoomVM : ViewModelBase
    {
        private List<string> _roomTypes = new List<string> { "Single", "Double", "Triple", "Quad", "Queen", "King", "Suite", "Executive Suite", "Penthouse Suite", "Family Suite" };
        private string _selectedRoomType;
        private int _count;
        private int _number;
        private double _price;
        private ObservableCollection<Room> _tempRooms = new ObservableCollection<Room>();
        private readonly WebApiRepository apiRepository;
        private readonly MainWindow mainWindow;
        public List<string> RoomTypes
        {
            get { return _roomTypes; }
        }
        public string SelectedRoomType
        {
            get { return _selectedRoomType; }
            set
            {
                SetProperty(ref _selectedRoomType, value);
            }
        }
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }
        public int Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }
        public double Price
        {
            get { return _price; }
            set
            {
                SetProperty(ref _price, value);
            }
        }

        public ObservableCollection<Room> TempRooms
        {
            get { return _tempRooms; }
            set
            {
                SetProperty(ref _tempRooms, value);
            }
        }
        public ICommand CloseCommand { get; set; }
        public ICommand AddRoomCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public AddRoomVM( WebApiRepository webApiRepository)
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            AddRoomCommand = new RelayCommand<object>(AddRoom);
            SaveCommand = new RelayCommand<object>(Save);
            CancelCommand = new RelayCommand<object>(Clear);
            CloseCommand = new RelayCommand(CloseWindow);
            apiRepository = webApiRepository;
            TempRooms = new ObservableCollection<Room>((mainWindow?.DataContext as MainWindowVM)?.SelectedItem.Rooms);
        }
        public AddRoomVM()
        {
            AddRoomCommand = new RelayCommand<object>(AddRoom);
            SaveCommand = new RelayCommand<object>(Save);
            CancelCommand = new RelayCommand<object>(Clear);
            //apiRepository = webApiRepository;
        }

        private void AddRoom(object obj)
        {
            if (Price <= 0||Price>100001)
            {
                MessageBox.Show("Invalid price.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_tempRooms.Count- (mainWindow?.DataContext as MainWindowVM)?.SelectedItem.Rooms.Count >= 5)
            {
                MessageBox.Show("The limit of 5 rooms has been reached.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                
            int nextNumber = mainWindow?.DataContext is MainWindowVM vm && vm.SelectedItem != null && vm.SelectedItem.Rooms.LastOrDefault()?.Number != null
            ? Convert.ToInt32(vm.SelectedItem.Rooms.Last().Number)
            : 100;

            _tempRooms.Add(new Room
            {
                Id = default,
                RoomType = _selectedRoomType,
                Price = _price,
                Number = (nextNumber + _tempRooms.Count + 1).ToString()
            });
        }

        private void Save(object obj)
        {
            foreach (var room in new ObservableCollection<Room>(_tempRooms.ToList()))
            {
                ((mainWindow?.DataContext as MainWindowVM)?.SelectedItem).Rooms = apiRepository.GetDataAsync(((mainWindow?.DataContext as MainWindowVM)?.SelectedItem as Hotel).HotelName).Result;
                if(((mainWindow?.DataContext as MainWindowVM)?.SelectedItem).Rooms.FirstOrDefault(x => x.Number == room.Number&&x.RoomType==room.RoomType, null) != null)
                    room.Id = ((mainWindow?.DataContext as MainWindowVM)?.SelectedItem).Rooms.FirstOrDefault(x => x.Number == room.Number && x.RoomType == room.RoomType).Id;
                else
                {
                    ((mainWindow?.DataContext as MainWindowVM)?.SelectedItem).Rooms.Add(room);
                    apiRepository.PutDataAsync(((mainWindow?.DataContext as MainWindowVM)?.SelectedItem));
                }
            }
            _tempRooms.Clear();
            CloseWindow();
        }

        private void Clear(object obj) =>
            _tempRooms.Clear();
        private static void CloseWindow()
            => Application.Current.Windows.OfType<View.AddWindow.AddRoomsForHotel>().SingleOrDefault(x => x.IsActive)?.Close();
    }

}

