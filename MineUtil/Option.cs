using System;

namespace MineUtil
{
    public interface IOption<out T> : IOptionable<T>
    {
        bool IsNone { get; }
        bool IsSome { get; }
        T RawValue { get; }
    }

    public interface IOptionable<out T>
    {
        IOption<T> GetOption();
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

        public IOption<T> GetOption()
        {
            return this;
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
        public static bool IsNone<T>(this IOptionable<T> optionable) => optionable.GetOption().IsNone;

        public static bool IsSome<T>(this IOptionable<T> optionable) => optionable.GetOption().IsSome;

        public static T Unwrap<T>(this IOptionable<T> optionable)
        {
            if (optionable.IsNone())
            {
                throw new InvalidOperationException("Optionの中身がNoneの値をUnwrapしました");
            }
            return optionable.GetOption().RawValue;
        }

        public static T UnwrapOr<T>(this IOptionable<T> optionable, T defaultValue)
        {
            return optionable.IsNone() ? defaultValue : optionable.GetOption().RawValue;
        }

        public static IOptionable<T> Or<T>(this IOptionable<T> optionable, IOptionable<T> another)
        {
            return optionable.IsNone() ? another : optionable;
        }

        public static IOptionable<T> Filter<T>(this IOptionable<T> optionable, Func<T, bool> predicate)
        {
            return optionable.IsNone() ? optionable : (predicate(optionable.GetOption().RawValue) ? optionable : Option.None<T>());
        }

        public static void DoSome<T>(this IOptionable<T> optionable, Action<T> f)
        {
            if (optionable.IsSome())
            {
                f(optionable.GetOption().RawValue);
            }
        }

        public static void DoNone<T>(this IOptionable<T> optionable, Action f)
        {
            if (optionable.IsNone())
            {
                f();
            }
        }

        public static IOptionable<U> Bind<T, U>(this IOptionable<T> optionable, Func<T, IOptionable<U>> f)
        {
            return optionable.IsNone() ? Option.None<U>() : f(optionable.GetOption().RawValue);
        }

        public static IOptionable<U> Select<T, U>(this IOptionable<T> optionable, Func<T, U> f)
        {
            return optionable.IsNone() ? Option.None<U>() : Option.Some(f(optionable.GetOption().RawValue));
        }

        public static IOptionable<V> SelectMany<T, U, V>(
                      this IOptionable<T> optionable,
                      Func<T, IOptionable<U>> selector,
                      Func<T, U, V> projector)
        {
            return optionable.Bind(selector).Select(u => projector(optionable.GetOption().RawValue, u));
        }

        public static IOption<T> ToOption<T>(this T value)
        {
            return value == null ? Option.None<T>() : Option.Some(value);
        }
    }
}
