using System.Numerics;
using System.Security.Cryptography;

namespace ECC.Models
{
    public class Anna(string name, EllipticCurvePoint basePoint, BigInteger subgroupOrder) : Participant(name, basePoint, subgroupOrder)
    {
        private BigInteger privateKey;

        private BigInteger sessionKey;

        public EllipticCurvePoint PublicKey { get; private set; } = null!;

        public (BigInteger r, BigInteger s) DigitalSignature { get; private set; }

        public void GeneratePrivateKey()
        {
            privateKey = new(RandomNumberGenerator.GetInt32(1, (int)prime));
            Console.WriteLine($"{name} выбирает ключ шифрования {privateKey}");
        }

        public void CalculatePublicKey()
        {
            PublicKey = privateKey * G;
            Console.WriteLine($"{name} вычисляет ключ расшифрования {PublicKey}");
        }

        public void GenerateSessionKey()
        {
            sessionKey = new(RandomNumberGenerator.GetInt32(1, (int)prime));
            Console.WriteLine($"{name} выбирает сеансовый ключ {sessionKey}");
        }

        public void CalculateDigitalSignature()
        {
            var r = new FiniteFieldNumber((sessionKey * G).X.Value, prime).Value;
            var s = new FiniteFieldNumber(r * privateKey + sessionKey * hash, prime).Value;
            DigitalSignature = (r, s);
            Console.WriteLine($"{name} вычисляет цифровую подпись {DigitalSignature}");
        }
    }
}
