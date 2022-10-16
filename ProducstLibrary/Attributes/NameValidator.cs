using System.Text.RegularExpressions;

using ProducstLibrary.Model;

namespace ProducstLibrary.Attributes
{
  // Валидатор продукта по названию - должно быть более двух букв.
  [AttributeUsage(AttributeTargets.Property)]
  public class NameValidator : Attribute
  {
    const int DefaultMinLenght = 2;
    public int minLenght;    
    #region Методы.
    // Метод, валидирующий продукт по названию - должно быть более двух букв.
    public bool NotValid<T>(T item, out string errorMessage)
    {
      errorMessage = $"Название менее двух символов или содержит пробелы, цифры, спецсимволы.";
      Regex regex = new(@"[^a-zA-Zа-яА-Я1-9]{minLenght,}");
      if (item is IProduct product) return (!regex.IsMatch(product.Name));
      return true;
    }
    #endregion

    #region Конструкторы.    
    public NameValidator(int MinLenght= DefaultMinLenght) => this.minLenght = MinLenght;
    #endregion
  }
}
