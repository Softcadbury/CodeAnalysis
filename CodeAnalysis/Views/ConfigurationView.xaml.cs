namespace CodeAnalysis.Views
{
    using System.Windows.Controls;
    using CodeAnalysis.ViewModels;

    /// <summary>
    /// Interaction logic for ConfigurationView.xaml
    /// </summary>
    public partial class ConfigurationView : UserControl
    {
        public static ConfigurationViewModel ViewModel;

        public ConfigurationView()
        {
            InitializeComponent();

            ViewModel = new ConfigurationViewModel();
            this.DataContext = ViewModel;
        }
    }
}