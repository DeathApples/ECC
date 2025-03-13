using System.Numerics;

namespace ECDH.Services
{
    /* TODO: подумать над названием класса */
    public static class LegendreSymbol
    {
        /*
        public static BigInteger Compute(BigInteger d, BigInteger l)
        {
            if (d == 1)
                return 1;

            if (d % 2 == 0)
                return Compute(d / 2, l) * BigInteger.Pow(-1, (int)((BigInteger.Pow(l, 2) - 1) / 8 % 2));

            return Compute(l % d, d) * BigInteger.Pow(-1, (int)((d - 1) * (l - 1) / 4 % 2));
        }
        */

        public static BigInteger FindNonSquareInFiniteField(BigInteger l)
        {
            BigInteger d = 2;

            while (d < l)
            {
                BigInteger candidate = BigInteger.ModPow(d, (l - 1) / 2, l);

                if (candidate == -1 || candidate == l - 1)
                    break;

                d++;
            }

            return d;
        }
    }
}
