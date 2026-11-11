using Microsoft.Win32;
using System.IO;
using System.Windows;
using XML_FAB.Models;
using XML_FAB.Services;

namespace XML_FAB;

public partial class MainWindow : Window
{
    private readonly XmlSchemaValidationService _validationService = new();
    private readonly XmlGeneratorService _xmlGenerator = new();
    private readonly DocumentStorageService _storage = new();

    private string? _xmlPath;
    private string? _schemaPath;
    private ExplanatoryNoteModel _draft = new();

    public MainWindow()
    {
        InitializeComponent();

        var bundledSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "explanatorynote-01-07.xsd");
        if (File.Exists(bundledSchema))
        {
            _schemaPath = bundledSchema;
            SchemaPathText.Text = bundledSchema;
        }
        else
        {
            SchemaPathText.Text = "Не найдена; выберите файл схемы вручную.";
        }

        MessagesList.Items.Add("Выберите XML-файл и нажмите «Проверить XML».");
    }

    private void NewDocument_Click(object sender, RoutedEventArgs e)
    {
        var editor = new EditorWindow { Owner = this };

        if (editor.ShowDialog() == true)
        {
            _draft = editor.Model;
            MessagesList.Items.Clear();
            MessagesList.Items.Add("Данные формы применены.");
            MessagesList.Items.Add($"Шифр: {_draft.ExplanatoryNoteNumber}");
            MessagesList.Items.Add($"Объект: {_draft.ObjectName}");
        }
    }

    // остальные методы без дубликатов
}
