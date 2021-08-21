using System;
using System.Collections.Generic;

namespace MineUtil.Algorithms
{
    public class TailRecursive<T>
    {
        public IEnumerable<T> Enumerable { get; }
        public TailRecursiveType Type { get; }

        public TailRecursive(IEnumerable<T> enumerable, TailRecursiveType type = TailRecursiveType.Queue)
        {
            Enumerable = enumerable;
            Type = type;
        }

        public void Execute(Action<T> nodeAction, Func<T, IEnumerable<T>> nodesSelectorFunc)
        {
            if (Type == TailRecursiveType.Queue)
                TailRecursiveQueue(nodeAction, nodesSelectorFunc);
            else if (Type == TailRecursiveType.Stack)
                TailRecursiveStack(nodeAction, nodesSelectorFunc);
            else
                throw new NotSupportedException(Type.ToString());
        }

        private void TailRecursiveQueue(Action<T> nodeAction, Func<T, IEnumerable<T>> nodesSelectorFunc)
        {
            var queue = new Queue<T>(Enumerable);
            while (queue.Count > 0)
            {
                T dequeue = queue.Dequeue();
                nodeAction?.Invoke(dequeue);
                if (dequeue != null)
                {
                    var enumerable = nodesSelectorFunc(dequeue);
                    foreach (var node in enumerable)
                    {
                        queue.Enqueue(node);
                    }
                }
            }
        }

        private void TailRecursiveStack(Action<T> nodeAction, Func<T, IEnumerable<T>> nodesSelectorFunc)
        {
            var queue = new Stack<T>(Enumerable);
            while (queue.Count > 0)
            {
                T dequeue = queue.Pop();
                nodeAction?.Invoke(dequeue);
                if (dequeue != null)
                {
                    var enumerable = nodesSelectorFunc(dequeue);
                    foreach (var node in enumerable)
                    {
                        queue.Push(node);
                    }
                }
            }
        }
    }

    public enum TailRecursiveType
    {
        Queue,
        Stack
    }
}