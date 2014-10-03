namespace CodeAnalysis.ViewModels
{
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

            Repository = Settings.Default.RepositoryURL;
            TrunkName = Settings.Default.TrunkName;
            BrancheName = Settings.Default.BrancheName;
        }

        public RelayCommand UpdateRepositoriesCommand { get; set; }
        public RelayCommand ProceedCodeMetricsCommand { get; set; }
        public RelayCommand ProceedCodeCoverageCommand { get; set; }

        private string repository;

        public string Repository
        {
            get { return repository; }
            set { Settings.Default.RepositoryURL = repository = value; OnPropertyChanged("Repository"); }
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

        /// <summary>
        /// Update reposiroties
        /// </summary>
        private void UpdateRepositories()
        {
            if (!string.IsNullOrWhiteSpace(Repository) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
            }
        }

        /// <summary>
        /// Generates code metrics
        /// </summary>
        private void ProceedCodeMetrics()
        {
            if (!string.IsNullOrWhiteSpace(Repository) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
            }
        }

        /// <summary>
        /// Generates code coverage
        /// </summary>
        private void ProceedCodeCoverage()
        {
            if (!string.IsNullOrWhiteSpace(Repository) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
            }
        }
    }
}