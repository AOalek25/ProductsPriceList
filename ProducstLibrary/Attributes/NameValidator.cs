using System.Text.RegularExpressions;
using ProductLibrary.Model;

// Валидатор продукта по названию - должно быть более двух букв.
namespace ProductLibrary.Attributes
{  
  [AttributeUsage(AttributeTargets.All)]
  public class NameValidator : Attribute
  {
    /// <summary>
    /// Минимальная длина наименования продукта по умолчанию.
    /// </summary>
    const int defaultMinLenght = 2;
    /// <summary>
    /// Минимальная длина наименования продукта.
    /// </summary>
    public int minLenght;
    #region Методы.
    /// <summary>
    /// Метод, валидирующий продукт по названию - должно быть более двух букв.
    /// </summary>
    /// <typeparam name="T"> Тип передаваемого объекта для валидации. </typeparam>
    /// <param name="item"> Передаваемый объект для валидации. </param>
    /// <param name="errorMessage"> Сообщение валидации, если объект не валиден. </param>
    /// <returns> Возвращает true, если объект невалиден, и false если валиден. </returns>
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
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="minLenght"> Аргумент, задающий минимальную длину наименования продукта. </param>
    public NameValidator(int minLenght= defaultMinLenght) => this.minLenght = minLenght;
    #endregion
  }
}
