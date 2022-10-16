using System.Reflection;

using ProducstLibrary.Exceptions;
using ProducstLibrary.Model;
using ProducstLibrary.Attributes;
using NaturalSort.Extension;

namespace ProducstLibrary
{
  public class ProductsRepo<T> where T: notnull, IProduct
  {
    #region Поля и свойства
    const string _alreadyExistMsg = "Такой продукт уже есть в списке";
    const string _notFoundMsg = "Продукт с таким именем и фамилией не найден";    
    private List<T> list;    
    private Dictionary<string, Type> dictTypes;    
    #endregion

    #region Методы
    public void Create(T newItem)
    {
      ProductsRepo<T>.Validate(newItem);
      
      foreach (IProduct item in list)       
        if (item.Equals(newItem)) throw new ProductAlreadyExistException(_alreadyExistMsg);
      list.Add(newItem);      
    }

    public T Read(string name, string manufacturer)
    {
      string itemToFind = $"{name} {manufacturer}";
      foreach (IProduct item in list)        
        if (string.Compare(item.ToString(), itemToFind, StringComparison.CurrentCultureIgnoreCase) == 0)
          return (T)item;
      throw new ProductNotFoundException(_notFoundMsg);
    }

    public void Update(T itemOldData, T itemNewData)
    {
      ProductsRepo<T>.Validate(itemNewData);
      foreach (IProduct item in list)
        if (item.Equals(itemNewData)) throw new ProductAlreadyExistException(_alreadyExistMsg);
      bool found=false;
      foreach (IProduct item in list)
        if (item.Equals(itemOldData)) found = true;
      if (!found) throw new ProductNotFoundException(_notFoundMsg);
      list.Remove(itemOldData);
      list.Add(itemNewData);      
    }

    public void Delete(string name, string manufacturer)
    {
      string itemToFind = $"{name} {manufacturer}";
      foreach (IProduct item in list)
        if (string.Compare(item.ToString(), itemToFind, StringComparison.CurrentCultureIgnoreCase) == 0)          
        {
          list.Remove((T)item);          
          return;
        }
      throw new ProductNotFoundException(_notFoundMsg);
    }
    // Метод, возвращающий все объекты из репозитория.
    public IEnumerable<T> GetAll() => this.list;
    private static void Validate(T item)
    {
      string errors = "";
      Type type = typeof(IProduct);
      object[] attributes = type.GetCustomAttributes(false);
      foreach (Attribute attr in attributes)
      {
        if ((attr is NameValidator nameValidation) && (nameValidation.NotValid(item, out string validationError)))
          errors += validationError;
        if ((attr is ManufacturerValidator manufacturerValidation) && (manufacturerValidation.NotValid(item, out string validateError)))
          errors += validateError;
      }        
      if (errors.Length > 0)
        throw new ValidationException($"{item} не валиден: {errors}");     
    }
    private void RegisterTypes()
    {
      Assembly assembly = Assembly.GetAssembly(typeof(T));
      Type[] alltypes = assembly.GetTypes();
      foreach (Type type in alltypes)
        if ((type.Namespace == "ProducstLibrary.Model") && (type.IsClass) && (!type.IsAbstract))
        {
          var obj = Activator.CreateInstance(type);
          string typeName = (string)type.GetMethod("PrintType").Invoke(obj, null);
          dictTypes.Add(typeName, type);
        }      
    }
    public Dictionary<string, Type> GetRegisteredTypes() => this.dictTypes;

    public void Clear() => this.list.Clear();

    public IEnumerable<T> SortedByName()
    {
      var sortedList = list.OrderBy(x => x.Name, StringComparison.OrdinalIgnoreCase.WithNaturalSort());
      this.list = sortedList.ToList();
      return sortedList;      
    }
    public IEnumerable<T> SortedtByPrice()
    {
      var sortedList = list.OrderBy(x => x.Price);
      this.list = sortedList.ToList();
      return sortedList;
    }

    #endregion


    #region Конструкторы
    public ProductsRepo()
    {     
      this.list = new();
      this.dictTypes = new();
      this.RegisterTypes();
    }    
    #endregion
  }
}