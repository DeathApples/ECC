using OxyPlot;
using System.Numerics;
using System.Windows.Input;

using ECC.Core;
using ECC.Core.Algorithms;
using ECC.GUI.Commands;
using ECC.GUI.Services;
using OxyPlot.Series;

namespace ECC.GUI.ViewModels
{
    internal class VisualViewModel : BaseViewModel
    {
        private string? _parameterA;
        public string? ParameterA
        {
            get => _parameterA;
            set
            {
                if (BigInteger.TryParse(value, out var a) && ECurve.A != a)
                    ECurve.A = a;

                SetProperty(ref _parameterA, value);
            }
        }

        private string? _parameterB;
        public string? ParameterB
        {
            get => _parameterB;
            set
            {
                if (BigInteger.TryParse(value, out var b) && ECurve.B != b)
                    ECurve.B = b;

                SetProperty(ref _parameterB, value);
            }
        }

        private string? _primeNumber;
        public string? PrimeNumber
        {
            get => _primeNumber;
            set
            {
                if (BigInteger.TryParse(value, out var p) && ECurve.Prime != p)
                    ECurve.Prime = p;

                SetProperty(ref _primeNumber, value);
            }
        }

        private string? _formulaEllipticCurve;
        public string? FormulaEllipticCurve
        {
            get => _formulaEllipticCurve;
            set => SetProperty(ref _formulaEllipticCurve, value);
        }

        private string? _schoofOrderEllipticCurve;
        public string? SchoofOrderEllipticCurve
        {
            get => _schoofOrderEllipticCurve;
            set => SetProperty(ref _schoofOrderEllipticCurve, value);
        }

        private string? _fullEnumerationOrderEllipticCurve;
        public string? FullEnumerationOrderEllipticCurve
        {
            get => _fullEnumerationOrderEllipticCurve;
            set => SetProperty(ref _fullEnumerationOrderEllipticCurve, value);
        }

        private readonly ECPoint _pointP;

        private string? _pointPx;
        public string? PointPx
        {
            get => _pointPx;
            set
            {
                if (BigInteger.TryParse(value, out var x))
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
                if (BigInteger.TryParse(value, out var y))
                {
                    _pointP.Y = y;
                    AddPoint();
                }

                SetProperty(ref _pointPy, value);
            }
        }

        private readonly ECPoint _pointQ;

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

        private ECPoint _pointR;

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

        private readonly ECPoint _pointS;

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

        private string? _orderPointS;
        public string? OrderPointS
        {
            get => _orderPointS;
            set => SetProperty(ref _orderPointS, value);
        }

        private ECPoint _pointT;

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
            BigInteger a, b;

            do
            {
                a = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                b = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
            } while (!ECurve.GenerateEllipticCurve(a, b, ECurve.Prime));

            ParameterA = ECurve.A.ToString();
            ParameterB = ECurve.B.ToString();
        }

        public ICommand GeneratePrimeCommand { get; }
        private void OnGeneratePrimeCommandExecuted(object? parameter)
        {
            PrimeNumber = MillerRabinPrimalityTest.GetPrimeNumber().ToString();
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
            Multiplier = System.Security.Cryptography.RandomNumberGenerator.GetInt32(2, 100).ToString();
        }

        private void EllipticCurveChangedHandle()
        {
            var curvePlotModel = OxyplotService.CreatePlotModel();
            OxyplotService.DrawEllipticCurve(curvePlotModel);

            var tablePlotModel = OxyplotService.CreatePlotModel(true);
            OxyplotService.DrawPointTable(tablePlotModel);

            CurvePlotModel = curvePlotModel; TablePlotModel = tablePlotModel;
            FormulaEllipticCurve = $"y² = x³{ECurve.Formula}";
            SchoofOrderEllipticCurve = $"Порядок кривой (алгоритм Шуфа): {ECurve.Order}";

            var series = (ScatterSeries)TablePlotModel.Series.First();
            FullEnumerationOrderEllipticCurve = $"Порядок кривой (полный перебор): {series.Points.Count + 1}";

            GetRandomPoint("P"); GetRandomPoint("Q"); GetRandomPoint("S");
        }

