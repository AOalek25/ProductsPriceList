using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

using OfficeOpenXml;
using ProducstLibrary.Exceptions;
using ProducstLibrary.Validators;

namespace ProducstLibrary
{
  public class ProductsRepo<T> : IEnumerable
  {
    #region Поля и свойства
    private List<IValidator> validators;
    private List<T> list;
    private List<T> newList;
    
    
    private string ExcelFilePath;
    

    #endregion

    #region Методы
    public void Create(T newItem)
    {
      Validate(newItem);
      foreach (var item in list)
        if (item.Equals(newItem)) throw new ProductAlreadyExistException(ProductLibConstants._alreadyExistMsg);
      list.Add(newItem);
      this.Save();
    }

    public T Read(string name, string manufacturer)
    {      
      foreach (var item in list)
        if (item.ToString().Equals($"{name} {manufacturer}", StringComparison.CurrentCultureIgnoreCase)) return item;
      throw new ProductNotFoundException(ProductLibConstants._notFoundMsg);
    }

    public void Update(T itemOldData, T itemNewData)
    {
      Validate(itemNewData);
      foreach (var item in list)
        if (item.Equals(itemNewData)) throw new ProductAlreadyExistException(ProductLibConstants._alreadyExistMsg);
      bool found=false;
      foreach (var item in list)
        if (item.Equals(itemOldData)) found = true;
      if (!found) throw new ProductNotFoundException(ProductLibConstants._notFoundMsg);
      list.Remove(itemOldData);
      list.Add(itemNewData);
      this.Save();
    }

    public void Delete(string name, string manufacturer)
    {
      foreach (var item in list)
        if (item.ToString().Equals($"{name} {manufacturer}", StringComparison.CurrentCultureIgnoreCase))
        {
          list.Remove(item);
          this.Save();
          return;
        }
      throw new ProductNotFoundException(ProductLibConstants._notFoundMsg);
    }
    // Метод, возвращающий все объекты из репозитория.
    public IEnumerable<T> GetAll() => this.list;
    private void Validate(T? item)
    {
      string errors = "";
      foreach (var validator in validators)
      {
        if (validator.NotValid(item, out string validateError))
          errors += validateError;
      }
      if (errors.Length > 0)
        throw new ArgumentException($"{item} не валиден: {errors}");
    }

    private void RegisterValidator(IValidator validator)
    {
      this.validators.Add(validator);
    }

    public IEnumerator GetEnumerator() => new ProductsEnumerator(this.list);

