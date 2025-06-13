using ECC.Core;

namespace ECC.CLI
{
    internal static class DiffieHellman
    {
        private static string[] AnnaSteps = [];
        private static string[] BorisSteps = [];

        private static int AnnaStepsCount;
        private static int BorisStepsCount;

        internal static void Execute(int delay = 0)
        {
            AnnaSteps = []; AnnaStepsCount = 0;
            BorisSteps = []; BorisStepsCount = 0;

            Task writer = Task.Run(WriteSteps);
            var (Ka, Kb) = ECDH.Execute(EllipticCurve.G, delay);

            writer.Wait();

            Console.Clear();
            ConsoleVisualizer.PrintParameters();

            var result = Ka == Kb ? $"Анна и Борис получили общую секретную точку K = {Ka}" : "Анна и Борис не смогли получить общую секретную точку";
            ConsoleVisualizer.DrawTable("Выполнение ECDH", result, [0], [[["Анна"], ["Борис"]], [AnnaSteps, BorisSteps]]);
        }

        private static void WriteSteps()
        {
            while (true)
            {
                if (ECDH.StepsQueue.TryDequeue(out (string owner, string action) step))
                {
                    if (step.owner == string.Empty)
                        break;

                    if (step.owner == "Анна")
                    {
                        if (AnnaStepsCount == AnnaSteps.Length)
                        {
                            AnnaSteps = [.. AnnaSteps, step.action];
                            BorisSteps = [.. BorisSteps, ""];
                        }
                        else
                        {
                            AnnaSteps[AnnaStepsCount] = step.action;
                        }

                        AnnaStepsCount++;
                    }
                    else
                    {
                        if (BorisStepsCount == BorisSteps.Length)
                        {
                            AnnaSteps = [.. AnnaSteps, ""];
                            BorisSteps = [.. BorisSteps, step.action];
                        }
                        else
                        {
                            BorisSteps[BorisStepsCount] = step.action;
                        }

                        BorisStepsCount++;
                    }

                    Console.Clear();

                    ConsoleVisualizer.PrintParameters();
                    ConsoleVisualizer.DrawTable("Выполнение ECDH", null, [0], [[["Анна"], ["Борис"]], [AnnaSteps, BorisSteps]]);
                }
            }
        }
    }
}
