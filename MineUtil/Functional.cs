using System;

namespace MineUtil
{
    public static class Functional
    {
        static public T Id<T>(T t) => t;

        static public Func<T2, T1, R> Flip<R, T1, T2>(this Func<T1, T2, R> func)
            => (t2, t1) => func(t1, t2);

        static public Action<T2, T1> Flip<T1, T2>(this Action<T1, T2> func)
            => (t2, t1) => func(t1, t2);

        static public Func<T1, T3> Merge<T1, T2, T3>(this Func<T1, T2> func1, Func<T2, T3> func2)
            => t1 => func2(func1(t1));

        static public Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> func)
            => t1 => t2 => func(t1, t2);

        static public Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> func)
            => t1 => t2 => t3 => func(t1, t2, t3);
    }
}
