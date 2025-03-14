using System.Numerics;

namespace ECDH.Services
{
    public static class MathService
    {
        public static BigInteger CeilSqrt(BigInteger n)
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
                {
                    low = middle + 1;
                }
            }

            return result;
        }

        public static BigInteger NormilizeInField(BigInteger a, BigInteger p)
        {
            return (a % p + p) % p;
        }
    }
}
