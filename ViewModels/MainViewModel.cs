using System;
using OxyPlot;
using System.Numerics;
using System.Windows.Input;

using ECDH.Models;
using ECDH.Commands;
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

        private BigInteger _primeNumber;
        public BigInteger PrimeNumber
        {
            get => _primeNumber;
            set => SetProperty(ref _primeNumber, value);
        }

        private PlotModel _curvePlotModel;
        public PlotModel CurvePlotModel
        {
            get => _curvePlotModel;
            set => SetProperty(ref _curvePlotModel, value);
        }

        public ICommand ActionCommand { get; }
        private void OnActionCommandExecuted(object? parameter)
        {
            //var rnd = new Random();
            //EllipticCurve = new EllipticCurve { a = rnd.Next(32) - 16, b = rnd.Next(32) - 16 };
            //var curvePlotModel = OxyplotService.CreatePlotModel();
            //OxyplotService.DrawEllipticCurve(curvePlotModel, EllipticCurve);

            //CurvePlotModel = curvePlotModel;

            var primeGenerator = new MillerRabinPrimeGenerator();
            PrimeNumber = primeGenerator.GeneratePrimeNumber();
        }

        public MainViewModel()
        {
            ActionCommand = new RelayCommand(OnActionCommandExecuted);

            _curvePlotModel = OxyplotService.CreatePlotModel();
            _ellipticCurve = new EllipticCurve { a = -2, b = 5 };

            OxyplotService.DrawEllipticCurve(CurvePlotModel, EllipticCurve);
        }
    }
}
