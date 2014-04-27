namespace CodeAnalysis.Views
{
    using CodeAnalysis.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for CodeMetricsView.xaml
    /// </summary>
    public partial class CodeMetricsView : UserControl
    {
        public CodeMetricsView()
        {
            InitializeComponent();
            this.DataContext = new CodeMetricsViewModel();
        }
    }
}