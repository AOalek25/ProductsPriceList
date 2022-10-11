using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducstLibrary.Model
{
  public class Milk : Product
  {
    #region Поля и свойства
    public int FatContent { get; set; }
    private string name;
    public override string Name { get => $"{this.name} {this.FatContent}"; }
    #endregion

    #region
    public Milk(string name, int fatContent, string manufacturer, decimal price) : base(name, manufacturer, price)
    {
      this.FatContent = fatContent;
    }
    #endregion
  }
}
