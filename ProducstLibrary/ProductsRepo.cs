using System.Collections;
using ProducstLibrary.Exceptions;
using ProducstLibrary.Validators;

namespace ProducstLibrary
{
  public class ProductsRepo<T> where T: notnull, IProduct
  {
    #region Поля и свойства
    public const string _alreadyExistMsg = "Такой продукт уже есть в списке";
    public const string _notFoundMsg = "Продукт с таким именем и фамилией не найден";    
    private List<T> list;    
    #endregion

    #region Методы
    public void Create(T newItem)
    {
      Validate(newItem);
      foreach (var item in list)
        if (item.Equals(newItem)) throw new ProductAlreadyExistException(_alreadyExistMsg);
      list.Add(newItem);      
    }

    public T Read(string name, string manufacturer)
    {      
      foreach (var item in list)
        if (item.ToString().Equals($"{name} {manufacturer}", StringComparison.CurrentCultureIgnoreCase)) return item;
      throw new ProductNotFoundException(_notFoundMsg);
    }

    public void Update(T itemOldData, T itemNewData)
    {
      Validate(itemNewData);
      foreach (var item in list)
        if (item.Equals(itemNewData)) throw new ProductAlreadyExistException(_alreadyExistMsg);
      bool found=false;
      foreach (var item in list)
        if (item.Equals(itemOldData)) found = true;
      if (!found) throw new ProductNotFoundException(_notFoundMsg);
      list.Remove(itemOldData);
      list.Add(itemNewData);      
    }

    public void Delete(string name, string manufacturer)
    {
      foreach (var item in list)
        if (item.ToString().Equals($"{name} {manufacturer}", StringComparison.CurrentCultureIgnoreCase))
        {
          list.Remove(item);          
          return;
        }
      throw new ProductNotFoundException(_notFoundMsg);
    }
    // Метод, возвращающий все объекты из репозитория.
    public IEnumerable<T> GetAll() => this.list;
    private void Validate(T item)
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
    #endregion


    #region Конструкторы
    public ProductsRepo()
    {     
      this.list = new();          
    }
    #endregion      
  }
}