namespace CodeAnalysis.ViewModels
{
    using CodeAnalysis.BusinessLogic;
    using CodeAnalysis.Core;
    using CodeAnalysis.Models;
    using Microsoft.Win32;
    using System.Collections.ObjectModel;
    using System.IO;

    /// <summary>
    /// ViewModel for CodeCoverageView
    /// </summary>
    public class CodeCoverageViewModel : BaseViewModel
    {
        public CodeCoverageViewModel()
        {
            BrowseCodeCoverageTrunkFileCommand = new RelayCommand(param => BrowseFiles(FileType.TrunkCoverage));
            BrowseCodeCoverageBrancheFileCommand = new RelayCommand(param => BrowseFiles(FileType.BrancheCoverage));
            ProceedCodeCoverageCommand = new RelayCommand(param => ProceedCodeCoverage());

            CodeCoverageTree = new ObservableCollection<CodeCoverageLineView>();

            CodeCoverageTrunkFilePath = CommandLineArguments.GetArgument("CodeCoverageTrunkFilePath");
            CodeCoverageBrancheFilePath = CommandLineArguments.GetArgument("CodeCoverageBrancheFilePath");
            ProceedCodeCoverage();
        }

        public RelayCommand BrowseCodeCoverageTrunkFileCommand { get; set; }
        public RelayCommand BrowseCodeCoverageBrancheFileCommand { get; set; }
        public RelayCommand ProceedCodeCoverageCommand { get; set; }

        private string codeCoverageTrunkFilePath;

        public string CodeCoverageTrunkFilePath
        {
            get { return codeCoverageTrunkFilePath; }
            set { codeCoverageTrunkFilePath = value; OnPropertyChanged("CodeCoverageTrunkFilePath"); }
        }

        private string codeCoverageBrancheFilePath;

        public string CodeCoverageBrancheFilePath
        {
            get { return codeCoverageBrancheFilePath; }
            set { codeCoverageBrancheFilePath = value; OnPropertyChanged("CodeCoverageBrancheFilePath"); }
        }

        private ObservableCollection<CodeCoverageLineView> codeCoverageTree;

        public ObservableCollection<CodeCoverageLineView> CodeCoverageTree
        {
            get { return codeCoverageTree; }
            set { codeCoverageTree = value; OnPropertyChanged("CodeCoverageTree"); }
        }

        private enum FileType
        {
            TrunkCoverage,
            BrancheCoverage
        }

        /// <summary>
        /// Opens a dialog to browse code coverage files
        /// </summary>
        private void BrowseFiles(FileType type)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open a code coverage file";
            dialog.Filter = "Code Coverage Files|*.coveragexml";

            if (dialog.ShowDialog() == true)
            {
                switch (type)
                {
                    case FileType.TrunkCoverage:
                        CodeCoverageTrunkFilePath = dialog.FileName;
                        break;

                    case FileType.BrancheCoverage:
                        CodeCoverageBrancheFilePath = dialog.FileName;
                        break;
                }
            }
        }

        /// <summary>
        /// Generates code coverage
        /// </summary>
        private void ProceedCodeCoverage()
        {
            if (File.Exists(CodeCoverageTrunkFilePath) && File.Exists(CodeCoverageBrancheFilePath))
            {
                var codeCoverageTrunkFile = new StreamReader(CodeCoverageTrunkFilePath);
                var codeCoverageBrancheFile = new StreamReader(CodeCoverageBrancheFilePath);

                CodeCoverageTree.Clear();
                var tree = CodeCoverageGenerator.Generate(codeCoverageTrunkFile, codeCoverageBrancheFile);
                CodeCoverageTree = new ObservableCollection<CodeCoverageLineView>(tree);

                AverageGenerator.AddCodeCoverageAverage(CodeCoverageTree);
            }
        }
    }
}