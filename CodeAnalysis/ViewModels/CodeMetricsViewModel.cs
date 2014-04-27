namespace CodeAnalysis.ViewModels
{
    using CodeAnalysis.BusinessLogic;
    using CodeAnalysis.Core;
    using CodeAnalysis.Models;
    using Microsoft.Win32;
    using System.Collections.ObjectModel;
    using System.IO;

    public class CodeMetricsViewModel : BaseViewModel
    {
        public CodeMetricsViewModel()
        {
            BrowseCodeMetricsTrunkFileCommand = new RelayCommand(param => BrowseFiles(FileType.TrunkMetrics));
            BrowseCodeMetricsBrancheFileCommand = new RelayCommand(param => BrowseFiles(FileType.BrancheMetrics));
            ProceedCodeMetricsCommand = new RelayCommand(param => ProceedCodeMetrics());
        }

        public RelayCommand BrowseCodeMetricsTrunkFileCommand { get; set; }
        public RelayCommand BrowseCodeMetricsBrancheFileCommand { get; set; }
        public RelayCommand ProceedCodeMetricsCommand { get; set; }

        private string codeMetricsTrunkFilePath;

        public string CodeMetricsTrunkFilePath
        {
            get { return codeMetricsTrunkFilePath; }
            set { codeMetricsTrunkFilePath = value; OnPropertyChanged("CodeMetricsTrunkFilePath"); }
        }

        private string codeMetricsBrancheFilePath;

        public string CodeMetricsBrancheFilePath
        {
            get { return codeMetricsBrancheFilePath; }
            set { codeMetricsBrancheFilePath = value; OnPropertyChanged("CodeMetricsBrancheFilePath"); }
        }

        private ObservableCollection<CodeMetricsLineView> codeMetricsTree;

        public ObservableCollection<CodeMetricsLineView> CodeMetricsTree
        {
            get { return codeMetricsTree; }
            set { codeMetricsTree = value; OnPropertyChanged("CodeMetricsTree"); }
        }

        private enum FileType
        {
            TrunkMetrics,
            BrancheMetrics
        }

        private void BrowseFiles(FileType type)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Open a code metrics file";
            dialog.Filter = "Code Metrics Files|*.xlsx";

            if (dialog.ShowDialog() == true)
            {
                switch (type)
                {
                    case FileType.TrunkMetrics:
                        CodeMetricsTrunkFilePath = dialog.FileName;
                        break;

                    case FileType.BrancheMetrics:
                        CodeMetricsBrancheFilePath = dialog.FileName;
                        break;
                }
            }
        }

        private void ProceedCodeMetrics()
        {
            if (!string.IsNullOrWhiteSpace(CodeMetricsTrunkFilePath) && !string.IsNullOrWhiteSpace(CodeMetricsBrancheFilePath))
            {
                var codeMetricsTrunkFile = new StreamReader(CodeMetricsTrunkFilePath);
                var codeMetricsBrancheFile = new StreamReader(CodeMetricsBrancheFilePath);

                var tmpTree = CodeMetricsGenerator.Generate(codeMetricsTrunkFile, codeMetricsBrancheFile);
                CodeMetricsTree = new ObservableCollection<CodeMetricsLineView>(tmpTree);
            }
        }
    }
}