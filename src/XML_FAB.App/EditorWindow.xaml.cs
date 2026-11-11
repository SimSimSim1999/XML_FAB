using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using XML_FAB.Models;

namespace XML_FAB;

public partial class EditorWindow : Window
{
    public EditorWindow()
    {
        InitializeComponent();
        NoteYearTextBox.Text = DateTime.Now.Year.ToString();
    }

    private void SaveDraft_Click(object sender, RoutedEventArgs e)
    {
        var selectedType = (ObjectTypeComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString()
                           ?? "NonIndustrialObject";

        var draft = new DocumentDraft
        {
            ExplanatoryNoteNumber = NoteNumberTextBox.Text.Trim(),
            ExplanatoryNoteYear = NoteYearTextBox.Text.Trim(),
            AccessRestriction = AccessRestrictionTextBox.Text.Trim(),

            AuthorFullName = AuthorFullNameTextBox.Text.Trim(),
            AuthorShortName = AuthorShortNameTextBox.Text.Trim(),
            AuthorOgrn = AuthorOgrnTextBox.Text.Trim(),
            AuthorInn = AuthorInnTextBox.Text.Trim(),

            ObjectType = selectedType,
            ObjectName = ObjectNameTextBox.Text.Trim(),
            ObjectId = ObjectIdTextBox.Text.Trim()
        };

        var dialog = new SaveFileDialog
        {
            Title = "Сохранить черновик",
            Filter = "Черновики XML FAB (*.json)|*.json",
            DefaultExt = ".json",
            AddExtension = true,
            FileName = "document-draft.json"
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            var json = JsonSerializer.Serialize(
                draft,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(dialog.FileName, json);

            MessageBox.Show(
                this,
                "Черновик сохранён локально. Это файл рабочего состояния, не итоговый XML.",
                "XML FAB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (IOException ex)
        {
            MessageBox.Show(
                this,
                $"Не удалось сохранить файл: {ex.Message}",
                "Ошибка сохранения",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show(
                this,
                $"Нет доступа к выбранному месту: {ex.Message}",
                "Ошибка сохранения",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
