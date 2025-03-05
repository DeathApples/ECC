using OxyPlot;
using System.Windows.Input;

using ECDH.Models;
using ECDH.Commands;
using ECDH.Services;

namespace ECDH.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private string? _parameterA;
        public string? ParameterA
        {
            get => _parameterA;
            set => SetProperty(ref _parameterA, value);
        }

        private string? _parameterB;
        public string? ParameterB
        {
            get => _parameterB;
            set => SetProperty(ref _parameterB, value);
        }

        private string? _primeNumber;
        public string? PrimeNumber
        {
            get => _primeNumber;
            set => SetProperty(ref _primeNumber, value);
        }

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

        private PlotModel _tablePlotModel;
        public PlotModel TablePlotModel
        {
            get => _tablePlotModel;
            set => SetProperty(ref _tablePlotModel, value);
        }

        private bool _isVisualization;
        public bool IsVisualization
        {
            get => _isVisualization;
            set => SetProperty(ref _isVisualization, value);
        }

        public ICommand GenerateParametrsCommand { get; }
        private void OnGenerateParametrsCommandExecuted(object? parameter)
        {
            var rnd = new Random();
            ParameterA = $"{rnd.Next(32) - 16}";
            ParameterB = $"{rnd.Next(32) - 16}";
        }

        public ICommand GeneratePrimeCommand { get; }
        private void OnGeneratePrimeCommandExecuted(object? parameter)
        {
            var primeGenerator = new MillerRabinPrimeGenerator();
            PrimeNumber = primeGenerator.GeneratePrimeNumber(false).ToString();
        }

        public ICommand CreateEllipticCurveCommand { get; }
        private void OnCreateEllipticCurveCommandExecuted(object? parameter)
        {
            if (!int.TryParse(ParameterA, out var a))
                return;

            if (!int.TryParse(ParameterB, out var b))
                return;

            EllipticCurve.a = a;
            EllipticCurve.b = b;

            var curvePlotModel = OxyplotService.CreatePlotModel();
            OxyplotService.DrawEllipticCurve(curvePlotModel, EllipticCurve);

            CurvePlotModel = curvePlotModel;
        }

        public ICommand CreatePointTableCommand { get; }
        private void OnCreatePointTableCommandExecuted(object? parameter)
        {
            OnCreateEllipticCurveCommandExecuted(null);

            if (!int.TryParse(PrimeNumber, out var p))
                return;

            var primeGenerator = new MillerRabinPrimeGenerator();
            if (!primeGenerator.IsPrimeNumber(p))
                return;

            EllipticCurve.p = p;
            var tablePlotModel = OxyplotService.CreatePlotModel(p);
            OxyplotService.DrawPointTable(tablePlotModel, EllipticCurve);

            TablePlotModel = tablePlotModel;
        }

        public ICommand ActionCommand { get; }
        private void OnActionCommandExecuted(object? parameter)
        {
            var primeGenerator = new MillerRabinPrimeGenerator();
            PrimeNumber = primeGenerator.GeneratePrimeNumber().ToString();
        }

        public MainViewModel()
        {
            ActionCommand = new RelayCommand(OnActionCommandExecuted);
            GeneratePrimeCommand = new RelayCommand(OnGeneratePrimeCommandExecuted);
            CreatePointTableCommand = new RelayCommand(OnCreatePointTableCommandExecuted);
            GenerateParametrsCommand = new RelayCommand(OnGenerateParametrsCommandExecuted);
            CreateEllipticCurveCommand = new RelayCommand(OnCreateEllipticCurveCommandExecuted);

            _curvePlotModel = OxyplotService.CreatePlotModel();
            _tablePlotModel = OxyplotService.CreatePlotModel(19);

            _ellipticCurve = EllipticCurve.Instance;
            _ellipticCurve.a = -7; _parameterA = "-7";
            _ellipticCurve.b = 10; _parameterB = "10";
            _ellipticCurve.p = 19; _primeNumber = "19";

            OxyplotService.DrawPointTable(TablePlotModel, EllipticCurve);
            OxyplotService.DrawEllipticCurve(CurvePlotModel, EllipticCurve);
        }
    }
}
