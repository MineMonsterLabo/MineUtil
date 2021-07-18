using System;
using System.Collections.Generic;
using System.Linq;

namespace MineUtil.Algorithms
{
    public class TailRecursive<T>
    {
        private readonly Func<T, bool> _filterFunc;
        private readonly Func<T, IEnumerable<T>> _nodesSelectorFunc;

        public IEnumerable<T> Enumerable { get; }
        public TailRecursiveType Type { get; }

        public TailRecursive(IEnumerable<T> enumerable, Func<T, bool> filterFunc,
            Func<T, IEnumerable<T>> nodesSelectorFunc, TailRecursiveType type = TailRecursiveType.Queue)
        {
            Enumerable = enumerable;
            Type = type;

            _filterFunc = filterFunc;
            _nodesSelectorFunc = nodesSelectorFunc;
        }

        public void Execute(Action<T> nodeAction)
        {
            if (Type == TailRecursiveType.Queue)
            {
                TailRecursiveQueue(nodeAction);
            }
            else if (Type == TailRecursiveType.Stack)
            {
                TailRecursiveStack(nodeAction);
            }
            else
            {
                throw new NotSupportedException(Type.ToString());
            }
        }

        private void TailRecursiveQueue(Action<T> nodeAction)
        {
            Queue<T> queue = new Queue<T>(Enumerable.Where(_filterFunc));
            while (queue.Count > 0)
            {
                T dequeue = queue.Dequeue();
                nodeAction?.Invoke(dequeue);
                if (dequeue != null)
                {
                    var enumerable = _nodesSelectorFunc(dequeue);
                    foreach (var node in enumerable)
                    {
                        queue.Enqueue(node);
                    }
                }
            }
        }

        private void TailRecursiveStack(Action<T> nodeAction)
        {
            Stack<T> queue = new Stack<T>(Enumerable.Where(_filterFunc));
            while (queue.Count > 0)
            {
                T dequeue = queue.Pop();
                nodeAction?.Invoke(dequeue);
                if (dequeue != null)
                {
                    var enumerable = _nodesSelectorFunc(dequeue);
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