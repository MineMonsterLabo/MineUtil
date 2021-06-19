using System;

namespace MineUtil
{
    public interface IResult<out T, out E>
    {
        bool IsError { get; }
        bool IsOk { get; }

        T RawValue { get; }
        E RawError { get; }
    }

    internal class Result<T, E> : IResult<T, E>
    {
        public bool IsError { get; private set; } = true;
        public bool IsOk => !IsError;

        public T RawValue => value;
        public E RawError => error;

        private T value;
        private E error;

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
        public static IResult<U, E> Bind<T, U, E>(this IResult<T, E> result, Func<T, IResult<U, E>> f)
        {
            return result.IsError ? Result.Error<U, E>(result.RawError) : f(result.RawValue);
        }

        public static IResult<U, E> Select<T, U, E>(this IResult<T, E> result, Func<T, U> f)
        {
            return result.IsError ? Result.Error<U, E>(result.RawError) : Result.Ok<U, E>(f(result.RawValue));
        }

        public static IResult<T, F> ErrorSelect<T, E, F>(this IResult<T, E> result, Func<E, F> f)
        {
            return result.IsOk ? Result.Ok<T, F>(result.RawValue) : Result.Error<T, F>(f(result.RawError));
        }

        public static IResult<V, E> SelectMany<T, U, V, E>(
                      this IResult<T, E> result,
                      Func<T, IResult<U, E>> selector,
                      Func<T, U, V> projector)
        {
            return result.Bind(selector).Select(projector.Curry()(result.RawValue));
        }

        public static T Unwrap<T, E>(this IResult<T, E> result)
        {
            if (result.IsError)
            {
                throw new InvalidOperationException("Resultの中身がErrorの値をUnwrapしました");
            }
            return result.RawValue;
        }

        public static E UnwrapError<T, E>(this IResult<T, E> result)
        {
            if (result.IsOk)
            {
                throw new InvalidOperationException("Resultの中身がOkの値をUnwrapErrorしました");
            }
            return result.RawError;
        }

        public static T UnwrapOr<T, E>(this IResult<T, E> result, T defaultValue)
        {
            return result.IsError ? defaultValue : result.RawValue;
        }

        public static E UnwrapErrorOr<T, E>(this IResult<T, E> result, E defaultError)
        {
            return result.IsOk ? defaultError : result.RawError;
        }

        public static IResult<T, E> Or<T, E>(this IResult<T, E> result, IResult<T, E> another)
        {
            return result.IsError ? another : result;
        }

        public static IResult<T, E> Flatten<T, E>(this IResult<IResult<T, E>,E> result)
        {
            return result.Bind(Functional.Id);
        }

        public static IOption<T> ChangeOption<T, E>(this IResult<T, E> result)
        {
            return result.IsError ? Option.None<T>() : result.RawValue.ToOption();
        }

        public static void DoOk<T, E>(this IResult<T, E> result, Action<T> f)
        {
            if (result.IsOk)
            {
                f(result.RawValue);
            }
        }

        public static void DoError<T, E>(this IResult<T, E> result, Action<E> f)
        {
            if (result.IsError)
            {
                f(result.RawError);
            }
        }

        public static IResult<T, F> CastError<T, E, F>(this IResult<T, E> result)
        {
            if (result.IsError)
            {
                throw new InvalidOperationException("Resultの中身がErrorの場合キャストできません");
            }
            return Result.Ok<T, F>(result.RawValue);
        }
    }
}
