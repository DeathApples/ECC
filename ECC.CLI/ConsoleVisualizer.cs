using ECC.Core;

namespace ECC.CLI
{
    internal static class ConsoleVisualizer
    {
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
            {
                Console.Write("└");

                for (var i = 0; i < columns - 1; i++)
                    Console.Write("{0}{1}", new string('─', columnWidth), "┴");

                Console.WriteLine("{0}┘\n", new string('─', lastColumnWidth));
            }
        }

        internal static void DrawTable(string header, string? footer, int[] centerRows, params string[][][] cells)
        {
            var width = 118;
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

        internal static void PrintParameters()
        {
            DrawTable("Параметры эллиптической кривой", null, [], [[[
                $"Эллиптическая кривая E: y^2 = x^3{ECurve.Formula}",
                $"Количество точек на кривой: {ECurve.Order}",
                $"Количество точек подгруппы, генерируемой точкой G{EllipticCurve.G}: {EllipticCurve.G.Order}"
            ]]]);
        }
    }
}
