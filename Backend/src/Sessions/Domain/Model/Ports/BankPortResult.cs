using OpenDDD.Domain.Model;

namespace Sessions.Domain.Model.Ports
{
    public class BankPortResult<T> : IValueObject
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public T? Value { get; private set; }

        private BankPortResult(bool success, string? errorMessage, T? value)
        {
            Success = success;
            ErrorMessage = errorMessage;
            Value = value;
        }

        public static BankPortResult<T> Create(bool success, string? errorMessage, T? value)
        {
            return new BankPortResult<T>(success, errorMessage, value);
        }

        public static BankPortResult<T> Ok(T value)
        {
            return Create(true, null, value);
        }

        public static BankPortResult<T> Fail(string errorMessage)
        {
            return Create(false, errorMessage, default);
        }
    }
}
