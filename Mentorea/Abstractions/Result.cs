namespace Mentorea.Abstractions
{
    public class Result
    {

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; } = default!;

        public Result(bool IsSuccess, Error Error)
        {
            if ((IsSuccess && Error != Error.None) || (!IsSuccess && Error == Error.None))
            {
                throw new InvalidOperationException();
            }

            this.IsSuccess = IsSuccess;
            this.Error = Error;
        }

        public static Result Success()
        {
            return new(true, Error.None);
        }
        public static Result Failure(Error error)
        {
            return new(false, error);
        }
        public static Result<TValue> Success<TValue>(TValue value)
        {
            return new(value, true, Error.None);
        }
        public static Result<TValue> Failure<TValue>(Error error)
        {
            return new(default, false, error);
        }
    }
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        public Result(TValue? value, bool IsSuccess, Error Error) : base(IsSuccess, Error)
        {
            this._value = value;
        }

        public TValue Value()
        {
            return IsSuccess ? _value! : throw new InvalidOperationException("Failure Results Cannot Have Value");
        }
    }
}
