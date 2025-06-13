using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using ECC.Core;
using System.Numerics;

namespace ECC.GUI.Services
{
    public class OxyplotService
    {
        public static PlotModel CreatePlotModel(bool isPointTable = false)
        {
            var curvePlotModel = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0) };
            curvePlotModel.Axes.Clear();

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = isPointTable ? (int)ECurve.Prime : 7,
                Minimum = isPointTable ? 0 : -7,
                Position = AxisPosition.Left,
                PositionAtZeroCrossing = !isPointTable,
                AxislineStyle = LineStyle.Solid,
                MinorGridlineStyle = isPointTable ? LineStyle.Solid : LineStyle.None,
                MajorGridlineStyle = isPointTable ? LineStyle.Solid : LineStyle.None,
                TickStyle = TickStyle.Crossing,
                TextColor = OxyColor.FromRgb(240, 240, 240),
                AxislineColor = OxyColor.FromRgb(140, 140, 140),
                TicklineColor = OxyColor.FromRgb(180, 180, 180),
                MinorGridlineColor = OxyColor.FromRgb(70, 70, 70),
                MajorGridlineColor = OxyColor.FromRgb(100, 100, 100)
            });

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = isPointTable ? (int)ECurve.Prime : 7,
                Minimum = isPointTable ? 0 : -7,
                Position = AxisPosition.Bottom,
                PositionAtZeroCrossing = !isPointTable,
                AxislineStyle = LineStyle.Solid,
                MinorGridlineStyle = isPointTable ? LineStyle.Solid : LineStyle.None,
                MajorGridlineStyle = isPointTable ? LineStyle.Solid : LineStyle.None,
                TickStyle = TickStyle.Crossing,
                TextColor = OxyColor.FromRgb(240, 240, 240),
                AxislineColor = OxyColor.FromRgb(140, 140, 140),
                TicklineColor = OxyColor.FromRgb(180, 180, 180),
                MinorGridlineColor = OxyColor.FromRgb(70, 70, 70),
                MajorGridlineColor = OxyColor.FromRgb(100, 100, 100)
            });

            return curvePlotModel;
        }

        public static void DrawEllipticCurve(PlotModel curvePlotModel)
        {
            curvePlotModel.Series.Clear();

            double temp, size = 10;
            int a = (int)ECurve.A, b = (int)ECurve.B;
            var lineSeries = new LineSeries { Color = OxyColor.FromRgb(202, 111, 30), StrokeThickness = 2 };

            if (ECurve.Discriminant >= 0)
            {
                for (double i = size; i > -size; i -= 0.01)
                {
                    temp = Math.Sqrt(i * i * i + a * i + b);
                    if (!double.IsNaN(temp))
                        lineSeries.Points.Add(new DataPoint(i, temp));
                }

                for (double i = -size; i < size; i += 0.01)
                {
                    temp = -Math.Sqrt(i * i * i + a * i + b);
                    if (!double.IsNaN(temp))
                        lineSeries.Points.Add(new DataPoint(i, temp));
                }

                curvePlotModel.Series.Add(lineSeries);
            }
            else
            {
                double breakpoint = -size;

                for (double i = size; i > size / 2; i -= 0.01)
                {
                    temp = Math.Sqrt(i * i * i + a * i + b);
                    if (!double.IsNaN(temp))
                        lineSeries.Points.Add(new DataPoint(i, temp));
                }

                for (double i = size / 2; i > -size; i -= 0.01)
                {
                    temp = Math.Sqrt(i * i * i + a * i + b);
                    if (double.IsNaN(temp))
                    {
                        breakpoint = i;
                        break;
                    }

                    lineSeries.Points.Add(new DataPoint(i, temp));
                }

                for (double i = breakpoint; i < size; i += 0.01)
                {
                    temp = -Math.Sqrt(i * i * i + a * i + b);
                    if (!double.IsNaN(temp))
                        lineSeries.Points.Add(new DataPoint(i, temp));
                }

                curvePlotModel.Series.Add(lineSeries);
                lineSeries = new LineSeries { Color = OxyColor.FromRgb(202, 111, 30), StrokeThickness = 2 };

                for (double i = -size; i < breakpoint; i += 0.01)
                {
                    temp = Math.Sqrt(i * i * i + a * i + b);
                    if (!double.IsNaN(temp))
                        lineSeries.Points.Add(new DataPoint(i, temp));
                }

                for (double i = breakpoint; i > -size; i -= 0.01)
                {
                    temp = -Math.Sqrt(i * i * i + a * i + b);
                    if (!double.IsNaN(temp))
                        lineSeries.Points.Add(new DataPoint(i, temp));
                }

                lineSeries.Points.Add(lineSeries.Points[0]);
                curvePlotModel.Series.Add(lineSeries);
            }
        }

        public static void DrawPointTable(PlotModel tablePlotModel)
        {
            tablePlotModel.Series.Clear();

            var scatterSeries = new ScatterSeries
            {
                MarkerFill = OxyColors.Transparent, 
                MarkerStroke = OxyColor.FromRgb(202, 111, 30),
                MarkerType = MarkerType.Circle,
                MarkerSize = 3
            };

            BigInteger a = ECurve.A, b = ECurve.B, p = ECurve.Prime;

            for (BigInteger y = 0; y < p; y++)
            {
                var leftSide = y * y % p;

                if (leftSide < 0)
                    leftSide += p;

                for (BigInteger x = 0; x < p; x++)
                {
                    var rightSide = (x * x * x + a * x + b) % p;

                    if (rightSide < 0)
                        rightSide += p;

                    if (leftSide == rightSide)
                        scatterSeries.Points.Add(new ScatterPoint((int)x, (int)y));
                }
            }

            tablePlotModel.Series.Add(scatterSeries);
        }
    }
}
