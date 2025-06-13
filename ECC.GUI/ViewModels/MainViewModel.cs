using System.Windows.Controls;

using ECC.GUI.Views;
using ECC.GUI.Models;
using ECC.GUI.Converters;

namespace ECC.GUI.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly Page[] _pages = [
            new VisualPage(),
            new DiffieHellmanPage(),
            new DigitalSignaturePage()
        ];

        private PageEnum? _currentPageEnum;
        public PageEnum? CurrentPageEnum
        {
            get => _currentPageEnum;
            set => SetProperty(ref _currentPageEnum, value);
        }

        private Page? _currentPage;
        public Page? CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private void OnChangePageHandler(PageEnum targetPage)
        {
            CurrentPageEnum = targetPage;
            CurrentPage = _pages[(int)targetPage];
        }

        public MainViewModel()
        {
            _currentPageEnum = PageEnum.VisualPage;
            _currentPage = _pages[(int)PageEnum.VisualPage];
            PageConverter.ChangePageEvent += OnChangePageHandler;
        }
    }
}
