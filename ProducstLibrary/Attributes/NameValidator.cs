using System.Text.RegularExpressions;
using ProductLibrary.Model;

// Валидатор продукта по названию - должно быть более двух букв.
namespace ProductLibrary.Attributes
{  
  [AttributeUsage(AttributeTargets.All)]
  public class NameValidator : Attribute
  {
    const int defaultMinLenght = 2;
    public int minLenght;    
    #region Методы.
    // Метод, валидирующий продукт по названию - должно быть более двух букв.
    public bool NotValid<T>(T item, out string errorMessage)
    {
      errorMessage = $"Название продукта менее двух символов или содержит пробелы, спецсимволы.";
      Regex regex = new(@"[^a-zA-Zа-яА-Я1-9 ]");
      if (item is Product product)
        if (product.Name.Length >= minLenght) return (regex.IsMatch(product.Name));
        else return true;
      else return true;
    }
    #endregion

    #region Конструкторы.    
    public NameValidator(int minLenght= defaultMinLenght) => this.minLenght = minLenght;
    #endregion
  }
}
