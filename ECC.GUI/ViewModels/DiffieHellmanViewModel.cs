using ECC.Core;
using ECC.Core.Algorithms;
using ECC.GUI.Commands;
using ECC.GUI.Models;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace ECC.GUI.ViewModels
{
    internal class DiffieHellmanViewModel : BaseViewModel
    {
        private string? _parameterA;
        public string? ParameterA
        {
            get => _parameterA;
            set
            {
                if (BigInteger.TryParse(value, out var a) && a > -17 && a < 17 && ECurve.A != a)
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
                if (BigInteger.TryParse(value, out var b) && b > -17 && b < 17 && ECurve.B != b)
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
                if (BigInteger.TryParse(value, out var p) && p > 6 && p < 998 && ECurve.Prime != p)
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

        private string? _resultProtocol;
        public string? ResultProtocol
        {
            get => _resultProtocol;
            set
            {
                VisibilityResult = value?.Length != 0 ? Visibility.Visible : Visibility.Hidden;
                SetProperty(ref _resultProtocol, value);
            }
        }

        private Visibility _visibilityResult = Visibility.Hidden;
        public Visibility VisibilityResult
        {
            get => _visibilityResult;
            set => SetProperty(ref _visibilityResult, value);
        }

        private readonly ECPoint _pointG = new(0, 1);

        private string? _pointGx;
        public string? PointGx
        {
            get => _pointGx;
            set
            {
                if (int.TryParse(value, out var x) && _pointG.X != x)
                {
                    _pointG.X = x;
                    ChangeBasePoint();
                }

                SetProperty(ref _pointGx, value);
            }
        }

        private string? _pointGy;
        public string? PointGy
        {
            get => _pointGy;
            set
            {
                if (int.TryParse(value, out var y) && _pointG.Y != y)
                {
                    _pointG.Y = y;
                    ChangeBasePoint();
                }

                SetProperty(ref _pointGy, value);
            }
        }

        private string? _orderPointG;
        public string? OrderPointG
        {
            get => _orderPointG;
            set => SetProperty(ref _orderPointG, value);
        }

        private List<DelayItem> _delayList = [new(), new(500), new(1000), new(1500)];
        public List<DelayItem> DelayList
        {
            get => _delayList;
            set => SetProperty(ref _delayList, value);
        }

        private DelayItem _selectedDelay = new();
        public DelayItem SelectedDelay
        {
            get => _selectedDelay;
            set => SetProperty(ref _selectedDelay, value);
        }

        private readonly object _AnnaLock = new();
        private readonly object _BorisLock = new();

        public ObservableCollection<string> AnnaSteps { get; } = [];
        public ObservableCollection<string> BorisSteps { get; } = [];

        public ICommand GenerateParametrsCommand { get; }
        private void OnGenerateParametrsCommandExecuted(object? parameter)
        {
            BigInteger a, b;

            do
            {
                a = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
                b = System.Security.Cryptography.RandomNumberGenerator.GetInt32(-16, 16);
            } while (!ECurve.TryGenerateEllipticCurve(a, b, ECurve.Prime) || !ECurve.TryGetBasePoint(out ECPoint _) || ECurve.Prime == ECurve.Order);

            ParameterA = ECurve.A.ToString();
            ParameterB = ECurve.B.ToString();
        }

        public ICommand GeneratePrimeCommand { get; }
        private void OnGeneratePrimeCommandExecuted(object? parameter)
        {
            BigInteger p;

            do
            {
                p = MillerRabinPrimalityTest.GetPrimeNumber();
            } while (!ECurve.TryGenerateEllipticCurve(ECurve.A, ECurve.B, p) || !ECurve.TryGetBasePoint(out ECPoint _) || ECurve.Prime == ECurve.Order);

            PrimeNumber = ECurve.Prime.ToString();
        }

        public ICommand GenerateBasePointCommand { get; }
        private void OnGenerateBasePointCommandExecuted(object? parameter)
        {
            GenerateBasePoint();
        }

        public ICommand StartProtocolCommand { get; }
        private void OnStartProtocolCommandExecuted(object? parameter)
        {
            if (_pointG.Order >= ECurve.Prime / 3 && ECurve.Prime != ECurve.Order)
            {
                ResultProtocol = string.Empty;
                AnnaSteps.Clear(); BorisSteps.Clear();

                Task.Run(WriteSteps);
                ECDH.Execute(_pointG, SelectedDelay.Value);
            }
        }

        private void EllipticCurveChangedHandle()
        {
            ParameterA = ECurve.A.ToString();
            ParameterB = ECurve.B.ToString();
            PrimeNumber = ECurve.Prime.ToString();
            FormulaEllipticCurve = $"y² = x³{ECurve.Formula}";
            SchoofOrderEllipticCurve = $"Порядок кривой (алгоритм Шуфа): {ECurve.Order}";
            FullEnumerationOrderEllipticCurve = $"Порядок кривой (полный перебор): {ECurve.Points.Count + 1}";

            GenerateBasePoint();
            ResultProtocol = string.Empty;
            AnnaSteps.Clear(); BorisSteps.Clear();
        }

        private void WriteSteps()
        {
            while (true)
            {
                if (ECDH.StepsQueue.TryDequeue(out (string owner, string action) step))
                {
                    if (step.owner == string.Empty)
                    {
                        ResultProtocol = step.action;
                        break;
                    }

                    if (step.owner == "Анна")
                        AnnaSteps.Add(step.action);
                    else
                        BorisSteps.Add(step.action);
                }
            }
        }

        private void GenerateBasePoint()
        {
            if (ECurve.TryGetBasePoint(out ECPoint basePoint))
            {
                PointGx = basePoint.X.ToString();
                PointGy = basePoint.Y.ToString();
            }
        }

        private void ChangeBasePoint()
        {
            if (_pointG.IsOnCurve)
            {
                ResultProtocol = string.Empty;
                AnnaSteps.Clear(); BorisSteps.Clear();

                _pointG.CalculateOrder();
                OrderPointG = $"Порядок подгруппы точки G равен {_pointG.Order}";
            }
        }

        public DiffieHellmanViewModel()
        {
            GeneratePrimeCommand = new RelayCommand(OnGeneratePrimeCommandExecuted);
            GenerateParametrsCommand = new RelayCommand(OnGenerateParametrsCommandExecuted);
            GenerateBasePointCommand = new RelayCommand(OnGenerateBasePointCommandExecuted);
            StartProtocolCommand = new RelayCommand(OnStartProtocolCommandExecuted);

            BindingOperations.EnableCollectionSynchronization(AnnaSteps, _AnnaLock);
            BindingOperations.EnableCollectionSynchronization(BorisSteps, _BorisLock);

            ECurve.EllipticCurveChanged += EllipticCurveChangedHandle;
            EllipticCurveChangedHandle();
        }
    }
}
