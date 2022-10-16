using System.Text.RegularExpressions;
using ProducstLibrary.Model;

// Валидатор продукта по названию - должно быть более двух букв.
namespace ProducstLibrary.Attributes
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ManufacturerValidator : Attribute
  {
    public int minLenght;
    #region Методы.
    // Метод, валидирующий продукт по названию - должно быть более двух букв.
    public bool NotValid<T>(T item, out string errorMessage)
    {
      errorMessage = $"Название менее двух символов или содержит пробелы, спецсимволы.";
      Regex regex = new(@"[^a-zA-Zа-яА-Я1-9]{minLenght,}");
      if (item is IProduct product) return (!regex.IsMatch(product.Manufacturer));
      return true;
    }
    #endregion

    #region Конструкторы.
    public ManufacturerValidator() { }
    public ManufacturerValidator(int MinLenght) => this.minLenght = MinLenght;
    #endregion
  }
}

