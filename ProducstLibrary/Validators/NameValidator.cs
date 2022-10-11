using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ProducstLibrary.Model;

namespace ProducstLibrary.Validators
{
  // Валидатор продукта по названию - должно быть более двух букв.
  public class NameValidator : IValidator
  {
    #region Методы.
    // Метод, валидирующий продукт по названию - должно быть более двух букв.
    public bool NotValid<T>(T item, out string errorMessage)
    {
      errorMessage = $"Название менее двух символов или содержит пробелы, цифры, спецсимволы.";
      Regex regex = new(@"[^a-zA-Zа-яА-Я1-9]{2,}");
      if (item is Product product) return (regex.IsMatch(product.Name));
      return false;
    }
    #endregion
  }
}
