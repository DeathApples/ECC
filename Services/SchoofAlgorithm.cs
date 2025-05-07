using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class SchoofAlgorithm
    {
        private readonly static List<BigInteger> listOfAs = [];
        private readonly static List<BigInteger> listOfNs = [];

        public static BigInteger Compute()
        {
            BigInteger M = 1, l = 2, q = EllipticCurve.P;
            BigInteger sqrt16q = MathService.CeilSqrt(16 * q);

            listOfAs.Clear(); listOfNs.Clear();

            while (M < sqrt16q)
            {
                if(l == 2)
                {
                    Polynomial.P = 2;
                    var polynomial1 = new Polynomial([(q, 1), (1, -1)]);
                    var polynomial2 = new Polynomial([(3, 1), (1, EllipticCurve.A), (0, EllipticCurve.B)]);

                    //var temp = ExtendedEuclideanAlgorithm.Compute(polynomial2, polynomial1).gcd;

                    if (ExtendedEuclideanAlgorithm.Compute(polynomial1, polynomial2).gcd.Degree < 1)
                    {
                        listOfAs.Add(1); listOfNs.Add(2);
                    }
                    else
                    {
                        listOfAs.Add(0); listOfNs.Add(2);
                    }
                }
                else
                {
                    Point left, right;
                    var ql = q % l;

                    ql = ql <= (l / 2) ? ql : ql - l;

                    EllipticCurve.P = l;
                    var point = EllipticCurve.GeneratePoint(false);
                    var pointq = new Point(BigInteger.ModPow(point.X, q, l), BigInteger.ModPow(point.Y, q, l));
                    var pointq2 = new Point(BigInteger.ModPow(point.X, q * q, l), BigInteger.ModPow(point.Y, q * q, l));

                    for (int tau = 0; tau <= (l - 1) / 2; tau++)
                    {
                        left = pointq2 + ql * point;
                        right = tau * pointq;

                        if (left == right)
                        {
                            listOfAs.Add(tau); listOfNs.Add(l);
                            break;
                        }
                        else if (left == -right)
                        {
                            listOfAs.Add(l - tau); listOfNs.Add(l);
                            break;
                        }
                    }
                }

                M *= l;
                l = MillerRabinPrimalityTest.NextPrimeNumber(l);
            }

            var t = ChineseRemainderTheorem.Compute(listOfAs, listOfNs);
            EllipticCurve.P = q;

            return q + 1 - t;
        }
    }
}
