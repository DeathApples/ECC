using System.Numerics;

namespace ECDH.Services
{
    public static class EuclideanAlgorithmService
    {
        public static BigInteger ExtendedEuclideanAlgorithm(BigInteger number, BigInteger module)
        {
            BigInteger coefficient = 0, prevCoefficient = 1;
            BigInteger remainder = module, prevRemainder = number;

            while (remainder != 0)
            {
                BigInteger quotient = prevRemainder / remainder;
                (prevRemainder, remainder) = (remainder, prevRemainder - quotient * remainder);
                (prevCoefficient, coefficient) = (coefficient, prevCoefficient - quotient * coefficient);
            }

            return prevRemainder == 1 ? (prevCoefficient + module) % module : -1;
        }
    }
}
