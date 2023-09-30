using System.Windows.Controls;
using System.Windows.Markup;

namespace DentalCore.Wpf.Views;

public partial class PatientCreateView : UserControl
{
    public PatientCreateView()
    {
        InitializeComponent();
        Language = XmlLanguage.GetLanguage("uk-UA");
    }
}