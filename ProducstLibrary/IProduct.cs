using ProducstLibrary.Validators;

namespace ProducstLibrary
{   
  public interface IProduct : IComparable
  {
    #region    
    Guid Id { init; get; }
    [NameValidator(2)]
    string Name { get; set; }
    [ManufacturerValidator(2)]
    string Manufacturer { get; set; }
    decimal Price { get; set; }
    #endregion

    #region
    string PrintInfo();

    string PrintPrice();
    #endregion
  }
}
