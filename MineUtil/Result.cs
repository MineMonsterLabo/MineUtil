using System;

namespace MineUtil
{
    public interface IResult<out T, out E> : IResultable<T, E>
    {
        bool IsError { get; }
        bool IsOk { get; }

        T RawValue { get; }
        E RawValueError { get; }
    }

    public interface IResultable<out T, out E>
    {
        IResult<T, E> GetResult();
    }

    internal class Result<T, E> : IResult<T, E>
    {
        public bool IsError { get; private set; } = true;
        public bool IsOk => !IsError;

        public T RawValue => value;
        public E RawValueError => error;

        private T value;
        private E error;

        public IResult<T, E> GetResult()
        {
            return this;
        }

        public static IResult<T, E> Error(E error)
        {
            var result = new Result<T, E>();
            result.error = error;
            return result;
        }

        public static IResult<T, E> Ok(T value)
        {
            var result = new Result<T, E>();
            result.value = value;
            result.IsError = false;
            return result;
        }
    }

    public static class Result
    {
        public static IResult<T, E> Error<T, E>(E error)
        => Result<T, E>.Error(error);

        public static IResult<T, E> Ok<T, E>(T value)
        => Result<T, E>.Ok(value);
    }

    public static class ResultExtentions
    {
        public static bool IsError<T, E>(this IResultable<T, E> resultable) => resultable.GetResult().IsError;

        public static bool IsOk<T, E>(this IResultable<T, E> resultable) => resultable.GetResult().IsOk;

        public static IResultable<U, E> Bind<T, U, E>(this IResultable<T, E> resultable, Func<T, IResultable<U, E>> f)
        {
            var result = resultable.GetResult();
            return result.IsError ? Result.Error<U, E>(result.RawValueError) : f(result.RawValue);
        }

        public static IResultable<U, E> Select<T, U, E>(this IResultable<T, E> resultable, Func<T, U> f)
        {
            var result = resultable.GetResult();
            return result.IsError ? Result.Error<U, E>(result.RawValueError) : Result.Ok<U, E>(f(result.RawValue));
        }

        public static IResultable<T, F> ErrorSelect<T, E, F>(this IResultable<T, E> resultable, Func<E, F> f)
        {
            var result = resultable.GetResult();
            return result.IsOk ? Result.Ok<T, F>(result.RawValue) : Result.Error<T, F>(f(result.RawValueError));
        }

        public static IResultable<V, E> SelectMany<T, U, V, E>(
                      this IResultable<T, E> resultable,
                      Func<T, IResultable<U, E>> selector,
                      Func<T, U, V> projector)
        {
            return resultable.Bind(selector).Select(u => projector(resultable.GetResult().RawValue, u));
        }

        public static T Unwrap<T, E>(this IResultable<T, E> resultable)
        {
            if (resultable.IsError())
            {
                throw new InvalidOperationException("Resultの中身がErrorの値をUnwrapしました");
            }
            return resultable.GetResult().RawValue;
        }

        public static E UnwrapError<T, E>(this IResultable<T, E> resultable)
        {
            if (resultable.IsOk())
            {
                throw new InvalidOperationException("Resultの中身がOkの値をUnwrapErrorしました");
            }
            return resultable.GetResult().RawValueError;
        }

        public static T UnwrapOr<T, E>(this IResultable<T, E> resultable, T defaultValue)
        {
            return resultable.IsError() ? defaultValue : resultable.GetResult().RawValue;
        }

        public static E UnwrapErrorOr<T, E>(this IResultable<T, E> resultable, E defaultError)
        {
            return resultable.IsOk() ? defaultError : resultable.GetResult().RawValueError;
        }

        public static IResultable<T, E> Or<T, E>(this IResultable<T, E> resultable, IResultable<T, E> another)
        {
            return resultable.IsError() ? another : resultable;
        }

        public static IOption<T> ChangeOption<T, E>(this IResultable<T, E> resultable)
        {
            return resultable.IsError() ? Option.None<T>() : resultable.GetResult().RawValue.ToOption();
        }

        public static void DoOk<T, E>(this IResultable<T, E> resultable, Action<T> f)
        {
            if (resultable.IsOk())
            {
                f(resultable.GetResult().RawValue);
            }
        }

        public static void DoError<T, E>(this IResultable<T, E> resultable, Action<E> f)
        {
            if (resultable.IsError())
            {
                f(resultable.GetResult().RawValueError);
            }
        }

        public static IResultable<U, E> CastOk<T, U, E>(this IResultable<T, E> resultable)
        {
            if (resultable.IsOk())
            {
                throw new InvalidOperationException("Resultの中身がOkの場合キャストできません");
            }
            return Result.Error<U, E>(resultable.GetResult().RawValueError);
        }

        public static IResultable<T, F> CastError<T, E, F>(this IResultable<T, E> resultable)
        {
            if (resultable.IsError())
            {
                throw new InvalidOperationException("Resultの中身がErrorの場合キャストできません");
            }
            return Result.Ok<T, F>(resultable.GetResult().RawValue);
        }
    }
}
