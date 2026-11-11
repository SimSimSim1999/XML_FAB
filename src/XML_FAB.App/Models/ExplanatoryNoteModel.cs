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
    public string AuthorKpp { get; set; } = "";

    public string DeveloperFullName { get; set; } = "";
    public string DeveloperShortName { get; set; } = "";
    public string DeveloperOgrn { get; set; } = "";
    public string DeveloperInn { get; set; } = "";
    public string DeveloperKpp { get; set; } = "";

    public string ObjectType { get; set; } = "NonIndustrialObject";
    public string ObjectName { get; set; } = "";
    public string ObjectId { get; set; } = "";

    public string ObjectRegionCode { get; set; } = "02";
    public string ObjectOktmoCode { get; set; } = "80701000001";
    public string ObjectOktmoName { get; set; } = "г. Уфа";
    public string ObjectCity { get; set; } = "Уфа";
    public string ObjectStreet { get; set; } = "Центральная";
    public string ObjectBuilding { get; set; } = "1";

    public string DesignNote { get; set; } = "Настоящая пояснительная записка подготовлена на основании технического задания и проектных данных.";

    public XElement ToXml()
    {
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

        var root = new XElement(
            "ExplanatoryNote",
            new XAttribute("SchemaVersion", SchemaVersion),
            new XAttribute("id", string.IsNullOrWhiteSpace(ObjectId) ? Guid.NewGuid().ToString().ToUpperInvariant() : ObjectId),
            new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
            new XAttribute(xsi + "noNamespaceSchemaLocation", "explanatorynote-01-07.xsd"));

        if (!string.IsNullOrWhiteSpace(AccessRestriction))
            root.Add(new XAttribute("AccessRestriction", AccessRestriction));

        root.Add(
            new XElement("ExplanatoryNoteNumber", ExplanatoryNoteNumber),
            new XElement("ExplanatoryNoteYear", ExplanatoryNoteYear),
            new XElement("IssueAuthor",
                new XElement("Organization",
                    new XAttribute("NOPRIZNumber", "П-171-009725085022-0614"),
                    new XElement("FullName", string.IsNullOrWhiteSpace(AuthorFullName) ? "Организация — автор пояснительной записки" : AuthorFullName),
                    new XElement("AbbreviatedName", string.IsNullOrWhiteSpace(AuthorShortName) ? "" : AuthorShortName),
                    new XElement("OGRN", string.IsNullOrWhiteSpace(AuthorOgrn) ? "" : AuthorOgrn),
                    new XElement("INN", string.IsNullOrWhiteSpace(AuthorInn) ? "" : AuthorInn),
                    new XElement("KPP", string.IsNullOrWhiteSpace(AuthorKpp) ? "" : AuthorKpp),
                    new XElement("Address",
                        new XElement("RussianAddress",
                            new XElement("RegionCode", ObjectRegionCode),
                            new XElement("OKTMOCode", ObjectOktmoCode),
                            new XElement("OKTMOName", ObjectOktmoName),
                            new XElement("City", ObjectCity),
                            new XElement("Street", ObjectStreet),
                            new XElement("Building", ObjectBuilding))))),
            new XElement("Developer",
                new XElement("Organization",
                    new XElement("FullName", string.IsNullOrWhiteSpace(DeveloperFullName) ? "Организация — разработчик" : DeveloperFullName),
                    new XElement("AbbreviatedName", string.IsNullOrWhiteSpace(DeveloperShortName) ? "" : DeveloperShortName),
                    new XElement("OGRN", string.IsNullOrWhiteSpace(DeveloperOgrn) ? "" : DeveloperOgrn),
                    new XElement("INN", string.IsNullOrWhiteSpace(DeveloperInn) ? "" : DeveloperInn),
                    new XElement("KPP", string.IsNullOrWhiteSpace(DeveloperKpp) ? "" : DeveloperKpp),
                    new XElement("Address",
                        new XElement("RussianAddress",
                            new XElement("RegionCode", ObjectRegionCode),
                            new XElement("OKTMOCode", ObjectOktmoCode),
                            new XElement("OKTMOName", ObjectOktmoName),
                            new XElement("City", ObjectCity),
                            new XElement("Street", ObjectStreet),
                            new XElement("Building", ObjectBuilding))))),
            new XElement("DesignerNote",
                new XAttribute("Title", "Состав и границы корректировки"),
                new XElement("Text", string.IsNullOrWhiteSpace(DesignNote) ? "Настоящая пояснительная записка подготовлена на основании технического задания и проектных данных." : DesignNote)),
            new XElement(ObjectType,
                new XAttribute("ObjectID", string.IsNullOrWhiteSpace(ObjectId) ? Guid.NewGuid().ToString().ToUpperInvariant() : ObjectId),
                new XAttribute("Placement", "1"),
                new XAttribute("DangerousAndComplex", "0"),
                new XAttribute("Unique", "0"),
                new XElement("Name", string.IsNullOrWhiteSpace(ObjectName) ? "Объект капитального строительства" : ObjectName),
                new XElement("ConstructionType", "1"),
                new XElement("Address",
                    new XElement("RussianAddress",
                        new XElement("RegionCode", ObjectRegionCode),
                        new XElement("OKTMOCode", ObjectOktmoCode),
                        new XElement("OKTMOName", ObjectOktmoName),
                        new XElement("City", ObjectCity),
                        new XElement("Street", ObjectStreet),
                        new XElement("Building", ObjectBuilding)))));

        return root;
    }
}
