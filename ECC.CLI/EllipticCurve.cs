using ECC.Core;
using System.Numerics;

namespace ECC.CLI
{
    internal static class EllipticCurve
    {
        internal static ECPoint G = null!;

        internal static void Prepare()
        {
            BigInteger a, b, p;

            do
            {
                a = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                b = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                p = Core.Algorithms.MillerRabinPrimalityTest.GetPrimeNumber();
            }
            while (!ECurve.TryGenerateEllipticCurve(a, b, p) || !ECurve.TryGetBasePoint(out G) || ECurve.Prime == ECurve.Order || G.Order <= 3);
        }
    }
}
