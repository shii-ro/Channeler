using Channeler.Command;
using Channeler.Model;
using Channeler.ViewModel.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Channeler.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BoardList _boardList;
        private Board _selectedBoard;
        private BoardContentViewModel _boardContent;

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
                    LoadCatalogAsync(null);
                    OnPropertyChanged(nameof(SelectedBoard));
                }
            }
        }

        public BoardContentViewModel BoardContent
        {
            get => _boardContent;
            set
            {
                _boardContent = value;
                OnPropertyChanged(nameof(BoardContent));
            }
        }
        private async void LoadCatalogAsync(object obj)
        {
            var newTab = new BoardCatalogViewModel();
            newTab.CurrentBoard = SelectedBoard;

            BoardContent.BoardTabs.Add(
            new TabItem
            {
                Header = CreateClosableHeader($"/{newTab.CurrentBoard.board}/ - Catalog"),
                Content = newTab,
            });
            await newTab.LoadAsync();
        }

        private async void LoadThreadAsync(object obj)
        {
            var t = obj as Model.Thread;
            var newTab = new BoardThreadViewModel();
            newTab.BoardName = SelectedBoard.board;
            newTab.CurrentThread = t;
            BoardContent.BoardTabs.Add(
                new TabItem
                {
                    Header = CreateClosableHeader($"/{newTab.BoardName}/ - {newTab.CurrentThread.sub ?? newTab.CurrentThread.com.Substring(0, Math.Min(newTab.CurrentThread.com.Length, 16))}"),
                    Content = newTab
                });
            await newTab.LoadAsync();
        }

        public MainViewModel(BoardContentViewModel boardContent)
        {
            BoardContent = boardContent;

            LoadThreadCommand = new DelegateCommand(LoadThreadAsync);
            BoardList = ApiHelper.GetBoards().Result;
        }

        private Grid CreateClosableHeader(string headerContent)
        {
            // there must be a better way of doing this
            Grid headerGrid = new Grid();
            ColumnDefinition headerText = new ColumnDefinition();
            ColumnDefinition headerCloseButton = new ColumnDefinition();
            headerGrid.ColumnDefinitions.Add(headerText);
            headerGrid.ColumnDefinitions.Add(headerCloseButton);

            // Create a TextBlock and set its properties
            TextBlock myTextBlock = new TextBlock();
            myTextBlock.Text = headerContent;
            myTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            myTextBlock.VerticalAlignment = VerticalAlignment.Center;

            // Create a Button and set its properties
            Button myButton = new Button();
            myButton.Content = "X";
            myButton.Margin = new Thickness(5, 0, -3, 0);
            myButton.BorderThickness = new Thickness(0);
            myButton.Background = Brushes.Transparent;
            myButton.FontWeight = FontWeights.Bold;
            myButton.HorizontalAlignment = HorizontalAlignment.Right;
            myButton.VerticalAlignment = VerticalAlignment.Center;
            myButton.Click += (sender, args) =>
            {
                // Get the parent TabItem of the header
                TabItem parentTabItem = (TabItem)((Grid)((Button)sender).Parent).Parent;
                BoardContent.BoardTabs.Remove(parentTabItem);
            };

            // Add the TextBlock and Button to the Grid
            headerGrid.Children.Add(myTextBlock);
            Grid.SetColumn(myTextBlock, 0);
            headerGrid.Children.Add(myButton);
            Grid.SetColumn(myButton, 1);

            return headerGrid;
        }
    }
}
