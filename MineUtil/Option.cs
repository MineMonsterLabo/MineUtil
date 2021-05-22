using System;

namespace MineUtil
{
    public class Option<T>
    {
        public bool IsNone { get; private set; } = true;
        public bool IsSome => !IsNone;
        internal T RawValue => value;

        private T value;

        public Option() { }

        public Option(T value)
        {
            this.value = value;
            IsNone = false;
        }

        public T Unwrap()
        {
            if (IsNone)
            {
                throw new InvalidOperationException("Optionの中身がNoneの値をUnwrapしました");
            }
            return value;
        }

        public T UnwrapOr(T defaultValue)
        {
            return IsNone ? defaultValue : value;
        }

        public Option<T> Or(Option<T> another)
        {
            return IsNone ? another : this;
        }

        public Option<T> Filter(Func<T, bool> predicate)
        {
            return IsNone ? this : (predicate(value) ? this : new Option<T>());
        }

        public void DoSome(Action<T> f)
        {
            if (IsSome)
            {
                f(value);
            }
        }

        public void DoNone(Action f)
        {
            if (IsNone)
            {
                f();
            }
        }

    }

    public static class OptionExtentions
    {
        public static Option<U> Bind<T,U>(this Option<T> option, Func<T, Option<U>> f)
        {
            return option.IsNone ? new Option<U>() : f(option.RawValue);
        }

        public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> f)
        {
            return option.IsNone ? new Option<U>() : new Option<U>(f(option.RawValue));
        }

        public static Option<V> SelectMany<T, U, V>(
                      this Option<T> option,
                      Func<T, Option<U>> selector,
                      Func<T, U, V> projector)
        {
            return option.Bind(selector).Select(u => projector(option.RawValue, u));
        }

        public static Option<T> ToOption<T>(this T value)
        {
            return value == null ? new Option<T>() : new Option<T>(value);
        }
    }
}
