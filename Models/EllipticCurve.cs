using ECDH.Services;
using System.Numerics;
using System.Security.Cryptography;

namespace ECDH.Models
{
    public class EllipticCurve
    {
        public static BigInteger A { get; set; } = 2;
        public static BigInteger B { get; set; } = 3;
        public static BigInteger Prime { get; set; } = 97;
        public static BigInteger Discriminant => 4 * BigInteger.Pow(A, 3) + 27 * BigInteger.Pow(B, 2);

        private static BigInteger GetLargeRandomNumber(bool isUnsigned) => new(RandomNumberGenerator.GetBytes(new Random().Next(32)), isUnsigned);

        private static int GetRandomNumber(int minValue, int maxValue) => new Random().Next(minValue, maxValue);

        public static void GenerateParameters(bool isLarge = true)
        {
            do
            {
                A = isLarge ? GetLargeRandomNumber(false) : GetRandomNumber(-16, 16);
                B = isLarge ? GetLargeRandomNumber(false) : GetRandomNumber(-16, 16);
            }
            while (Discriminant == 0);
        }

        public static void GeneratePrimeNumber(bool isLarge = true)
        {
            BigInteger number;

            do
                number = isLarge ? GetLargeRandomNumber(true) : GetRandomNumber(11, 500);
            while (!MillerRabinPrimalityTest.IsPrimeNumber(number));

            Prime = number;
        }

        public static EllipticCurvePoint GetRandomPoint(bool isLarge = true)
        {
            EllipticCurvePoint point;

            do
            {
                var x = isLarge ? GetLargeRandomNumber(false) : GetRandomNumber(-(int)Prime, (int)Prime);
                var y = isLarge ? GetLargeRandomNumber(false) : GetRandomNumber(-(int)Prime, (int)Prime);
                point = new(x, y);
            }
            while (!point.IsOnCurve);

            return point;
        }

        public static new string? ToString()
        {
            string formula = "y² = x³";

            if (A != 0)
                formula += A == 1
                    ? " + x"
                    : A == -1
                    ? " - x"
                    : A < 0
                    ? $" - {-A}x"
                    : A > 0
                    ? $" + {A}x"
                    : "";

            if (B != 0)
                formula += B < 0
                    ? $" - {-B}"
                    : B > 0
                    ? $" + {B}"
                    : "";

            return formula;
        }
    }
}
