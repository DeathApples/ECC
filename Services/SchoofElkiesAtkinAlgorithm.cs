using ECDH.Models;
using System.Numerics;

namespace ECDH.Services
{
    public static class SchoofElkiesAtkinAlgorithm
    {
        public static BigInteger Compute()
        {
            BigInteger M = 1, l = 2, t = 0;
            BigInteger j = EllipticCurve.JInvariant;

            var primalityTest = new MillerRabinPrimalityTest();

            while (M < 4 /* * BigInteger.Sqrt(EllipticCurve.P) */)
            {
                // 

                M *= l;
                l = primalityTest.NextPrimeNumber(l);
            }

            // восстановить t используя наборы Ap и Ep
            return EllipticCurve.P + 1 - t;
        }
    }
}
