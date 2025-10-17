namespace FreshlyBackendNew.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }
        public List<string> Errors { get; }

        private Result(bool isSuccess, T value, string error, List<string> errors = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(string error) => new(false, default, error);
        public static Result<T> Failure(List<string> errors) => new(false, default, null, errors);
    }
}