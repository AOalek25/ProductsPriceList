using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducstLibrary.Exceptions
{
  public class ProductAlreadyExistException : Exception
  {
    // Исключение "продукт уже существует".
    public ProductAlreadyExistException(string? message) : base(message)
    {
    }
  }
}
