
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace ProducstLibrary.Model
{
  public class Product : IProduct
  {
    #region Поля и свойства
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public decimal Price { get; set; }
    #endregion

    #region Методы
    public int CompareTo(object? obj) => ((Product)obj).Name.CompareTo(this.Name);           
    public override bool Equals(object? obj) => (obj is Product product) && (this.ToString() == product.ToString());        
    public string PrintInfo() => $"{Id}: {Name} {Manufacturer} {PrintPrice()}";
    public string PrintPrice() => string.Format(CultureInfo.CurrentCulture, "{0:c2}", this.Price);
    public override string ToString() => $"{Name} {Manufacturer}";       
    public override int GetHashCode() => this.PrintInfo().GetHashCode();
    #endregion

    #region Конструкторы    
    public Product(string name = "Undefined", string manufacturer = "Undefined", decimal price = 0m)
    {      
      this.Id = Guid.NewGuid();
      this.Name = name;
      this.Manufacturer = manufacturer;
      this.Price = price;
    }
    public Product(Guid id, string name, string manufacturer, decimal price) : this(name, manufacturer, price)
    {
      this.Id = id;
    }

    public Product()
    {      
    }

    #endregion
  }  
}
