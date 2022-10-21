namespace ProductLibrary.Exceptions
{
  // Исключение "Невалидное поле".
  public class ValidationException : Exception
  {
    public ValidationException(string? message) : base(message)
    {
    }
  }
}
