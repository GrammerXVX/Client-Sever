using Client.Model.Service.ApiRepository;
using Client.View.AddWindow;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Client.Model.Entity;

namespace Client.ViewModel.GeneralWindow.UpdateVM
{
    public class UpdateHotelVM : ViewModelBase
    {
        private readonly MainWindow mainWindow;
        private readonly WebApiRepository apiRepository;
        private int Id;
        private string _hotelName;
        private byte[] _picture;
        public string? HotelName
        {
            get { return _hotelName; }
            set
            {
                if (apiRepository.GetDataAsync().Result.FirstOrDefault(x => x.HotelName == value.TrimEnd(' ').TrimStart(' '), null) == null)
                {
                    _hotelName = value;
                }
                else
                {
                    MessageBox.Show($"A hotel with name '{value.TrimEnd(' ').TrimStart(' ')}' already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public double? Rating { get; set; }
        public byte[]? Picture { get { return _picture; } set { SetProperty(ref _picture, value, nameof(Picture)); } }
        private readonly Hotel currentHotel;
        public ICommand UpdateCommand { get; }
        public RelayCommand SelectImageCommand { get; private set; }

        public UpdateHotelVM() =>
            UpdateCommand = new RelayCommand<object>(UpdateHotel);
        public UpdateHotelVM(WebApiRepository webApiRepository,Hotel currentHotel)
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            apiRepository = webApiRepository;
            UpdateCommand = new RelayCommand<object>(UpdateHotel);
            SelectImageCommand = new RelayCommand(SelectImage);
            this.currentHotel = currentHotel;
            Id = currentHotel.Id;
            _hotelName = currentHotel.HotelName;
            Phone = currentHotel.Phone;
            Address = currentHotel.Address;
            Rating = currentHotel.Rating;
            Picture = currentHotel.Picture;
        }

        private async void UpdateHotel(object obj)
        {
            if (_hotelName == null || _hotelName.All(x => x.Equals(' ')))
            {
                MessageBox.Show("Hotel name was incorrect", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Hotel newHotel = new()
            {
                Id = Id,
                HotelName = HotelName,
                Phone = Phone,
                Address = Address,
                Rating = Rating,
                Picture = Picture
            };
            try
            {
                if (newHotel.Picture == null)
                {
                    if (MessageBox.Show("Image not selected. Are you sure you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        await apiRepository.PutDataAsync(newHotel);
                        MessageBox.Show("Hotel data update successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        ((Application.Current.MainWindow as MainWindow).DataContext as MainWindowVM).Hotel.Remove(currentHotel);
                        ((Application.Current.MainWindow as MainWindow).DataContext as MainWindowVM).Hotel.Add(newHotel);
                        CloseWindow();
                    }
                }
                else
                {
                    await apiRepository.PutDataAsync(newHotel);
                    ((Application.Current.MainWindow as MainWindow).DataContext as MainWindowVM).Hotel.Remove(currentHotel);
                    ((Application.Current.MainWindow as MainWindow).DataContext as MainWindowVM).Hotel.Add(newHotel);
                    MessageBox.Show("Hotel data update successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    CloseWindow();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update hotel\nDetails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void SelectImage()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = openFileDialog.FileName;

                byte[] imageData;
                using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }
                Picture = imageData;
            }
        }
        private static void CloseWindow()
            => Application.Current.Windows.OfType<View.UpdateWindow.UpdateHotel>().SingleOrDefault(x => x.IsActive)?.Close();
    }
}
