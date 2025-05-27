using System.Numerics;

namespace ECC.Services
{
    public class LegendreSymbol
    {
        public static BigInteger Compute(BigInteger a, BigInteger p)
        {
            if (a % p == 0)
                return 0;

            var (x, y, L) = (a, p, BigInteger.One);

            while (true)
            {
                x %= y;

                if (x > y / 2)
                {
                    x = y - x;

                    if (y % 4 == 3)
                        L *= -1;
                }

                while (x % 4 == 0)
                    x /= 4;

                if (x % 2 == 0)
                {
                    x /= 2;

                    if (y % 8 == 3 || y % 8 == 5)
                        L *= -1;
                }

                if (x == 1)
                    return L;

                if (x % 4 == 3 && y % 4 == 3)
                    L *= -1;

                (x, y) = (y, x);
            }
        }

        public static BigInteger GetNonSquare(BigInteger p)
        {
            var a = (BigInteger)2;

            while (Compute(a, p) != -1)
                a++;

            return a;
        }
    }
}
