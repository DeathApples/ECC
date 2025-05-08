using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class SchoofAlgorithm
    {
        private readonly static List<FiniteFieldElement> remainders = [];

        public static BigInteger Compute()
        {
            BigInteger M = 1, l = 2, q = EllipticCurve.Prime;
            BigInteger sqrt16q = MathService.CeilSqrt(16 * q);

            remainders.Clear();

            while (M < sqrt16q)
            {
                if(l == 2)
                {
                    Polynomial.Prime = 2;
                    BigInteger prime = Polynomial.Prime;

                    Polynomial polynomial1 = new([(q, new(1, prime)), (1, new(-1, prime))]);
                    Polynomial polynomial2 = new([(3, new(1, prime)), (1, new(EllipticCurve.A, prime)), (0, new(EllipticCurve.B, prime))]);

                    if (ExtendedEuclideanAlgorithm.Compute(polynomial1, polynomial2).gcd.Degree < 1)
                        remainders.Add(new(1, prime));
                    else
                        remainders.Add(new(0, prime));
                }
                
                else
                {
                    EllipticCurvePoint left, right;
                    var ql = q % l;

                    ql = ql <= (l / 2) ? ql : ql - l;

                    EllipticCurve.Prime = l;

                    var point = EllipticCurve.GetRandomPoint(false);
                    EllipticCurvePoint pointq = new(FiniteFieldElement.Pow(point.X, q), FiniteFieldElement.Pow(point.Y, q));
                    EllipticCurvePoint pointq2 = new(FiniteFieldElement.Pow(point.X, q * q), FiniteFieldElement.Pow(point.Y, q * q));

                    for (int tau = 0; tau <= (l - 1) / 2; tau++)
                    {
                        left = pointq2 + ql * point;
                        right = tau * pointq;

                        if (left == right)
                        {
                            remainders.Add(new(tau, l));
                            break;
                        }
                        
                        else if (left == -right)
                        {
                            remainders.Add(new(-tau, l));
                            break;
                        }
                    }
                }

                M *= l;
                l = MillerRabinPrimalityTest.NextPrimeNumber(l);
            }

            var t = ChineseRemainderTheorem.Compute(remainders).Value;
            EllipticCurve.Prime = q;

            return q + 1 - t;
        }
    }
}
