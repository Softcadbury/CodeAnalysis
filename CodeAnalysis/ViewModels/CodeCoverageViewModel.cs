namespace CodeAnalysis.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Data;

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
            IsNotLoading = true;
            BrowseCodeCoverageTrunkFileCommand = new RelayCommand(param => BrowseFiles(FileType.TrunkCoverage));
            BrowseCodeCoverageBrancheFileCommand = new RelayCommand(param => BrowseFiles(FileType.BrancheCoverage));
            ProceedCodeCoverageCommand = new RelayCommand(param => ProceedCodeCoverage());

            CodeCoverageTree = new ObservableCollection<CodeCoverageLineView>();
            BindingOperations.EnableCollectionSynchronization(CodeCoverageTree, _lock);
        }

        public RelayCommand BrowseCodeCoverageTrunkFileCommand { get; set; }
        public RelayCommand BrowseCodeCoverageBrancheFileCommand { get; set; }
        public RelayCommand ProceedCodeCoverageCommand { get; set; }
        private readonly object _lock = new object();

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
            set
            {
                codeCoverageTree = value;
                IsTreeVisible = codeCoverageTree != null && codeCoverageTree.Any();
                OnPropertyChanged("CodeCoverageTree");
            }
        }

        private bool isTreeVisible;
        public bool IsTreeVisible
        {
            get { return isTreeVisible; }
            set { isTreeVisible = value; OnPropertyChanged("IsTreeVisible"); }
        }

        private bool isNotLoading;
        public bool IsNotLoading
        {
            get { return !isNotLoading; }
            set { isNotLoading = !value; OnPropertyChanged("IsNotLoading"); }
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
            var dialog = new OpenFileDialog
            {
                Title = "Open a code coverage file",
                Filter = "Code Coverage Files|*.coveragexml"
            };

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
        public void ProceedCodeCoverage()
        {
            if (File.Exists(CodeCoverageTrunkFilePath) && File.Exists(CodeCoverageBrancheFilePath))
            {
                CodeCoverageTree.Clear();
                IsNotLoading = false;

                Task.Factory.StartNew(() =>
                {
                    var codeCoverageTrunkFile = new StreamReader(CodeCoverageTrunkFilePath);
                    var codeCoverageBrancheFile = new StreamReader(CodeCoverageBrancheFilePath);

                    var tree = CodeCoverageGenerator.Generate(codeCoverageTrunkFile, codeCoverageBrancheFile);
                    AverageGenerator.AddCodeCoverageAverage(tree);
                    CodeCoverageTree = new ObservableCollection<CodeCoverageLineView>(tree);
                    IsNotLoading = true;
                });
            }
        }
    }
}