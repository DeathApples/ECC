using ECC.Models;
using System.Numerics;

namespace ECC.Services
{
    public class EllipticCurveDigitalSignatureAlgorithm
    {
        private static Anna A = null!;
        private static Boris B = null!;

        public static void Init()
        {
            Console.WriteLine("\nПодготовка параметров для ECDSA\n");

            while (true)
            {
                EllipticCurve.GenerateParameters(false);
                EllipticCurve.Prime = MillerRabinPrimalityTest.GetPrimeNumber();

                try
                {
                    var pointsCount = SchoofAlgorithm.GetPointsCount();
                    var G = EllipticCurve.GetBasePoint(pointsCount, out BigInteger subgroupOrder);
                    
                    A = new("Анна", G, subgroupOrder);
                    B = new("Борис", G, subgroupOrder);

                    Console.WriteLine($"Эллиптическая кривая E = {EllipticCurve.ToString()}");
                    Console.WriteLine($"Модуль p = {EllipticCurve.Prime}");
                    Console.WriteLine($"Количество точек на кривой: {pointsCount}");
                    Console.WriteLine($"Количество точек подгруппы, генерируемой точкой {G}: {subgroupOrder}");

                    break;
                }

                catch
                {
                    continue;
                }
            }
        }

        public static void Execute(string message)
        {
            Console.WriteLine("\nВыполнение ECDSA\n");

            A.GeneratePrivateKey();
            A.CalculatePublicKey();
            A.CalculateHash(message);
            A.GenerateSessionKey();
            A.CalculateDigitalSignature();

            B.CalculateHash(message);
            B.TakeDigitalSignatureFrom(A);
            B.CalculateVerificationPointWithPublicKeyFrom(A);
            B.CheckDigitalSignature();
        }
    }
}
