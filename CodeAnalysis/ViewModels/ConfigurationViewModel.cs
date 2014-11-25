namespace CodeAnalysis.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using CodeAnalysis.Core;
    using CodeAnalysis.Properties;
    using CodeAnalysis.Views;

    /// <summary>
    /// ViewModel for ConfigurationView
    /// </summary>
    public class ConfigurationViewModel : BaseViewModel
    {
        public ConfigurationViewModel()
        {
            ProceedAnalysisCommand = new RelayCommand(param => ProceedAnalysis());

            RepositoryUrl = Settings.Default.RepositoryUrl;
            TrunkName = Settings.Default.TrunkName;
            BrancheName = Settings.Default.BrancheName;
        }

        public RelayCommand ProceedAnalysisCommand { get; set; }

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

        /// <summary>
        /// Procced the analysis on the specified branche and trunk
        /// </summary>
        private void ProceedAnalysis()
        {
            if (!string.IsNullOrWhiteSpace(RepositoryUrl) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
                IsNotLoading = false;

                Task.Factory.StartNew(() =>
                {
                    string rootPath = AppDomain.CurrentDomain.BaseDirectory + "data";
                    string trunkPath = rootPath + "\\" + TrunkName;
                    string branchePath = rootPath + "\\" + BrancheName;
                    string analysisPath = rootPath + "\\analysis";

                    Directory.CreateDirectory(rootPath);
                    Directory.CreateDirectory(trunkPath);
                    Directory.CreateDirectory(branchePath);
                    Directory.CreateDirectory(analysisPath);

                    string trunkCommands = GetCommands(TrunkName, trunkPath, analysisPath);
                    string brancheCommands = GetCommands(BrancheName, branchePath, analysisPath);

                    var cmdProcessForTrunk = new Process
                    {
                        StartInfo =
                        {
                            FileName = "cmd.exe",
                            UseShellExecute = false,
                            Arguments = trunkCommands
                        }
                    };

                    var cmdProcessForBranche = new Process
                    {
                        StartInfo =
                        {
                            FileName = "cmd.exe",
                            UseShellExecute = false,
                            Arguments = brancheCommands
                        }
                    };

                    cmdProcessForTrunk.Start();
                    cmdProcessForBranche.Start();

                    cmdProcessForTrunk.WaitForExit();
                    cmdProcessForBranche.WaitForExit();

                    IsNotLoading = true;

                    const string TemplateMetricsResults = @"{0}\MetricsResults-{1}.xml";
                    CodeMetricsView.ViewModel.CodeMetricsTrunkFilePath = string.Format(TemplateMetricsResults, analysisPath, TrunkName);
                    CodeMetricsView.ViewModel.CodeMetricsBrancheFilePath = string.Format(TemplateMetricsResults, analysisPath, BrancheName);
                    CodeMetricsView.ViewModel.ProceedCodeMetrics();
                });
            }
        }

        /// <summary>
        /// Get the command for a specified name and path
        /// </summary>
        private string GetCommands(string name, string solutionPath, string analysisPath)
        {
            const string CommandInitializer = "/K ";
            const string CommandSeparator = " & ";
            const string PauseAndExit = " & pause & exit";

            // Template git commands
            const string TemplateCommandCd = "cd {0}";
            const string TemplateCommandGitInit = "git init";
            const string TemplateCommandGitRemote = " git remote add -t {0} -f origin {1}";
            const string TemplateCommandGitCheckout = "git checkout {0}";

            // Template build commands
            const string TemplateCommandNuggetRestore = @"{0}\.nuget\NuGet.exe restore {0}\iTS.sln";
            const string TemplateCommandBuild = @"""C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.Exe"" {0}\iTS.sln /v:q";

            // Template metrics command
            const string MetricsGeneratorPath = @"""C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Static Analysis Tools\FxCop\metrics.exe""";
            const string DotNetOpenAuthPath = @"../../Library/DotNetOpenAuth.AspNet.dll";
            const string TemplateCommandMetrics = @"{0} /f:iTS.Web.SPA\bin\iTS.*.dll /o:{1}\MetricsResults-{2}.xml /igc /gac /ref:{3}";

            return CommandInitializer
                   + string.Format(TemplateCommandCd, solutionPath) + CommandSeparator
                   + TemplateCommandGitInit + CommandSeparator
                   + string.Format(TemplateCommandGitRemote, name, RepositoryUrl) + CommandSeparator
                   + string.Format(TemplateCommandGitCheckout, name) + CommandSeparator
                   + string.Format(TemplateCommandNuggetRestore, solutionPath) + CommandSeparator
                   + string.Format(TemplateCommandBuild, solutionPath) + CommandSeparator
                   + string.Format(TemplateCommandMetrics, MetricsGeneratorPath, analysisPath, name, DotNetOpenAuthPath)
                   + PauseAndExit;
        }
    }
}