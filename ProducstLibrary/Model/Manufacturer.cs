using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentNHibernate.Mapping;

using ProductLibrary.Attributes;

namespace ProductLibrary.Model
{
  [NameValidator(2)]
  public class Manufacturer
  {
    public Manufacturer(string name, string adressee)
    {
      this.Name = name;
      this.Adressee = adressee;
    }
    public Manufacturer()
    {
      this.Name = "";
      this.Adressee = "";
    }

    public virtual Guid Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Adressee { get; set; }   
  }
}
