namespace CodeAnalysis.ViewModels
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;

    using CodeAnalysis.BusinessLogic;
    using CodeAnalysis.Core;
    using CodeAnalysis.Models;
    using Microsoft.Win32;

    /// <summary>
    /// ViewModel for CodeMetricsView
    /// </summary>
    public class CodeMetricsViewModel : BaseViewModel
    {
        public CodeMetricsViewModel()
        {
            BrowseCodeMetricsTrunkFileCommand = new RelayCommand(param => BrowseFiles(FileType.TrunkMetrics));
            BrowseCodeMetricsBrancheFileCommand = new RelayCommand(param => BrowseFiles(FileType.BrancheMetrics));
            ProceedCodeMetricsCommand = new RelayCommand(param => ProceedCodeMetrics());

            CodeMetricsTree = new ObservableCollection<CodeMetricsLineView>();
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
            set
            {
                codeMetricsTree = value;
                IsTreeVisible = codeMetricsTree != null && codeMetricsTree.Any();
                OnPropertyChanged("CodeMetricsTree");
            }
        }

        private bool isTreeVisible;

        public bool IsTreeVisible
        {
            get { return isTreeVisible; }
            set { isTreeVisible = value; OnPropertyChanged("IsTreeVisible"); }
        }

        private enum FileType
        {
            TrunkMetrics,
            BrancheMetrics
        }

        /// <summary>
        /// Opens a dialog to browse code metrics files
        /// </summary>
        private void BrowseFiles(FileType type)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open a code metrics file",
                Filter = "Code Metrics Files|*.xlsx;*.xml"
            };

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

        /// <summary>
        /// Generates code metrics
        /// </summary>
        private void ProceedCodeMetrics()
        {
            if (File.Exists(CodeMetricsTrunkFilePath) && File.Exists(CodeMetricsBrancheFilePath))
            {
                const string XlsxFile = ".xlsx";
                const string XmlFile = ".xml";

                CodeMetricsTree.Clear();

                if (CodeMetricsTrunkFilePath.EndsWith(XlsxFile) && CodeMetricsBrancheFilePath.EndsWith(XlsxFile))
                {
                    var codeMetricsTrunkFile = new StreamReader(CodeMetricsTrunkFilePath);
                    var codeMetricsBrancheFile = new StreamReader(CodeMetricsBrancheFilePath);
                    CodeMetricsTree = new ObservableCollection<CodeMetricsLineView>(CodeMetricsGeneratorFromExcel.Generate(codeMetricsTrunkFile, codeMetricsBrancheFile));
                }
                else if (CodeMetricsTrunkFilePath.EndsWith(XmlFile) && CodeMetricsBrancheFilePath.EndsWith(XmlFile))
                {
                    var codeMetricsTrunkFile = new StreamReader(CodeMetricsTrunkFilePath);
                    var codeMetricsBrancheFile = new StreamReader(CodeMetricsBrancheFilePath);
                    CodeMetricsTree = new ObservableCollection<CodeMetricsLineView>(CodeMetricsGeneratorFromXml.Generate(codeMetricsTrunkFile, codeMetricsBrancheFile));
                }

                if (CodeMetricsTree != null)
                {
                    AverageGenerator.AddCodeMetricsAverage(CodeMetricsTree);
                }
            }
        }
    }
}