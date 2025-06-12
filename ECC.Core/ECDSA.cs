using ECC.Core.Models;
using System.Numerics;

namespace ECC.Core
{
    public static class ECDSA
    {
        public static (BigInteger r, BigInteger s) GenerateDigitalSignature(ECPoint G, BigInteger privateKey, BigInteger hash, out BigInteger sessionKey)
        {
            BigInteger r;

            do
            {
                sessionKey = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                r = new FiniteFieldNumber((sessionKey * G).X, G.Order).Value;
            }
            while (r == 0);

            var s = (new FiniteFieldNumber(hash + r * privateKey, G.Order) * new FiniteFieldNumber(sessionKey, G.Order).Inverse()).Value;
            
            return (r, s);
        }

        public static bool VerifyDigitalSignature(ECPoint G, (BigInteger r, BigInteger s) signature, ECPoint publicKey, BigInteger hash, out ECPoint verificationPoint)
        {
            var w = new FiniteFieldNumber(signature.s, G.Order).Inverse().Value;
            var z1 = new FiniteFieldNumber(hash * w, G.Order).Value;
            var z2 = new FiniteFieldNumber(signature.r * w, G.Order).Value;

            verificationPoint = z1 * G + z2 * publicKey;

            if (!verificationPoint.IsInfinite)
                return new FiniteFieldNumber(verificationPoint.X, G.Order).Value == signature.r;

            return false;
        }
    }
}
