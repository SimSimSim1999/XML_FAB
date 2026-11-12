using System.Text.Json;
using XML_FAB.Models;

namespace XML_FAB.Services;

public sealed class ProjectStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public void Save(string path, ExplanatoryNoteModel model)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(model);

        var fullPath = Path.GetFullPath(path);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(model, JsonOptions);
        File.WriteAllText(fullPath, json);
    }

    public ExplanatoryNoteModel Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<ExplanatoryNoteModel>(json, JsonOptions)
            ?? throw new InvalidDataException("Файл проекта пуст или содержит некорректные данные.");
    }
}