    private void Load()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(ExcelFilePath))
      {        
        ExcelWorksheet priceListSheet = excelPackage.Workbook.Worksheets[ProductLibConstants.DefaultPriceListSheetName];        
        var columnInfo = Enumerable.Range(1, priceListSheet.Dimension.Columns).ToList().Select(n =>
                new { Index = n, ColumnName = priceListSheet.Cells[1, n].Value.ToString() }
                );
        for (int row = 2; row <= priceListSheet.Dimension.Rows; row++)
        {
          T obj = (T)Activator.CreateInstance(typeof(T));

          foreach (var prop in typeof(T).GetProperties())
          {            
            int col = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name).Index;
            var val = priceListSheet.Cells[row, col].Value;
            var propType = prop.PropertyType;
            if (propType.Name == "Guid") prop.SetValue(obj, Guid.Parse(val.ToString()));
            else prop.SetValue(obj, Convert.ChangeType(val, propType));
          }
          list.Add(obj);
        }

      }
    }

    private void Save()
    {
      this.Save(list, ProductLibConstants.DefaultPriceListSheetName);
    }

    public void Save (List<T> items, string sheetName)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(ExcelFilePath))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[sheetName];            
        workSheet.Cells.Clear();        
        workSheet.Cells["A1"].LoadFromCollection(items, true);
        var table = workSheet.Cells[1, 1, workSheet.Dimension.Rows, workSheet.Dimension.Columns];
        table.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
        table.Style.Border.Diagonal.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
        table.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);
        table.AutoFitColumns();
        workSheet.Cells[1,1,1, workSheet.Dimension.Columns].Style.Font.Bold = true;
        workSheet.Cells[1,1,1, workSheet.Dimension.Columns].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        for (int row = 2; row <= workSheet.Dimension.Rows; row++)
        {
          using (var range = workSheet.Cells[row, 4])
          {
            range.Style.Numberformat.Format = "#,##0.00";                        
          }
        }
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

    public void PrintAllPriceTags()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(ExcelFilePath))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ProductLibConstants.DefaultPriceTagsSheetName];               
        ExcelWorksheet priceListSheet = excelPackage.Workbook.Worksheets[ProductLibConstants.DefaultPriceListSheetName];
        workSheet.Cells.Clear();

        var columnInfo = Enumerable.Range(1, priceListSheet.Dimension.Columns).ToList().Select(n =>
                new { Index = n, ColumnName = priceListSheet.Cells[1, n].Value.ToString() });
        int rowIndex = 1;
        for (int row = 2; row <= priceListSheet.Dimension.Rows; row++)
        {
          T obj = (T)Activator.CreateInstance(typeof(T));          
          foreach (var prop in typeof(T).GetProperties())
          {
            int col = columnInfo.SingleOrDefault(c => c.ColumnName == prop.Name).Index;
            var val = priceListSheet.Cells[row, col].Value;
            var propType = prop.PropertyType;
            if (propType.Name == "Guid") prop.SetValue(obj, Guid.Parse(val.ToString()));
            else prop.SetValue(obj, Convert.ChangeType(val, propType));
            workSheet.Cells[rowIndex, 1].Value = prop.Name;
            if (prop.Name == "Price")              
            {
              workSheet.Cells[rowIndex, 2].Value = string.Format(CultureInfo.CurrentCulture, "{0:c2}", val);
              workSheet.Cells[rowIndex, 2].Style.Font.Bold = true;
              workSheet.Cells[rowIndex, 2].Style.Font.Size = 24;
              workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
              rowIndex++;
            }
            else workSheet.Cells[rowIndex, 2].Value = val.ToString();
            rowIndex++;
          }          
        }
        workSheet.Cells[1, 1, workSheet.Dimension.Rows, workSheet.Dimension.Columns].AutoFitColumns();
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

    #endregion


    #region Конструкторы
    public ProductsRepo(string excelFileName = ProductLibConstants.DefaultExcelFileName)
    {
      this.validators = new();
      this.list = new();
      this.newList = new();
      this.RegisterValidator(new NameValidator());
      this.ExcelFilePath = Path.Combine(Environment.CurrentDirectory, excelFileName);
      if (!File.Exists(ExcelFilePath))
      {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var excelPackage = new ExcelPackage(ExcelFilePath))
        {
          excelPackage.Workbook.Worksheets.Add(ProductLibConstants.DefaultPriceListSheetName);
          excelPackage.Workbook.Worksheets.Add(ProductLibConstants.DefaultPriceTagsSheetName);
          excelPackage.Workbook.Worksheets.Add(ProductLibConstants.DefaultReportExcelSheetName);
          excelPackage.Save();
        }
      }
      else this.Load();
    }

    #endregion

      #region Вложенные классы
    private class ProductsEnumerator : IEnumerator
    {
      private List<T> list;
      int position = -1;

      public ProductsEnumerator(List<T> List) => this.list = List;

      public object Current
      {
        get
        {
          if ((position == -1) || (position >= list.Count) || (!list.Any()))
            throw new ArgumentException();
          return list[position];
        }
      }

      public bool MoveNext()
      {
        if (position < list.Count - 1)
        {
          position++;
          return true;
        }
        else
          return false;
      }

      public void Reset() => position = -1;
    }    
    #endregion
  }
}