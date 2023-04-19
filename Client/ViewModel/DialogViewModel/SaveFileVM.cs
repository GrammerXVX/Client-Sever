using Client.Model.Entity;
using Client.Model.Service.Serialize;
using Client.View.Dialog;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModel.DialogViewModel
{
    public class SaveFileVM : INotifyPropertyChanged
    {
        private FileModel _file;
        private List<Hotel> _data;
        public List<Hotel> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged(nameof(File));
            }
        }
        public FileModel File
        {
            get { return _file; }
            set
            {
                _file = value;
                OnPropertyChanged(nameof(File));
            }
        }
        public List<string> FileTypes { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public SaveFileVM()
        {
            File = new FileModel();
            FileTypes = new List<string>() { "JSON", "XML", "XLSX" };
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Save()
        {
            switch (File.FileType)
            {
                case "JSON":
                    {
                        SerializeData.SerializeToJson(Data, File.FileName);
                        MessageBox.Show($"File {File.FileName ?? Data.GetHashCode().ToString()}.json saved in {Environment.CurrentDirectory}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "XML":
                    {
                        SerializeData.SerializeToXml(Data, File.FileName);
                        MessageBox.Show($"File {File.FileName ?? Data.GetHashCode().ToString()}.xml saved in {Environment.CurrentDirectory}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "XLSX":
                    {

                        SerializeData.SerializeToXlsx(Data, File.FileName);
                        MessageBox.Show($"File {File.FileName ?? Data.GetHashCode().ToString()}.xlsx saved in {Environment.CurrentDirectory}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                default:
                    {
                        SerializeData.SerializeToJson(Data, File.FileName);
                        MessageBox.Show($"File {File.FileName ?? Data.GetHashCode().ToString()}.json saved in default format in {Environment.CurrentDirectory}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
            }
            Cancel();
        }

        public void Cancel() =>
            Application.Current.Windows.OfType<SaveFile>().SingleOrDefault(x => x.IsActive)?.Close();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
