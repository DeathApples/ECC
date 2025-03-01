using System;
using System.Numerics;
using System.Security.Cryptography;

namespace ECDH.Services
{
    class MillerRabinPrimeGenerator(int precisionFactor = 10)
    {
        private readonly int _precisionFactor = precisionFactor;
        public int PrecisionFactor => _precisionFactor;

        public BigInteger GeneratePrimeNumber()
        {
            var rnd = new Random();
            BigInteger number;

            do
            {
                number = new BigInteger(SHA256.HashData(RandomNumberGenerator.GetBytes(rnd.Next(256))), true);
            } while (!IsPrimeNumber(number));

            return number;
        }

        public bool IsPrimeNumber(BigInteger number)
        {
            // ToDo: Реализовать проверку числа на простоту с указанной точностью 'precision'

            throw new NotImplementedException();
        }
    }
}
