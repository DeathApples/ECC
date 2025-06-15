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
            ECDH.Execute(EllipticCurve.G, delay);

            writer.Wait();
        }

        private static void WriteSteps()
        {
            while (true)
            {
                if (ECDH.StepsQueue.TryDequeue(out (string owner, string action) step))
                {
                    if (step.owner == string.Empty)
                    {
                        Console.SetCursorPosition(0, 0);
                        ConsoleVisualizer.PrintParameters();
                        ConsoleVisualizer.DrawTable("Выполнение ECDH", step.action, [0], [[["Анна"], ["Борис"]], [AnnaSteps, BorisSteps]]);

                        break;
                    }

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
                    ConsoleVisualizer.DrawTable("Выполнение ECDH", null, [0], [[["Анна"], ["Борис"]], [AnnaSteps, BorisSteps]]);
                }
            }
        }
    }
}
