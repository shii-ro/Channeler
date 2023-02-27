using Channeler.Command;
using Channeler.Model;
using Channeler.View;
using Channeler.ViewModel.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Channeler.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BoardList _boardList;
        private Board _selectedBoard;
        public BoardCatalogViewModel BoardCatalog { get; }
        public BoardThreadViewModel BoardThread { get; }

        public DelegateCommand SelectViewModelCommand { get; }
        public DelegateCommand LoadThreadCommand { get; }

        public BoardList BoardList
        {
            get { return _boardList; }
            set { _boardList = value; }
        }
        public Board SelectedBoard
        {
            get => _selectedBoard;
            set
            {
                if (_selectedBoard != value)
                {
                    _selectedBoard = value;
                    LoadCatalog(null);
                    OnPropertyChanged(nameof(SelectedBoard));
                }
            }
        }

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (CurrentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        private async void SelectViewModel(object obj)
        {
            var viewModel = obj as ViewModelBase;
            CurrentViewModel = viewModel;
            await LoadAsync();
        }

        public async override Task LoadAsync()
        {
            if (CurrentViewModel is not null)
            {
                await CurrentViewModel.LoadAsync();
            }
        }

        private void LoadCatalog(object obj)
        {
            if (SelectedBoard != null) BoardCatalog.CurrentBoard = SelectedBoard;
            SelectViewModel(BoardCatalog);
        }

        private void LoadThread(object obj)
        {
            var t = obj as Model.Thread;
            BoardThread.BoardName = SelectedBoard.board;
            BoardThread.CurrentThread = t;
            SelectViewModel(BoardThread);
        }

        public MainViewModel(BoardCatalogViewModel boardCatalog, BoardThreadViewModel boardThread)
        {
            BoardCatalog = boardCatalog;
            BoardThread = boardThread;
            SelectViewModelCommand = new DelegateCommand(SelectViewModel);
            LoadThreadCommand = new DelegateCommand(LoadThread);
            BoardList = ApiHelper.GetBoards();
        }
    }
}
