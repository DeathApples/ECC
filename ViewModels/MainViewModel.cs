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

        private string? _formulaEllipticCurve;
        public string? FormulaEllipticCurve
        {
            get => _formulaEllipticCurve;
            set => SetProperty(ref _formulaEllipticCurve, value);
        }

        private string? _actionResult;
        public string? ActionResult
        {
            get => _actionResult;
            set => SetProperty(ref _actionResult, value);
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

            EllipticCurve.A = a; EllipticCurve.B = b;

            var curvePlotModel = OxyplotService.CreatePlotModel();
            OxyplotService.DrawEllipticCurve(curvePlotModel);

            CurvePlotModel = curvePlotModel;
            FormulaEllipticCurve = EllipticCurve.ToString();
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

            EllipticCurve.P = p;
            var tablePlotModel = OxyplotService.CreatePlotModel(true);
            OxyplotService.DrawPointTable(tablePlotModel);

            TablePlotModel = tablePlotModel;
        }

        public ICommand ActionCommand { get; }
        private void OnActionCommandExecuted(object? parameter)
        {
            var point = new Point(0, 10);
            ActionResult = $"{5} * {point} = {5 * point}";
        }

        public MainViewModel()
        {
            ActionCommand = new RelayCommand(OnActionCommandExecuted);
            GeneratePrimeCommand = new RelayCommand(OnGeneratePrimeCommandExecuted);
            CreatePointTableCommand = new RelayCommand(OnCreatePointTableCommandExecuted);
            GenerateParametrsCommand = new RelayCommand(OnGenerateParametrsCommandExecuted);
            CreateEllipticCurveCommand = new RelayCommand(OnCreateEllipticCurveCommandExecuted);

            _curvePlotModel = OxyplotService.CreatePlotModel();
            _tablePlotModel = OxyplotService.CreatePlotModel(true);

            EllipticCurve.A = 2; _parameterA = "2";
            EllipticCurve.B = 3; _parameterB = "3";
            EllipticCurve.P = 97; _primeNumber = "97";
            FormulaEllipticCurve = EllipticCurve.ToString();

            OxyplotService.DrawPointTable(TablePlotModel);
            OxyplotService.DrawEllipticCurve(CurvePlotModel);
        }
    }
}
