using System.Text.RegularExpressions;
using ProductLibrary.Model;

// Валидатор наименования производителя - должно быть более двух букв.
namespace ProductLibrary.Attributes
{
  [AttributeUsage(AttributeTargets.All)]
  public class ManufacturerValidator : Attribute
  {
    /// <summary>
    /// Минимальная длина наименования производителя по умолчанию.
    /// </summary>
    const int defaultMinLenght = 2;
    /// <summary>
    /// Минимальная длина наименования производителя.
    /// </summary>
    public int minLenght;
    #region Методы.
    /// <summary>
    /// Метод, валидирующий наименование производителя - должно быть более двух букв.
    /// </summary>
    /// <typeparam name="T"> Тип передаваемого объекта для валидации. </typeparam>
    /// <param name="item"> Передаваемый объект для валидации. </param>
    /// <param name="errorMessage"> Сообщение валидации, если item невалиден. </param>
    /// <returns> Возвращает true, если объект невалиден, и false если валиден. </returns>
    public bool NotValid<T>(T item, out string errorMessage)
    {
      errorMessage = $"Наименование производителя менее {minLenght} символов или содержит пробелы, спецсимволы.";
      Regex regex = new(@"[^a-zA-Zа-яА-Я1-9 ]");
      if (item is Product product)
        if (product.Manufacturer.Length >= minLenght) return (regex.IsMatch(product.Manufacturer));
        else return true;
      else return true;
    }
    #endregion

    #region Конструкторы.  
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="minLenght"> Аргумент, задающий минимальную длину наименования производителя. </param>
    public ManufacturerValidator(int minLenght = defaultMinLenght) => this.minLenght = minLenght;
    #endregion
  }
}

