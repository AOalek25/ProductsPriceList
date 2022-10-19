using NetBarcode;
using OfficeOpenXml;
using System.Globalization;
using Type = System.Type;

namespace ExcelService
{
  public class ExcelService<T> where T: notnull, ProducstLibrary.Model.IProduct
  {
    #region Конструкторы.
    public ExcelService (string directory, string priceListfileName= ExcelServiceConstants.DefaultPriceListFileName, 
      string priceTagsFileName = ExcelServiceConstants.DefaultPriceTagsFileName, string reportFileName = ExcelServiceConstants.DefaultReportFileName)
    {
      if (!File.Exists(Path.Combine(directory, priceListfileName))&& !File.Exists(Path.Combine(directory, priceTagsFileName)) &&!File.Exists(Path.Combine(directory, reportFileName)))
        this.AddFileAndSheetsIfNotExists(directory, priceListfileName, priceTagsFileName, reportFileName);
    }
    #endregion

    #region Методы.
    public IEnumerable<T> LoadFromFile(string directory , string excelFileName= ExcelServiceConstants.DefaultPriceListFileName)
    {
      List<T> items = new List<T>();
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      string ExcelFilePath = Path.Combine(directory, excelFileName);
      this.AddFileAndSheetsIfNotExists(directory, ExcelServiceConstants.DefaultPriceListFileName, ExcelServiceConstants.DefaultPriceTagsFileName, ExcelServiceConstants.DefaultReportFileName);
      using (var excelPackage = new ExcelPackage(ExcelFilePath))
      {
        ExcelWorksheet priceListSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.DefaultPriceListSheetName];
        if (priceListSheet.Cells[1, 1].Value != null)
        {
          var columnInfo = Enumerable.Range(1, priceListSheet.Dimension.Columns).ToList().Select(n =>
                  new { Index = n, ColumnName = priceListSheet.Cells[1, n].Value.ToString() });
          for (int row = 2; row <= priceListSheet.Dimension.Rows; row++)
          {
            Type type = Type.GetType(priceListSheet.Cells[row, 2].Value.ToString());
            T obj = (T)Activator.CreateInstance(type);
            foreach (var prop in typeof(T).GetProperties())
            {
              int col = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name).Index;
              var val = priceListSheet.Cells[row, col].Value;
              var propType = prop.PropertyType;
              if (propType.Name == "Guid") prop.SetValue(obj, Guid.Parse(val.ToString()));
              else prop.SetValue(obj, Convert.ChangeType(val, propType));
            }
            items.Add(obj);
          }
        }
      }
      return items;
    }    

    public void SaveToFile(IEnumerable<T> items, string directory, string sheetName)
    {    
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      this.AddFileAndSheetsIfNotExists(directory, ExcelServiceConstants.DefaultPriceListFileName, ExcelServiceConstants.DefaultPriceTagsFileName, ExcelServiceConstants.DefaultReportFileName);
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, ExcelServiceConstants.DefaultPriceListFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[sheetName];
        workSheet.Cells.Clear();
        workSheet.Cells["A1"].LoadFromCollection(items, true);
        var table = workSheet.Cells[1, 1, workSheet.Dimension.Rows, workSheet.Dimension.Columns];
        foreach (var cell in table) cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Medium);                       
        table.AutoFitColumns();
        workSheet.Cells[1, 1, 1, workSheet.Dimension.Columns].Style.Font.Bold = true;
        workSheet.Cells[1, 1, 1, workSheet.Dimension.Columns].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        var priceColumn = Enumerable.Range(1, workSheet.Dimension.Columns).ToList().Select(n =>
                new { Index = n, ColumnName = workSheet.Cells[1, n].Value.ToString() }).SingleOrDefault(c => c.ColumnName == "Price");
        workSheet.Cells[1, priceColumn.Index, workSheet.Dimension.Rows, priceColumn.Index].Style.Numberformat.Format = "#,##0.00";        
        try
        {
          excelPackage.Save();
        }
        catch (InvalidOperationException ex)
        {
          Console.WriteLine($"Неуспешно, файл недоступен. {ex}");
        }
      }
    }

    public void PrintAllPriceTags(string directory, string fileName)
    {   
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      this.AddFileAndSheetsIfNotExists(directory, ExcelServiceConstants.DefaultPriceListFileName, ExcelServiceConstants.DefaultPriceTagsFileName, ExcelServiceConstants.DefaultReportFileName);
      using (var excel2Package = new ExcelPackage(Path.Combine(directory, ExcelServiceConstants.DefaultPriceListFileName)))
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, fileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.DefaultPriceTagsSheetName];
        ExcelWorksheet priceListSheet = excel2Package.Workbook.Worksheets[ExcelServiceConstants.DefaultPriceListSheetName];
        if (workSheet !=null) workSheet.Cells.Clear();
        if (workSheet != null) workSheet.Drawings.Clear();
        
        var columnInfo = Enumerable.Range(1, priceListSheet.Dimension.Columns).ToList().Select(n =>
                new { Index = n, ColumnName = priceListSheet.Cells[1, n].Value.ToString() });
        int rowIndex = 1;
        for (int row = 2; row <= priceListSheet.Dimension.Rows; row++)
        {
          Type type = Type.GetType(priceListSheet.Cells[row, 2].Value.ToString());
          T obj = (T)Activator.CreateInstance(type);
          foreach (var prop in typeof(T).GetProperties())
          {
            int col = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name).Index;
            var val = priceListSheet.Cells[row, col].Value;
            var propType = prop.PropertyType;
            if (propType.Name.Equals("Guid")) prop.SetValue(obj, Guid.Parse(val.ToString()));
            else prop.SetValue(obj, Convert.ChangeType(val, propType));
            workSheet.Cells[rowIndex, 1].Value = prop.Name;
            if (prop.Name == "Id")
            {
              var barcode = new Barcode(val.ToString(), NetBarcode.Type.Code128, true);
              var buffer = barcode.GetByteArray();
              workSheet.Row(rowIndex).Height = 75.00D;
              using (var stream = new System.IO.MemoryStream(buffer))
              {
                stream.Position = 0;
                var picture = workSheet.Drawings.AddPicture(val.ToString(), stream, OfficeOpenXml.Drawing.ePictureType.Jpg);
                picture.SetSize(250, 100);
                picture.SetPosition(rowIndex - 1, 0, 1, 0);
              }
            }
            if (prop.Name == "Price")
            {
              workSheet.Cells[rowIndex, 2].Value = string.Format(CultureInfo.CurrentCulture, "{0:c2}", val);
              workSheet.Cells[rowIndex, 2].Style.Font.Bold = true;
              workSheet.Cells[rowIndex, 2].Style.Font.Size = 24;
              workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
              rowIndex++;
            }
            if ((prop.Name != "Id") || (prop.Name != "Price")) workSheet.Cells[rowIndex, 2].Value = val.ToString();
            rowIndex++;
          }
        }
        workSheet.Cells[1, 1, workSheet.Dimension.Rows, workSheet.Dimension.Columns].AutoFitColumns();
        try
        {
          excelPackage.Save();
          excel2Package.Save();
        }
        catch (InvalidOperationException ex)
        {
          Console.WriteLine($"Неуспешно, файл недоступен. {ex}");
        }
      }
    }

    public void AddFileAndSheetsIfNotExists(string directory, string excelFileName, string priceTagsFileName, string reportfileName)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, excelFileName)))
      {
        ExcelWorksheet priceListSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.DefaultPriceListSheetName];
        if (priceListSheet == null) excelPackage.Workbook.Worksheets.Add(ExcelServiceConstants.DefaultPriceListSheetName);
        excelPackage.Save();
      }
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, priceTagsFileName)))
      {
        ExcelWorksheet pricetagsSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.DefaultPriceTagsFileName];
        if (pricetagsSheet == null) excelPackage.Workbook.Worksheets.Add(ExcelServiceConstants.DefaultPriceTagsFileName);
        excelPackage.Save();
      }
      using (var excelPackage = new ExcelPackage(Path.Combine(directory, reportfileName)))
      {
        ExcelWorksheet reportSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.DefaultReportExcelSheetName];
        if (reportSheet == null) excelPackage.Workbook.Worksheets.Add(ExcelServiceConstants.DefaultReportExcelSheetName);
        excelPackage.Save();
      }
    }

    #endregion
  }
}