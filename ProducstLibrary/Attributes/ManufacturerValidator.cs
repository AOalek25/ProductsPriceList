using System.Text.RegularExpressions;
using ProductLibrary.Model;

// Валидатор продукта по названию - должно быть более двух букв.
namespace ProductLibrary.Attributes
{
  [AttributeUsage(AttributeTargets.All)]
  public class ManufacturerValidator : Attribute
  {
    const int DefaultMinLenght = 2;
    public int minLenght;
    #region Методы.
    // Метод, валидирующий наименование производителя - должно быть более двух букв.
    public bool NotValid<T>(T item, out string errorMessage)
    {
      errorMessage = $"Наименование производителя менее двух символов или содержит пробелы, спецсимволы.";
      Regex regex = new(@"[^a-zA-Zа-яА-Я1-9]");
      if (item is Product product)
        if (product.Manufacturer.Length >= minLenght) return (regex.IsMatch(product.Manufacturer));
        else return true;
      else return true;
    }
    #endregion

    #region Конструкторы.    
    public ManufacturerValidator(int MinLenght = DefaultMinLenght) => this.minLenght = MinLenght;
    #endregion
  }
}

