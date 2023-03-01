﻿using Channeler.Command;
using Channeler.Model;
using Channeler.ViewModel.Helpers;
using System.Threading.Tasks;

namespace Channeler.ViewModel
{
    public class BoardThreadViewModel : ViewModelBase
    {
        public string BoardName { get; set; }
        private ThreadPosts _threadPosts;
        private Thread _currentThread;
        public Thread CurrentThread
        {
            get { return _currentThread; } 
            set 
            {
                _currentThread = value;
                OnPropertyChanged(nameof(CurrentThread));
            }
        }

        public DelegateCommand BackToCatalogCommand { get; }
        public DelegateCommand ScrollToPostCommand { get; }
        public BoardThreadViewModel()
        {
            BackToCatalogCommand = new DelegateCommand(BackToCatalog);
        }

        private void ScrollToPost(object obj)
        {

        }
        public override async Task LoadAsync()
        {
            ThreadPosts = await ApiHelper.GetThreadPosts(BoardName, CurrentThread.no.ToString());
        }

        public ThreadPosts ThreadPosts
        {
            get => _threadPosts;
            set
            {
                _threadPosts = value;
                OnPropertyChanged(nameof(ThreadPosts));
            }
        }

        private void BackToCatalog(object obj)
        {
            var vm = obj as MainViewModel;
            vm.CurrentViewModel = vm;
        }
    }
}
