using ProductLibrary.Model;
using ProductLibrary;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using ProductLibrary.Exceptions;
using ExcelService;
using OfficeOpenXml;
using System.IO;

namespace UnitTests
{
  public class Tests
  {    
    private readonly ProductRepo repo=new();    
    private Product? testProduct;
    private Product? newTestProduct;
    readonly Random decimalGenerator = new();
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
      { UseNumber = true, UseSpecial = false, UseSpace = false, Seed = 10 });
      validName = stringFactory.Generate();
      validManufacturer = stringFactory.Generate();
      var shortStringFactory = RandomizerFactory.GetRandomizer(new FieldOptionsText
      { UseNumber = true, UseSpecial = false, UseSpace = false, Seed = 2 });
      tooShortName = "a";
      tooShortManufacturer = "A";
      var stringUnvalidFactory = RandomizerFactory.GetRandomizer(new FieldOptionsText
      { UseNumber = true, UseSpecial = true, UseSpace = true, Seed = 10 });
      unvalidName = stringUnvalidFactory.Generate();
      unvalidManufacturer = stringUnvalidFactory.Generate();
      validPrice = decimalGenerator.Next(0, int.MaxValue).ToString();
      negativePrice = decimalGenerator.Next(int.MinValue, 0).ToString();
      unvalidPrice = stringUnvalidFactory.Generate();
      if ((validName != null) && (validManufacturer != null) && (validPrice != null))
      {
        testProduct = new Product(validName, validManufacturer, validPrice);
        newTestProduct = new Product("new" + validName, "new" + validManufacturer, validPrice);
      }
    }

    [Test]
    public void CreateProductTest()
    {      
      if (testProduct != null)
        repo.Create(testProduct);
      Assert.That(repo.GetAll().Any(x => x.Equals(testProduct)), Is.True);
      Assert.That((repo.GetAll().Count() ==1), Is.True);
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
        Assert.IsTrue(repo.GetAll().Any(x => x.Equals(testProduct)));
        Assert.IsTrue(repo.GetAll().Any(x => x.Equals(newTestProduct)));
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
      testProduct = new Product("product 15", "0", "0");
      repo.Create(testProduct);
      repo.Create(new Product("product 1","0","0"));      
      repo.Create(new Product("product 2", "0", "0"));
      repo.Create(new Product("_product", "0", "0"));
      repo.SortedByName();
      var list = repo.GetAll().ToList();
      Assert.That((list.Last().Equals(testProduct)), Is.True);
    }

    [Test]
    public void ProductRepoSortedByPriceTest()
    {
      testProduct = new Product("product 2", "0", "2,5");
      repo.Create(testProduct);
      repo.Create(new Product("product 1", "0", "2,4"));
      repo.Create(new Product("product 15", "0", "2,3"));
      repo.Create(new Product("_product", "0", "2,45"));
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
    public void LoadAndSaveToExcelTest()
    {
      excelservice = new(Environment.CurrentDirectory);
      if (testProduct != null)
        repo.Create(testProduct);
      excelservice.SaveToFileAsync(repo.GetAll(), Environment.CurrentDirectory);
      repo.Clear();
      excelservice.LoadFromFile(Environment.CurrentDirectory);
      Assert.That(repo.GetAll().Any(x => x.Equals(testProduct)), Is.True);
      Assert.That((repo.GetAll().Count() == 1), Is.True);
    }


    [Test]
    public void PriceTagsSaveToFileTest()
    {      
      excelservice = new(Environment.CurrentDirectory);
      if (testProduct != null)
        repo.Create(testProduct);
      excelservice.SaveToFileAsync(repo.GetAll(), Environment.CurrentDirectory);
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
      using (var excelPackage = new ExcelPackage(Path.Combine(Environment.CurrentDirectory, ExcelServiceConstants.PriceTagsFileName)))
      {
        ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[ExcelServiceConstants.PriceTagsSheetName];
        if (workSheet != null)
        {
          workSheet.Cells.Clear();
          excelservice.PrintAllPriceTagsAsync(repo.GetAll(), Environment.CurrentDirectory);
          Assert.That((workSheet.Cells["A1"].Value.ToString().Equals("Идентификатор")), Is.True);
        }
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