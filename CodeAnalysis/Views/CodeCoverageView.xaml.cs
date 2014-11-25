namespace CodeAnalysis.Views
{
    using System.Windows.Controls;
    using CodeAnalysis.ViewModels;

    /// <summary>
    /// Interaction logic for CodeCoverageView.xaml
    /// </summary>
    public partial class CodeCoverageView : UserControl
    {
        public static CodeCoverageViewModel ViewModel;

        public CodeCoverageView()
        {
            InitializeComponent();

            ViewModel = new CodeCoverageViewModel();
            this.DataContext = ViewModel;
        }
    }
}