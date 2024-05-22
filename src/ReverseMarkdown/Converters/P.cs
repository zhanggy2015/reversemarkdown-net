using System;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class P : ConverterBase
    {
        public P(Converter converter) : base(converter)
        {
            Converter.Register("p", this);
        }

        public override string Convert(HtmlNode node)
        {
            var content = TreatChildren(node);
            var indentation = IndentationFor(node);
            var newlineAfter = NewlineAfter(node);

            var styles = StringUtils.ParseStyle(node.GetAttributeValue("style", ""));
            string pattern = @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>";

            if (!string.IsNullOrEmpty(node.InnerHtml.Trim())
                &&!Regex.IsMatch(node.InnerHtml, pattern, RegexOptions.Compiled)//图片不能居中
                &&!content.Contains("**"))//加粗的不能居中
            {
                //styles.TryGetValue("text-indent", out var value);
                //if (!string.IsNullOrEmpty(value))
                //{
                //    content = $"<p style='text-indent:{value};'>{content}</p>";
                //}
                styles.TryGetValue("text-align", out var align);
                switch (align?.Trim())
                {
                    case "center":
                        content = $"<center>{content}</center>";
                        break;
                    default:
                        break;
                }
            }

            return $"{indentation}{content}{newlineAfter}";
        }

        private static string IndentationFor(HtmlNode node)
        {
            string parentName = node.ParentNode.Name.ToLowerInvariant();

            // If p follows a list item, add newline and indent it
            var length = node.Ancestors("ol").Count() + node.Ancestors("ul").Count();
            bool parentIsList = parentName == "li" || parentName == "ol" || parentName == "ul";
            if (parentIsList && node.ParentNode.FirstChild != node)
                return Environment.NewLine + (new string(' ', length * 4));

            // If p is at the start of a table cell, no leading newline
            return Td.FirstNodeWithinCell(node) ? "" : Environment.NewLine;
        }

        private static string NewlineAfter(HtmlNode node)
        {
            return Td.LastNodeWithinCell(node) ? "" : Environment.NewLine;
        }
    }
}