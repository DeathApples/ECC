using ECDH.Models;
using ECDH.Services;
using ECDH.Commands;
using System.Windows.Input;

namespace ECDH.ViewModels
{
    internal class DiffieHellmanViewModel : BaseViewModel
    {
        private string? _formulaEllipticCurve;
        public string? FormulaEllipticCurve
        {
            get => _formulaEllipticCurve;
            set => SetProperty(ref _formulaEllipticCurve, value);
        }

        private string? _primeNumber;
        public string? PrimeNumber
        {
            get => _primeNumber;
            set => SetProperty(ref _primeNumber, value);
        }

        private string? _pointsNumber;
        public string? PointsNumber
        {
            get => _pointsNumber;
            set => SetProperty(ref _pointsNumber, value);
        }

        public ICommand GetPointsNumberCommand { get; }
        private void OnGetPointsNumberCommandExecuted(object? parameter)
        {
            PointsNumber = SchoofAlgorithm.Compute().ToString();
        }

        public DiffieHellmanViewModel()
        {
            Polynomial.P = 97;

            EllipticCurve.A = 2;
            EllipticCurve.B = 3;
            EllipticCurve.P = 97; _primeNumber = "97";
            _formulaEllipticCurve = EllipticCurve.ToString();

            GetPointsNumberCommand = new RelayCommand(OnGetPointsNumberCommandExecuted);
        }
    }
}
