using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Channeler.View
{
    /// <summary>
    /// Interação lógica para BoardThread.xaml
    /// </summary>
    public partial class BoardThread : UserControl
    {
        public BoardThread()
        {
            InitializeComponent();
        }

        public void TextBlock_ScrollToPost(object sender, MouseButtonEventArgs e)
        {
            SolidColorBrush highlitedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xd6, 0xba, 0xd0));
            SolidColorBrush defaultBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xd6, 0xda, 0xf0));

            var textBlock = sender as TextBlock;
            var postNo = ((Run)textBlock.Inlines.Last()).Text;


            foreach (var item in FindVisualChildren<Grid>(postsListView))
            {
                foreach (var txtBlock in FindVisualChildren<TextBlock>(item))
                {
                    if (item.Name.Equals("postGrid"))
                    {
                        if (txtBlock.Text == postNo)
                        {
                            item.Background = highlitedBackground;
                            item.BringIntoView();
                            item.Name = "postGrid";
                            break;
                        }

                        else if (item.Name.Equals("postGrid")
                                && item.Background.ToString()
                                == highlitedBackground.ToString()
                                && txtBlock.Text != postNo
                                )
                        {
                            item.Background = defaultBackground;
                            item.Name = "postGrid";
                        }
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
    }
}
