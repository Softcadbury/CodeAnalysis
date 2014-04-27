namespace CodeAnalysis.Views
{
    using CodeAnalysis.ViewModels;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for CodeCoverageView.xaml
    /// </summary>
    public partial class CodeCoverageView : UserControl
    {
        public CodeCoverageView()
        {
            InitializeComponent();
            this.DataContext = new CodeCoverageViewModel();
        }
    }
}