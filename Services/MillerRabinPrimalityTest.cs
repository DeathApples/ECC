using System.Numerics;
using System.Security.Cryptography;

namespace ECDH.Services
{
    public static class MillerRabinPrimalityTest
    {
        public static int PrecisionFactor { get; private set; } = 100;

        public static BigInteger NextPrimeNumber(BigInteger number)
        {
            if (number == 2)
                return 3;

            do
                number += 2;
            while (!IsPrimeNumber(number));

            return number;
        }

        public static bool IsPrimeNumber(BigInteger number)
        {
            if (number == 2 || number == 3)
                return true;

            if (number < 2 || number % 2 == 0)
                return false;

            BigInteger t = number - 1;

            int s = 0;

            while (t % 2 == 0)
            {
                t /= 2;
                s += 1;
            }

            for (int i = 0; i < PrecisionFactor; i++)
            {
                BigInteger a;

                do
                    a = new BigInteger(RandomNumberGenerator.GetBytes(number.ToByteArray().Length), true);
                while (a < 2 || a >= number - 2);

                BigInteger x = BigInteger.ModPow(a, t, number);

                if (x == 1 || x == number - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, number);

                    if (x == 1)
                        return false;

                    if (x == number - 1)
                        break;
                }

                if (x != number - 1)
                    return false;
            }

            return true;
        }
    }
}
