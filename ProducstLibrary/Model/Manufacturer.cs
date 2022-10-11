using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducstLibrary.Model
{  
  internal class Manufacturer
  {
    #region Поля и свойства
    public string Name { get; set; }
    public string Adresse { get; set; }
    public int Id { get; set; }
    #endregion

    #region Методы
    public override bool Equals(object? obj)
    {
      return obj is Manufacturer manufacturer &&
             Name == manufacturer.Name &&
             Adresse == manufacturer.Adresse &&
             Id == manufacturer.Id;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(Name, Adresse, Id);
    }
    #endregion

    #region Конструкторы
    public Manufacturer(string name, string adresse, int id)
    {
      this.Name = name;
      this.Adresse = adresse;
      this.Id = id;
    }
    #endregion
  }
}
