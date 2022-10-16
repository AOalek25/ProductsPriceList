using System.Globalization;
using System.Runtime.CompilerServices;

using ProducstLibrary.Attributes;

namespace ProducstLibrary.Model
{
  public class Bread : IProduct
  {
    #region Поля и свойства
    const string AssemblyName = "ProducstLibrary";
    const string undefined = "undefined";
    public Guid Id { get; init; }
    public string TypeName { get; init; }
    [NameValidator(2)]
    public string Name { get; set; }
    [ManufacturerValidator(2)]
    public string Manufacturer { get; set; }
    public decimal Price { get; set; }
    
    #endregion

    #region Методы
    public int CompareTo(object? obj)
    {
      if (obj is IProduct product) return product.Name.CompareTo(this.Name);
      else return 1;
    }
    public override bool Equals(object? obj) => (obj is IProduct product) && (this.ToString() == product.ToString());        
    public string PrintInfo() => $"{Id}: {Name} {Manufacturer} {PrintPrice()}";
    public string PrintPrice() => string.Format(CultureInfo.CurrentCulture, "{0:c2}", this.Price);
    public override string ToString() => $"{Name} {Manufacturer}";       
    public override int GetHashCode() => this.PrintInfo().GetHashCode();

    public string PrintType() => "Хлеб";
    #endregion

    #region Конструкторы    
    [DefaultConstructor]
    public Bread(string name = "Undefined", string manufacturer = "Undefined", decimal price = 0m)
    { 
      this.Id = Guid.NewGuid();
      this.TypeName = $"{typeof(Bread).FullName}, {AssemblyName}";
      this.Name = name;
      this.Manufacturer = manufacturer;
      this.Price = price;
    }
    public Bread(Guid id, string typeName, string name, string manufacturer, decimal price) : this(name, manufacturer, price)
    {
      this.Id = id;
      this.TypeName=typeName;
    }

    public Bread()
    {
      this.Name = undefined;
      this.Manufacturer = undefined;
      this.TypeName = typeof(Bread).Name;
    }

    #endregion
  }  
}
