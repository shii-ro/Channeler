using Channeler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        static ListView _list;
        static DataTemplate postPreview;

        public static ListView List { get => _list; set => _list = value; }

        public FormattedTextBlock()
        {
            quotelinkStyle = Application.Current.FindResource("quotelinkStyle") as Style;
            quoteStyle = Application.Current.FindResource("quoteStyle") as Style;
            spoilerTextStyle = Application.Current.FindResource("spoilerTextStyle") as Style;
            postPreview = Application.Current.FindResource("postMiniTemplate") as DataTemplate;
            TextWrapping = TextWrapping.Wrap;
        }

        public ListView PostList
        {
            get { return (ListView)GetValue(PostListProperty); }
            set { SetValue(PostListProperty, value); }
        }

        public Post Post
        {
            get { return (Post)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }

        public Thread Thread
        {
            get { return (Thread)GetValue(ThreadProperty); }
            set { SetValue(ThreadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thread.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThreadProperty =
            DependencyProperty.Register("Thread", typeof(Thread), typeof(FormattedTextBlock), new PropertyMetadata(null, OnThreadChanged));

        // Using a DependencyProperty as the backing store for Post.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostProperty =
            DependencyProperty.Register("Post", typeof(Post), typeof(FormattedTextBlock), new PropertyMetadata(null, OnPostChanged));

        // Using a DependencyProperty as the backing store for PostList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostListProperty =
            DependencyProperty.Register("PostList", typeof(ListView), typeof(FormattedTextBlock), new PropertyMetadata(null, OnPostListChanged));

        private static void OnPostListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is null) return;
            ListView list = e.NewValue as ListView;
            List = list;
        }

        private static void OnPostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(!(d is FormattedTextBlock textBlock)) return;

            Post? newPost = e.NewValue as Post;

            if (string.IsNullOrEmpty(newPost?.com))
            {
                return;
            }

            textBlock.Text = null;

            FormatText(textBlock, newPost.com, newPost);
        }

        private static void OnThreadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FormattedTextBlock textBlock)) return;

            Thread? newThread = e.NewValue as Thread;

            if (string.IsNullOrEmpty(newThread?.com))
            {
                return;
            }

            textBlock.Text = null;

            FormatText(textBlock, newThread.com, null);
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
        private static void FormatText(TextBlock textBlock, string htmlString, Post? currentPost)
        {
            bool insideElement = false;
            List<HtmlParse> parsedElements = new List<HtmlParse>();
            HtmlParse currentElement = new HtmlParse();
            char currentCharacter = htmlString[0];

            for (int characterCount = 1; characterCount < htmlString.Length;)
            {
                // start of a HTMl element ?
                // just straight text then, convert to a <p> element
                // check if its not a closing element
                if (currentCharacter == '<')
                {
                    if (!insideElement)
                    {
                        // get the attributes and tag
                        string newTag = "";
                        // read until end blank character or '>'
                        while (currentCharacter != ' ')
                        {
                            newTag += currentCharacter;

                            currentCharacter = htmlString[characterCount++];

                            if (currentCharacter == '=') continue;
                            else if (currentCharacter == '>')
                            {
                                newTag += currentCharacter;
                                break;
                            }
                            else if (currentCharacter == ' ')
                            {
                                newTag += '>';
                                break;
                            }

                        }
                        currentElement.tag = newTag;

                        // current character is ' ', may have attributes
                        if (currentCharacter is ' ')
                        {
                            currentElement.attributes = new List<Attribute>();
                            while (currentCharacter != '>')
                            {
                                // get key
                                string key = "";
                                while (currentCharacter != '=')
                                {
                                    currentCharacter = htmlString[characterCount++];
                                    if (currentCharacter == '=') continue;
                                    key += currentCharacter;
                                }
                                string value = "";
                                // get value ( attributes sometimes doesnt hav a value)
                                if (currentCharacter == '=')
                                {
                                    while (currentCharacter != ' ' && currentCharacter != '>')
                                    {
                                        currentCharacter = htmlString[characterCount++];
                                        value += currentCharacter;
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
                    if (characterCount >= htmlString.Length) return;
                    currentCharacter = htmlString[characterCount++];

                    if (sClosingTags.Contains(currentElement.tag))
                    {
                        insideElement = false;
                        parsedElements.Add(currentElement);
                        currentElement = new HtmlParse();
                    }

                    else if (currentCharacter == '/')
                    {
                        // check if the next character is '/', if not, its a nested element
                        parsedElements.Add(currentElement);
                        currentElement = new HtmlParse();
                        while (currentCharacter != '>' && characterCount < htmlString.Length)
                        {
                            currentCharacter = htmlString[characterCount++];
                        }
                        insideElement = false;
                        if (characterCount < htmlString.Length)
                            currentCharacter = htmlString[characterCount++];
                    }
                    else if (currentCharacter != '<')
                    {
                        while (currentCharacter != '<' && characterCount < htmlString.Length)
                        {
                            content += currentCharacter;
                            currentCharacter = htmlString[characterCount++];
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
                        while (currentCharacter != '<' && characterCount < htmlString.Length)
                        {
                            currentElement.content += currentCharacter;
                            currentCharacter = htmlString[characterCount++];
                        }
                        if (characterCount == htmlString.Length)
                            currentElement.content += htmlString[characterCount - 1];

                        insideElement = false;
                        parsedElements.Add(currentElement);
                        currentElement = new HtmlParse();
                    }
                }
            }


            foreach (HtmlParse h in parsedElements)
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
                    case "<a>":
                        Run quotelinkText = new Run();
                        quotelinkText.Text = System.Net.WebUtility.HtmlDecode(h.content);
                        quotelinkText.Style = quotelinkStyle;

                        if (currentPost?.QuotesPosts?.Count > 0)
                        {
                            foreach (var quote in currentPost.QuotesPosts)
                            {
                                string currentQuoteNo = quotelinkText.Text.Substring(2);
                                string compareQuoteNo = quote.no.ToString();

                                if (currentQuoteNo == compareQuoteNo)
                                {
                                    Popup postPopUpPreview = CreatePostPopupPreview(quote, textBlock, quotelinkText);

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

        private static Popup CreatePostPopupPreview(Post post, FrameworkElement placementTarget, Run quoteLinkText)
        {
            Popup postPopUpPreview = new Popup
            {
                Child = new ContentPresenter() 
                    { 
                        Content = post,
                        ContentTemplate = postPreview
                    },
                PlacementTarget = placementTarget,
                Placement = PlacementMode.Top,
            };

            postPopUpPreview.SetBinding(
                Popup.IsOpenProperty,
                new Binding()
                {
                    Source = quoteLinkText,
                    Path = new PropertyPath("IsMouseOver"),
                    Mode = BindingMode.OneWay,
                });

            return postPopUpPreview;
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
