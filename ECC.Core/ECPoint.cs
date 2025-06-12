using ECC.Core.Models;
using System.Numerics;

namespace ECC.Core
{
    public sealed class ECPoint
    {
        private readonly EllipticCurvePoint Point;

        public BigInteger X
        {
            get => Point.X.Value;
            set => Point.X = new(value, EllipticCurve.Prime);
        }

        public BigInteger Y
        {
            get => Point.Y.Value;
            set => Point.Y = new(value, EllipticCurve.Prime);
        }

        public BigInteger Order => Point.Order;

        public bool IsOnCurve => Point.IsOnCurve && Point.X.Prime == EllipticCurve.Prime && Point.Y.Prime == EllipticCurve.Prime;
        public bool IsInfinite => Point.IsInfinite;

        public static ECPoint Infinite => new();

        private ECPoint()
        {
            Point = EllipticCurvePoint.Infinite;
        }

        internal ECPoint(EllipticCurvePoint point)
        {
            Point = point;
        }

        public ECPoint(BigInteger x, BigInteger y, bool isInfinite = false)
        {
            Point = new(x, y, isInfinite);
        }

        public void CalculateOrder() => Point.CalculateOrder();

        public static ECPoint operator +(ECPoint P, ECPoint Q) => new(P.Point + Q.Point);

        public static ECPoint operator *(BigInteger n, ECPoint P) => new(n * P.Point);

        public static ECPoint operator *(ECPoint P, BigInteger n) => n * P;

        public static ECPoint operator -(ECPoint P) => new(P.X, -P.Y, P.IsInfinite);

        public static ECPoint operator -(ECPoint P, ECPoint Q) => P + -Q;

        public static bool operator ==(ECPoint P, ECPoint Q) => P.Equals(Q);

        public static bool operator !=(ECPoint P, ECPoint Q) => !P.Equals(Q);

        public override bool Equals(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            if (obj is ECPoint P)
                return X == P.X && Y == P.Y;
            else
                throw new ArgumentException("Argument not a point");
        }

        public override int GetHashCode() => Point.GetHashCode();

        public override string? ToString()
        {
            if (Point.IsInfinite)
                return "(Inf, Inf)";

            return $"({Point.X.Value}, {Point.Y.Value})";
        }
    }
}
