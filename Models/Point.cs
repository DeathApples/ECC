using ECDH.Services;
using System.Numerics;

namespace ECDH.Models
{
    public class Point
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        public static Point Infinity => new();

        public bool IsOnCurve
        {
            get
            {
                BigInteger left = NormilizeInField(BigInteger.ModPow(Y, 2, EllipticCurve.P));
                BigInteger right = NormilizeInField(BigInteger.ModPow(X, 3, EllipticCurve.P) + EllipticCurve.A * X + EllipticCurve.B);

                return left == right;
            }
        }

        private static BigInteger NormilizeInField(BigInteger a) => MathService.NormilizeInField(a, EllipticCurve.P);
        private static BigInteger ModularInverse(BigInteger a) => ExtendedEuclideanAlgorithm.ModularInverse(a, EllipticCurve.P);

        public Point() {  }
        public Point(Point point) { X = point.X; Y = point.Y; }
        public Point(BigInteger x, BigInteger y) { X = NormilizeInField(x); Y = NormilizeInField(y); }

        public static Point operator +(Point left, Point right)
        {
            if (left == Infinity)
                return right;

            if (right == Infinity)
                return left;

            if (left == -right)
                return Infinity;

            BigInteger x, y, lambda;

            if (left == right)
                lambda = (3 * BigInteger.Pow(left.X, 2) + EllipticCurve.A) * ModularInverse(2 * left.Y) % EllipticCurve.P;
            else
                lambda = (right.Y - left.Y) * ModularInverse(NormilizeInField(right.X - left.X)) % EllipticCurve.P;

            x = NormilizeInField(BigInteger.Pow(lambda, 2) - left.X - right.X);
            y = NormilizeInField(lambda * (left.X - x) - left.Y);

            return new(x, y);
        }

        public static Point operator *(BigInteger number, Point point)
        {
            Point result = Infinity;
            Point addend = new(point);

            while (number > 0)
            {
                if ((number & 1) == 1)
                    result += addend;

                addend += addend;
                number >>= 1;
            }

            return result;
        }

        public static Point operator *(Point point, BigInteger number)
        {
            Point result = Infinity;
            Point addend = new(point);

            while (number > 0)
            {
                if ((number & 1) == 1)
                    result += addend;

                addend += addend;
                number >>= 1;
            }

            return result;
        }

        public static Point operator -(Point point)
        {
            return new Point(point.X, -point.Y + EllipticCurve.P);
        }

        public static Point operator -(Point left, Point right)
        {
            return left + (-right);
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object? obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            if (obj is Point point)
                return X == point.X && Y == point.Y;
            else
                throw new ArgumentException("Argument “obj” must be type of Class “Point”", nameof(obj));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string? ToString()
        {
            return $"({X}; {Y})";
        }
    }
}
