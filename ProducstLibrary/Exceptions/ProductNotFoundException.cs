namespace ProducstLibrary.Exceptions
{
  public class ProductNotFoundException : Exception
  {
    // Исключение "Продукт не найден".
    public ProductNotFoundException(string? message) : base(message)
    {
    }
  }
}
