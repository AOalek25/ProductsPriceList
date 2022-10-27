using ProductLibrary.Model;
using ProductLibrary;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using ProductLibrary.Exceptions;
using ExcelService;
using OfficeOpenXml;

namespace UnitTests
{
  public class Tests
  {
    private const string Manufacturer = "manufacturer" ;
    private const string Price2_5 = "2,5";
    private const string Price2_3 = "2,3";
    private const string Price2_4 = "2,4";
    private const string Price2_35 = "2,35";
    private const string Product1 = "Product1";
    private const string Product15 = "Product15";
    private const string Product2 = "Product2";
    private const string Item3 = "Item3";
    private const string tooShortString = "a";
    private readonly ProductRepo repo=new();    
    private Product? testProduct;
    private Product? newTestProduct;
    readonly Random intGenerator = new();
    private string? validName;
    private string? tooShortName;
    private string? unvalidName;
    private string? validManufacturer;
    private string? tooShortManufacturer;
    private string? unvalidManufacturer;
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
      validManufacturer = stringFactory.Generate();      
      tooShortName = tooShortString;
      tooShortManufacturer = tooShortString;
      var stringUnvalidFactory = RandomizerFactory.GetRandomizer(new FieldOptionsText
                                                                { UseNumber = true, 
                                                                  UseSpecial = true, 
                                                                  UseSpace = true, 
                                                                  Seed = 10 });
      unvalidName = stringUnvalidFactory.Generate();
      unvalidManufacturer = stringUnvalidFactory.Generate();
      validPrice = (intGenerator.Next(0, int.MaxValue)/100).ToString();
      negativePrice = (intGenerator.Next(int.MinValue, 0)/100).ToString();
      unvalidPrice = stringUnvalidFactory.Generate();
      if ((validName != null) && (validManufacturer != null) && (validPrice != null))
      {
        testProduct = new Product(validName, validManufacturer, validPrice);
        newTestProduct = new Product($"new{validName}", $"new{validManufacturer}", validPrice);
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
      if ((validName != null) && (validManufacturer != null))
        Assert.That(repo.Read(validName, validManufacturer), Is.EqualTo(testProduct));
    }

    [Test]
    public void ProductRepoReadNotExistedItemTest()
    {
      if ((unvalidName != null) && (unvalidManufacturer != null))
        Assert.Throws<ProductNotFoundException>(() => repo.Read(unvalidName, unvalidManufacturer));
    }

    [Test]
    public void ProductRepoUpdateItemTest()
    {
      if ((testProduct != null) && (newTestProduct != null))
      {
        repo.Create(testProduct);
        repo.Update(testProduct, newTestProduct);
        Assert.That(repo.GetAll().Any(x => x.Equals(newTestProduct)), Is.True);
      }
    }

    [Test]
    public void ProductRepoDeleteItemTest()
    {
      if (testProduct != null)
        repo.Create(testProduct);
      if ((validName != null) && (validManufacturer != null))      
        repo.Delete(validName, validManufacturer);        
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void ProductRepoDeleteNotExistingItemTest()
    {
      if ((validName != null) && (validManufacturer != null))
        Assert.Throws<ProductNotFoundException>(() => repo.Delete(validName, validManufacturer));
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
      testProduct = new Product(Product15, Manufacturer, Price2_5);
      repo.Create(testProduct);
      repo.Create(new Product(Product1, Manufacturer, Price2_4));      
      repo.Create(new Product(Product2, Manufacturer, Price2_3));
      repo.Create(new Product(Item3, Manufacturer, Price2_35));
      repo.SortedByName();
      var list = repo.GetAll().ToList();
      Assert.That((list.Last().Equals(testProduct)), Is.True);
    }

    [Test]
    public void ProductRepoSortedByPriceTest()
    {
      testProduct = new Product(Product2, Manufacturer, Price2_5);
      repo.Create(testProduct);
      repo.Create(new Product(Product1, Manufacturer, Price2_4));
      repo.Create(new Product(Product15, Manufacturer, Price2_3));
      repo.Create(new Product(Item3, Manufacturer, Price2_35));
      repo.SortedtByPrice();
      var list = repo.GetAll().ToList();
      Assert.That((list.Last().Equals(testProduct)), Is.True);
    }

    [Test]
    public void CreateProductTooShortNameTest()
    {
      if ((tooShortName != null) && (validManufacturer != null) && (validPrice != null))
        testProduct = new Product(tooShortName, validManufacturer, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductUnvalidNameTest()
    {
      if ((unvalidName != null) && (validManufacturer != null) && (validPrice != null))
        testProduct = new Product(unvalidName, validManufacturer, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductTooShortManufacturerTest()
    {
      if ((validName != null) && (tooShortManufacturer != null) && (validPrice != null))
        testProduct = new Product(validName, tooShortManufacturer, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductUnvalidManufacturerTest()
    {
      if ((validName != null) && (unvalidManufacturer != null) && (validPrice != null))
        testProduct = new Product(validName, unvalidManufacturer, validPrice);
      if (testProduct != null)
        Assert.Throws<ValidationException>(() => repo.Create(testProduct));
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductNegativePriceTest()
    {
      if ((validName != null) && (validManufacturer != null) && (negativePrice != null))
        Assert.Throws<ValidationException>(() => new Product(validName, validManufacturer, negativePrice)); 
      Assert.That((repo.GetAll().Count() == 0), Is.True);
    }

    [Test]
    public void CreateProductUnvalidPriceTest()
    {
      if ((validName != null) && (validManufacturer != null) && (unvalidPrice != null))
        Assert.Throws<ValidationException>(() => new Product(validName, validManufacturer, unvalidPrice));
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