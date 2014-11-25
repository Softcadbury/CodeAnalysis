namespace CodeAnalysis.Views
{
    using System.Windows.Controls;
    using CodeAnalysis.ViewModels;

    /// <summary>
    /// Interaction logic for CodeMetricsView.xaml
    /// </summary>
    public partial class CodeMetricsView : UserControl
    {
        public static CodeMetricsViewModel ViewModel;

        public CodeMetricsView()
        {
            InitializeComponent();

            ViewModel = new CodeMetricsViewModel();
            this.DataContext = ViewModel;
        }
    }
}