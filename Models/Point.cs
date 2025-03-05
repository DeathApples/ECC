using System.Numerics;

namespace ECDH.Models
{
    public class Point
    {
        public BigInteger x { get; set; }
        public BigInteger y { get; set; }

        public Point() {  }

        public Point(BigInteger x, BigInteger y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point? operator +(Point? left, Point? right)
        {
            if (left == null)
                return right;
            else if (right == null)
                return left;
            else
            {
                BigInteger lambda;
                BigInteger p = EllipticCurve.Instance.p;

                Point result = new();

                if (left.x == right.x)
                {
                    if (left.y == right.y)
                    {
                        BigInteger divisible = (3 * BigInteger.ModPow(left.x, 2, p) + EllipticCurve.Instance.a) % p;
                        BigInteger divider = 2 * left.y % p;

                        if (divider == 0)
                            return null;

                        while (divisible % divider != 0)
                            divisible += p;

                        lambda = divisible / divider % p;

                        BigInteger x = (BigInteger.ModPow(lambda, 2, p) - 2 * left.x) % p;
                        BigInteger y = (lambda * (left.x - x) - left.y) % p;

                        result.x = x < 0 ? x + p : x;
                        result.y = y < 0 ? y + p : y;
                    }
                    else
                        return null;
                }
                else
                {
                    BigInteger divisible = (right.y - left.y) % p;
                    BigInteger divider = (right.x - left.x) % p;

                    if (divider == 0)
                        return null;

                    while (divisible % divider != 0)
                        divisible += p;

                    lambda = divisible / divider % p;

                    BigInteger x = (BigInteger.ModPow(lambda, 2, p) - left.x - right.x) % p;
                    BigInteger y = (lambda * (left.x - x) - left.y) % p;

                    result.x = x < 0 ? x + p : x;
                    result.y = y < 0 ? y + p : y;
                }

                return result;
            }
        }

        public static Point? operator *(BigInteger left, Point? right)
        {
            if (right == null)
                return null;
            else
            {
                Point? result = null;
                Point? addend = new(right.x, right.y);

                while (left > 0)
                {
                    if ((left & 1) == 1)
                        result += addend;

                    addend += addend;
                    left >>= 1;
                }

                return result;
            }
        }

        public override string? ToString()
        {
            return $"({x}; {y})";
        }
    }
}
