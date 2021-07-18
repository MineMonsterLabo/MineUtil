using System;

namespace MineUtil
{
    public static class Functional
    {
        public static T Id<T>(T t)
        {
            return t;
        }

        public static Func<T2, T1, R> Flip<R, T1, T2>(this Func<T1, T2, R> func)
        {
            return (t2, t1) => func(t1, t2);
        }

        public static Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> func)
        {
            return (t2, t1) => func(t1, t2);
        }

        public static Func<T1, T3> Merge<T1, T2, T3>(this Func<T1, T2> func1, Func<T2, T3> func2)
        {
            return t1 => func2(func1(t1));
        }

        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> func)
        {
            return t1 => t2 => func(t1, t2);
        }

        public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> func)
        {
            return t1 => t2 => t3 => func(t1, t2, t3);
        }
    }
}