using Microsoft.Win32;
using System.IO;
using System.Text;
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
    private XmlValidationResult? _lastValidationResult;
    private DateTimeOffset? _lastValidationAt;

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
        _lastValidationResult = null;
        _lastValidationAt = null;
        UpdateProjectStatus("Новый проект — ещё не сохранён.");
        ShowProjectSummary("Создан новый проект.");

        // Предлагаем сразу выбрать имя и место для JSON-проекта.
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
            _lastValidationResult = null;
            _lastValidationAt = null;
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
        _lastValidationResult = null;
        _lastValidationAt = null;
        UpdateProjectStatus(string.IsNullOrWhiteSpace(_projectPath)
            ? "Новый проект — ещё не сохранён."
            : _projectPath);
        ShowProjectSummary("Изменения применены. Не забудьте сохранить проект.");
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
        _lastValidationResult = null;
        _lastValidationAt = null;
        MessagesList.Items.Clear();
        MessagesList.Items.Add($"Выбран XML: {_xmlPath}");
        MessagesList.Items.Add("Нажмите «Проверить XML», чтобы получить результаты проверки.");
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
        _lastValidationResult = null;
        _lastValidationAt = null;
        MessagesList.Items.Clear();
        MessagesList.Items.Add("Схема выбрана. Можно запускать проверку.");
        MessagesList.Items.Add(_schemaPath);
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
            _lastValidationResult = null;
            _lastValidationAt = null;

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

        try
        {
            _lastValidationResult = _validationService.Validate(_xmlPath, _schemaPath);
            _lastValidationAt = DateTimeOffset.Now;

            MessagesList.Items.Clear();
            MessagesList.Items.Add(_lastValidationResult.IsValid
                ? "XML прошёл проверку по XSD."
                : "XML не прошёл проверку либо схема содержит ошибку.");
            MessagesList.Items.Add($"Проверено: {_lastValidationAt:yyyy-MM-dd HH:mm:ss}");
            foreach (var message in _lastValidationResult.Messages)
                MessagesList.Items.Add(message);
        }
        catch (Exception ex)
        {
            _lastValidationResult = new XmlValidationResult(false, new[] { $"Не удалось выполнить проверку: {ex.Message}" });
            _lastValidationAt = DateTimeOffset.Now;
            MessagesList.Items.Clear();
            MessagesList.Items.Add("Не удалось выполнить проверку XML.");
            MessagesList.Items.Add(ex.Message);
        }
    }

    private void ExportValidationReport_Click(object sender, RoutedEventArgs e)
    {
        if (_lastValidationResult is null)
        {
            MessageBox.Show(this,
                "Сначала выберите XML-файл и нажмите «Проверить XML». Затем можно будет выгрузить отчет с результатом и ошибками.",
                "Отчет проверки XML",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Сохранить отчет проверки XML",
            Filter = "Текстовый отчет (*.txt)|*.txt",
            DefaultExt = ".txt",
            AddExtension = true,
            FileName = $"xml-validation-report-{DateTime.Now:yyyyMMdd-HHmmss}.txt"
        };

        if (dialog.ShowDialog(this) != true)
            return;

        var report = new StringBuilder();
        report.AppendLine("XML FAB — отчет проверки XML по XSD");
        report.AppendLine($"Дата проверки: {_lastValidationAt?.ToString("yyyy-MM-dd HH:mm:ss zzz") ?? "не указана"}");
        report.AppendLine($"Результат: {(_lastValidationResult.IsValid ? "УСПЕШНО" : "ОШИБКИ / ПРЕДУПРЕЖДЕНИЯ")}");
        report.AppendLine($"XML-файл: {_xmlPath ?? "не указан"}");
        report.AppendLine($"XSD-схема: {_schemaPath ?? "не указана"}");
        report.AppendLine();
        report.AppendLine("Сообщения проверки:");

        if (_lastValidationResult.Messages.Count == 0)
        {
            report.AppendLine("Сообщений нет.");
        }
        else
        {
            for (var index = 0; index < _lastValidationResult.Messages.Count; index++)
                report.AppendLine($"{index + 1}. {_lastValidationResult.Messages[index]}");
        }

        try
        {
            File.WriteAllText(dialog.FileName, report.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            MessagesList.Items.Add($"Отчет проверки сохранён: {dialog.FileName}");
            MessageBox.Show(this, "Отчет сохранён. Его можно прикрепить к сообщению или отправить разработчику.", "Отчет проверки XML", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Не удалось сохранить отчет: {ex.Message}", "Отчет проверки XML", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
