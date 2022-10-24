using ProducstLibrary.Attributes;

namespace ProducstLibrary.Model
{   
  public interface IProduct : IComparable
  {
    #region    
    Guid Id { init; get; }
    string TypeName { get; init; }
    [NameValidator(2)]
    string Name { get; set; }
    [ManufacturerValidator(2)]
    string Manufacturer { get; set; }
    decimal Price { get; set; }
    #endregion

    #region
    string PrintInfo();
    string PrintId();
    string PrintType();
    string PrintPrice();    
    #endregion
  }
}
