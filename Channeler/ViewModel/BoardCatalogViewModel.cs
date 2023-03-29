using Channeler.Model;
using Channeler.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Channeler.ViewModel
{

    public class BoardCatalogViewModel : ViewModelBase
    {
        public class ComboBoxKeyValue<T>
        {
            public string Key { get; set; }
            public T Value { get; set; }
        }

        private Board _currentBoard;
        private readonly Random _random = new();
        private ObservableCollection<Model.Thread> _threads = new();
        private List<BoardPage> _boardCatalog = new();
        private string _filter;
        private int _currentImageSize;
        private SortDescription _currentSortDescription;
        private bool _showOpComment;

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
                if (value != _currentBoard) 
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
            get => $"https://s.4cdn.org/image/title/{_random.Next(260)}.png";
        }
        public string Filter
        {
            get => _filter;
            set
            {
                if (value != _filter)
                {
                    _filter = value;
                    ThreadsViewSource.Refresh();
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

        public ICollectionView ThreadsViewSource { get; set; }

        public ObservableCollection<ComboBoxKeyValue<int>> ImageSizes { get; } = new ObservableCollection<ComboBoxKeyValue<int>>
            {
                new ComboBoxKeyValue<int>{ Key = "Small", Value = 150 },
                new ComboBoxKeyValue<int>{ Key = "Large", Value = 300 }
            };

        public ObservableCollection<ComboBoxKeyValue<bool>> OpCommentOptions { get; } = new ObservableCollection<ComboBoxKeyValue<bool>>
        {
            new ComboBoxKeyValue<bool>{ Key = "On", Value = true },
            new ComboBoxKeyValue<bool>{ Key = "Off", Value = false}
        };

        public ObservableCollection<ComboBoxKeyValue<SortDescription>> SortOptions { get; } = new ObservableCollection<ComboBoxKeyValue<SortDescription>>
        {
            new ComboBoxKeyValue<SortDescription> { Key = "Bump Order", Value = new SortDescription("replies", ListSortDirection.Descending) },
            new ComboBoxKeyValue<SortDescription> { Key = "Last Reply", Value = new SortDescription("last_replies.Last().now", ListSortDirection.Descending) },
            new ComboBoxKeyValue<SortDescription> { Key = "Creation Date", Value = new SortDescription("now", ListSortDirection.Descending) },
            new ComboBoxKeyValue<SortDescription> { Key = "Reply Count", Value = new SortDescription("replies", ListSortDirection.Descending) },
        };

        public bool ShowOpComment
        {
            get => _showOpComment;
            set
            {
                _showOpComment = value;
                OnPropertyChanged(nameof(ShowOpComment));
            }
        }

        public SortDescription CurrentSortDescription
        {
            get => _currentSortDescription;
            set
            {
                _currentSortDescription = value;
                ThreadsViewSource.SortDescriptions.Clear();
                if(_currentSortDescription != SortOptions[0].Value) 
                    ThreadsViewSource.SortDescriptions.Add(CurrentSortDescription);
                OnPropertyChanged(nameof(CurrentSortDescription));
            }
        }

        public BoardCatalogViewModel()
        {
            CurrentImageSize = ImageSizes[0].Value;
            ShowOpComment = OpCommentOptions[0].Value;
        }

        public override async Task LoadAsync()
        {
            List<BoardPage> newCatalog = await Task.Run( () => ApiHelper.GetBoardCatalog(CurrentBoard.board) );

            Threads = new ObservableCollection<Model.Thread>();

            newCatalog.ForEach(page =>
            {
                page.threads.ForEach(thread => Threads.Add(thread));
            });

            ThreadsViewSource = CollectionViewSource.GetDefaultView(Threads);
            Thread t = new Thread();


            ThreadsViewSource.Filter = t =>
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

            CurrentSortDescription = SortOptions[0].Value;
            BoardCatalog = newCatalog;
        }
    }
}
