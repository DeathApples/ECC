using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

using ECDH.Models;

namespace ECDH.Services
{
    public static class OxyplotService
    {
        public static PlotModel CreatePlotModel()
        {
            var curvePlotModel = new PlotModel();
            curvePlotModel.PlotAreaBorderThickness = new OxyThickness(0);
            curvePlotModel.Axes.Clear();

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = 6,
                Minimum = -6,
                Position = AxisPosition.Left,
                PositionAtZeroCrossing = true,
                AxislineStyle = LineStyle.Solid,
                TickStyle = TickStyle.Crossing,
                TextColor = OxyColor.FromRgb(240, 240, 240),
                AxislineColor = OxyColor.FromRgb(140, 140, 140),
                TicklineColor = OxyColor.FromRgb(180, 180, 180),
                MajorGridlineColor = OxyColor.FromRgb(100, 100, 100)
            });

            curvePlotModel.Axes.Add(new LinearAxis
            {
                Maximum = 6,
                Minimum = -6,
                Position = AxisPosition.Bottom,
                PositionAtZeroCrossing = true,
                AxislineStyle = LineStyle.Solid,
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

            if (ellipticCurve.Discriminant > 0)
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
    }
}
