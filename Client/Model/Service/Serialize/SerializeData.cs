using Client.Model.Entity;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Client.Model.Service.Serialize
{
    public class SerializeData
    {
        //private static readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public static void SerializeToJson<T>(List<T> obj, string nameFile = null)
        {
            if(obj is null)
                throw new Exception("Data cannot be null");
            try
            {
                File.WriteAllText(" " + (nameFile ?? $"NewFile{obj.GetHashCode()}") + ".json", JsonConvert.SerializeObject(obj, Formatting.Indented));
            } 
            catch(Exception ex) 
            {
                MessageBox.Show($"Save file occurred failed.\nDetails: {ex.Message}", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
            
        public static void SerializeToXml<T>(List<T> obj, string nameFile = null)
        {
            if (obj is null)
                throw new Exception("Data cannot be null");

            try
            {
                XmlSerializer serializer = new(typeof(List<T>));
                using TextWriter writer = new StreamWriter(" " + (nameFile ?? $"NewFile{obj.GetHashCode()}") + ".xml");
                serializer.Serialize(writer, obj);
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"Save file occurred failed.\nDetails: {ex.Message}", "Save error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            
        }
        public static void SerializeToXlsx(List<Hotel> obj, string nameFile=null)
        {
            if (obj is null)
                throw new Exception("Data cannot be null");
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelPackage excelPackage = new();
                var worksheet = excelPackage.Workbook.Worksheets.Add(nameFile ?? $"NewFile{obj.GetHashCode()}");

                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Phone";
                worksheet.Cells[1, 4].Value = "Address";
                worksheet.Cells[1, 5].Value = "Rating";
                worksheet.Cells[1, 6].Value = "Image";

                ushort row = 2;
                foreach (var hotel in obj)
                {
                    worksheet.Cells[row, 1].Value = hotel.Id;
                    worksheet.Cells[row, 2].Value = hotel.HotelName;
                    worksheet.Cells[row, 3].Value = hotel.Phone;
                    worksheet.Cells[row, 4].Value = hotel.Address;
                    worksheet.Cells[row, 5].Value = hotel.Rating;
                    worksheet.Cells[row, 6].Value = hotel.Picture;
                    row++;

                    foreach (var room in hotel.Rooms)
                    {
                        worksheet.Cells[row, 2].Value = $"Room {room.Number}";
                        worksheet.Cells[row, 3].Value = room.Price;
                        worksheet.Cells[row, 4].Value = room.RoomType;
                        row++;
                    }
                }

                FileInfo excelFile = new((nameFile ?? $"NewFile{obj.GetHashCode()}") + @".xlsx");
                excelPackage.SaveAs(excelFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Save file occurred failed.\nDetails: {ex.Message}", "Save error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
