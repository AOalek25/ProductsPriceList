using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using System.Globalization;

namespace ProducstLibrary.Model
{
  public class Product : ICloneable, IComparable
  {
    #region Поля и свойства    
    public Guid Id { init;  get; }
    public virtual string Name { get; set; }    
    public string Manufacturer { get; set; }    
    public decimal Price  { get; set; }    
    #endregion

    #region Методы

    public virtual string PrintInfo() => $"{Id}: {Name} {Manufacturer} {FormattedPrice()}";

    public string FormattedPrice() => string.Format(CultureInfo.CurrentCulture, "{0:c2}", this.Price);

    public object Clone() => MemberwiseClone();

    public int CompareTo(object? obj)
    {      
      return (((Product)obj).PrintInfo().CompareTo(this.PrintInfo()));           
    }

    public override bool Equals(object? obj)
    {
      return obj is Product product &&
        this.Id == product.Id;        
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Id, Name, Price, Manufacturer);
    }    
    public override string ToString() => $"{Name} {Manufacturer}";

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
