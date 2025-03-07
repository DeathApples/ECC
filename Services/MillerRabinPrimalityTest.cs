using System.Numerics;
using System.Security.Cryptography;

namespace ECDH.Services
{
    public class MillerRabinPrimalityTest()
    {
        public int PrecisionFactor { get; private set; } = 100;

        public BigInteger GeneratePrimeNumber(bool isLarge = true)
        {
            var rnd = new Random();
            BigInteger number;

            do
            {
                number = isLarge ? new BigInteger(RandomNumberGenerator.GetBytes(32), true) : rnd.Next(11, 500);
            } while (!IsPrimeNumber(number));

            return number;
        }

        public BigInteger NextPrimeNumber(BigInteger number)
        {
            do
            {
                number += 2;
            } while (!IsPrimeNumber(number));

            return number;
        }

        public bool IsPrimeNumber(BigInteger number)
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
                {
                    a = new BigInteger(RandomNumberGenerator.GetBytes(number.ToByteArray().Length));
                }
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
