using System;

namespace MineUtil
{
    public class Result<T,E>
    {
        public bool IsError { get; private set; } = true;
        public bool IsOk=> !IsError;

        internal T RawValue => value;
        internal E RawValueError => error;

        private T value;
        private E error;

        public static Result<T,E> Error(E error)
        {
            var result = new Result<T,E>();
            result.error = error;
            return result;
        }

        public static Result<T, E> Ok(T value)
        {
            var result = new Result<T, E>();
            result.value= value;
            result.IsError = false;
            return result;
        }

        public T Unwrap()
        {
            if (IsError)
            {
                throw new InvalidOperationException("Resultの中身がErrorの値をUnwrapしました");
            }
            return value;
        }

        public E UnwrapError()
        {
            if (IsOk)
            {
                throw new InvalidOperationException("Resultの中身がOkの値をUnwrapErrorしました");
            }
            return error;
        }

        public T UnwrapOr(T defaultValue)
        {
            return IsError ? defaultValue : value;
        }

        public E UnwrapErrorOr(E defaultError)
        {
            return IsOk ? defaultError : error;
        }

        public Result<T,E> Or(Result<T,E> another)
        {
            return IsError ? another : this;
        }

        public Option<T> ChangeOption()
        {
            return IsError ? new Option<T>() : value.ToOption();
        }

        public void DoOk(Action<T> f)
        {
            if (IsOk)
            {
                f(value);
            }
        }

        public void DoError(Action<E> f)
        {
            if (IsError)
            {
                f(error);
            }
        }
    }

    public static class ResultExtentions
    {
        public static Result<U, E> Bind<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> f)
        {
            return result.IsError ? Result<U, E>.Error(result.RawValueError) : f(result.RawValue);
        }

        public static Result<U,E> Select<T, U,E>(this Result<T,E> result, Func<T, U> f)
        {
            return result.IsError ? Result<U,E>.Error(result.RawValueError) : Result<U,E>.Ok(f(result.RawValue));
        }

        public static Result<T, F> ErrorSelect<T, E, F>(this Result<T, E> result, Func<E, F> f)
        {
            return result.IsOk ? Result<T, F>.Ok(result.RawValue) : Result<T, F>.Error(f(result.RawValueError));
        }

        public static Result<V,E> SelectMany<T, U, V,E>(
                      this Result<T,E> result,
                      Func<T, Result<U,E>> selector,
                      Func<T, U, V> projector)
        {
            return result.Bind(selector).Select(u => projector(result.RawValue, u));
        }
    }
}
