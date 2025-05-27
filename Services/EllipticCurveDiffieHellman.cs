using ECC.Models;
using System.Numerics;

namespace ECC.Services
{
    public class EllipticCurveDiffieHellman
    {
        private static Anna A = null!;
        private static Boris B = null!;

        public static void Init()
        {
            Console.WriteLine("\nПодготовка параметров для ECDH\n");

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

        public static void Execute()
        {
            Console.WriteLine("\nВыполнение ECDH\n");

            A.GenerateRandomNumber();
            B.GenerateRandomNumber();

            A.CalculatePoint();
            B.CalculatePoint();

            A.SendPointTo(B);
            B.SendPointTo(A);

            A.CalculateSecretPoint();
            B.CalculateSecretPoint();

            Participant.CompareSecretPointsFrom(A, B);
        }
    }
}
