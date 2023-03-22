using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;

namespace Channeler.ViewModel
{
    public class BoardContentViewModel : ViewModelBase
    {
        public ObservableCollection<TabItem> BoardTabs { get; set; }


        public BoardContentViewModel()
        {
            BoardTabs = new ObservableCollection<TabItem>();
            BoardTabs.CollectionChanged += BoardTabs_CollectionChanged;
        }

        private void BoardTabs_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems.Count > 0)
            {
                // Select the newly added item
                TabItem newItem = e.NewItems[0] as TabItem;
                newItem.IsSelected = true;
            }
        }
    }
}
