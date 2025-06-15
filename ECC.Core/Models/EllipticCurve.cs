using ECC.Core.Algorithms;
using System.Numerics;

namespace ECC.Core.Models
{
    internal class EllipticCurve
    {
        internal static BigInteger A { get; set; } = 2;

        internal static BigInteger B { get; set; } = 3;

        internal static BigInteger Prime { get; set; } = 97;

        internal static BigInteger Order { get; set; } = 100;

        internal static BigInteger Discriminant => (4 * BigInteger.Pow(A, 3) + 27 * BigInteger.Pow(B, 2)) % Prime;

        internal static bool TryGetBasePoint(out EllipticCurvePoint basePoint)
        {
            var divisors = GetDivisorsOfOrder();
            var xList = Enumerable.Range(0, (int)Prime).ToList();

            BigInteger subgroupOrder = 0;
            int index;
            bool hasFound;

            do
            {
                do
                {
                    index = System.Security.Cryptography.RandomNumberGenerator.GetInt32(xList.Count);
                    hasFound = TryGetPointByX(xList[index], out basePoint);
                    xList.RemoveAt(index);
                }
                while (!hasFound && xList.Count > 0);

                if (xList.Count == 0)
                    return false;

                foreach (var divisor in divisors)
                {
                    if ((divisor * basePoint).IsInfinite)
                    {
                        if (MillerRabinPrimalityTest.IsPrimeNumber(divisor))
                            subgroupOrder = divisor;

                        break;
                    }
                }
            }
            while (subgroupOrder < Order / 3);

            basePoint.Order = subgroupOrder;
            return true;
        }

        internal static List<BigInteger> GetDivisorsOfOrder()
        {
            var divisors = new List<BigInteger>();

            for (BigInteger i = 1; i * i <= Order; i++)
            {
                if (Order % i == 0)
                {
                    if (i * i == Order)
                        divisors.Add(i);

                    else
                        divisors.Add(i);
                    divisors.Add(Order / i);
                }
            }

            divisors.Sort();
            return divisors;
        }

        internal static bool TryGetPointByX(BigInteger x, out EllipticCurvePoint point)
        {
            var xFF = new FiniteFieldNumber(x, Prime);
            var xPart = FiniteFieldNumber.Pow(xFF, 3) + A * xFF + B;

            if (LegendreSymbol.Compute(xPart.Value, Prime) == 1)
            {
                var y = TonelliShanksAlgorithm.GetSquareRoot(xPart.Value, Prime);
                point = new(x, y);
                return true;
            }

            else if (xPart.Value == 0)
            {
                point = new(x, 0);
                return true;
            }

            point = EllipticCurvePoint.Infinite;
            return false;
        }
    }
}
