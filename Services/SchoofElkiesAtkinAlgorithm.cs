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
                // Выполнить проверку, является ли число l простым Элкиса или Аткина

                M *= l;
                l = primalityTest.NextPrimeNumber(l);
            }

            // Восстановить t используя наборы Ap и Ep по Китайской теореме об остатках
            return EllipticCurve.P + 1 - t;
        }
    }
}
