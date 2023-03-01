using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Channeler.View
{
    /// <summary>
    /// Interação lógica para BoardThread.xam
    /// </summary>
    public partial class BoardThread : UserControl
    {
        public BoardThread()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var textBlock = sender as TextBlock;
            var postNo = ((Run)textBlock.Inlines.Last()).Text;
            ListView list = textBlock.Tag as ListView;


            foreach (var item in FindVisualChildren<Border>(list))
            {
                foreach(var txtBlock in FindVisualChildren<TextBlock>(item))
                {
                    if (txtBlock.Text == postNo)
                    {
                        item.BringIntoView();
                        break;
                    }
                }
            }
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        //    private childItem FindVisualChild<childItem>(DependencyObject obj)
        //where childItem : DependencyObject
        //    {
        //        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        //        {
        //            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
        //            if (child != null && child is childItem)
        //            {
        //                return (childItem)child;
        //            }
        //            else
        //            {
        //                childItem childOfChild = FindVisualChild<childItem>(child);
        //                if (childOfChild != null)
        //                    return childOfChild;
        //            }
        //        }
        //        return null;
        //    }
    }
}
