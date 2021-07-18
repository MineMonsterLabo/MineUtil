using System;

namespace MineUtil
{
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        private static readonly Unit _value = new Unit();
        public static ref readonly Unit Value => ref _value;

        public int CompareTo(Unit other)
        {
            return 0;
        }

        int IComparable.CompareTo(object obj)
        {
            return 0;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }
    }
}