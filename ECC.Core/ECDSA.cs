using ECC.Core.Models;
using System.Collections.Concurrent;
using System.Numerics;

namespace ECC.Core
{
    public static class ECDSA
    {
        public static ConcurrentQueue<(string, string)> StepsQueue { get; } = new();

        private static readonly AutoResetEvent AnnaSignal = new(false);
        private static (BigInteger, BigInteger) Signature;
        private static ECPoint PublicKey = null!;

        public static async void Execute(string message, ECPoint G, int delay = 0)
        {
            int delayA = 0, delayB = 0;

            if (delay != 0)
            {
                delayA = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-delay / 2, delay / 2 + 1) + delay;
                delayB = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-delay / 2, delay / 2 + 1) + delay;
            }

            Task anna = Task.Run(async () =>
            {
                await Task.Delay(delayA);

                var privateKey = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                StepsQueue.Enqueue(("Анна", $"Выбирает закрытый ключ: {privateKey}"));

                await Task.Delay(delayA);

                var publicKey = privateKey * G;
                StepsQueue.Enqueue(("Анна", $"Вычисляет открытый ключ и публикует его: {publicKey}"));
                PublicKey = publicKey;

                await Task.Delay(delayA);

                var hash = new FiniteFieldNumber(message.GetHashCode(), G.Order).Value;
                StepsQueue.Enqueue(("Анна", $"Вычисляет хеш сообщения: {hash}"));

                await Task.Delay(delayA);

                var signature = GenerateDigitalSignature(G, privateKey, hash, out BigInteger sessionKey);
                StepsQueue.Enqueue(("Анна", $"Выбирает сеансовый ключ: {sessionKey}"));

                await Task.Delay(delayA);

                StepsQueue.Enqueue(("Анна", $"Вычисляет цифровую подпись: {signature}"));

                await Task.Delay(delayA);

                Signature = signature;
                AnnaSignal.Set();
                StepsQueue.Enqueue(("Анна", $"Отправляет Борису сообщение и цифровую подпись"));
            });

            Task<bool> boris = Task.Run(async () =>
            {
                AnnaSignal.WaitOne();
                var signature = Signature;
                StepsQueue.Enqueue(("Борис", $"Получает от Анны сообщение и цифровую подпись: {signature}"));

                await Task.Delay(delayB);

                var publicKey = PublicKey;
                StepsQueue.Enqueue(("Борис", $"Узнаёт открытый ключ Анны: {publicKey}"));

                await Task.Delay(delayB);

                var hash = new FiniteFieldNumber(message.GetHashCode(), G.Order).Value;
                StepsQueue.Enqueue(("Борис", $"Вычисляет хеш сообщения: {hash}"));

                await Task.Delay(delayB);

                var isVerified = VerifyDigitalSignature(G, signature, publicKey, hash, out ECPoint verificationPoint);
                StepsQueue.Enqueue(("Борис", $"Вычисляет точку проверки: {verificationPoint}"));

                await Task.Delay(delayB);

                StepsQueue.Enqueue(("Борис", $"Проверяет цифровую подпись"));

                return isVerified;
            });

            await anna;
            var isVerified = await boris;

            var result = isVerified ? "Подпись верна" : "Подпись не верна";

            StepsQueue.Enqueue(("", result));
        }

        private static (BigInteger r, BigInteger s) GenerateDigitalSignature(ECPoint G, BigInteger privateKey, BigInteger hash, out BigInteger sessionKey)
        {
            BigInteger r, s;

            do
            {
                sessionKey = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                r = new FiniteFieldNumber((sessionKey * G).X, G.Order).Value;
                s = (new FiniteFieldNumber(hash + r * privateKey, G.Order) * new FiniteFieldNumber(sessionKey, G.Order).Inverse()).Value;
            }
            while (r == 0 || s == 0);
            
            return (r, s);
        }

        private static bool VerifyDigitalSignature(ECPoint G, (BigInteger r, BigInteger s) signature, ECPoint publicKey, BigInteger hash, out ECPoint verificationPoint)
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
