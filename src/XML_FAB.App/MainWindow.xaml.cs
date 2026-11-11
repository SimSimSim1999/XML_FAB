using Microsoft.Win32;
using System.IO;
using System.Windows;
using XML_FAB.Services;

namespace XML_FAB;

public partial class MainWindow : Window
{
    private readonly XmlSchemaValidationService _validationService = new();
    private string? _xmlPath;
    private string? _schemaPath;

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

    
    private void OpenXml_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Открыть XML-документ",
            Filter = "XML-документы (*.xml)|*.xml|Все файлы (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == true)
        {
            _xmlPath = dialog.FileName;
            MessagesList.Items.Clear();
            MessagesList.Items.Add($"Выбран XML: {_xmlPath}");
        }
    }

    private void OpenSchema_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выбрать XML-схему",
            Filter = "XML-схемы (*.xsd)|*.xsd|Все файлы (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == true)
        {
            _schemaPath = dialog.FileName;
            SchemaPathText.Text = _schemaPath;
            MessagesList.Items.Clear();
            MessagesList.Items.Add("Схема выбрана. Можно запускать проверку.");
        }
    }

    private void NewDocument_Click(object sender, RoutedEventArgs e)
    {
        var window = new EditorWindow { Owner = this };
        window.ShowDialog();
    }

    private void Validate_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_xmlPath) || !File.Exists(_xmlPath))
        {
            OpenXml_Click(sender, e);
            if (string.IsNullOrWhiteSpace(_xmlPath) || !File.Exists(_xmlPath)) return;
        }

        if (string.IsNullOrWhiteSpace(_schemaPath) || !File.Exists(_schemaPath))
        {
            MessageBox.Show(this, "Не найдена XSD-схема. Выберите файл схемы.", "Проверка XML", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessagesList.Items.Clear();
        var result = _validationService.Validate(_xmlPath, _schemaPath);
        MessagesList.Items.Add(result.IsValid ? "XML прошёл проверку по XSD." : "XML не прошёл проверку либо схема содержит ошибку.");
        foreach (var message in result.Messages)
            MessagesList.Items.Add(message);
    }

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void About_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(this, "XML FAB — автономный WPF-каркас для заполнения и проверки пояснительной записки.\n\nТекущий этап: выбор XML/XSD и проверка XML по схеме.", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
}
