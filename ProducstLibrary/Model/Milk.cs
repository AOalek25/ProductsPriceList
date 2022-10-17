using System.Globalization;
using ProducstLibrary.Attributes;

namespace ProducstLibrary.Model
{
  public class Milk : IProduct
  {
    #region Поля и свойства        
    const string AssemblyName = "ProducstLibrary";
    public Guid Id { get; init; }
    public string TypeName { get; init; }
    [NameValidator(2)]
    public string Name { get; set; }
    [ManufacturerValidator(2)]
    public string Manufacturer { get; set; }
    public decimal Price { get; set; }
    public decimal FatContent { get; set; }
    
    #endregion

    #region Методы.
    public int CompareTo(object? obj)
    {
      if (obj is IProduct product) return product.Name.CompareTo(this.Name);
      else return 1;      
    }
    public string PrintInfo() => $"{PrintId()} {PrintType()} {Name} {FatContent} {Manufacturer} {PrintPrice()}";  
    public string PrintPrice() => string.Format(CultureInfo.CurrentCulture, "{0:c2}", this.Price);
    public string PrintId() => this.Id.ToString("N");

    public string PrintType() => "Молоко";    
    #endregion

    #region Конструкторы
    public Milk(Guid id, string typeName, string name, string manufacturer, decimal price, int fatContent)
    {
      this.Id = id;
      this.TypeName= typeName;
      this.Name = name;
      this.Manufacturer = manufacturer;
      this.Price = price;
      this.FatContent = fatContent;
    }
    [DefaultConstructor]
    public Milk(string name = "Undefined", string manufacturer = "Undefined", decimal price = 0m)
    {
      this.Id = Guid.NewGuid();
      this.TypeName = $"{typeof(Milk).FullName}, {AssemblyName}";
      this.Name = name;
      this.Manufacturer = manufacturer;
      this.Price = price;
      this.FatContent = 5m;
    }
    public Milk()
    {
      this.TypeName = typeof(Milk).FullName;
      this.Name = "Undefined";
      this.Manufacturer = "Undefiined";      
    }
    #endregion
  }
}
