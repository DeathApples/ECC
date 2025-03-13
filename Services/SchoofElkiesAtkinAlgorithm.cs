using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class SchoofElkiesAtkinAlgorithm
    {
        public static BigInteger Compute()
        {
            BigInteger M = 1, l = 2, t = 0;
            BigInteger j = EllipticCurve.JInvariant;

            var primalityTest = new MillerRabinPrimalityTest();
            BigInteger sqrt16p = CeilSqrt(16 * EllipticCurve.P);

            while (M < sqrt16p)
            {
                // Выполнить проверку, является ли число l простым Элкиса или Аткина

                M *= l;
                l = primalityTest.NextPrimeNumber(l);
            }

            // Восстановить t используя наборы Ap и Ep по Китайской теореме об остатках
            return EllipticCurve.P + 1 - t;
        }

        private static BigInteger FloorSqrt(BigInteger n)
        {
            BigInteger low = 1, high = n;
            BigInteger result = 1;

            while (low <= high)
            {
                BigInteger middle = low + (high - low) / 2;

                if (middle * middle <= n)
                {
                    result = middle;
                    low = middle + 1;
                }

                else
                    high = middle - 1;
            }

            return result;
        }

        private static BigInteger CeilSqrt(BigInteger n)
        {
            BigInteger low = 1, high = n;
            BigInteger result = n;

            while (high >= low)
            {
                BigInteger middle = high - (high - low) / 2;

                if (middle * middle >= n)
                {
                    result = middle;
                    high = middle - 1;
                }

                else
                    low = middle + 1;
            }

            return result;
        }
    }
}
