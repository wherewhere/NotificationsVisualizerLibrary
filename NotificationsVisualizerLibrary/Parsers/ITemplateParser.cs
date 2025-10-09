namespace NotificationsVisualizerLibrary.Parsers
{
    public interface ITemplateParser
    {
        bool IsMatch(string text);

        ParseResult Parse(string text);
    }
}
