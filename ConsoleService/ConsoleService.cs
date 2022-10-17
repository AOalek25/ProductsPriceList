using ExcelService;
using ProducstLibrary;
using ProducstLibrary.Exceptions;
using ProducstLibrary.Model;

namespace ConsoleService
{
  public class ConsolService
  {
    public int id;
    public string typeName;
    public string name;
    public decimal price;
    public string manufacturer;
    ProductsRepo<IProduct> repo; 
    ExcelService<IProduct> excelService; 
  
    public ConsolService()
    {
      this.name = "undefined";
      this.manufacturer = "undefined";
      this.repo = new();
      this.excelService = new(Environment.CurrentDirectory);
      try
      {
        foreach (var item in excelService.LoadFromFile(Environment.CurrentDirectory))
          repo.Create(item);
      }
      catch (Exception ex)
      {
        ShowMessage(ex.Message);        
      }
    }

    public static void PrintMenu()
    {
      Console.WriteLine("Выберите, что хотите сделать: ");
      Console.WriteLine("1. Распечатать информацию об продукте из прайс-листа.");
      Console.WriteLine("2. Обновить информацию об продукте прайс-листа.");
      Console.WriteLine("3. Добавить новый пордукт в прайс-лист.");
      Console.WriteLine("4. Удалить продукт из прайс-листа");
      Console.WriteLine("5. Распечатать весь прайс-лист");
      Console.WriteLine("6. Добавить новый файл прайс-листа и сохранить отчет об изменении цен.");
      Console.WriteLine("7. Распечатать всe ценники в Excel");
      Console.WriteLine("8. Загрузить новый прайс-лист");
      Console.WriteLine("9. Отсортировать прайс-лист по названию");
      Console.WriteLine("10. Отсортировать прайс-лист по цене");
      Console.WriteLine("11. Выход");
    }

    internal static void ShowMessage(string msg) => Console.WriteLine(msg);      
    
    internal void Create()
    {
      Console.WriteLine("Введите тип");
      if (ReadStringNotValid(out typeName)) return;
      Console.WriteLine("Введите название");
      if (ReadStringNotValid(out name)) return;
      Console.WriteLine("Введите производителя");
      if (ReadStringNotValid(out manufacturer)) return;
      Console.WriteLine("Введите цену");
      if (!Decimal.TryParse(Console.ReadLine(), out price)) return;
      foreach (var registeredType in repo.GetRegisteredTypes())
        if (this.typeName.Equals(registeredType.Key, StringComparison.CurrentCultureIgnoreCase))
        {         
          try
          {
            var obj = Activator.CreateInstance(registeredType.Value, new object[] { name, manufacturer, price });          
            repo.Create((IProduct)obj);
            excelService.SaveToFile(repo.GetAll(), Environment.CurrentDirectory, ExcelServiceConstants.DefaultPriceListSheetName);
            return;
          }
          catch (ProductNotFoundException ex)
          {
            Console.WriteLine($" Ошибка: {ex.Message}");
            return;
          }
        }
      ShowMessage("Такой тип не найден");
    }

    internal void Delete()
    {
      Console.WriteLine("Введите название");
      if (ReadStringNotValid(out name)) return;
      Console.WriteLine("Введите производителя");
      if (ReadStringNotValid(out manufacturer)) return;
      try
      {
        repo.Delete(name, manufacturer);
        excelService.SaveToFile(repo.GetAll(), Environment.CurrentDirectory, ExcelServiceConstants.DefaultPriceListSheetName);
      }
      catch (ProductNotFoundException ex)
      {
        Console.WriteLine($" Ошибка: {ex.Message}");
        return;
      }
    }

    internal void PrintAll()
    {
      var list = excelService.LoadFromFile(Environment.CurrentDirectory);
      foreach (var item in list)
        Console.WriteLine(item.PrintInfo());
    }

