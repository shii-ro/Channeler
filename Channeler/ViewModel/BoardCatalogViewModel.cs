using Channeler.Model;
using Channeler.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Channeler.ViewModel
{
    public class BoardCatalogViewModel : ViewModelBase
    {
        private Random r;
        private  Board _currentBoard;
        public Board CurrentBoard { 
            get { return _currentBoard; }
            set { _currentBoard = value; }
        }

        private ObservableCollection<BoardPage> _boardCatalog;
        public ObservableCollection<BoardPage> BoardCatalog
        {
            get => _boardCatalog;
            set
            {
                _boardCatalog = value;
                OnPropertyChanged(nameof(BoardCatalog));
            }
        }
        public string BannerSource 
        {
            get => $"https://s.4cdn.org/image/title/{r.Next(260)}.png";
        }

        public BoardCatalogViewModel( )
        {
            r = new Random();
            _boardCatalog = new ObservableCollection<BoardPage>();
        }

        public override async Task LoadAsync()
        {
            BoardCatalog = await ApiHelper.GetBoardCatalog(CurrentBoard.board);
        }
    }
}
