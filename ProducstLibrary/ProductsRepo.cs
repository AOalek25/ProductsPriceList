using ProductLibrary.Exceptions;
using ProductLibrary.Model;
using ProductLibrary.Attributes;
using NaturalSort.Extension;
using System.Text;

namespace ProductLibrary
{
  public class ProductRepo
  {
    #region Поля и свойства    
    private List<Product> list;          
    #endregion

    #region Методы
    public void Create(Product newItem)
    {
      Validate(newItem);
      foreach (Product item in list)
        if (item.Equals(newItem)) throw new ProductAlreadyExistException();
      list.Add(newItem);
      
    }

    public Product Read(string name, string manufacturer)
    {
      string itemToFind = $"{name} {manufacturer}";
      foreach (Product item in list)        
        if (string.Compare(item.ToString(), itemToFind, StringComparison.CurrentCultureIgnoreCase) == 0) return item;
      throw new ProductNotFoundException();
    }

    public void Update(Product itemOldData, Product itemNewData)
    {
      Validate(itemNewData);
      foreach (Product item in list)
        if (item.Equals(itemNewData)) throw new ProductAlreadyExistException();
      Read(itemOldData.Name, itemOldData.Manufacturer);
      list.Remove(itemOldData);
      list.Add(itemNewData);      
    }

    public void Delete(string name, string manufacturer)
    {
      if (Read(name, manufacturer) is Product product)
        list.Remove(product);     
      else throw new ProductNotFoundException();
        
    }
    // Метод, возвращающий все объекты из репозитория.
    public IEnumerable<Product> GetAll() => this.list;
    private static void Validate(Product item)
    {
      StringBuilder stringBuilder = new();      
      Type type = typeof(Product);
      object[] attributes = type.GetCustomAttributes(false);
      foreach (Attribute attr in attributes)
      {
        if ((attr is NameValidator nameValidation) && (nameValidation.NotValid(item, out string validationError)))
          stringBuilder.Append(validationError + Environment.NewLine);
        if ((attr is ManufacturerValidator manufacturerValidation) && (manufacturerValidation.NotValid(item, out string validateError)))
          stringBuilder.Append(validateError + Environment.NewLine);
      }        
      if (stringBuilder.ToString().Length > 0)
        throw new ValidationException(stringBuilder.ToString());     
    }   

    public void Clear() => this.list.Clear();

    public IEnumerable<Product> SortedByName()
    {
      var sortedList = this.list.OrderBy(product => product.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort());
      this.list = sortedList.ToList();
      return sortedList;      
    }
    public IEnumerable<Product> SortedtByPrice()
    {
      var sortedList = list.OrderBy(product => product.Price);
      this.list = sortedList.ToList();
      return sortedList;
    }

    #endregion


    #region Конструкторы
    public ProductRepo()
    {     
      this.list = new();      
    }    
    #endregion
  }
}