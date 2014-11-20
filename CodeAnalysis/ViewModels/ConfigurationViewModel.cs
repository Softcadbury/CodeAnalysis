namespace CodeAnalysis.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using CodeAnalysis.Core;
    using CodeAnalysis.Properties;

    /// <summary>
    /// ViewModel for ConfigurationView
    /// </summary>
    public class ConfigurationViewModel : BaseViewModel
    {
        public ConfigurationViewModel()
        {
            UpdateRepositoriesCommand = new RelayCommand(param => UpdateRepositories());
            ProceedCodeMetricsCommand = new RelayCommand(param => ProceedCodeMetrics());
            ProceedCodeCoverageCommand = new RelayCommand(param => ProceedCodeCoverage());

            RepositoryUrl = Settings.Default.RepositoryUrl;
            TrunkName = Settings.Default.TrunkName;
            BrancheName = Settings.Default.BrancheName;
        }

        public RelayCommand UpdateRepositoriesCommand { get; set; }
        public RelayCommand ProceedCodeMetricsCommand { get; set; }
        public RelayCommand ProceedCodeCoverageCommand { get; set; }

        private string repository;
        public string RepositoryUrl
        {
            get { return repository; }
            set { Settings.Default.RepositoryUrl = repository = value; OnPropertyChanged("RepositoryUrl"); }
        }

        private string trunkName;
        public string TrunkName
        {
            get { return trunkName; }
            set { Settings.Default.TrunkName = trunkName = value; OnPropertyChanged("TrunkName"); }
        }

        private string brancheName;
        public string BrancheName
        {
            get { return brancheName; }
            set { Settings.Default.BrancheName = brancheName = value; OnPropertyChanged("BrancheName"); }
        }

        private bool isNotLoading;
        public bool IsNotLoading
        {
            get { return !isNotLoading; }
            set { isNotLoading = !value; OnPropertyChanged("IsNotLoading"); }
        }

        private string consoleOutput;
        public string ConsoleOutput
        {
            get { return consoleOutput; }
            set { consoleOutput = value; OnPropertyChanged("ConsoleOutput"); }
        }

        /// <summary>
        /// Update reposiroties
        /// </summary>
        private void UpdateRepositories()
        {
            if (!string.IsNullOrWhiteSpace(RepositoryUrl) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
                IsNotLoading = false;

                Task.Factory.StartNew(() =>
                {
                    IsNotLoading = true;
                });
            }
        }

        /// <summary>
        /// Generates code metrics
        /// </summary>
        private void ProceedCodeMetrics()
        {
            if (!string.IsNullOrWhiteSpace(RepositoryUrl) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
            }
        }

        /// <summary>
        /// Generates code coverage
        /// </summary>
        private void ProceedCodeCoverage()
        {
            if (!string.IsNullOrWhiteSpace(RepositoryUrl) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
            }
        }
    }
}