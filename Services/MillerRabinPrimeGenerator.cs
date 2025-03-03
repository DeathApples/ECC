using System;
using System.Numerics;
using System.Security.Cryptography;

namespace ECDH.Services
{
    public class MillerRabinPrimeGenerator()
    {
        public int PrecisionFactor { get; private set; } = 100;

        public BigInteger GeneratePrimeNumber(bool isLarge = true)
        {
            var rnd = new Random();
            BigInteger number;

            do
            {
                number = isLarge ? new BigInteger(RandomNumberGenerator.GetBytes(32), true) : rnd.Next(11, 1000);
            } while (!IsPrimeNumber(number));

            return number;
        }

        public bool IsPrimeNumber(BigInteger number)
        {
            // ToDo: Реализовать проверку числа на простоту с указанной точностью 'precision'

            return true;
        }
    }
}
