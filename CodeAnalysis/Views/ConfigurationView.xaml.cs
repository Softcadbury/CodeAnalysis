namespace CodeAnalysis.Views
{
    using CodeAnalysis.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        public ConfigurationView()
        {
            InitializeComponent();
            this.DataContext = new ConfigurationViewModel();
        }
    }
}