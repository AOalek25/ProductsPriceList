using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducstLibrary.Validators
{
    // Интерфейс для валидаторов.
    public interface IValidator
    {
        #region Методы.
        // Метод валидации, который должны реализовать все классы, реализующие интерфейс.
        public bool NotValid<T>(T item, out string errorMessage);
        #endregion
    }
}