namespace XML_FAB.Models;

public sealed class DocumentDraft
{
    public string SchemaVersion { get; set; } = "01.07";

    // Экран 1: основные сведения
    public string ExplanatoryNoteNumber { get; set; } = "";
    public string ExplanatoryNoteYear { get; set; } = DateTime.Now.Year.ToString();
    public string AccessRestriction { get; set; } = "";

    // Экран 2: организация — автор пояснительной записки
    public string AuthorFullName { get; set; } = "";
    public string AuthorShortName { get; set; } = "";
    public string AuthorOgrn { get; set; } = "";
    public string AuthorInn { get; set; } = "";

    // Экран 3: объект
    public string ObjectType { get; set; } = "NonIndustrialObject";
    public string ObjectName { get; set; } = "";
    public string ObjectId { get; set; } = "";
}
