using OxyPlot;
using System.Windows.Input;

using ECDH.Models;
using ECDH.Commands;
using ECDH.Services;
using System.Numerics;

namespace ECDH.ViewModels
{
    internal class VisualViewModel : BaseViewModel
    {
        private string? _parameterA;
        public string? ParameterA
        {
            get => _parameterA;
            set
            {
                if (BigInteger.TryParse(value, out var a))
                {
                    EllipticCurve.A = a;
                    CreateGraphics();
                }
                
                SetProperty(ref _parameterA, value);
            }
        }

        private string? _parameterB;
        public string? ParameterB
        {
            get => _parameterB;
            set
            {
                if (BigInteger.TryParse(value, out var b))
                {
                    EllipticCurve.B = b;
                    CreateGraphics();
                }

                SetProperty(ref _parameterB, value);
            }
        }

        private string? _primeNumber;
        public string? PrimeNumber
        {
            get => _primeNumber;
            set
            {
                if (BigInteger.TryParse(value, out var p))
                {
                    EllipticCurve.Prime = p;
                    CreateGraphics();
                }

                SetProperty(ref _primeNumber, value);
            }
        }

        private string? _formulaEllipticCurve;
        public string? FormulaEllipticCurve
        {
            get => _formulaEllipticCurve;
            set => SetProperty(ref _formulaEllipticCurve, value);
        }

        private readonly EllipticCurvePoint _pointP;

        private string? _pointPx;
        public string? PointPx
        {
            get => _pointPx;
            set
            {
                if (BigInteger.TryParse(value, out var x))
                {
                    _pointP.X = new(x, EllipticCurve.Prime);
                    AddPoint();
                }

                SetProperty(ref _pointPx, value);
            }
        }

        private string? _pointPy;
        public string? PointPy
        {
            get => _pointPy;
            set
            {
                if (BigInteger.TryParse(value, out var y))
                {
                    _pointP.Y = new(y, EllipticCurve.Prime);
                    AddPoint();
                }

                SetProperty(ref _pointPy, value);
            }
        }

        private readonly EllipticCurvePoint _pointQ;

        private string? _pointQx;
        public string? PointQx
        {
            get => _pointQx;
            set
            {
                if (int.TryParse(value, out var x))
                {
                    _pointQ.X = new(x, EllipticCurve.Prime);
                    AddPoint();
                }

                SetProperty(ref _pointQx, value);
            }
        }

        private string? _pointQy;
        public string? PointQy
        {
            get => _pointQy;
            set
            {
                if (int.TryParse(value, out var y))
                {
                    _pointQ.Y = new(y, EllipticCurve.Prime);
                    AddPoint();
                }

                SetProperty(ref _pointQy, value);
            }
        }

        private EllipticCurvePoint _pointR;

        private string? _pointRx;
        public string? PointRx
        {
            get => _pointRx;
            set => SetProperty(ref _pointRx, value);
        }

        private string? _pointRy;
        public string? PointRy
        {
            get => _pointRy;
            set => SetProperty(ref _pointRy, value);
        }

        private int _n;

        private string? _multiplier;
        public string? Multiplier
        {
            get => _multiplier;
            set
            {
                if (int.TryParse(value, out var n))
                {
                    _n = n;
                    MultiplyPoint();
                }

                SetProperty(ref _multiplier, value);
            }
        }

        private readonly EllipticCurvePoint _pointS;

        private string? _pointSx;
        public string? PointSx
        {
            get => _pointSx;
            set
            {
                if (int.TryParse(value, out var x))
                {
                    _pointS.X = new(x, EllipticCurve.Prime);
                    MultiplyPoint();
                }

                SetProperty(ref _pointSx, value);
            }
        }

        private string? _pointSy;
        public string? PointSy
        {
            get => _pointSy;
            set
            {
                if (int.TryParse(value, out var y))
                {
                    _pointS.Y = new(y, EllipticCurve.Prime);
                    MultiplyPoint();
                }

                SetProperty(ref _pointSy, value);
            }
        }

        private EllipticCurvePoint _pointT;

        private string? _pointTx;
        public string? PointTx
        {
            get => _pointTx;
            set => SetProperty(ref _pointTx, value);
        }

