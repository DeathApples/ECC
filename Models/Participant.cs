using System.Numerics;
using System.Security.Cryptography;

namespace ECC.Models
{
    abstract public class Participant(string name, EllipticCurvePoint basePoint, BigInteger subgroupOrder)
    {
        private BigInteger n;

        private EllipticCurvePoint P = null!;

        private EllipticCurvePoint receivedP = null!;

        protected BigInteger hash;

        protected readonly string name = name;

        protected readonly EllipticCurvePoint G = basePoint;

        protected readonly BigInteger prime = subgroupOrder;

        public EllipticCurvePoint K { get; private set; } = null!;

        public void GenerateRandomNumber()
        {
            n = new(RandomNumberGenerator.GetInt32((int)-prime, (int)prime));
            Console.WriteLine($"{name} выбирает число {n}");
        }

        public void CalculatePoint()
        {
            P = n * G;
            Console.WriteLine($"{name} вычисляет точку {P}");
        }

        public void SendPointTo(Participant participant)
        {
            participant.receivedP = P;
            Console.WriteLine($"{name} отправляет участнику с именем {participant.name} точку {P}");
        }

        public void CalculateSecretPoint()
        {
            K = n * receivedP;
            Console.WriteLine($"{name} вычисляет секретную точку {K}");
        }

        public void CalculateHash(string message)
        {
            hash = new FiniteFieldNumber(message.Length, prime).Value;
            Console.WriteLine($"{name} вычисляет хеш {hash}");
        }

        public static void CompareSecretPointsFrom(Participant A, Participant B)
        {
            if (A.K == B.K)
                Console.WriteLine($"{A.name} и {B.name} получили общую секретную точку K = {A.K}");
            else
                Console.WriteLine($"{A.name} и {B.name} не смогли получить общую секретную точку");
        }
    }
}
