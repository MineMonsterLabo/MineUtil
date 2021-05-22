using System;

namespace MineUtil
{
    public interface IOption<out T>
    {
        bool IsNone { get; }
        bool IsSome { get; }
        T RawValue { get; }
    }

    internal class Option<T> : IOption<T>
    {
        public bool IsNone { get; private set; } = true;
        public bool IsSome => !IsNone;
        public T RawValue => value;

        private T value;

        public Option() { }

        public Option(T value)
        {
            this.value = value;
            IsNone = false;
        }
    }

    public static class Option
    {
        public static IOption<T> None<T>()
            => new Option<T>();

        public static IOption<T> Some<T>(T value)
            => new Option<T>(value);
    }

    public static class OptionExtentions
    {

        public static T Unwrap<T>(this IOption<T> option)
        {
            if (option.IsNone)
            {
                throw new InvalidOperationException("Optionの中身がNoneの値をUnwrapしました");
            }
            return option.RawValue;
        }

        public static T UnwrapOr<T>(this IOption<T> option, T defaultValue)
        {
            return option.IsNone ? defaultValue : option.RawValue;
        }

        public static IOption<T> Or<T>(this IOption<T> option, IOption<T> another)
        {
            return option.IsNone ? another : option;
        }

        public static IOption<T> Filter<T>(this IOption<T> option, Func<T, bool> predicate)
        {
            return option.IsNone ? option : (predicate(option.RawValue) ? option : Option.None<T>());
        }

        public static void DoSome<T>(this IOption<T> option, Action<T> f)
        {
            if (option.IsSome)
            {
                f(option.RawValue);
            }
        }

        public static void DoNone<T>(this IOption<T> option, Action f)
        {
            if (option.IsNone)
            {
                f();
            }
        }

        public static IOption<U> Bind<T, U>(this IOption<T> option, Func<T, IOption<U>> f)
        {
            return option.IsNone ? Option.None<U>() : f(option.RawValue);
        }

        public static IOption<U> Select<T, U>(this IOption<T> option, Func<T, U> f)
        {
            return option.IsNone ? Option.None<U>() : Option.Some(f(option.RawValue));
        }

        public static IOption<V> SelectMany<T, U, V>(
                      this IOption<T> option,
                      Func<T, IOption<U>> selector,
                      Func<T, U, V> projector)
        {
            return option.Bind(selector).Select(u => projector(option.RawValue, u));
        }

        public static IOption<T> ToOption<T>(this T value)
        {
            return value == null ? Option.None<T>() : Option.Some(value);
        }
    }
}
