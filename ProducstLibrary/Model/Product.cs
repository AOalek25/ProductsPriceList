using System.Globalization;
using ProductLibrary.Attributes;
using ProductLibrary.Exceptions;

namespace ProductLibrary.Model
{
  [NameValidator(2)]
  [ManufacturerValidator(2)]
  public class Product
  {
    #region Поля и свойства    
    const string Undefined = "Undefined";
    const string IncorrectPriceMessage = "Введено неверно значение цены.";
    const string IncorrectIdMessage = "Введено неверное значение id.";
    private readonly Guid id;
    private decimal price;
    public string Id { get => this.id.ToString(); }        
    public string Name { get; set; }    
    public string Manufacturer { get; set; }    
    public string Price 
    {
      get
      { 
        return string.Format(CultureInfo.CurrentCulture, "{0:f2}", this.price); 
      }      
      set
      {
        if ((decimal.TryParse(value, out decimal decimalValue)) && (decimalValue >= 0))
          this.price = decimalValue;
        else throw new ValidationException(IncorrectPriceMessage);
      }
    }    
    #endregion

    #region Методы
    public int CompareTo(object? obj)
    {
      if (obj is Product product) return product.ToString().CompareTo(this.Name);
      else return 1;
    }
    public override bool Equals(object? obj) => (obj is Product product) && (this.ToString() == product.ToString());        
    public string PrintInfo() => $"{Id} {Name} {Manufacturer} {Price}";    
    public override string ToString() => $"{Name} {Manufacturer}";       
    public override int GetHashCode() => this.PrintInfo().GetHashCode();            
    #endregion

    #region Конструкторы        
    public Product(string name, string manufacturer, string price)
    { 
      this.id = Guid.NewGuid();            
      this.Name = name;
      this.Manufacturer = manufacturer;
      this.Price = price;
    }
    public Product(string id, string name, string manufacturer, string price) : this(name, manufacturer, price)
    {
      if (Guid.TryParse(id, out Guid idGuid))
        this.id = idGuid;
      else throw new ValidationException(IncorrectIdMessage);
    }

    public Product()
    {
      this.id = Guid.NewGuid();
      this.Name = Undefined;
      this.Manufacturer = Undefined;
      this.price = 0m;
    }
    #endregion
  }  
}
