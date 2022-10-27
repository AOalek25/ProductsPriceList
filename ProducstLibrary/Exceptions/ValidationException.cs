namespace ProductLibrary.Exceptions
{
  // Исключение "Невалидное значение".
  public class ValidationException : Exception
  {
    public ValidationException(string? message) : base(message)
    {     }
  }
}
