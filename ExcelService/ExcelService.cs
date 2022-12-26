﻿using NetBarcode;
using OfficeOpenXml;
using ProductLibrary.Model;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace ExcelService
{
  /// <summary>
  /// Сервисный класс для работы с Excel.
  /// </summary>
  public class ExcelService
  {
    #region Методы.
    /// <summary>
    /// Метод для добавления продуктов из файла Excel в коллекцию продуктов.
    /// </summary>
    /// <param name="directory"> Путь до директории с рабочими файлами Excel. </param>
    /// <param name="customFileName"> Название файла со списком продуктов. </param>
    /// <param name="customSheetName"> Название листа со списком продуктов. </param>
    /// <returns> Возвращает IEnumerable-коллекцию продуктов.</returns>
    public IEnumerable<Product> LoadFromFile(string directory, 
                                             string customFileName = ExcelServiceConstants.PriceListFileName, 
                                             string customSheetName= ExcelServiceConstants.PriceListSheetName)
    {
      List<Product> items = new();
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;            
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, customFileName)))
      {
        ExcelWorksheet priceListSheet = excelPackage.Workbook.Worksheets[customSheetName];
        var ColumnsInfo = Enumerable.Range(1, priceListSheet.Dimension.Columns).ToList().Select(n =>
               new { Index = n, Name = priceListSheet.Cells[1, n].Value.ToString() });
        for (int row = 2; row <= priceListSheet.Dimension.Rows; row++)
        {
          string? id = string.Empty;
          string? name = string.Empty; 
          int manufacturerId = 0;
          string? price = string.Empty;
          foreach (var column in ColumnsInfo)
            switch (column.Name)
            {
              case ExcelServiceConstants.Id:
                id = priceListSheet.Cells[row, column.Index].Value.ToString();
                break;
              case ExcelServiceConstants.Name:
                name = priceListSheet.Cells[row, column.Index].Value.ToString();
                break;
              case ExcelServiceConstants.Manufacturer:
                manufacturerId = int.Parse(priceListSheet.Cells[row, column.Index].Value.ToString());
                break;
              case ExcelServiceConstants.Price:
                price = priceListSheet.Cells[row, column.Index].Value.ToString();
                break;
            }       
          if ((id != null) && (name != null) && (manufacturerId != null) && (price != null))
            items.Add(new Product( name, Guid.Parse("DEA29528-56F1-4A7F-8B56-7B8181F17631"), price));
        }        
      }
      return items;
    }
    /// <summary>
    /// Метод для сохранения коллекции продуктов в файл Excel.
    /// </summary>
    /// <param name="items"> Передаваемая коллекция продуктов. </param>
    /// <param name="directory"> Путь до директории с рабочими файлами Excel. </param>
    /// <param name="customFileName"> Название файла со списком продуктов. </param>
    /// <param name="customSheetName"> Название листа со списком продуктов. </param>
    /// <returns> Возвращает async Task. </returns>
    public async Task SaveToFileAsync (IEnumerable<Product> items, 
                                       string directory,
                                       string customFileName= ExcelServiceConstants.PriceListFileName,
                                       string customSheetName= ExcelServiceConstants.PriceListSheetName)
    {    
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      CreateFileAndSheetsIfNotExists(directory, customFileName, customSheetName);
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, customFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[customSheetName];
        workSheet.Cells.Clear();
        workSheet.Cells["A1"].LoadFromCollection(items, true);
        var table = workSheet.Cells[1, 1, workSheet.Dimension.Rows, workSheet.Dimension.Columns];
        foreach (var cell in table) cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);                       
        table.AutoFitColumns();
        workSheet.Cells[1, 1, 1, workSheet.Dimension.Columns].Style.Font.Bold = true;
        workSheet.Cells[1, 1, 1, workSheet.Dimension.Columns].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;   
        await excelPackage.SaveAsync();        
      }      
    }
    /// <summary>
    /// Метод для формирования и сохранения в файл Excel ценников всех продуктов из коллекции.
    /// </summary>
    /// <param name="list"> Передаваемая коллекция продуктов. </param>
    /// <param name="directory"> Путь до директории с рабочими файлами Excel. </param>
    /// <returns> Возвращает async Task. </returns>
    public async Task PrintAllPriceTagsAsync(IEnumerable<Product> list, string directory)
    {   
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      CreateFileAndSheetsIfNotExists(directory);
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, ExcelServiceConstants.PriceTagsFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.PriceTagsSheetName];
        if (workSheet != null)
        {
          workSheet.Cells.Clear();
          workSheet.Drawings.Clear();
          int rowIndex = 1;
          foreach (Product product in list)
          {
            workSheet.Cells[rowIndex, 1].Value = ExcelServiceConstants.IdLocalized;
            workSheet.Row(rowIndex).Height = 75.00D;
            var barcode = new Barcode(product.Id.ToString(), NetBarcode.Type.Code128, true);
            var barcodeBuffer = barcode.GetByteArray();
            using (var barcodeStream = new MemoryStream(barcodeBuffer))
            {
              barcodeStream.Position = 0;
              var barcodePicture = workSheet.Drawings.AddPicture(product.Id.ToString(), barcodeStream, OfficeOpenXml.Drawing.ePictureType.Jpg);
              barcodePicture.SetSize(250, 100);
              barcodePicture.SetPosition(rowIndex-1, 0, 1, 0);
              rowIndex++;
            }
            workSheet.Cells[rowIndex, 1].Value = ExcelServiceConstants.NameLocalized;
            workSheet.Cells[rowIndex++, 2].Value = product.Name;
            workSheet.Cells[rowIndex, 1].Value = ExcelServiceConstants.ManufacturerLocalized;
            workSheet.Cells[rowIndex++, 2].Value = product.ManufacturerId;
            workSheet.Cells[rowIndex, 1].Value = ExcelServiceConstants.PriceLocalized;
            workSheet.Cells[rowIndex, 2].Value = product.Price;
            workSheet.Cells[rowIndex, 2].Style.Font.Bold = true;
            workSheet.Cells[rowIndex, 2].Style.Font.Size = 24;
            workSheet.Cells[rowIndex++, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            rowIndex++;
          }
          workSheet.Column(1).AutoFit();
          workSheet.Column(2).Width = 37.00D;
        }
        await excelPackage.SaveAsync();
      }      
    }
    /// <summary>
    /// Метод, генерирующий отчет об изменении цен из сравнения двух файлов прайс-листов (старый и новый).
    /// </summary>
    /// <param name="directory"> Путь до дректории с рабочими файлами Excel. </param>
    /// <returns> Возвращает async Task. </returns>
    /// <exception cref="FileNotFoundException"> Если нет нового файла прайс-листа, то вырасывается исключение.  </exception>
    public async Task GenerateReport(string directory)
    {
      List<Product> products = new();
      List<Product> newProducts = new();
      List<Product> comparisonResult = new();
      var ReportFilePath = Path.Combine(directory, ExcelServiceConstants.ReportFileName);
      var NewPriceListFilePath = Path.Combine(directory, ExcelServiceConstants.NewPriceListFileName);
      products = this.LoadFromFile(directory).ToList();
      if (File.Exists(NewPriceListFilePath))
        newProducts = this.LoadFromFile(directory, ExcelServiceConstants.NewPriceListFileName).ToList();
      else throw new FileNotFoundException();
      foreach (Product product in products)
        foreach (Product newProduct in newProducts)
          if ((product.Name == newProduct.Name) && (product.ManufacturerId == newProduct.ManufacturerId) && (product.Price != newProduct.Price))
            comparisonResult.Add(newProduct);
          else continue;
      await this.SaveToFileAsync(comparisonResult, directory, ExcelServiceConstants.ReportFileName, ExcelServiceConstants.ReportSheetName);    
    }


    /// <summary>
    ////Метод для создания раобчих файлов и листов Excel.
    /// </summary>
    /// <param name="directory"> Путь до директории с рабочими файлами Excel. </param>
    /// <param name="customFileName"> Название файла со списком продуктов. </param>
    /// <param name="customSheetName"> Название листа со списком продуктов. </param>
    private void CreateFileAndSheetsIfNotExists(string directory, string customFileName="", string customSheetName="")
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, ExcelServiceConstants.PriceListFileName)))
      {
        ExcelWorksheet priceListSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.PriceListSheetName];
        if (priceListSheet == null) excelPackage.Workbook.Worksheets.Add(ExcelServiceConstants.PriceListSheetName);
        excelPackage.Save();
      }
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, ExcelServiceConstants.PriceTagsFileName)))
      {
        ExcelWorksheet priceTagsSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.PriceTagsSheetName];
        if (priceTagsSheet == null) excelPackage.Workbook.Worksheets.Add(ExcelServiceConstants.PriceTagsSheetName);
        excelPackage.Save();
      }
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, ExcelServiceConstants.ReportFileName)))
      {
        ExcelWorksheet reportSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.ReportSheetName];
        if (reportSheet == null) excelPackage.Workbook.Worksheets.Add(ExcelServiceConstants.ReportSheetName);
        excelPackage.Save();
      }
      if ((string.IsNullOrEmpty(customFileName)) || (string.IsNullOrEmpty(customSheetName))) return;
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, customFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[customSheetName];
        if (workSheet == null) excelPackage.Workbook.Worksheets.Add(customSheetName);
        excelPackage.Save();
      }
    }    
    #endregion

    #region Конструкторы.
    /// <summary>
    /// Конструктор сервисного класса Excel.
    /// </summary>
    /// <param name="directory"> Путь до директории с рабочими файлами Excel. </param>
    public ExcelService(string directory)
    {
      if ((!File.Exists(Path.Combine(directory, ExcelServiceConstants.PriceListFileName)))
          && (!File.Exists(Path.Combine(directory, ExcelServiceConstants.PriceTagsFileName)))
          && (!File.Exists(Path.Combine(directory, ExcelServiceConstants.ReportFileName)))
         )
        CreateFileAndSheetsIfNotExists(directory);
    }
    #endregion
  }
}