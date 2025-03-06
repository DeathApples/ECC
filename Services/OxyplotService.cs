using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using ECDH.Models;

namespace ECDH.Services
{
    public static class OxyplotService
    {
        public static PlotModel CreatePlotModel(bool isPointTable = false)
        {
            var curvePlotModel = new PlotModel { PlotAreaBorderThickness = new OxyThickness(0) };
            curvePlotModel.Axes.Clear();

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = isPointTable ? (int)EllipticCurve.P : 7,
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
                Maximum = isPointTable ? (int)EllipticCurve.P : 7,
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
            int a = (int)EllipticCurve.A, b = (int)EllipticCurve.B;
            var lineSeries = new LineSeries { Color = OxyColors.Orange, StrokeThickness = 2 };

            if (EllipticCurve.Discriminant >= 0)
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

        public static void DrawPointTable(PlotModel tablePlotModel)
        {
            tablePlotModel.Series.Clear();

            var scatterSeries = new ScatterSeries
            {
                MarkerFill = OxyColors.Transparent, 
                MarkerStroke = OxyColors.Orange,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3
            };

            int a = (int)EllipticCurve.A, b = (int)EllipticCurve.B, p = (int)EllipticCurve.P;
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
