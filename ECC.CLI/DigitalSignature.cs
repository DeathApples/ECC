using ECC.Core;

namespace ECC.CLI
{
    internal static class DigitalSignature
    {
        private static string[] AnnaSteps = [];
        private static string[] BorisSteps = [];

        private static int AnnaStepsCount;
        private static int BorisStepsCount;

        internal static void Execute(string message, int delay = 0)
        {
            AnnaSteps = []; AnnaStepsCount = 0;
            BorisSteps = []; BorisStepsCount = 0;

            Task writer = Task.Run(WriteSteps);
            var isVerified = ECDSA.Execute(message, EllipticCurve.G, delay);

            writer.Wait();

            Console.SetCursorPosition(0, 0);
            ConsoleVisualizer.PrintParameters();

            var result = isVerified ? "Подпись верна" : "Подпись не верна";
            ConsoleVisualizer.DrawTable("Выполнение ECDSA", result, [0], [[["Анна"], ["Борис"]], [AnnaSteps, BorisSteps]]);
        }

        private static void WriteSteps()
        {
            while (true)
            {
                if (ECDSA.StepsQueue.TryDequeue(out (string owner, string action) step))
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

                    Console.SetCursorPosition(0, 0);

                    ConsoleVisualizer.PrintParameters();
                    ConsoleVisualizer.DrawTable("Выполнение ECDSA", null, [0], [[["Анна"], ["Борис"]], [AnnaSteps, BorisSteps]]);
                }
            }
        }
    }
}
