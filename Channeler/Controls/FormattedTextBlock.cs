using Channeler.ViewModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Channeler.Controls
{
    public class FormattedTextBlock : TextBlock
    {
        static Style quotelinkStyle;
        static Style quoteStyle;
        static Style spoilerTextStyle;
        public FormattedTextBlock()
        {
            quotelinkStyle = Application.Current.FindResource("quotelinkStyle") as Style;
            quoteStyle = Application.Current.FindResource("quoteStyle") as Style;
            spoilerTextStyle = Application.Current.FindResource("spoilerTextStyle") as Style;
            TextWrapping = TextWrapping.Wrap;
        }

        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FormattedTextBlock), new PropertyMetadata(null, OnTextChanged));


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
                    //characterCount++;
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
                    case "<a>":
                        Run quotelinkText = new Run();
                        quotelinkText.Text = System.Net.WebUtility.HtmlDecode(h.content);
                        quotelinkText.Style = quotelinkStyle;
                        textBlock.Inlines.Add(quotelinkText);
                        break;
                    default: break;

                }
            }
        }

    }
}
