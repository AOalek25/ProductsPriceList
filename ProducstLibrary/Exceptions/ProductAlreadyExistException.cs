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
