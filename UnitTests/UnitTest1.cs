using ProductLibrary.Model;
using ProductLibrary;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using ProductLibrary.Exceptions;
using ExcelService;
using OfficeOpenXml;
using System.Linq;

namespace UnitTests
{
  public class Tests
  {
    private const int ManufacturerId = 0;
    private const string Price2_5 = "2,5";
    private const string Price2_3 = "2,3";
    private const string Price2_4 = "2,4";
    private const string Price2_35 = "2,35";
    private const string Product1 = "Product1";
    private const string Product15 = "Product15";
    private const string Product2 = "Product2";
    private const string Item3 = "Item3";
    private const string tooShortString = "a";
    private readonly ProductRepo repo = new();
    private Product? testProduct;
    private Product? newTestProduct;
    readonly Random intGenerator = new();
    private string? validName;
    private string? tooShortName;
    private string? unvalidName;
    private Guid validManufacturerId;    
    private Guid unvalidManufacturerId;
    private string? validPrice;
    private string? unvalidPrice;
    private string? negativePrice;
    private ExcelService.ExcelService? excelservice;

    [SetUp]
    public void Setup()
    {
      var stringFactory = RandomizerFactory.GetRandomizer(new FieldOptionsText
      { UseNumber = false,
        UseSpecial = false,
        UseSpace = false,
        Seed = 10 });
      validName = stringFactory.Generate();
      validManufacturerId = Guid.NewGuid();
      tooShortName = tooShortString;      
      var stringUnvalidFactory = RandomizerFactory.GetRandomizer(new FieldOptionsText
      { UseNumber = true,
        UseSpecial = true,
        UseSpace = true,
        Seed = 10 });
      unvalidName = stringUnvalidFactory.Generate();
      unvalidManufacturerId = Guid.Parse("DEA29528-56F1-4A7F-8B56-B8181F17631");
      validPrice = (intGenerator.Next(0, int.MaxValue) / 100).ToString();
      negativePrice = (intGenerator.Next(int.MinValue, 0) / 100).ToString();
      unvalidPrice = stringUnvalidFactory.Generate();
      if ((validName != null) && (validPrice != null))
      {
        testProduct = new Product(validName, validManufacturerId, validPrice);
        newTestProduct = new Product($"new{validName}", validManufacturerId, validPrice);
      }
    }

    [Test]
    public void CreateProductTest()
    {
      if (testProduct != null)
        repo.Create(testProduct);
      Assert.Multiple(() =>
      {
        Assert.That(repo.GetAll().Any(x => x.Equals(testProduct)), Is.True);
        Assert.That((repo.GetAll().Count() == 1), Is.True);
      });
    }

    [Test]
    public void AddRangeProductsTest()
    {
      List<Product> list=new();
      if ((testProduct != null) && (newTestProduct != null))
      {
        list.Add(testProduct);
        list.Add(newTestProduct);
      }
      repo.AddRange(list);
      Assert.Multiple(() =>
      {
        Assert.That(repo.GetAll().Any(x => x.Equals(testProduct)), Is.True);
        Assert.That(repo.GetAll().Any(x => x.Equals(newTestProduct)), Is.True);
        Assert.That((repo.GetAll().Count() == 2), Is.True);
      });
    }

    [Test]
    public void ProductRepoAddExistingItemTest()
    {
      if (testProduct != null)
      {
        repo.Create(testProduct);
        Assert.Throws<ProductAlreadyExistException>(() => repo.Create(testProduct));
      }
    }

    [Test]
    public void ProductRepoReadItemTest()
    {
      if (testProduct != null)
        repo.Create(testProduct);
    //  if (validName != null) 
     //   Assert.That(repo.Read(validName, validManufacturerId), Is.EqualTo(testProduct));
    }

    [Test]
    public void ProductRepoReadNotExistedItemTest()
    {
    //  if (unvalidName != null) 
    //    Assert.Throws<ProductNotFoundException>(() => repo.Read(unvalidName, unvalidManufacturerId));
    }

    [Test]
    public void ProductRepoUpdateItemTest()
    {
      if ((testProduct != null) && (newTestProduct != null))
      {
        repo.Create(testProduct);
   //     repo.Update(testProduct, newTestProduct);
        Assert.That(repo.GetAll().Any(x => x.Equals(newTestProduct)), Is.True);
      }
    }

