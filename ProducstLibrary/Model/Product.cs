using System.Globalization;
using ProductLibrary.Attributes;
using ProductLibrary.Exceptions;

namespace ProductLibrary.Model
{
  /// <summary>
  /// Класс "Продукт".
  /// </summary>
  [NameValidator(2)]
  [ManufacturerValidator(2)]
  public class Product
  {
    #region Поля и свойства            
    const string IncorrectPriceMessage = "Введено неверно значение цены.";
    const string IncorrectIdMessage = "Введено неверное значение id.";
    /// <summary>
    /// Идентификатор продукта, присваивается только при создании объекта.
    /// </summary>
    private readonly Guid _id;
    /// <summary>
    /// Цена продукта.
    /// </summary>
    private decimal _price;
    /// <summary>
    /// Строковое свойство для идентификатора продукта.
    /// </summary>
    public string Id { get => this._id.ToString(); }        
    /// <summary>
    /// Наименование пордукта.
    /// </summary>
    public string Name { get; set; }    
    /// <summary>
    /// Наименование производителя продукта.
    /// </summary>
    public string Manufacturer { get; set; }   
    /// <summary>
    /// Строковое свойство для цены продукта. Возвращает цену в культуре клиента.
    /// </summary>
    public string Price 
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
    public override string ToString() => $"{Name} {Manufacturer}";                      
    #endregion

    #region Конструкторы        
    /// <summary>
    /// Конструктор, принимающий три параметра(наименование, производитель, цена). Идентификатор задается автоматически.
    /// </summary>
    /// <param name="name"> Наименование продукта. </param>
    /// <param name="manufacturer"> ПРоизводитель продукта. </param>
    /// <param name="price"> Цена продукта. </param>
    public Product(string name, string manufacturer, string price)
    { 
      this._id = Guid.NewGuid();            
      this.Name = name;
      this.Manufacturer = manufacturer;
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
    public Product(string id, string name, string manufacturer, string price) : this(name, manufacturer, price)
    {
      if (Guid.TryParse(id, out Guid idGuid))
        this._id = idGuid;
      else throw new ValidationException(IncorrectIdMessage);
    }
    #endregion
  }  
}
