using System;
using OxyPlot;

using ECDH.Models;
using ECDH.Services;

namespace ECDH.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private EllipticCurve _ellipticCurve;
        public EllipticCurve EllipticCurve
        {
            get => _ellipticCurve;
            set => SetProperty(ref _ellipticCurve, value);
        }

        private PlotModel _curvePlotModel;
        public PlotModel CurvePlotModel
        {
            get => _curvePlotModel;
            set => SetProperty(ref _curvePlotModel, value);
        }

        public MainViewModel()
        {
            _ellipticCurve = new EllipticCurve { a = -4, b = 1 };
            _curvePlotModel = OxyplotService.CreatePlotModel();

            OxyplotService.DrawEllipticCurve(_curvePlotModel, _ellipticCurve);
        }
    }
}
