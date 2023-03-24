using Channeler.Command;
using Channeler.Model;
using Channeler.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Channeler.ViewModel
{
    
    public class BoardCatalogViewModel : ViewModelBase
    {
        public class ComboBoxKeyValue
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        private Random r;
        private Board _currentBoard;
        private ICollectionView _threadsView;
        private ObservableCollection<Model.Thread> _threads;
        private string _filter;
        private List<BoardPage> _boardCatalog;
        private int _currentImageSize;
        private int _showOpComment;

        public ObservableCollection<Model.Thread> Threads 
        { 
            get => _threads;
            private set
            {
                _threads = value;
                OnPropertyChanged(nameof(Threads));
            }
        }


        public Board CurrentBoard
        {
            get { return _currentBoard; }
            set
            {
                if (_currentBoard == value) return;
                else
                {
                    _currentBoard = value;
                    OnPropertyChanged(nameof(CurrentBoard));
                }
            }
        }

        public List<BoardPage> BoardCatalog
        {
            get => _boardCatalog;
            set
            {
                if (value != _boardCatalog)
                {
                    _boardCatalog = value;
                    OnPropertyChanged(nameof(BoardCatalog));
                }
            }
        }
        public string BannerSource
        {
            get => $"https://s.4cdn.org/image/title/{r.Next(260)}.png";
        }
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    _threadsView.Refresh();
                    OnPropertyChanged(nameof(Filter));
                }
            }
        }

        public int CurrentImageSize
        {
            get => _currentImageSize;
            set
            {
                _currentImageSize = value;
                OnPropertyChanged(nameof(CurrentImageSize));
            }
        }

        public ObservableCollection<ComboBoxKeyValue> ImageSizes { get; set; } = new ObservableCollection<ComboBoxKeyValue>
            {
                new ComboBoxKeyValue{ Name = "Small", Value = 150 },
                new ComboBoxKeyValue { Name = "Large", Value = 300 }
            };
        public ObservableCollection<ComboBoxKeyValue> OpCommentOptions { get; set; } = new ObservableCollection<ComboBoxKeyValue>
        {
            new ComboBoxKeyValue{ Name = "On", Value = 0 },
            new ComboBoxKeyValue { Name = "Off", Value = 1 }
        };
        public int ShowOpComment
        { 
            get => _showOpComment;
            set
            {
                _showOpComment = value;
                OnPropertyChanged(nameof(ShowOpComment));
            }
        }

        public BoardCatalogViewModel()
        {
            r = new Random();
            Threads = new ObservableCollection<Model.Thread>();
            CurrentImageSize = ImageSizes[0].Value;
            ShowOpComment = OpCommentOptions[0].Value;
        }

        public override async Task LoadAsync()
        {
            List<BoardPage> newCatalog = await ApiHelper.GetBoardCatalog(CurrentBoard.board);
           
            Threads = new ObservableCollection<Model.Thread>();
            newCatalog.ForEach(page =>
            {
                page.threads.ForEach(thread => Threads.Add(thread));
            });

            _threadsView = CollectionViewSource.GetDefaultView(Threads);
            _threadsView.Filter = t =>
            {
                const string pattern = @"<[^>]*>";
                Model.Thread thread = t as Model.Thread;

                string content = thread.com != null ? thread.com.ToLower() : "";
                string title = thread.sub != null ? thread.sub.ToLower() : "";


                if (string.IsNullOrEmpty(Filter)) return true;
                else if (thread.com is not null)
                {
                    string contentWithoutTags = Regex.Replace(content, pattern, "");

                    if (contentWithoutTags.Contains(Filter.ToLower()) ||
                        title.Contains(Filter.ToLower()))
                        return true;
                    else return false;
                }
                else return false;
            };
            BoardCatalog = newCatalog;
        }
    }
}
