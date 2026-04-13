using System;

namespace _Project.Core
{
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public readonly int X;
        public readonly int Y;

        public GridPosition(int x, int y) { X = x; Y = y; }
        
        public static GridPosition operator +(GridPosition a, GridPosition b)
            => new(a.X + b.X, a.Y + b.Y);

        public static readonly GridPosition Up    = new( 0,  1);
        public static readonly GridPosition Down  = new( 0, -1);
        public static readonly GridPosition Left  = new(-1,  0);
        public static readonly GridPosition Right = new( 1,  0);

        public bool Equals(GridPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is GridPosition g && Equals(g);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator ==(GridPosition a, GridPosition b) => a.Equals(b);
        public static bool operator !=(GridPosition a, GridPosition b) => !a.Equals(b);
        public override string ToString() => $"({X}, {Y})";
    }
}