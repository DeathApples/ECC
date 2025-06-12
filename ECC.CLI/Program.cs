using ECC.Core;
using System.Numerics;

namespace ECC.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ExecuteECDH();
            ExecuteECDSA("Привет, мир!");
        }

        static void ExecuteECDH()
        {
            ECPoint G;
            BigInteger a, b, p;

            do
            {
                a = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                b = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                p = Core.Algorithms.MillerRabinPrimalityTest.GetPrimeNumber();
            } while (!ECurve.GenerateEllipticCurve(a, b, p) || !ECurve.TryGetBasePoint(out G));

            DrawTable("Подготовка параметров для ECDH", null, [], [[[
                $"Эллиптическая кривая E: y^2 = x^3{ECurve.Formula}",
                $"Количество точек на кривой: {ECurve.Order}",
                $"Количество точек подгруппы, генерируемой точкой G{G}: {G.Order}"
            ]]]);

            var nA = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
            var nB = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
            var pA = nA * G;
            var pB = nB * G;
            var kA = nA * pB;
            var kB = nB * pA;

            DrawTable("Выполнение ECDH", kA == kB ? $"Анна и Борис получили общую секретную точку K = {kA}" : "Анна и Борис не смогли получить общую секретную точку", [0], [
                [["Анна"], ["Борис"]], [
                    [
                        $"Выбирает число n: {nA}",
                        $"Вычисляет точку P = n * G: {pA}",
                        $"Отправляет Борису точку P",
                        $"Получает от Бориса точку Q",
                        $"Вычисляет секретную точку K = n * Q: {kA}"
                    ],
                    [
                        $"Выбирает число m: {nB}",
                        $"Вычисляет точку Q = m * G: {pB}",
                        $"Отправляет Анне точку Q",
                        $"Получает от Анны точку P",
                        $"Вычисляет секретную точку K = m * P: {kB}"
                    ]
                ]
            ]);
        }

        static void ExecuteECDSA(string message)
        {
            ECPoint G;
            BigInteger a, b, p;

            do
            {
                a = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                b = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                p = Core.Algorithms.MillerRabinPrimalityTest.GetPrimeNumber();
            } while (!ECurve.GenerateEllipticCurve(a, b, p) || !ECurve.TryGetBasePoint(out G));

            DrawTable("Подготовка параметров для ECDSA", null, [], [[[
                $"Эллиптическая кривая E: y^2 = x^3{ECurve.Formula}",
                $"Количество точек на кривой: {ECurve.Order}",
                $"Количество точек подгруппы, генерируемой точкой G{G}: {G.Order}"
            ]]]);

            var privateKey = System.Security.Cryptography.RandomNumberGenerator.GetInt32(1, (int)G.Order);
            var publicKey = privateKey * G;
            var hashA = message.GetHashCode() % G.Order;
            var signature = ECDSA.GenerateDigitalSignature(G, privateKey, hashA, out BigInteger sessionKey);
            var hashB = message.GetHashCode() % G.Order;
            var isVerified = ECDSA.VerifyDigitalSignature(G, signature, publicKey, hashB, out ECPoint verificationPoint);

            DrawTable("Выполнение ECDSA", isVerified ? "Подпись верна" : "Подпись неверна", [0], [
                [["Анна"], ["Борис"]], [
                    [
                        $"Выбирает ключ шифрования: {privateKey}",
                        $"Вычисляет ключ расшифрования: {publicKey}",
                        $"Вычисляет хеш сообщения: {hashA}",
                        $"Выбирает сеансовый ключ: {sessionKey}",
                        $"Вычисляет цифровую подпись: {signature}",
                        ""
                    ],
                    [
                        "",
                        "",
                        $"Вычисляет хеш сообщения: {hashB}",
                        "",
                        $"Получает от Анны цифровую подпись {signature}",
                        $"Вычисляет точку проверки {verificationPoint}"
                    ]
                ]
            ]);
        }

        private static void DrawHeader(string text, int width, int columns)
        {
            Console.WriteLine("┌{0}┐", new string('─', width));

            var leftPadding = (width - text.Length) / 2;
            var rightPadding = width - text.Length - leftPadding;
            Console.WriteLine("│{0}{1}{2}│", new string(' ', leftPadding), text, new string(' ', rightPadding));

            var columnWidth = (width - columns + 1) / columns;
            var lastColumnWidth = width - (columns - 1) * (columnWidth + 1);
            
            Console.Write("├");

            for (var i = 0; i < columns - 1; i++)
                Console.Write("{0}{1}", new string('─', columnWidth), "┬");
            
            Console.WriteLine("{0}┤", new string('─', lastColumnWidth));
        }

        private static void DrawFooter(string text, int width, int columns)
        {
            var columnWidth = (width - columns + 1) / columns;
            var lastColumnWidth = width - (columns - 1) * (columnWidth + 1);

            Console.Write("├");
            
            for (var i = 0; i < columns - 1; i++)
                Console.Write("{0}{1}", new string('─', columnWidth), "┴");
            
            Console.WriteLine("{0}┤", new string('─', lastColumnWidth));

            var leftPadding = (width - text.Length) / 2;
            var rightPadding = width - text.Length - leftPadding;
            Console.WriteLine("│{0}{1}{2}│", new string(' ', leftPadding), text, new string(' ', rightPadding));

            Console.WriteLine("└{0}┘\n", new string('─', width));
        }

        private static void DrawRows(string[][][] cells, int[] centerRows, int width, bool hasFooter)
        {
            var rows = cells.Length;
            var columns = cells[0].Length;

            var columnWidth = (width - columns + 1) / columns;
            var lastColumnWidth = width - (columns - 1) * (columnWidth + 1);

            for (var row = 0; row < rows; row++)
            {
                for (var textLine = 0; textLine < cells[row].Max(x => x.Length); textLine++)
                {
                    Console.Write("│");

                    if (centerRows.Contains(row))
                    {
                        int leftPadding, rightPadding;
                        string text;

                        for (var column = 0; column < columns - 1; column++)
                        {
                            text = cells[row][column][textLine];
                            leftPadding = (columnWidth - text.Length) / 2;
                            rightPadding = columnWidth - text.Length - leftPadding;
                            Console.Write("{0}{1}{2}│", new string(' ', leftPadding), text, new string(' ', rightPadding));
                        }

                        text = cells[row][^1][textLine];
                        leftPadding = (lastColumnWidth - text.Length) / 2;
                        rightPadding = lastColumnWidth - text.Length - leftPadding;
                        Console.WriteLine("{0}{1}{2}│", new string(' ', leftPadding), text, new string(' ', rightPadding));
                    }

                    else
                    {
                        for (var column = 0; column < columns - 1; column++)
                            Console.Write("{0}│", cells[row][column][textLine].PadRight(columnWidth));

                        Console.WriteLine("{0}│", cells[row][^1][textLine].PadRight(lastColumnWidth));
                    }
                }

                if (row < rows - 1)
                {
                    Console.Write("├");

                    for (var i = 0; i < columns - 1; i++)
                        Console.Write("{0}{1}", new string('─', columnWidth), "┼");

                    Console.WriteLine("{0}┤", new string('─', lastColumnWidth));
                }
            }

            if (!hasFooter)
                Console.WriteLine("└{0}┘\n", new string('─', width));
        }

        private static void DrawTable(string header, string? footer, int[] centerRows, params string[][][] cells)
        {
            var width = 110;
            var columns = cells[0].Length;

            DrawHeader(header, width, columns);

            if (footer != null)
            {
                DrawRows(cells, centerRows, width, true);
                DrawFooter(footer, width, columns);
            }

            else
                DrawRows(cells, centerRows, width, false);
        }
    }
}
