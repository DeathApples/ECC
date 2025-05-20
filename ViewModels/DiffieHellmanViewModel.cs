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
            FormulaEllipticCurve = EllipticCurve.ToString();
            PrimeNumber = EllipticCurve.Prime.ToString();
            PointsNumber = SchoofAlgorithm.GetPointsCount().ToString();
        }

        public DiffieHellmanViewModel()
        {
            //_formulaEllipticCurve = EllipticCurve.ToString();
            //_primeNumber = EllipticCurve.Prime.ToString();

            GetPointsNumberCommand = new RelayCommand(OnGetPointsNumberCommandExecuted);
        }
    }
}
