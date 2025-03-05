using System.Numerics;

namespace ECDH.Models
{
    public class Point
    {
        public BigInteger x { get; set; }
        public BigInteger y { get; set; }

        public Point()
        {
            x = 0;
            y = 0;
        }

        public Point(BigInteger x, BigInteger y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point left, Point right)
        {
            BigInteger lambda;
            Point result = new();

            if (left.x == right.x)
            {
                if (left.y == right.y)
                {
                    lambda = (3 * BigInteger.Pow(left.x, 2) + EllipticCurve.Instance.a) / (2 * left.y);
                    result.x = BigInteger.Pow(lambda, 2) - 2 * left.x;
                    result.y = lambda * (left.x - result.x) - left.y;
                }
            }
            else
            {
                lambda = (right.y - left.y) / (right.x - left.x);
                result.x = BigInteger.Pow(lambda, 2) - left.x - right.x;
                result.y = lambda * (left.x = result.x) - left.y;
            }

            return result;
        }

        public static BigInteger operator *(BigInteger left, Point right)
        {
            // TODO: реализовать алгоритм быстрого умножения числа и точки
            return 0;
        }

        public override string? ToString()
        {
            return $"({x}; {y})";
        }
    }
}
