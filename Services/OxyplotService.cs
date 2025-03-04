using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using ECDH.Models;

namespace ECDH.Services
{
    public static class OxyplotService
    {
        public static PlotModel CreatePlotModel(int p = 0)
        {
            var curvePlotModel = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0) };
            curvePlotModel.Axes.Clear();

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = p == 0 ? 7 : p,
                Minimum = p == 0 ? -7 : 0,
                Position = AxisPosition.Left,
                PositionAtZeroCrossing = p == 0,
                AxislineStyle = LineStyle.Solid,
                MajorGridlineStyle = p == 0 ? LineStyle.None : LineStyle.Solid,
                TickStyle = TickStyle.Crossing,
                TextColor = OxyColor.FromRgb(240, 240, 240),
                AxislineColor = OxyColor.FromRgb(140, 140, 140),
                TicklineColor = OxyColor.FromRgb(180, 180, 180),
                MajorGridlineColor = OxyColor.FromRgb(100, 100, 100)
            });

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = p == 0 ? 7 : p,
                Minimum = p == 0 ? -7 : 0,
                Position = AxisPosition.Bottom,
                PositionAtZeroCrossing = p == 0,
                AxislineStyle = LineStyle.Solid,
                MajorGridlineStyle = p == 0 ? LineStyle.None : LineStyle.Solid,
                TickStyle = TickStyle.Crossing,
                TextColor = OxyColor.FromRgb(240, 240, 240),
                AxislineColor = OxyColor.FromRgb(140, 140, 140),
                TicklineColor = OxyColor.FromRgb(180, 180, 180),
                MajorGridlineColor = OxyColor.FromRgb(100, 100, 100)
            });

            return curvePlotModel;
        }

        public static void DrawEllipticCurve(PlotModel curvePlotModel, EllipticCurve ellipticCurve)
        {
            curvePlotModel.Series.Clear();

            double temp, size = 10;
            int a = (int)ellipticCurve.a, b = (int)ellipticCurve.b;
            var lineSeries = new LineSeries { Color = OxyColors.Orange, StrokeThickness = 2 };

            if (ellipticCurve.Discriminant >= 0)
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
                lineSeries = new LineSeries { Color = OxyColors.Orange, StrokeThickness = 2 };

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

        public static void DrawPointTable(PlotModel tablePlotModel, EllipticCurve ellipticCurve)
        {
            tablePlotModel.Series.Clear();

            var scatterSeries = new ScatterSeries
            {
                MarkerFill = OxyColors.Transparent, 
                MarkerStroke = OxyColors.Orange,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3
            };

            int a = (int)ellipticCurve.a, b = (int)ellipticCurve.b, p = (int)ellipticCurve.p;
            var leftSide = new List<int>(); var rightSide = new List<int>();
            for (int i = 0; i < p; i++)
            {
                leftSide.Add(i * i % p);
                rightSide.Add((i * i * i + a * i + b) % p );
            }

            for (int i = 0; i < p; i++)
            {
                for (int k = 0; k < p; k++)
                {
                    if (leftSide[i] == rightSide[k])
                        scatterSeries.Points.Add(new ScatterPoint(k, i));
                }
            }

            tablePlotModel.Series.Add(scatterSeries);
        }
    }
}
