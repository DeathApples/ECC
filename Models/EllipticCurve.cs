using ECC.Services;
using System.Numerics;
using System.Security.Cryptography;

namespace ECC.Models
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

        public static EllipticCurvePoint GetBasePoint(BigInteger groupOrder, out BigInteger subgroupOrder)
        {
            subgroupOrder = 0;
            var divisors = new List<BigInteger>();

            for (BigInteger i = 1; i * i <= groupOrder; i++)
            {
                if (groupOrder % i == 0)
                {
                    if (i * i == groupOrder)
                        divisors.Add(i);

                    else
                        divisors.AddRange(i, groupOrder / i);
                }
            }

            divisors.Sort();
            var point = EllipticCurvePoint.Infinite;
            var x = BigInteger.Zero;

            while (subgroupOrder < groupOrder / 3)
            {
                do
                {
                    point = GetPointByX(x);
                    x++;
                }
                while (point.IsInfinite && x < Prime);

                if (x == Prime)
                    throw new Exception("There is no base point");

                foreach (var divisor in divisors)
                {
                    if ((divisor * point).IsInfinite)
                    {
                        if (MillerRabinPrimalityTest.IsPrimeNumber(divisor))
                            subgroupOrder = divisor;

                        break;
                    }
                }
            }

            return point;
        }

        public static EllipticCurvePoint GetPointByX(BigInteger x)
        {
            var xFF = new FiniteFieldNumber(x, Prime);
            var xPart = FiniteFieldNumber.Pow(xFF, 3) + A * xFF + B;

            if (LegendreSymbol.Compute(xPart.Value, Prime) == 1)
            {
                var y = TonelliShanksAlgorithm.GetSquareRoot(xPart.Value, Prime);
                return new(x, y);
            }

            return EllipticCurvePoint.Infinite;
        }

        public static new string? ToString()
        {
            string formula = "y² = x³";

            if (A != 0)
                formula += A == 1 ? " + x" : A == -1 ? " - x" : A < 0 ? $" - {-A}x" : A > 0 ? $" + {A}x" : "";

            if (B != 0)
                formula += B < 0 ? $" - {-B}" : B > 0 ? $" + {B}" : "";

            return formula;
        }
    }
}
