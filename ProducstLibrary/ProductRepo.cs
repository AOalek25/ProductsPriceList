using ProductLibrary.Exceptions;
using ProductLibrary.Model;
using ProductLibrary.Attributes;
using NaturalSort.Extension;
using System.Text;
using System.Collections;

namespace ProductLibrary
{
  /// <summary>
  /// Репозиторий для продуктов.
  /// </summary>
  public class ProductRepo
  {
    #region Поля и свойства         
    /// <summary>
    /// Список для работы с продуктами.
    /// </summary>
    private List<Product> _list;          
    #endregion

    #region Методы
    /// <summary>
    ////Метод для добавления в писок нового продукта.
    /// </summary>
    /// <param name="newItem"> Передаваемый объект класса Продукт для добавления в список.</param>
    /// <exception cref="ProductAlreadyExistException"> Исключение, выбрасываемое, если добавляемый продукт уже есть в списке. </exception>
    public void Create(Product newItem)
    {
      Validate(newItem);
      foreach (Product item in _list)
        if (item.Equals(newItem)) throw new ProductAlreadyExistException();
      _list.Add(newItem);      
    }
    /// <summary>
    /// Метод для чтения продукта из списка.
    /// </summary>
    /// <param name="name"> Наименование продукта для чтения. </param>
    /// <param name="manufacturer"> Производитель продукта для чтения. </param>
    /// <returns> Возвращает продукт, найденный по имени и производителю. </returns>
    /// <exception cref="ProductNotFoundException"> Исключение, выбрасываемое, если продукта нет в списке. </exception>
    public Product Read(string name, int manufacturerId)
    {
      string itemToFind = $"{name} {manufacturerId}";
      foreach (Product item in _list)        
        if (string.Compare(item.ToString(), itemToFind, StringComparison.CurrentCultureIgnoreCase) == 0) return item;
      throw new ProductNotFoundException();
    }
    /// <summary>
    /// Метод для обновления продукта из списка.
    /// </summary>
    /// <param name="itemOldData"> Передаваемый объект класса Продукт для удаления из списка.</param>
    /// <param name="itemNewData"> Передаваемый объект класса Продукт для добавления в список.</param>
    /// <exception cref="ProductAlreadyExistException"> Исключение, выбрасываемое, если добавляемый продукт уже есть в списке. </exception>
 /*   public void Update(Product itemOldData, Product itemNewData)
    {
      Validate(itemNewData);
      foreach (Product item in _list)
        if (item.Equals(itemNewData)) throw new ProductAlreadyExistException();
      Read(itemOldData.Name, itemOldData.ManufacturerId);
      _list.Remove(itemOldData);
      _list.Add(itemNewData);      
    } */
    /// <summary>
    /// Метод для удаления продукта из списка.
    /// </summary>
    /// <param name="name"> Наименование продукта для удаления. </param>
    /// <param name="manufacturer"> Производитель продукта для удаления. </param>
    /// <exception cref="ProductNotFoundException"> Исключение, выбрасываемое, если продукта нет в списке. </exception>
    public void Delete(string name, int manufacturerId)
    {
      if (Read(name, manufacturerId) is Product product)
        _list.Remove(product);     
      else throw new ProductNotFoundException();
        
    }
    /// <summary>
    /// Метод, возвращающий все объекты из репозитория.
    /// </summary>
    /// <returns> Возвращает Ienumerable всех продуктов из репозитория.</returns>
    public IEnumerable<Product> GetAll() => this._list;
    /// <summary>
    /// Метод, добавляющий коллекцию продуктов в репозиторий.
    /// </summary>
    /// <param name="items"> Передаваемая коллекция продуктов для добавления в реощиторий. </param>
    /// <returns> Возвращает IEnumerable всех продуктов из репозитория. </returns>
    public IEnumerable<Product> AddRange(IEnumerable<Product> items)
    {
      var distinctedList = items.Distinct().ToList();
      foreach (var product in distinctedList) Validate(product);
      this._list.AddRange(distinctedList);
      return _list;
    }
    /// <summary>
    /// Метод для удаления всех объектов из репозитория.
    /// </summary>
    public void Clear() => this._list.Clear();
    /// <summary>
    /// Метод для натуральной сортировки по наименованию всех продуктов в репозитории. 
    /// </summary>
    /// <returns> Возвращает коллекцию всех продуктов из репозитория, отсортированную по наименованию продуктов. </returns>
    public IEnumerable<Product> SortedByName()
    {
      var sortedList = this._list.OrderBy(product => product.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort());
      this._list = sortedList.ToList();
      return sortedList;      
    }
    /// <summary>
    /// Меод для сортировки по цене всех продуктов из репозитория.
    /// </summary>
    /// <returns> Возвращает коллекцию всех продуктов из репозитория, отсортированную по цене продуктов. </returns>
    public IEnumerable<Product> SortedtByPrice()
    {
      var sortedList = this._list.OrderBy(product => product.Price, StringComparison.OrdinalIgnoreCase.WithNaturalSort());
      this._list = sortedList.ToList();
      return sortedList;
    }
    /// <summary>
    /// Метод для валидации наименования и производителя продукта.
    /// </summary>
    /// <param name="item"> Передаваемый продукт для валидации. </param>
    /// <exception cref="ValidationException"> Исключение, возникающее когда объект невалиден. </exception>
    private static void Validate(Product item)
    {
      StringBuilder stringBuilder = new();
      Type type = typeof(Product);
      object[] attributes = type.GetCustomAttributes(false);
      foreach (Attribute attr in attributes)
      {
        if (attr is NameValidator nameValidation)
          if (nameValidation.NotValid(item, out string validationError)) stringBuilder.Append(validationError + Environment.NewLine);        
      }
      if (stringBuilder.ToString().Length > 0)
        throw new ValidationException(stringBuilder.ToString());
    }
    
    #endregion


    #region Конструкторы
    /// <summary>
    /// Конструктор репозитория.
    /// </summary>
    public ProductRepo()
    {     
      this._list = new();      
    }    
    #endregion
  }
}