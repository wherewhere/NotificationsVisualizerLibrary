using NotificationsVisualizerLibrary.Parsers;
using System.Xml.Linq;

namespace NotificationsVisualizerLibrary.Model.TileElements
{
    internal class TileSubgroup : Subgroup
    {
        protected override void HandleChild(ParseResult result, XElement child, string baseUri, bool addImageQuery)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case "text":

                    TileTextField text = new TileTextField();
                    text.Parse(result, child);

                    if (!result.IsOkForRender())
                    { throw new IncompleteElementException(); }

                    if (text != null)
                    { Add(text); }

                    break;

                case "image":

                    TileImage image = new TileImage();
                    image.Parse(result, child, baseUri, addImageQuery);

                    if (!result.IsOkForRender())
                    { throw new IncompleteElementException(); }

                    if (image != null)
                    { Add(image); }

                    break;

                default:
                    result.AddWarning($"Invalid child \"{child.Name.LocalName}\" under subgroup element. It will be ignored.", GetErrorPositionInfo(child));
                    break;
            }
        }
    }
}
