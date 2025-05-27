using System.Numerics;

namespace ECC.Models
{
    public class Boris(string name, EllipticCurvePoint basePoint, BigInteger subgroupOrder) : Participant(name, basePoint, subgroupOrder)
    {
        private (BigInteger r, BigInteger s) receivedDigitalSignature;

        private EllipticCurvePoint C = null!;

        public void TakeDigitalSignatureFrom(Anna anna)
        {
            receivedDigitalSignature = anna.DigitalSignature;
            Console.WriteLine($"{name} получает цифровую подпись {receivedDigitalSignature}");
        }

        public void CalculateVerificationPointWithPublicKeyFrom(Anna anna)
        {
            var v = new FiniteFieldNumber(hash, prime).Inverse().Value;
            var z1 = new FiniteFieldNumber(receivedDigitalSignature.s * v, prime).Value;
            var z2 = new FiniteFieldNumber(-receivedDigitalSignature.r * v, prime).Value;
            C = z1 * G + z2 * anna.PublicKey;
            Console.WriteLine($"{name} вычисляет точку проверки {C}");
        }

        public void CheckDigitalSignature()
        {
            if (new FiniteFieldNumber(C.X.Value, prime).Value == receivedDigitalSignature.r)
                Console.WriteLine($"Подпись верна");
            else
                Console.WriteLine($"Подпись неверна");
        }
    }
}
