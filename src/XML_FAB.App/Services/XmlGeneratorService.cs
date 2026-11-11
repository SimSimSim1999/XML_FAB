using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using XML_FAB.Models;

namespace XML_FAB.Services;

public sealed class XmlGeneratorService
{
    public XDocument Generate(ExplanatoryNoteModel model)
    {
        var xml = model.ToXml();
        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            xml);
    }

    public void SaveXml(string path, XDocument xml)
    {
        xml.Save(path);
    }
}

public sealed class DocumentStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public void SaveDraft(string path, ExplanatoryNoteModel model)
    {
        var json = JsonSerializer.Serialize(model, JsonOptions);
        File.WriteAllText(path, json);
    }

    public ExplanatoryNoteModel LoadDraft(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ExplanatoryNoteModel>(json) ?? new ExplanatoryNoteModel();
    }
}
