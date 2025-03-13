using ECDH.Services;
using System.Numerics;
using System.Security.Cryptography;

namespace ECDH.Models
{
    public static class EllipticCurve
    {
        public static BigInteger A { get; set; }
        public static BigInteger B { get; set; }
        public static BigInteger P { get; set; }

        public static BigInteger Discriminant => 4 * BigInteger.Pow(A, 3) + 27 * BigInteger.Pow(B, 2);
        public static BigInteger JInvariant => 1728 * 4 * BigInteger.Pow(A, 3) / Discriminant;

        public static void GenerateParameters(bool isLarge = true)
        {
            var rnd = new Random();

            do 
            {
                A = isLarge ? new BigInteger(RandomNumberGenerator.GetBytes(rnd.Next(32))) : rnd.Next(32) - 16;
                B = isLarge ? new BigInteger(RandomNumberGenerator.GetBytes(rnd.Next(32))) : rnd.Next(32) - 16;
            } while (Discriminant == 0);
        }

        public static void GeneratePrimeNumber(bool isLarge = true)
        {
            var primalityTest = new MillerRabinPrimalityTest();
            P = primalityTest.GeneratePrimeNumber(isLarge);
        }

        public static new string? ToString()
        {
            string formula = "y² = x³";

            if (A != 0)
                formula += A == 1 ? " + x" : A == -1 ? " - x" : A < 0 ? $" - {BigInteger.Abs(A)}x" : $" + {A}x";

            if (B != 0)
                formula += B < 0 ? $" - {BigInteger.Abs(B)}" : $" + {B}";

            return formula;
        }
    }
}
