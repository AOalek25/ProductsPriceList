namespace ProducstLibrary.Exceptions
{
  public class ValidationException : Exception
  {
    // Исключение "Невалидное поле".
    public ValidationException(string? message) : base(message)
    {
    }
  }
}
