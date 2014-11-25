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

                    string trunkCommands = GetCommands(TrunkName, trunkPath);
                    string brancheCommands = GetCommands(BrancheName, branchePath);

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
                });
            }
        }

        /// <summary>
        /// Get the command for a specified name and path
        /// </summary>
        private string GetCommands(string name, string path)
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
            const string TemplateCommandNuggetRestore = @"{0}\.nuget\NuGet.exe restore {1}\iTS.sln";
            const string TemplateCommandBuild = @"""C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.Exe"" {0}\iTS.sln /v:q";

            return CommandInitializer
                   + string.Format(TemplateCommandCd, path) + CommandSeparator
                   + TemplateCommandGitInit + CommandSeparator
                   + string.Format(TemplateCommandGitRemote, name, RepositoryUrl) + CommandSeparator
                   + string.Format(TemplateCommandGitCheckout, name) + CommandSeparator
                   + string.Format(TemplateCommandNuggetRestore, path, name) + CommandSeparator
                   + string.Format(TemplateCommandBuild, path)
                   + PauseAndExit;
        }
    }
}