using System;
using System.Collections.Generic;

namespace MineUtil
{
    public static class EnumerableExtentions
    {
        /// <summary>
        /// 無限リストの生成を行う
        /// </summary>
        public static IEnumerable<T> Infinity<T>(this T t)
        {
            while (true) yield return t;
        }

        /// <summary>
        /// 途中で止めることのできるAggregate
        /// </summary>
        public static ACC AggregateIf<T, ACC>
            (this IEnumerable<T> list, ACC seed, Func<ACC, T, (ACC, bool)> func)
        {

            var acc = seed;

            foreach (var t in list)
            {
                var (nextAcc, continueFlag) = func(acc, t);
                if (continueFlag == false)
                {
                    return nextAcc;
                }

                acc = nextAcc;
            }

            return acc;
        }
    }
}
