using OxyPlot;
using System.Windows.Input;

using ECDH.Models;
using ECDH.Commands;
using ECDH.Services;

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
                if (int.TryParse(value, out var a))
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
                if (int.TryParse(value, out var b))
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
                if (int.TryParse(value, out var p))
                {
                    EllipticCurve.P = p;
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

        private readonly Point _pointP;

        private string? _pointPx;
        public string? PointPx
        {
            get => _pointPx;
            set
            {
                if (int.TryParse(value, out var x))
                {
                    _pointP.X = x;
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
                if (int.TryParse(value, out var y))
                {
                    _pointP.Y = y;
                    AddPoint();
                }

                SetProperty(ref _pointPy, value);
            }
        }

        private readonly Point _pointQ;

        private string? _pointQx;
        public string? PointQx
        {
            get => _pointQx;
            set
            {
                if (int.TryParse(value, out var x))
                {
                    _pointQ.X = x;
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
                    _pointQ.Y = y;
                    AddPoint();
                }

                SetProperty(ref _pointQy, value);
            }
        }

        private Point _pointR;

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

        private readonly Point _pointS;

        private string? _pointSx;
        public string? PointSx
        {
            get => _pointSx;
            set
            {
                if (int.TryParse(value, out var x))
                {
                    _pointS.X = x;
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
                    _pointS.Y = y;
                    MultiplyPoint();
                }

                SetProperty(ref _pointSy, value);
            }
        }

        private Point _pointT;

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
            EllipticCurve.GeneratePrimeNumber(false);
            PrimeNumber = EllipticCurve.P.ToString();
        }

        public ICommand GeneratePointCommand { get; }
        private void OnGeneratePointCommandExecuted(object? parameter)
        {
        }

        public ICommand GenerateMultiplierCommand { get; }
        private void OnGenerateMultiplierCommandExecuted(object? parameter)
        {
        }

        private void CreateGraphics()
        {
            if (EllipticCurve.Discriminant == 0)
                return;

            var primalityTest = new MillerRabinPrimalityTest();
            if (!primalityTest.IsPrimeNumber(EllipticCurve.P))
                return;

            var curvePlotModel = OxyplotService.CreatePlotModel();
            OxyplotService.DrawEllipticCurve(curvePlotModel);

            var tablePlotModel = OxyplotService.CreatePlotModel(true);
            OxyplotService.DrawPointTable(tablePlotModel);

            CurvePlotModel = curvePlotModel; TablePlotModel = tablePlotModel;
            FormulaEllipticCurve = EllipticCurve.ToString();
        }

        private void AddPoint()
        {
            _pointR = _pointQ + _pointQ;
            PointRx = _pointR.X.ToString();
            PointRy = _pointR.Y.ToString();
        }

        private void MultiplyPoint()
        {
            _pointT = _n * _pointS;
            PointTx = _pointT.X.ToString();
            PointTy = _pointT.Y.ToString();
        }

        public VisualViewModel()
        {
            GeneratePrimeCommand = new RelayCommand(OnGeneratePrimeCommandExecuted);
            GenerateParametrsCommand = new RelayCommand(OnGenerateParametrsCommandExecuted);
            GeneratePointCommand = new RelayCommand(OnGeneratePointCommandExecuted);
            GenerateMultiplierCommand = new RelayCommand(OnGenerateMultiplierCommandExecuted);

            EllipticCurve.A = 2; _parameterA = "2";
            EllipticCurve.B = 3; _parameterB = "3";
            EllipticCurve.P = 97; _primeNumber = "97";
            _formulaEllipticCurve = EllipticCurve.ToString();

            _curvePlotModel = OxyplotService.CreatePlotModel();
            _tablePlotModel = OxyplotService.CreatePlotModel(true);

            OxyplotService.DrawPointTable(TablePlotModel);
            OxyplotService.DrawEllipticCurve(CurvePlotModel);

            _pointQ = new(95, 31); _pointP = new(17, 10);
            _pointPx = _pointP.X.ToString(); _pointPy = _pointP.Y.ToString();
            _pointQx = _pointQ.X.ToString(); _pointQy = _pointQ.Y.ToString();

            _pointR = _pointP + _pointQ;
            _pointRx = _pointR.X.ToString(); _pointRy = _pointR.Y.ToString();

            _n = 2;  _pointS = new(3, 6);
            _multiplier = _n.ToString();
            _pointSx = _pointS.X.ToString(); _pointSy = _pointS.Y.ToString();

            _pointT = _n * _pointS;
            _pointTx = _pointT.X.ToString(); _pointTy = _pointT.Y.ToString();
        }
    }
}
