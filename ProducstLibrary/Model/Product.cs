using System.Globalization;
using FluentNHibernate.Mapping;
using ProductLibrary.Attributes;
using ProductLibrary.Exceptions;

namespace ProductLibrary.Model
{
  /// <summary>
  /// Класс "Продукт".
  /// </summary>
  [NameValidator(2)]  
  public class Product
  {
    #region Поля и свойства            
    const string IncorrectPriceMessage = "Введено неверно значение цены.";
    const string IncorrectIdMessage = "Введено неверное значение id.";
    /// <summary>
    /// Идентификатор продукта, присваивается только при создании объекта.
    /// </summary>
    public virtual Guid Id { get; set; }
    /// <summary>
    /// Цена продукта.
    /// </summary>
    private decimal _price;
        
    /// <summary>
    /// Наименование продукта.
    /// </summary>
    public virtual string Name { get; set; }
    /// <summary>
    /// Идентификатор производителя продукта.
    /// </summary>
    public virtual Guid ManufacturerId { get; set; }
    /// <summary>
    /// Строковое свойство для цены продукта. Возвращает цену в культуре клиента.
    /// </summary>
    public virtual string Price
    {
      get
      {
        return string.Format(CultureInfo.CurrentCulture, "{0:f2}", this._price);
      }
      set
      {
        if ((decimal.TryParse(value, out decimal decimalValue)) && (decimalValue >= 0))
          this._price = decimalValue;
        else throw new ValidationException(IncorrectPriceMessage);
      }
    }
    #endregion

    #region Методы
    /// <summary>
    /// Метод для определения равны ли два объекта.
    /// </summary>
    /// <param name="obj"> Передаваемый объект для сравнения с текущим. </param>
    /// <returns> Возвращает true, если объекты равны, и false, если не равны. </returns>
    public override bool Equals(object? obj) => (obj is Product product) && (this.ToString() == product.ToString());
    /// <summary>
    /// Метод для строкового представления объекта.
    /// </summary>
    /// <returns> Возвращает строку с наименованием продукта и производителем. </returns>
    public override string ToString() => $"{Name} {ManufacturerId}";
    public override int GetHashCode()
    {
      return this.GetHashCode();
    }
    #endregion
    
    #region Конструкторы        
    /// <summary>
    /// Конструктор, принимающий три параметра(наименование, производитель, цена). Идентификатор задается автоматически.
    /// </summary>
    /// <param name="name"> Наименование продукта. </param>
    /// <param name="manufacturer"> Производитель продукта. </param>
    /// <param name="price"> Цена продукта. </param>
    public Product(string name, Guid manufacturerId, string price)
    {      
   //   this.Id = Guid.NewGuid();
      this.Name = name;
      this.ManufacturerId = manufacturerId;
      this.Price = price;
    }
    /// <summary>
    /// Конструктор, принимающий четыре параметра (идентификатор, наименование продукта, производитель, цена).
    /// </summary>
    /// <param name="id"> Идентификатор продукта. </param>
    /// <param name="name"> Наименование продукта. </param>
    /// <param name="manufacturer"> Производитель продукта. </param>
    /// <param name="price"> Цена продукта. </param>
    /// <exception cref="ValidationException"> Исключение, выбрасываемое при невалидном аргументе "идентификатор". </exception>
    public Product(string id, string name, Guid manufacturerId, string price) : this(name, manufacturerId, price)
    {
      if (Guid.TryParse(id, out Guid idGuid))
        this.Id = idGuid;
      else throw new ValidationException(IncorrectIdMessage);
    }

    public Product()
    {      
 //     this.Id = Guid.NewGuid();
      this.Name = "";
      this.ManufacturerId = Guid.Parse("DEA29528-56F1-4A7F-8B56-7B8181F17631");
      this.Price = "0";
    }

    #endregion
    
  }
}
