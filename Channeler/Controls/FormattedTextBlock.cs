using Channeler.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Channeler.Controls
{ 
    public class FormattedTextBlock : TextBlock
    {
        static Style quotelinkStyle;
        static Style quoteStyle;
        static Style spoilerTextStyle;
        static Style viewPostToolTip;
        static List<Post> _quotes;
        static ListView _list;
        static DataTemplate postPreview;
        public static List<Post> QuotesPosts { get => _quotes; set => _quotes = value; }
        public static ListView List { get => _list; set => _list = value; }

        public FormattedTextBlock()
        {
            quotelinkStyle = Application.Current.FindResource("quotelinkStyle") as Style;
            quoteStyle = Application.Current.FindResource("quoteStyle") as Style;
            spoilerTextStyle = Application.Current.FindResource("spoilerTextStyle") as Style;
            postPreview = Application.Current.FindResource("postMiniTemplate") as DataTemplate;
            viewPostToolTip = Application.Current.FindResource("viewPostToolTip") as Style;
            TextWrapping = TextWrapping.Wrap;
        }

        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public List<Post> Replies
        {
            get { return (List<Post>)GetValue(RepliesProperty); }
            set { SetValue(RepliesProperty, value); }
        }
        public ListView PostList
        {
            get { return (ListView)GetValue(PostListProperty); }
            set { SetValue(PostListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostListProperty =
            DependencyProperty.Register("PostList", typeof(ListView), typeof(FormattedTextBlock), new PropertyMetadata(null, OnPostListChanged));

        // Using a DependencyProperty as the backing store for Replies.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepliesProperty =
            DependencyProperty.Register("Replies", typeof(List<Post>), typeof(FormattedTextBlock), new PropertyMetadata(null, OnRepliesChanged));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FormattedTextBlock), new PropertyMetadata(null, OnTextChanged));

        private static void OnPostListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is null) return;
            ListView list = e.NewValue as ListView;
            List = list;
        }

        private static void OnRepliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FormattedTextBlock textBlock))
            {
                return;
            }

            QuotesPosts = e.NewValue as List<Post>;
        }

        private static void OnTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (!(source is FormattedTextBlock textBlock))
            {
                return;
            }

            textBlock.Inlines.Clear();

            string? newText = e.NewValue as string;

            if (string.IsNullOrEmpty(newText))
            {
                return;
            }
            FormatText(textBlock, newText);
        }

        private static string[] sClosingTags =
        {
            "<area>","<base>", "<br>", "<col>", "<hr>", "<embed>", "<wbr>"// i think the rest does not have any use in the posts
        };

        private struct Attribute
        {
            public string key;
            public string value;
        }

        private class HtmlParse
        {
            public string tag;
            public string content;
            public List<Attribute> attributes;
            public HtmlParse child;
        }
        private static void FormatText(TextBlock textBlock, string newText)
        {
            bool insideElement = false;
            List<HtmlParse> root = new List<HtmlParse>();
            HtmlParse currentElement = new HtmlParse();
            char c = newText[0];
            for (int characterCount = 1; characterCount < newText.Length;)
            {
                // start of a HTMl element ?
                // just straight text then, convert to a <p> element
                // check if its not a closing element
                if (c == '<')
                {
                    if (!insideElement)
                    {
                        // get the attributes and tag
                        string newTag = "";
                        // read until end blank character or '>'
                        while (c != ' ')
                        {
                            newTag += c;

                            c = newText[characterCount++];

                            if (c == '=') continue;
                            else if (c == '>')
                            {
                                newTag += c;
                                break;
                            }
                            else if (c == ' ')
                            {
                                newTag += '>';
                                break;
                            }

                        }
                        currentElement.tag = newTag;

                        // current character is ' ', may have attributes
                        if (c is ' ')
                        {
                            currentElement.attributes = new List<Attribute>();
                            while (c != '>')
                            {
                                // get key
                                string key = "";
                                while (c != '=')
                                {
                                    c = newText[characterCount++];
                                    if (c == '=') continue;
                                    key += c;
                                }
                                string value = "";
                                // get value ( attributes sometimes doesnt hav a value)
                                if (c == '=')
                                {
                                    while (c != ' ' && c != '>')
                                    {
                                        c = newText[characterCount++];
                                        value += c;
                                    }
                                }
                                Attribute currentAttribute;
                                currentAttribute.value = value;
                                currentAttribute.key = key;

                                currentElement.attributes.Add(currentAttribute);
                            }
                        }
                    }

                    insideElement = true;

                    // now checks if the attribute has content inside if the tag is not self closing
                    string content = "";
                    if (characterCount >= newText.Length) return;
                    c = newText[characterCount++];

                    if (sClosingTags.Contains(currentElement.tag))
                    {
                        insideElement = false;
                        root.Add(currentElement);
                        currentElement = new HtmlParse();
                    }

                    else if (c == '/')
                    {
                        // check if the next character is '/', if not, its a nested element
                        root.Add(currentElement);
                        currentElement = new HtmlParse();
                        while (c != '>' && characterCount < newText.Length)
                        {
                            c = newText[characterCount++];
                        }
                        insideElement = false;
                        if (characterCount < newText.Length)
                            c = newText[characterCount++];
                    }
                    else if (c != '<')
                    {
                        while (c != '<' && characterCount < newText.Length)
                        {
                            content += c;
                            c = newText[characterCount++];
                        }
                        insideElement = true;
                        currentElement.content = content;
                    }



                }

                // parse texts without <p>
                else
                {
                    if (currentElement.tag == null)
                    {
                        currentElement.tag = "<p>";
                        currentElement.content = "";
                        while (c != '<' && characterCount < newText.Length)
                        {
                            currentElement.content += c;
                            c = newText[characterCount++];
                        }
                        if (characterCount == newText.Length)
                            currentElement.content += newText[characterCount - 1];

                        insideElement = false;
                        root.Add(currentElement);
                        currentElement = new HtmlParse();
                    }
                }
            }
            foreach (HtmlParse h in root)
            {

                switch (h.tag)
                {
                    case "<s>":
                        Run spoilerText = new Run();
                        spoilerText.Text = System.Net.WebUtility.HtmlDecode(h.content);
                        spoilerText.Style = spoilerTextStyle;
                        textBlock.Inlines.Add(spoilerText);
                        break;
                    case "<p>": textBlock.Inlines.Add(System.Net.WebUtility.HtmlDecode(h.content)); break;
                    case "<br>": textBlock.Inlines.Add("\n"); break;
                    case "<wbr>": textBlock.Inlines.Add("\n"); break;
                    case "<span>":
                        Run quoteText = new Run();
                        quoteText.Style = quoteStyle;
                        quoteText.Text = System.Net.WebUtility.HtmlDecode(h.content);
                        textBlock.Inlines.Add(quoteText);
                        break;
                    case "<a>": //  ugly and repetititve code
                        Run quotelinkText = new Run();
                        quotelinkText.Text = System.Net.WebUtility.HtmlDecode(h.content);
                        quotelinkText.Style = quotelinkStyle;

                        if (QuotesPosts is not null)
                            if (QuotesPosts.Count > 0)
                            {
                                foreach (var quote in QuotesPosts)
                                {
                                    string currentQuoteNo = quotelinkText.Text.Substring(2);
                                    string compareQuoteNo = quote.no.ToString();
                                    
                                    if (currentQuoteNo == compareQuoteNo)
                                    {

                                        Popup postPopUpPreview = new Popup
                                        {
                                            Child = new ContentPresenter() { Content = quote, ContentTemplate = postPreview },
                                            PlacementTarget = textBlock,
                                            Placement = PlacementMode.Top,
                                        };

                                        postPopUpPreview.SetBinding(
                                            Popup.IsOpenProperty,
                                            new Binding()
                                            {
                                                Source = quotelinkText,
                                                Path = new PropertyPath("IsMouseOver"),
                                                Mode = BindingMode.OneWay,
                                            });

                                        var parent = textBlock.Parent as Grid;
                                        parent.Children.Add(postPopUpPreview);
                                        quotelinkText.MouseLeftButtonDown += ScrollToPostOnClick;
                                    }
                                }
                            }
                        textBlock.Inlines.Add(quotelinkText);
                        break;
                    default: break;

                }
            }
        }

        private static void ScrollToPostOnClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SolidColorBrush highlitedBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xd6, 0xba, 0xd0));
            SolidColorBrush defaultBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xd6, 0xda, 0xf0));

            Run quote = sender as Run;
            string postNo = quote.Text.Substring(2);

            if (quote != null)
            {
                foreach (var item in FindVisualChildren<Grid>(List))
                {
                    foreach (var txtBlock in FindVisualChildren<TextBlock>(item))
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

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
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