    internal void Read()
    {
      Console.WriteLine("Введите название");
      if (ReadStringNotValid(out name)) return;
      Console.WriteLine("Введите производителя");
      if (ReadStringNotValid(out manufacturer)) return;
      try
      {
        var item = repo.Read(name,manufacturer);
        Console.WriteLine(item.PrintInfo());
      }
      catch (ProductNotFoundException ex)
      {
        Console.WriteLine($" Ошибка: {ex.Message}");
        return;
      }
    }
    internal void Update()
    {
      this.Read();
      var oldData = repo.Read(name, manufacturer);
      Console.WriteLine("Введите новое название");
      if (ReadStringNotValid(out name)) return;
      Console.WriteLine("Введите нового производителя");
      if (ReadStringNotValid(out manufacturer)) return;
      Console.WriteLine("Введите Price");      
      if (!Decimal.TryParse(Console.ReadLine(), out price)) return;
      try
      {
        repo.Update(oldData, new Bread(name, manufacturer, price));
        excelService.SaveToFile(repo.GetAll(), Environment.CurrentDirectory, ExcelServiceConstants.DefaultPriceListSheetName);
      }
      catch (ProductNotFoundException ex)
      {
        ShowMessage($" Ошибка: {ex.Message}");
        return;
      }
      catch (ProductAlreadyExistException ex)
      {
        ShowMessage($" Ошибка: {ex.Message}");
        return;
      }
    }

    static bool ReadStringNotValid(out string param)
    {
      param = Console.ReadLine() ?? "";
      if (string.IsNullOrEmpty(param))
      {
        ShowMessage("warning");
        return true;
      }
      return false;
    }

    static bool ReadIntNotValid(out int param)
    {
      if (Int32.TryParse(Console.ReadLine(), out param)) return false;
      else 
      {
        ShowMessage("warning");
        return true;
      }      
    }
    
    internal void LoadNewPriceList(string fileName = "ProductsPriceList.xlsx")
    {       
      Console.WriteLine("Веедите путь до нового файла прайс-листа");
      string? sourcePath = Console.ReadLine();
      if (File.Exists(sourcePath))
      {
        string destPath=  Path.Combine(Environment.CurrentDirectory, fileName);        
        File.Copy(sourcePath, destPath, true);
        repo.Clear();
        foreach (var item in excelService.LoadFromFile(Environment.CurrentDirectory))
          repo.Create(item);
      }
      else Console.WriteLine("Файл не найден");
    }

    internal void GenerateReportChangedPrices() 
    {
      List<IProduct> comparisonResult = new();
      List<IProduct> products = excelService.LoadFromFile(Environment.CurrentDirectory).ToList();
      this.LoadNewPriceList("NewProductsPriceList.xlsx");
      List<IProduct> newProducts = excelService.LoadFromFile("NewProductsPriceList.xlsx").ToList();
      foreach (IProduct product in products)
        foreach (IProduct newProduct in newProducts)
          if ((product.Name == newProduct.Name) && (product.Manufacturer == newProduct.Manufacturer) && (product.Price!=newProduct.Price))
            comparisonResult.Add((IProduct)newProduct);
      foreach (var item in comparisonResult)
        Console.WriteLine(item.PrintInfo());
      excelService.SaveToFile(comparisonResult, Environment.CurrentDirectory, ExcelServiceConstants.DefaultReportExcelSheetName);      
      Console.WriteLine("отчет сохранен");
    }

    internal void PrintAllPriceTags ()
    {
      excelService.PrintAllPriceTags();
    }

    internal void SortByName()
    {        
      excelService.SaveToFile(repo.SortedByName(), Environment.CurrentDirectory, ExcelServiceConstants.DefaultPriceListSheetName);    
    }
    internal void SortByPrice()
    {      
      excelService.SaveToFile(repo.SortedtByPrice(), Environment.CurrentDirectory, ExcelServiceConstants.DefaultPriceListSheetName);   
    }

  }
}
