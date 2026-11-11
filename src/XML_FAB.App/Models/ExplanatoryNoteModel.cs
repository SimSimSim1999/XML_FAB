using System.Xml.Linq;

namespace XML_FAB.Models;

public sealed class ExplanatoryNoteModel
{
    public string SchemaVersion { get; set; } = "01.07";

    public string ExplanatoryNoteNumber { get; set; } = "";
    public string ExplanatoryNoteYear { get; set; } = DateTime.Now.Year.ToString();
    public string AccessRestriction { get; set; } = "";

    public string AuthorFullName { get; set; } = "";
    public string AuthorShortName { get; set; } = "";
    public string AuthorOgrn { get; set; } = "";
    public string AuthorInn { get; set; } = "";

    public string ObjectType { get; set; } = "NonIndustrialObject";
    public string ObjectName { get; set; } = "";
    public string ObjectId { get; set; } = "";

    public XElement ToXml()
    {
        var root = new XElement("ExplanatoryNote",
            new XAttribute("SchemaVersion", SchemaVersion),
            new XAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            new XAttribute("xsi:noNamespaceSchemaLocation", "explanatorynote-01-07.xsd"));

        if (!string.IsNullOrWhiteSpace(AccessRestriction))
            root.Add(new XAttribute("AccessRestriction", AccessRestriction));

        root.Add(
            new XElement("ExplanatoryNoteNumber", ExplanatoryNoteNumber),
            new XElement("ExplanatoryNoteYear", ExplanatoryNoteYear),
            new XElement("IssueAuthor",
                new XElement("Organization",
                    new XElement("FullName", AuthorFullName),
                    new XElement("AbbreviatedName", AuthorShortName),
                    new XElement("OGRN", AuthorOgrn),
                    new XElement("INN", AuthorInn))),
            new XElement(ObjectType,
                new XAttribute("ObjectID", string.IsNullOrWhiteSpace(ObjectId) ? Guid.NewGuid().ToString() : ObjectId),
                new XElement("Name", ObjectName),
                new XElement("ConstructionType", "1"),
                new XElement("Address",
                    new XElement("RussianAddress",
                        new XElement("RegionCode", "02"),
                        new XElement("OKTMOCode", "80701000001"),
                        new XElement("OKTMOName", "г. Уфа"),
                        new XElement("City", "Уфа"),
                        new XElement("Street", "Центральная"),
                        new XElement("Building", "1")))));

        return root;
    }
}
