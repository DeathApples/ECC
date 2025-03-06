using System.Numerics;

namespace ECDH.Models
{
    public class Point
    {
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        public static Point Infinity => new();

        public Point() {  }
        public Point(Point point) { X = point.X; Y = point.Y; }
        public Point(BigInteger x, BigInteger y) { X = x; Y = y; }

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
                lambda = (3 * BigInteger.Pow(left.X, 2) + EllipticCurve.a) * ModularInverse(2 * left.Y, EllipticCurve.p) % EllipticCurve.p;
            else
                lambda = (right.Y - left.Y) * ModularInverse(right.X - left.X, EllipticCurve.p) % EllipticCurve.p;

            x = (BigInteger.Pow(lambda, 2) - left.X - right.X) % EllipticCurve.p;
            y = (lambda * (left.X - x) - left.Y) % EllipticCurve.p;

            x = x < 0 ? x + EllipticCurve.p : x;
            y = y < 0 ? y + EllipticCurve.p : y;

            return new(x, y);
        }

        public static Point operator *(BigInteger left, Point right)
        {
            Point result = Infinity;
            Point addend = new(right);

            while (left > 0)
            {
                if ((left & 1) == 1)
                    result += addend;

                addend += addend;
                left >>= 1;
            }

            return result;
        }

        public static Point operator -(Point point)
        {
            return new Point(point.X, -point.Y);
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

        private static BigInteger ModularInverse(BigInteger number, BigInteger module)
        {
            BigInteger coefficient = 0, prevCoefficient = 1;
            BigInteger remainder = module, prevRemainder = number;

            while (remainder != 0)
            {
                BigInteger quotient = prevRemainder / remainder;
                (prevRemainder, remainder) = (remainder, prevRemainder - quotient * remainder);
                (prevCoefficient, coefficient) = (coefficient, prevCoefficient - quotient * coefficient);
            }

            return prevRemainder == 1 ? (prevCoefficient + module) % module : -1;
        }

        public override string? ToString()
        {
            return $"({X}; {Y})";
        }
    }
}
