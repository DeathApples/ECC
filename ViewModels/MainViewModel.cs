using System.Windows.Controls;

using ECDH.Views;
using ECDH.Models;
using ECDH.Converters;

namespace ECDH.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private readonly Page[] _pages = [
            new VisualPage(),
            new DiffieHellmanPage()
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