        private string? _pointTy;
        public string? PointTy
        {
            get => _pointTy;
            set => SetProperty(ref _pointTy, value);
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

        public ICommand GenerateParametrsCommand { get; }
        private void OnGenerateParametrsCommandExecuted(object? parameter)
        {
            EllipticCurve.GenerateParameters(false);
            ParameterA = EllipticCurve.A.ToString();
            ParameterB = EllipticCurve.B.ToString();
        }

        public ICommand GeneratePrimeCommand { get; }
        private void OnGeneratePrimeCommandExecuted(object? parameter)
        {
            EllipticCurve.Prime = MillerRabinPrimalityTest.GetPrimeNumber(false);
            PrimeNumber = EllipticCurve.Prime.ToString();
        }

        public ICommand GeneratePointCommand { get; }
        private void OnGeneratePointCommandExecuted(object? parameter)
        {
            if (parameter is string pointName)
                GetRandomPoint(pointName);
        }

        public ICommand GenerateMultiplierCommand { get; }
        private void OnGenerateMultiplierCommandExecuted(object? parameter)
        {
            var rnd = new Random();
            Multiplier = rnd.Next(2, 100).ToString();
        }

        private void CreateGraphics()
        {
            if (EllipticCurve.Discriminant == 0)
                return;

            if (!MillerRabinPrimalityTest.IsPrimeNumber(EllipticCurve.Prime))
                return;

            var curvePlotModel = OxyplotService.CreatePlotModel();
            OxyplotService.DrawEllipticCurve(curvePlotModel);

            var tablePlotModel = OxyplotService.CreatePlotModel(true);
            OxyplotService.DrawPointTable(tablePlotModel);

            CurvePlotModel = curvePlotModel; TablePlotModel = tablePlotModel;
            FormulaEllipticCurve = EllipticCurve.ToString();
        }

        private void GetRandomPoint(string pointName)
        {
            var point = EllipticCurve.GetRandomPoint(false);
            switch (pointName)
            {
                case "P":
                    PointPx = point.IsInfinite ? "∞" : point.X.Value.ToString();
                    PointPy = point.IsInfinite ? "∞" : point.Y.Value.ToString();
                    break;

                case "Q":
                    PointQx = point.IsInfinite ? "∞" : point.X.Value.ToString();
                    PointQy = point.IsInfinite ? "∞" : point.Y.Value.ToString();
                    break;

                case "S":
                    PointSx = point.IsInfinite ? "∞" : point.X.Value.ToString();
                    PointSy = point.IsInfinite ? "∞" : point.Y.Value.ToString();
                    break;

                default:
                    break;
            }
        }

        private void AddPoint()
        {
            if(_pointP.IsOnCurve && _pointQ.IsOnCurve)
            {
                _pointR = _pointP + _pointQ;
                PointRx = _pointR.IsInfinite ? "∞" : _pointR.X.Value.ToString();
                PointRy = _pointR.IsInfinite ? "∞" : _pointR.Y.Value.ToString();
            }
        }

        private void MultiplyPoint()
        {
            if (_pointS.IsOnCurve)
            {
                _pointT = _n * _pointS;
                PointTx = _pointT.IsInfinite ? "∞" : _pointT.X.Value.ToString();
                PointTy = _pointT.IsInfinite ? "∞" : _pointT.Y.Value.ToString();
            }
        }

        public VisualViewModel()
        {
            GeneratePrimeCommand = new RelayCommand(OnGeneratePrimeCommandExecuted);
            GenerateParametrsCommand = new RelayCommand(OnGenerateParametrsCommandExecuted);
            GeneratePointCommand = new RelayCommand(OnGeneratePointCommandExecuted);
            GenerateMultiplierCommand = new RelayCommand(OnGenerateMultiplierCommandExecuted);

            EllipticCurve.A = 2; _parameterA = "2";
            EllipticCurve.B = 3; _parameterB = "3";
            EllipticCurve.Prime = 97; _primeNumber = "97";
            _formulaEllipticCurve = EllipticCurve.ToString();

            _curvePlotModel = OxyplotService.CreatePlotModel();
            _tablePlotModel = OxyplotService.CreatePlotModel(true);

            OxyplotService.DrawPointTable(TablePlotModel);
            OxyplotService.DrawEllipticCurve(CurvePlotModel);

            _pointQ = new(95, 31); _pointP = new(17, 10);
            _pointPx = _pointP.X.Value.ToString(); _pointPy = _pointP.Y.Value.ToString();
            _pointQx = _pointQ.X.Value.ToString(); _pointQy = _pointQ.Y.Value.ToString();

            _pointR = _pointP + _pointQ;
            _pointRx = _pointR.X.Value.ToString(); _pointRy = _pointR.Y.Value.ToString();

            _n = 2;  _pointS = new(3, 6);
            _multiplier = _n.ToString();
            _pointSx = _pointS.X.Value.ToString(); _pointSy = _pointS.Y.Value.ToString();

            _pointT = _n * _pointS;
            _pointTx = _pointT.X.Value.ToString(); _pointTy = _pointT.Y.Value.ToString();
        }
    }
}
