using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using XML_FAB.Models;

namespace XML_FAB;

public partial class EditorWindow : Window
{
    public ExplanatoryNoteModel Model { get; }

    public EditorWindow() : this(new ExplanatoryNoteModel())
    {
    }

    public EditorWindow(ExplanatoryNoteModel model)
    {
        InitializeComponent();
        Model = model;
        EnsureBoundCollections();
        DataContext = Model;
    }

    private void Apply_Click(object sender, RoutedEventArgs e)
    {
        // Flush the currently edited control and every other editor binding before
        // the dialog closes. This ensures values in list/nested-object bindings are
        // present when the project is saved or XML is generated.
        UpdateBindings(this);
        DialogResult = true;
    }

    private void EnsureBoundCollections()
    {
        Model.ExplanatoryNoteModifications ??= new List<ModificationItem>();
        if (Model.ExplanatoryNoteModifications.Count == 0)
            Model.ExplanatoryNoteModifications.Add(new ModificationItem());

        Model.IssueAuthor ??= new OrganizationInfo();
        Model.IssueAuthor.Address ??= new AddressInfo();
        Model.Developer ??= new OrganizationInfo();
        Model.Developer.Address ??= new AddressInfo();

        Model.Signers ??= new List<SignerInfo>();
        if (Model.Signers.Count == 0)
            Model.Signers.Add(new SignerInfo());

        Model.UsedNorms ??= new List<string>();
        if (Model.UsedNorms.Count == 0)
            Model.UsedNorms.Add(string.Empty);

        Model.ProjectDecisionDocuments ??= new List<ProjectDocument>();
        if (Model.ProjectDecisionDocuments.Count == 0)
            Model.ProjectDecisionDocuments.Add(new ProjectDocument());
        Model.ProjectDecisionDocuments[0] ??= new ProjectDocument();
        Model.ProjectDecisionDocuments[0].File ??= new XML_FAB.Models.FileInfo();

        Model.ProjectInitialDocuments ??= new List<ProjectDocument>();
        if (Model.ProjectInitialDocuments.Count == 0)
            Model.ProjectInitialDocuments.Add(new ProjectDocument());
        Model.ProjectInitialDocuments[0] ??= new ProjectDocument();

        Model.AdditionalData ??= new List<AdditionalDataItem>();
        if (Model.AdditionalData.Count == 0)
            Model.AdditionalData.Add(new AdditionalDataItem());
        Model.AdditionalData[0] ??= new AdditionalDataItem();
        Model.AdditionalData[0].File ??= new XML_FAB.Models.FileInfo();

        Model.NonIndustrialObject ??= new ObjectInfo();
        Model.NonIndustrialObject.Address ??= new AddressInfo();

        Model.DesignerNotes ??= new List<DesignerNoteItem>();
        if (Model.DesignerNotes.Count == 0)
            Model.DesignerNotes.Add(new DesignerNoteItem());
        Model.DesignerNotes[0] ??= new DesignerNoteItem();
        Model.DesignerNotes[0].Text ??= new List<string>();
        if (Model.DesignerNotes[0].Text.Count == 0)
            Model.DesignerNotes[0].Text.Add(string.Empty);

        Model.DesignerAssurance ??= new List<string>();
        if (Model.DesignerAssurance.Count == 0)
            Model.DesignerAssurance.Add(string.Empty);
    }

    private static void UpdateBindings(DependencyObject parent)
    {
        if (parent is TextBox textBox)
            textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

        if (parent is ComboBox comboBox)
            comboBox.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();

        var childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var index = 0; index < childCount; index++)
            UpdateBindings(VisualTreeHelper.GetChild(parent, index));
    }
}
