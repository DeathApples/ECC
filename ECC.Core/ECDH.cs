using System.Collections.Concurrent;

namespace ECC.Core
{
    public static class ECDH
    {
        public static ConcurrentQueue<(string, string)> StepsQueue { get; } = new();

        private static readonly AutoResetEvent AnnaSignal = new(false);
        private static readonly AutoResetEvent BorisSignal = new(false);

        private static ECPoint AnnaPoint = null!;
        private static ECPoint BorisPoint = null!;

        public static (ECPoint, ECPoint) Execute(ECPoint G, int delay = 0)
        {
            int delayA = 0, delayB = 0;

            if (delay != 0)
            {
                delayA = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-delay / 2, delay / 2 + 1) + delay;
                delayB = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-delay / 2, delay / 2 + 1) + delay;
            }

            Task<ECPoint> anna = Task.Run(() =>
            {
                var n = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                StepsQueue.Enqueue(("Анна", $"Выбирает число n: {n}"));

                Thread.Sleep(delayA);

                var P = n * G;
                StepsQueue.Enqueue(("Анна", $"Вычисляет точку P = n * G: {P}"));

                Thread.Sleep(delayA);

                AnnaPoint = P;
                AnnaSignal.Set();
                StepsQueue.Enqueue(("Анна", $"Отправляет Борису точку P"));

                Thread.Sleep(delayA);

                BorisSignal.WaitOne();
                var Q = BorisPoint;
                StepsQueue.Enqueue(("Анна", $"Получает от Бориса точку Q"));

                Thread.Sleep(delayA);

                var K = n * Q;
                StepsQueue.Enqueue(("Анна", $"Вычисляет секретную точку K = n * Q: {K}"));

                Thread.Sleep(delayA);
                return K;
            });

            Task<ECPoint> boris = Task.Run(() =>
            {
                var m = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                StepsQueue.Enqueue(("Борис", $"Выбирает число m: {m}"));

                Thread.Sleep(delayB);

                var Q = m * G;
                StepsQueue.Enqueue(("Борис", $"Вычисляет точку Q = m * G: {Q}"));

                Thread.Sleep(delayB);

                BorisPoint = Q;
                BorisSignal.Set();
                StepsQueue.Enqueue(("Борис", $"Отправляет Анне точку Q"));

                Thread.Sleep(delayB);

                AnnaSignal.WaitOne();
                var P = AnnaPoint;
                StepsQueue.Enqueue(("Борис", $"Получает от Анны точку P"));

                Thread.Sleep(delayB);

                var K = m * P;
                StepsQueue.Enqueue(("Борис", $"Вычисляет секретную точку K = m * P: {K}"));

                Thread.Sleep(delayB);
                return K;
            });

            anna.Wait();
            boris.Wait();

            StepsQueue.Enqueue(("", ""));
            return (anna.Result, boris.Result);
        }
    }
}
