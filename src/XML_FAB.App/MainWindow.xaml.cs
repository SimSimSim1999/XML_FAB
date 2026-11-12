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
    private readonly ProjectStorageService _projectStorage = new();

    private string? _xmlPath;
    private string? _schemaPath;
    private string? _projectPath;
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

        UpdateProjectStatus("Проект не выбран. Создайте новый или откройте файл проекта.");
        MessagesList.Items.Add("Создайте проект или откройте существующий файл проекта (.json).");
    }

    private void NewDocument_Click(object sender, RoutedEventArgs e)
    {
        var editor = new EditorWindow { Owner = this };
        if (editor.ShowDialog() != true)
            return;

        _draft = editor.Model;
        _projectPath = null;
        _xmlPath = null;
        UpdateProjectStatus("Новый проект — ещё не сохранён.");
        ShowProjectSummary("Создан новый проект.");

        // Предлагаем сразу сохранить проект в выбранное пользователем место.
        SaveProject_Click(sender, e);
    }

    private void OpenProject_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Открыть проект XML FAB",
            Filter = "Проекты XML FAB (*.json)|*.json|Все файлы (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            _draft = _projectStorage.Load(dialog.FileName);
            _projectPath = Path.GetFullPath(dialog.FileName);
            _xmlPath = null;
            UpdateProjectStatus(_projectPath);
            ShowProjectSummary("Проект открыт.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не удалось открыть проект: {ex.Message}", "XML FAB", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EditProject_Click(object sender, RoutedEventArgs e)
    {
        var editor = new EditorWindow(_draft) { Owner = this };
        if (editor.ShowDialog() != true)
            return;

        _draft = editor.Model;
        UpdateProjectStatus(string.IsNullOrWhiteSpace(_projectPath)
            ? "Новый проект — ещё не сохранён."
            : _projectPath);
        ShowProjectSummary("Изменения применены.");
    }

    private void SaveProject_Click(object sender, RoutedEventArgs e)
    {
        var path = _projectPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            var dialog = new SaveFileDialog
            {
                Title = "Сохранить проект XML FAB",
                Filter = "Проекты XML FAB (*.json)|*.json",
                DefaultExt = ".json",
                AddExtension = true,
                FileName = string.IsNullOrWhiteSpace(_draft.ExplanatoryNoteNumber)
                    ? "project.json"
                    : $"{_draft.ExplanatoryNoteNumber}.json"
            };

            if (dialog.ShowDialog(this) != true)
                return;

            path = dialog.FileName;
        }

        try
        {
            _projectStorage.Save(path, _draft);
            _projectPath = Path.GetFullPath(path);
            UpdateProjectStatus(_projectPath);
            ShowProjectSummary("Проект сохранён.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не удалось сохранить проект: {ex.Message}", "XML FAB", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenXml_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Открыть XML-документ",
            Filter = "XML-документы (*.xml)|*.xml|Все файлы (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != true)
            return;

        _xmlPath = dialog.FileName;
        MessagesList.Items.Clear();
        MessagesList.Items.Add($"Выбран XML: {_xmlPath}");
    }

    private void OpenSchema_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Выбрать XML-схему",
            Filter = "XML-схемы (*.xsd)|*.xsd|Все файлы (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != true)
            return;

        _schemaPath = dialog.FileName;
        SchemaPathText.Text = _schemaPath;
        MessagesList.Items.Clear();
        MessagesList.Items.Add("Схема выбрана. Можно запускать проверку.");
    }

    private void GenerateXml_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_draft.ExplanatoryNoteNumber))
        {
            MessageBox.Show(this, "Сначала заполните хотя бы шифр пояснительной записки.", "XML FAB", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Сохранить сформированный XML",
            Filter = "XML-документы (*.xml)|*.xml",
            DefaultExt = ".xml",
            AddExtension = true,
            FileName = "explanatorynote.xml"
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            var xml = _xmlGenerator.Generate(_draft);
            _xmlGenerator.SaveXml(dialog.FileName, xml);
            _xmlPath = dialog.FileName;

            MessagesList.Items.Clear();
            MessagesList.Items.Add($"XML сохранён: {_xmlPath}");
            MessagesList.Items.Add("XML сформирован. Проверка выполняется отдельно кнопкой «Проверить XML».");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не удалось сформировать XML: {ex.Message}", "XML FAB", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Validate_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_xmlPath) || !File.Exists(_xmlPath))
        {
            OpenXml_Click(sender, e);
            if (string.IsNullOrWhiteSpace(_xmlPath) || !File.Exists(_xmlPath))
                return;
        }

        if (string.IsNullOrWhiteSpace(_schemaPath) || !File.Exists(_schemaPath))
        {
            MessageBox.Show(this, "Не найдена XSD-схема. Выберите файл схемы.", "Проверка XML", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessagesList.Items.Clear();
        var result = _validationService.Validate(_xmlPath, _schemaPath);
        MessagesList.Items.Add(result.IsValid
            ? "XML прошёл проверку по XSD."
            : "XML не прошёл проверку либо схема содержит ошибку.");
        foreach (var message in result.Messages)
            MessagesList.Items.Add(message);
    }

    private void ShowProjectSummary(string title)
    {
        MessagesList.Items.Clear();
        MessagesList.Items.Add(title);
        MessagesList.Items.Add($"Шифр: {ValueOrDash(_draft.ExplanatoryNoteNumber)}");
        MessagesList.Items.Add($"Год: {ValueOrDash(_draft.ExplanatoryNoteYear)}");
        MessagesList.Items.Add($"Объект: {ValueOrDash(_draft.NonIndustrialObject?.Name)}");
        MessagesList.Items.Add($"Файл проекта: {(_projectPath ?? "не сохранён")}");
    }

    private void UpdateProjectStatus(string text)
    {
        ProjectStatusText.Text = text;
        ProjectStatusText.ToolTip = text;
    }

    private static string ValueOrDash(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "—" : value;

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void About_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show(this,
            "XML FAB — приложение для заполнения и генерации XML пояснительной записки.",
            "О программе",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
}
