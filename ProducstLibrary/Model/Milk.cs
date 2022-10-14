using System.Globalization;

namespace ProducstLibrary.Model
{
  public class Milk : IProduct
  {   
    #region Поля и свойства        
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public decimal Price { get; set; }
    public int FatContent { get; set; }
    #endregion

    #region Методы.
    public int CompareTo(object? obj) => (((Product)obj).Name.CompareTo(this.Name));    
    public string PrintInfo() => $"{Id}: {Name} {Manufacturer} {PrintPrice()}";  
    public string PrintPrice() => string.Format(CultureInfo.CurrentCulture, "{0:c2}", this.Price);    
    #endregion

    #region Конструкторы
    public Milk(Guid id, string name, string manufacturer, decimal price, int fatContent)
    {
      Id = id;
      Name = name;
      Manufacturer = manufacturer;
      Price = price;
      FatContent = fatContent;
    }

    public Milk()
    { }

    #endregion
  }
}
