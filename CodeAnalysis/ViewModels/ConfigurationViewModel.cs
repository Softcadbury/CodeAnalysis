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

            var cmdProcess = new Process
                                 {
                                     StartInfo =
                                     {
                                         FileName = "cmd.exe",
                                         UseShellExecute = false,
                                         CreateNoWindow = true,
                                         RedirectStandardOutput = true,
                                         RedirectStandardInput = true,
                                         RedirectStandardError = true
                                     }
                                 };

            cmdProcess.OutputDataReceived += AddConsoleOutput;
            cmdProcess.ErrorDataReceived += AddConsoleOutput;
            cmdProcess.Start();
            cmdProcess.BeginErrorReadLine();
            cmdProcess.BeginOutputReadLine();

            cmdStreamWriter = cmdProcess.StandardInput;
        }

        private readonly StreamWriter cmdStreamWriter;

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
        /// Display commads result in the console
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="outLine">The data received</param>
        private void AddConsoleOutput(object sender, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                ConsoleOutput += (Environment.NewLine + outLine.Data);

                if (outLine.Data == "Update completed")
                {
                    IsNotLoading = true;
                }
            }
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
                    string rootPath = AppDomain.CurrentDomain.BaseDirectory + "data";
                    string trunkPath = rootPath + "\\" + TrunkName;
                    string branchePath = rootPath + "\\" + BrancheName;
                    string analysisPath = rootPath + "\\analysis";

                    Directory.CreateDirectory(rootPath);
                    Directory.CreateDirectory(trunkPath);
                    Directory.CreateDirectory(branchePath);
                    Directory.CreateDirectory(analysisPath);

                    // Git commands
                    string templateCommandCd = "cd {0}" + Environment.NewLine;
                    string templateCommandGitInit = "git init" + Environment.NewLine;
                    string templateCommandGitRemote = " git remote add -t {0} -f origin {1}" + Environment.NewLine;
                    string templateCommandGitCheckout = "git checkout {0}" + Environment.NewLine;

                    string commandGitTrunk = string.Format(templateCommandCd, trunkPath)
                                            + templateCommandGitInit
                                            + string.Format(templateCommandGitRemote, TrunkName, RepositoryUrl)
                                            + string.Format(templateCommandGitCheckout, TrunkName);

                    string commandGitBranche = string.Format(templateCommandCd, branchePath)
                                            + templateCommandGitInit
                                            + string.Format(templateCommandGitRemote, BrancheName, RepositoryUrl)
                                            + string.Format(templateCommandGitCheckout, BrancheName);

                    string templateCommandNuggetRestore = @"{0}\.nuget\NuGet.exe restore {1}\iTS.sln" + Environment.NewLine;
                    string templateCommandBuild = @"""C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.Exe"" {0}\iTS.sln /v:q" + Environment.NewLine;

                    // Build commands
                    string commandBuildTrunk = string.Format(templateCommandNuggetRestore, trunkPath, TrunkName)
                                            + string.Format(templateCommandBuild, trunkPath);

                    string commandBuildBranche = string.Format(templateCommandNuggetRestore, branchePath, BrancheName)
                                            + string.Format(templateCommandBuild, branchePath);

                    // Execute commands
                    string commandEchoCompleted = "echo Update completed" + Environment.NewLine;
                    cmdStreamWriter.Write(commandGitTrunk + commandGitBranche + commandBuildTrunk + commandBuildBranche + commandEchoCompleted);
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