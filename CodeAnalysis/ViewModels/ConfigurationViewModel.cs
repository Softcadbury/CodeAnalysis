﻿namespace CodeAnalysis.ViewModels
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

            Process cmdProcess = new Process
                                 {
                                     StartInfo =
                                     {
                                         FileName = "cmd.exe",
                                         UseShellExecute = false,
                                         CreateNoWindow = true,
                                         RedirectStandardOutput = true,
                                         RedirectStandardInput = true
                                     }
                                 };

            cmdProcess.OutputDataReceived += (sendingProcess, outLine) =>
            {
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    ConsoleOutput += (Environment.NewLine + outLine.Data);

                    if (outLine.Data == "Update completed")
                    {
                        IsNotLoading = true;
                    }
                }
            };

            cmdProcess.Start();
            cmdStreamWriter = cmdProcess.StandardInput;
            cmdProcess.BeginOutputReadLine();
        }

        private void haha(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                ConsoleOutput += (Environment.NewLine + outLine.Data);
            }
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
        /// Update reposiroties
        /// </summary>
        private void UpdateRepositories()
        {
            if (!string.IsNullOrWhiteSpace(RepositoryUrl) && !string.IsNullOrWhiteSpace(TrunkName) && !string.IsNullOrWhiteSpace(BrancheName))
            {
                IsNotLoading = false;

                Task.Factory.StartNew(() =>
                {
                    string commandCd = "cd {0}" + Environment.NewLine;
                    string commandGitInit = "git init" + Environment.NewLine;
                    string commandGitRemote = " git remote add -t {0} -f origin {1}" + Environment.NewLine;
                    string commandGitCheckout = "git checkout {0}" + Environment.NewLine;
                    string commandEchoCompleted = "echo Update completed" + Environment.NewLine;

                    string rootPath = AppDomain.CurrentDomain.BaseDirectory + "data";
                    string trunkPath = rootPath + "\\" + TrunkName;
                    string branchePath = rootPath + "\\" + BrancheName;
                    string analysisPath = rootPath + "\\analysis";

                    Directory.CreateDirectory(rootPath);
                    Directory.CreateDirectory(trunkPath);
                    Directory.CreateDirectory(branchePath);
                    Directory.CreateDirectory(analysisPath);

                    string commandTrunk = string.Format(commandCd, trunkPath)
                                          + commandGitInit
                                          + string.Format(commandGitRemote, TrunkName, RepositoryUrl)
                                          + string.Format(commandGitCheckout, TrunkName);

                    string commandBranche = string.Format(commandCd, branchePath)
                                             + commandGitInit
                                             + string.Format(commandGitRemote, BrancheName, RepositoryUrl)
                                             + string.Format(commandGitCheckout, BrancheName);

                    cmdStreamWriter.Write(commandTrunk + commandBranche + commandEchoCompleted);
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