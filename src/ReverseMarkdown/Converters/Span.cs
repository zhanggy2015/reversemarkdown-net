using HtmlAgilityPack;

namespace ReverseMarkdown.Converters
{
    public class Span : ConverterBase
    {
        public Span(Converter converter) : base(converter)
        {
            Converter.Register("span", this);
        }

        public override string Convert(HtmlNode node)
        {
            var content = TreatChildren(node);
            if (!string.IsNullOrEmpty(node.InnerHtml.Trim()))
            {
                var styles = StringUtils.ParseStyle(node.GetAttributeValue("style", ""));
                styles.TryGetValue("text-align", out var align);
                switch (align?.Trim())
                {
                    case "center":
                        content = $"<center>{content}</center>";
                        break;
                    default:
                        break;
                }
                styles.TryGetValue("font-weight", out var fontWeight);
                switch (fontWeight)
                {
                    case "bold":
                        content = content.EmphasizeContentWhitespaceGuard("**", " ");
                        break;
                    default:
                        break;
                }
            }
            return content;
        }


    }
}
