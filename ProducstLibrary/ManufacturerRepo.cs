using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProductLibrary.Attributes;
using ProductLibrary.Exceptions;
using ProductLibrary.Model;

namespace ProductLibrary
{
  public class ManufacturerRepo
  {
    private List<Manufacturer> _list = new();
    public void Create(Manufacturer manufacturer)
    {
      Validate(manufacturer);
      _list.Add(manufacturer);
    }

    public Manufacturer Read (string name)
    {     
      foreach (var manufacturer in _list)
      {
        if (manufacturer.Name == name) return manufacturer;
      }
      throw new Exception("Производитель с таким именем не найден.");
    }

    public void Delete(Manufacturer manufacturer)
    {      
      _list.Remove(manufacturer);
    }

    public void Clear()
    {
      _list.Clear();
    }
    public IEnumerable<Manufacturer> AddRange(IEnumerable<Manufacturer> items)
    {
      var distinctedList = items.Distinct();
      foreach (Manufacturer manufacturer in distinctedList) Validate(manufacturer);
      this._list.AddRange(distinctedList);
      return _list;
    }

    private static void Validate(Manufacturer item)
    {
      StringBuilder stringBuilder = new();
      Type type = typeof(Manufacturer);
      object[] attributes = type.GetCustomAttributes(false);
      foreach (Attribute attr in attributes)
      {
        if (attr is NameValidator nameValidation)
          if (nameValidation.NotValid(item, out string validationError)) stringBuilder.Append(validationError + Environment.NewLine);
      }
      if (stringBuilder.ToString().Length > 0)
        throw new ValidationException(stringBuilder.ToString());
    }

    public IEnumerable<Manufacturer> GetAll() => this._list;

  }
}
