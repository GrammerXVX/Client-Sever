using Client.Model.Service.ApiRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Client.Model.Entity;
using DocumentFormat.OpenXml.Vml;
using System.IO;
using Microsoft.Win32;
using Client.View.Dialog;
using Client.View.AddWindow;
using System.Windows.Navigation;
using System.Text.RegularExpressions;

namespace Client.ViewModel.GeneralWindow
{
    public partial class AddHotelVM : ViewModelBase
    {
        private readonly MainWindow mainWindow;
        private readonly WebApiRepository apiRepository;
        private readonly Regex reg;
        private string _hotelName;
        private byte[] _picture;
        public string? HotelName { get { return _hotelName; } set 
            {
                if(apiRepository.GetDataAsync().Result.FirstOrDefault(x=>x.HotelName== value, null) == null)
                {
                    _hotelName = value;
                }
                else
                {
                    MessageBox.Show($"A hotel with name '{value}' already exists.", "Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
                    _hotelName = null;
                }
            } 
        }
        private string _phone;
        public string? Phone { get { return _phone; } set 
            {
                if (reg.IsMatch(value.Trim(' ')))
                    _phone = value.Trim(' ');
                else
                    MessageBox.Show("Phone number incorrect. \nExample:\n1)+375291234567\n2)375291234567", "Info", MessageBoxButton.OK, MessageBoxImage.Question);
            } 
        }
        public string? Address { get; set; }
        public double? Rating { get; set; }
        public byte[]? Picture { get { return _picture; } set { SetProperty(ref _picture, value, "Picture"); } }

        public ICommand AddCommand { get; }
        public RelayCommand SelectImageCommand { get; private set; }

        public AddHotelVM()=>
            AddCommand = new RelayCommand<object>(AddHotel);
        public AddHotelVM(WebApiRepository webApiRepository)
        {
            reg = checkPhoneNumber();
            mainWindow = Application.Current.MainWindow as MainWindow;
            apiRepository = webApiRepository;
            AddCommand = new RelayCommand<object>(AddHotel);
            SelectImageCommand = new RelayCommand(SelectImage);
        }

        private async void AddHotel(object obj)
        {
            if (_hotelName == null||_hotelName.All(x=>x.Equals(' ')))
            {
                MessageBox.Show("Hotel name was incorrect", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Hotel newHotel = new()
            {
                HotelName = HotelName,
                Phone = Phone,
                Address = Address,
                Rating = Rating,
                Picture = Picture
            };

            try
            {
                if (newHotel.Picture == null && MessageBox.Show("Image not selected. Are you sure you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
                await apiRepository.PostDataAsync(newHotel);
                newHotel.Id = apiRepository.GetDataAsync().Result.First(x => x.HotelName == newHotel.HotelName).Id;
                ((Application.Current.MainWindow as MainWindow).DataContext as MainWindowVM).Hotel.Add(newHotel);
                ((Application.Current.MainWindow as MainWindow).DataContext as MainWindowVM).SelectedItem=newHotel;
                MessageBox.Show("Hotel added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add hotel\nDetails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            => Application.Current.Windows.OfType<AddHotel>().SingleOrDefault(x => x.IsActive)?.Close();

        [GeneratedRegex("^(\\+375|375)?(25|29|33|44)(\\d{3})(\\d{2})(\\d{2})$", RegexOptions.Compiled)]
        private static partial Regex checkPhoneNumber();
    }
}