        private void GetRandomPoint(string pointName)
        {
            ECPoint point;
            BigInteger x, y;

            do
            {
                x = System.Security.Cryptography.RandomNumberGenerator.GetInt32((int)ECurve.Prime);
                y = System.Security.Cryptography.RandomNumberGenerator.GetInt32((int)ECurve.Prime);
                point = new(x, y);
            } while (!point.IsOnCurve);

            switch (pointName)
            {
                case "P":
                    PointPx = point.X.ToString();
                    PointPy = point.Y.ToString();
                    break;

                case "Q":
                    PointQx = point.X.ToString();
                    PointQy = point.Y.ToString();
                    break;

                case "S":
                    PointSx = point.X.ToString();
                    PointSy = point.Y.ToString();
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
                PointRx = _pointR.IsInfinite ? "∞" : _pointR.X.ToString();
                PointRy = _pointR.IsInfinite ? "∞" : _pointR.Y.ToString();
            }
        }

        private void MultiplyPoint()
        {
            if (_pointS.IsOnCurve)
            {
                _pointT = _n * _pointS;
                PointTx = _pointT.IsInfinite ? "∞" : _pointT.X.ToString();
                PointTy = _pointT.IsInfinite ? "∞" : _pointT.Y.ToString();

                _pointS.CalculateOrder();
                OrderPointS = $"Порядок подгруппы, генерируемой точкой P равен {_pointS.Order}";
            }
        }

        public VisualViewModel()
        {
            GeneratePrimeCommand = new RelayCommand(OnGeneratePrimeCommandExecuted);
            GenerateParametrsCommand = new RelayCommand(OnGenerateParametrsCommandExecuted);
            GeneratePointCommand = new RelayCommand(OnGeneratePointCommandExecuted);
            GenerateMultiplierCommand = new RelayCommand(OnGenerateMultiplierCommandExecuted);

            ECurve.EllipticCurveChanged += EllipticCurveChangedHandle;

            _parameterA = ECurve.A.ToString();
            _parameterB = ECurve.B.ToString();
            _primeNumber = ECurve.Prime.ToString();
            _formulaEllipticCurve = $"y² = x³{ECurve.Formula}";
            _schoofOrderEllipticCurve = $"Порядок кривой (алгоритм Шуфа): {ECurve.Order}";

            _curvePlotModel = OxyplotService.CreatePlotModel();
            _tablePlotModel = OxyplotService.CreatePlotModel(true);

            OxyplotService.DrawPointTable(TablePlotModel);
            OxyplotService.DrawEllipticCurve(CurvePlotModel);

            var series = (ScatterSeries)TablePlotModel.Series.First();
            _fullEnumerationOrderEllipticCurve = $"Порядок кривой (полный перебор): {series.Points.Count + 1}";

            _pointQ = new(95, 31); _pointP = new(17, 10);
            _pointPx = _pointP.X.ToString(); _pointPy = _pointP.Y.ToString();
            _pointQx = _pointQ.X.ToString(); _pointQy = _pointQ.Y.ToString();

            _pointR = _pointP + _pointQ;
            _pointRx = _pointR.X.ToString(); _pointRy = _pointR.Y.ToString();

            _n = 2;  _pointS = new(3, 6);
            _multiplier = _n.ToString();
            _pointSx = _pointS.X.ToString(); _pointSy = _pointS.Y.ToString();
            _orderPointS = $"Порядок подгруппы, генерируемой точкой P равен 5";

            _pointT = _n * _pointS;
            _pointTx = _pointT.X.ToString(); _pointTy = _pointT.Y.ToString();
        }
    }
}
