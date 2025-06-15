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

        public static async void Execute(ECPoint G, int delay = 0)
        {
            int delayA = 0, delayB = 0;

            if (delay != 0)
            {
                delayA = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-delay / 2, delay / 2 + 1) + delay;
                delayB = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-delay / 2, delay / 2 + 1) + delay;
            }

            Task<ECPoint> anna = Task.Run(async () =>
            {
                await Task.Delay(delayA);

                var n = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                StepsQueue.Enqueue(("Анна", $"Выбирает число n: {n}"));

                await Task.Delay(delayA);

                var P = n * G;
                StepsQueue.Enqueue(("Анна", $"Вычисляет точку P = n * G: {P}"));

                await Task.Delay(delayA);

                AnnaPoint = P;
                AnnaSignal.Set();
                StepsQueue.Enqueue(("Анна", $"Отправляет Борису точку P"));

                await Task.Delay(delayA);

                BorisSignal.WaitOne();
                var Q = BorisPoint;
                StepsQueue.Enqueue(("Анна", $"Получает от Бориса точку Q"));

                await Task.Delay(delayA);

                var K = n * Q;
                StepsQueue.Enqueue(("Анна", $"Вычисляет секретную точку K = n * Q: {K}"));

                return K;
            });

            Task<ECPoint> boris = Task.Run(async () =>
            {
                await Task.Delay(delayB);

                var m = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
                StepsQueue.Enqueue(("Борис", $"Выбирает число m: {m}"));

                await Task.Delay(delayB);

                var Q = m * G;
                StepsQueue.Enqueue(("Борис", $"Вычисляет точку Q = m * G: {Q}"));

                await Task.Delay(delayB);

                BorisPoint = Q;
                BorisSignal.Set();
                StepsQueue.Enqueue(("Борис", $"Отправляет Анне точку Q"));

                await Task.Delay(delayB);

                AnnaSignal.WaitOne();
                var P = AnnaPoint;
                StepsQueue.Enqueue(("Борис", $"Получает от Анны точку P"));

                await Task.Delay(delayB);

                var K = m * P;
                StepsQueue.Enqueue(("Борис", $"Вычисляет секретную точку K = m * P: {K}"));

                return K;
            });

            var kA = await anna;
            var kB = await boris;

            var result = kA == kB 
                ? $"Анна и Борис получили общую секретную точку K = {kA}" 
                : "Анна и Борис не смогли получить общую секретную точку";

            StepsQueue.Enqueue(("", result));
        }
    }
}
