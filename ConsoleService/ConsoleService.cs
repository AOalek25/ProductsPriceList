using System.Collections.Generic;
using ExcelService;
using ProducstLibrary;
using ProducstLibrary.Exceptions;
using ProducstLibrary.Model;

namespace ConsoleService
{
  public class ConsolService
  {
    public int id;
    public string name;
    public decimal price;
    public string manufacturer;
    ProductsRepo<IProduct> repo;
    ProductsRepo<IProduct> newRepo;
    ExcelService<IProduct> excelService;
    ExcelService<IProduct> newExcelService;
  
    public ConsolService()
    {
      repo = new();
      newRepo = new();      
      excelService = new();      
      newExcelService = new();

  //    repo.Create(new Product("молоко", "Игра", 10m));
  //    repo.Create(new Product("молоко", "Можга", 10m));
  //    repo.Create(new Product("Хлеб","ХЗ№3", 5m));
    }

    public static void PrintMenu()
    {
      Console.WriteLine("Выберите, что хотите сделать: ");
      Console.WriteLine("1. Распечатать информацию об продукте из прайс-листа.");
      Console.WriteLine("2. Обновить информацию об продукте прайс-листа.");
      Console.WriteLine("3. Добавить новый пордукт в прайс-лист.");
      Console.WriteLine("4. Удалить продукт из прайс-листа");
      Console.WriteLine("5. Распечатать весь прайс-лист");
      Console.WriteLine("6. Добавить новый файл прайс-листа");
      Console.WriteLine("7. Распечатать всe ценники в Excel");
      Console.WriteLine("8. Отсортировать прайс-лист по названию");
      Console.WriteLine("9. Отсортировать прайс-лист по цене");
      Console.WriteLine("10. Выход");
    }

    internal static void ShowMessage(string msg) => Console.WriteLine(msg);      
    
    internal void Create()
    {
      Console.WriteLine("Введите название");
      if (ReadStringNotValid(out name)) return;
      Console.WriteLine("Введите производителя");
      if (ReadStringNotValid(out manufacturer)) return;
      Console.WriteLine("Введите Price");
      if (!Decimal.TryParse(Console.ReadLine(), out price)) return;            
      try
      {        
        repo.Create(new Product(name, manufacturer, price));
        excelService.SaveToFile(repo.GetAll(), ExcelServiceConstants.DefaultPriceListSheetName);
      }
      catch (ProductNotFoundException ex)
      {
        Console.WriteLine($" Ошибка: {ex.Message}");
        return;
      }
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
        excelService.SaveToFile(repo.GetAll(), ExcelServiceConstants.DefaultPriceListSheetName);
      }
      catch (ProductNotFoundException ex)
      {
        Console.WriteLine($" Ошибка: {ex.Message}");
        return;
      }
    }

    internal void PrintAll()
    {
      var list = excelService.LoadFromFile();
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
        repo.Update(oldData, new Product(name, manufacturer, price));
        excelService.SaveToFile(repo.GetAll(), ExcelServiceConstants.DefaultPriceListSheetName);
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
    
    internal void AddNewPriceList()
    {
      Console.WriteLine("Веедите путь до нового файла прайс-листа");
      string sourcePath =  Console.ReadLine();
      if (File.Exists(sourcePath))
      {
        string destPath=  Path.Combine(Environment.CurrentDirectory, "ProductsNewPriceList.xlsx");        
        File.Copy(sourcePath, destPath, true);
        newExcelService = new("ProductsNewPriceList.xlsx");        
      }
      else Console.WriteLine("Файл не найден");
    }

    internal void GenerateReportChangedPrices() 
    {
      List<IProduct> result = new();
      List<IProduct> products = excelService.LoadFromFile();
      List<IProduct> newproducts = excelService.LoadFromFile("ProductsNewPriceList.xlsx");
      foreach (Product item in products)
        foreach (Product itemNewList in newproducts)
          if (item.Equals(itemNewList) && (item.Price != itemNewList.Price)) result.Add((Product)itemNewList);
      foreach (var item in result)
        Console.WriteLine(item.PrintInfo());
      excelService.SaveToFile(result, ExcelServiceConstants.DefaultReportExcelSheetName);      
      Console.WriteLine("отчет сохранен");
    }

    internal void PrintAllPriceTags ()
    {
      excelService.PrintAllPriceTags();
    }

    internal void Sort()
    {
      IProduct temp;
      List<IProduct> l = repo.GetAll().ToList();
      for (int i = 0; i < repo.GetAll().Count() - 1; i++)
      {
        for (int j = i + 1; j < repo.GetAll().Count(); j++)
        {          
          if (l[i].CompareTo(l[j]) >0) 
          {
            temp = l[i];
            l[i] = l[j];
            l[j] = temp;
          }
        }
      }
      excelService.SaveToFile(l, ExcelServiceConstants.DefaultReportExcelSheetName);
      excelService.LoadFromFile();
    }
  }
}
