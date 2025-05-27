using System.Numerics;

namespace ECC.Services
{
    public class TonelliShanksAlgorithm
    {
        public static BigInteger GetSquareRoot(BigInteger n, BigInteger p)
        {
            var Q = p - 1;
            var S = BigInteger.Zero;

            while (Q % 2 == 0)
            {
                Q /= 2;
                S += 1;
            }

            if (S == 1)
                return BigInteger.ModPow(n, (p + 1) / 4, p);

            var z = LegendreSymbol.GetNonSquare(p);
            var c = BigInteger.ModPow(z, Q, p);

            var R = BigInteger.ModPow(n, (Q + 1) / 2, p);
            var t = BigInteger.ModPow(n, Q, p);
            var M = S;

            while (true)
            {
                if (t % p == 1)
                    return R;

                BigInteger i;

                for (i = 1; i < M; i++)
                    if (BigInteger.ModPow(t, BigInteger.Pow(2, (int)i), p) == 1)
                        break;

                var b = BigInteger.ModPow(c, BigInteger.Pow(2, (int)(M - i - 1)), p);
                R = R * b % p;
                t = t * b * b % p;
                c = BigInteger.ModPow(b, 2, p);
                M = i;
            }
        }
    }
}
