namespace CodeAnalysis.Views
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;

    using CodeAnalysis.Core;
    using CodeAnalysis.Models;
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

        /// <summary>
        /// Open the difference of code between the trunk and the branche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenFileDiff(object sender, MouseButtonEventArgs e)
        {
            var tree = sender as TreeListView;
            if (tree == null)
            {
                return;
            }

            var lineView = tree.SelectedValue as CodeMetricsLineView;
            if (lineView == null)
            {
                return;
            }

            if (lineView.Type != null && !string.IsNullOrWhiteSpace(ConfigurationView.ViewModel.TrunkName) && !string.IsNullOrWhiteSpace(ConfigurationView.ViewModel.BrancheName))
            {
                const string TortoisemergeExe = "C:/Program Files/TortoiseSVN/bin/TortoiseMerge.exe";

                string trunkPath = AppDomain.CurrentDomain.BaseDirectory + "data\\" + ConfigurationView.ViewModel.TrunkName;
                string branchePath = AppDomain.CurrentDomain.BaseDirectory + "data\\" + ConfigurationView.ViewModel.BrancheName;

                var indexOfBadChar = lineView.Type.IndexOf('<');
                var fileName = indexOfBadChar > -1 ? lineView.Type.Remove(indexOfBadChar) : lineView.Type;
                fileName += ".cs";

                var devFile = Directory.GetFiles(trunkPath, fileName, SearchOption.AllDirectories).FirstOrDefault();
                var sprintFile = Directory.GetFiles(branchePath, fileName, SearchOption.AllDirectories).FirstOrDefault();

                Process.Start(TortoisemergeExe, "/base:\"" + devFile + "\" /mine:\"" + sprintFile + "\"");
            }
        }
    }
}