    [Test]
    public void ProductRepoDeleteItemTest()
    {
      if (testProduct != null)
        repo.Create(testProduct);
   //   if (validName != null)
   //     repo.Delete(validName, validManufacturerId);        
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void ProductRepoDeleteNotExistingItemTest()
    {
    //  if (validName != null) 
   //     Assert.Throws<ProductNotFoundException>(() => repo.Delete(validName, validManufacturerId));
    }

    [Test]
    public void ProductRepoGetAllItemsTest()
    {
      if ((testProduct != null) && (newTestProduct != null))
      {
        repo.Create(testProduct);
        repo.Create(newTestProduct);
        Assert.Multiple(() =>
        {
          Assert.That(repo.GetAll().Any(x => x.Equals(testProduct)), Is.True);
          Assert.That(repo.GetAll().Any(x => x.Equals(newTestProduct)), Is.True);
        });
      }
    }

    [Test]
    public void ProductRepoClearTest()
    {
      if (testProduct != null)
        repo.Create(testProduct);
      repo.Clear();
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void ProductRepoSortedByNameTest()
    {
   //   testProduct = new Product(Product15, ManufacturerId, Price2_5);
      repo.Create(testProduct);
   //   repo.Create(new Product(Product1, ManufacturerId, Price2_4));      
  //    repo.Create(new Product(Product2, ManufacturerId, Price2_3));
  //    repo.Create(new Product(Item3, ManufacturerId, Price2_35));
      repo.SortedByName();
      var list = repo.GetAll().ToList();
      Assert.That((list.Last().Equals(testProduct)), Is.True);
    }

    [Test]
    public void ProductRepoSortedByPriceTest()
    {
   //   testProduct = new Product(Product2, ManufacturerId, Price2_5);
      repo.Create(testProduct);
   //   repo.Create(new Product(Product1, ManufacturerId, Price2_4));
  //    repo.Create(new Product(Product15, ManufacturerId, Price2_3));
  //    repo.Create(new Product(Item3, ManufacturerId, Price2_35));
      repo.SortedtByPrice();
      var list = repo.GetAll().ToList();
      Assert.That((list.Last().Equals(testProduct)), Is.True);
    }

    [Test]
    public void CreateProductTooShortNameTest()
    {
      if ((tooShortName != null) && (validManufacturerId != null) && (validPrice != null))
        testProduct = new Product(tooShortName, validManufacturerId, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductUnvalidNameTest()
    {
      if ((unvalidName != null) && (validPrice != null))
        testProduct = new Product(unvalidName, validManufacturerId, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }
    
    [Test]
    public void CreateProductUnvalidManufacturerTest()
    {
      if ((validName != null) && (validPrice != null))
        testProduct = new Product(validName, unvalidManufacturerId, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductNegativePriceTest()
    {
      if ((validName != null) && (negativePrice != null))
        Assert.Throws<ValidationException>(() => new Product(validName, validManufacturerId, negativePrice)); 
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductUnvalidPriceTest()
    {
      if ((validName != null) && (unvalidPrice != null))
        Assert.Throws<ValidationException>(() => new Product(validName, validManufacturerId, unvalidPrice));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void PriceListFileExistsTest()
    {
      excelservice = new(Environment.CurrentDirectory);
      Assert.That(File.Exists(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceListFileName)), Is.True);
    }

    [Test]
    public void PriceTagsFileExistsTest()
    {
      excelservice = new(Environment.CurrentDirectory);
      Assert.That(File.Exists(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceTagsFileName)), Is.True);
    }

    [Test]
    public void ReportFileExistsTest()
    {
      excelservice = new(Environment.CurrentDirectory);
      Assert.That(File.Exists(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.ReportFileName)), Is.True);
    }

    [Test]
    public async Task LoadAndSaveToExcelTestAsync()
    {
      excelservice = new(Environment.CurrentDirectory);
      if (testProduct != null)
        repo.Create(testProduct);
      await excelservice.SaveToFileAsync(repo.GetAll(), Environment.CurrentDirectory);
      repo.Clear();
      var list = excelservice.LoadFromFile(Environment.CurrentDirectory);
      foreach (var product in list)
        repo.Create(product);
      Assert.That(repo.GetAll().Any(x => x.Equals(testProduct)), Is.True);
      Assert.That((repo.GetAll().Count() == 1), Is.True);
    }


    [Test]
    public async Task PriceTagsSaveToFileTest()
    {      
      excelservice = new(Environment.CurrentDirectory);
      if (testProduct != null)
        repo.Create(testProduct);
      await excelservice.SaveToFileAsync(repo.GetAll(), Environment.CurrentDirectory);
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceTagsFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.PriceTagsSheetName];
        if (workSheet != null)
        {
          workSheet.Cells.Clear();
          workSheet.Drawings.Clear();
          await excelservice.PrintAllPriceTagsAsync(repo.GetAll(), Environment.CurrentDirectory);
        }        
      }
      using (var excelPackage = new ExcelPackage(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceTagsFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.PriceTagsSheetName];
        if (workSheet != null)
          Assert.That(workSheet.Drawings.Any(), Is.True);
      }
    }
    [Test]
    public async Task ReportSaveToFileTest()
    {
      excelservice = new(Environment.CurrentDirectory);
      if (testProduct != null)
        repo.Create(testProduct);
      await excelservice.SaveToFileAsync(repo.GetAll(), Environment.CurrentDirectory);
      if (testProduct != null)
      {
        decimal decimalPrice = decimal.Parse(testProduct.Price);
        decimal newDecimalPrice = decimalPrice + 1;
        testProduct.Price= newDecimalPrice.ToString();
        repo.Clear();
        repo.Create(testProduct);
      }
      await excelservice.SaveToFileAsync(repo.GetAll(), Environment.CurrentDirectory, ExcelServiceConstants.NewPriceListFileName);
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;      
      using (var excelPackage = new ExcelPackage(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.ReportFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.ReportSheetName];
        if (workSheet != null)
        {
          workSheet.Cells.Clear();
          workSheet.Drawings.Clear();
          await excelservice.GenerateReport(Environment.CurrentDirectory);
        }
      }
      using (var excelPackage = new ExcelPackage(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.ReportFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.ReportSheetName];
        if (workSheet != null)
          Assert.That((workSheet.Cells[2,1].Value !=null), Is.True);
      }
    }

    [TearDown]
    public void TearDown()
    {     
      repo.Clear();     
      File.Delete(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceListFileName));
      File.Delete(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceTagsFileName));
      File.Delete(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.ReportFileName));
    }
  }
}