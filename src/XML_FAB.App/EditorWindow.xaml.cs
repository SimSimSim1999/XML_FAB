using System.Windows;
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
        DataContext = Model;
    }

    private void Apply_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}
