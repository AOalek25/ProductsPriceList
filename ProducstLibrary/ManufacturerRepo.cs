using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ProductLibrary.Model;

namespace ProductLibrary
{
  public class ManufacturerRepo 
  {
    private List<Manufacturer> _list = new();
    public void Create(Manufacturer manufacturer)
    {
      _list.Add(manufacturer);
    }

    public void Delete(Manufacturer manufacturer)
    {
      _list.Remove(manufacturer);
    }

    public void Update(Manufacturer manufacturerOld, Manufacturer manufacturerNew)
    {
      _list.Add(manufacturerNew);
      _list.Remove(manufacturerOld);
    }
  }
}
