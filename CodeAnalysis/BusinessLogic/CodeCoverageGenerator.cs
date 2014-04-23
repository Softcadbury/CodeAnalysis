namespace CodeAnalysis.BusinessLogic
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using CodeAnalysis.Models;

    /// <summary>
    /// This class compares two code coverage files
    /// </summary>
    public class CodeCoverageGenerator
    {
        /// <summary>
        /// Generates a list of CodeCoverageLineView with two code coverage files
        /// </summary>
        public static IEnumerable<CodeCoverageLineView> Generate(StreamReader codeCoverageTrunkFile, StreamReader codeCoverageBrancheFile)
        {
            List<CodeCoverageLineModel> codeCoverageTrunk = InitCodeCoverage(codeCoverageTrunkFile);
            codeCoverageTrunkFile.Close();

            List<CodeCoverageLineModel> codeCoverageBranche = InitCodeCoverage(codeCoverageBrancheFile);
            codeCoverageBrancheFile.Close();

            return InitializeCodeCoverageDifferences(codeCoverageTrunk, codeCoverageBranche);
        }

        /// <summary>
        /// Initializes a list of CodeCoverageLineModel with a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> InitCodeCoverage(StreamReader file)
        {
            var document = XDocument.Parse(file.ReadLine());
            return GetModules(document);
        }

        /// <summary>
        /// Gets the modules from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetModules(XDocument document)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var module in document.Descendants("Module"))
            {
                var moduleNameElement = module.Element("ModuleName");
                if (moduleNameElement != null)
                {
                    string moduleName = moduleNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Children = GetNamespaces(module, moduleName)
                    };

                    GetStatistics(module, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the namespaces from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetNamespaces(XElement module, string moduleName)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var namespacee in module.Descendants("NamespaceTable"))
            {
                var namespaceNameElement = namespacee.Element("NamespaceName");
                if (namespaceNameElement != null)
                {
                    string namespaceName = namespaceNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Children = GetClasses(namespacee, moduleName, namespaceName)
                    };

                    GetStatistics(namespacee, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the classes from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetClasses(XElement namespacee, string moduleName, string namespaceName)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var classs in namespacee.Descendants("Class"))
            {
                var classNameElement = classs.Element("ClassName");
                if (classNameElement != null)
                {
                    string className = classNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Type = className,
                        Children = GetMethods(classs, moduleName, namespaceName, className)
                    };

                    GetStatistics(classs, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the methods from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetMethods(XElement classs, string moduleName, string namespaceName, string className)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var method in classs.Descendants("Method"))
            {
                var methodNameElement = method.Element("MethodName");
                if (methodNameElement != null)
                {
                    string methodName = methodNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Type = className,
                        Member = methodName
                    };

                    GetStatistics(method, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Get the statistics from a coverage code file
        /// </summary>
        private static void GetStatistics(XElement element, CodeCoverageLineModel line)
        {
            int linesCovered = (int)element.Element("LinesCovered");
            int linesNotCovered = (int)element.Element("LinesNotCovered");

            int linesTotal = linesCovered + linesNotCovered;
            linesTotal = linesTotal != 0 ? linesTotal : 1;

            int blocksCovered = (int)element.Element("BlocksCovered");
            int blocksNotCovered = (int)element.Element("BlocksNotCovered");

            int blocksTotal = blocksCovered + blocksNotCovered;
            blocksTotal = blocksTotal != 0 ? blocksTotal : 1;

            line.CoveredLines = linesCovered;
            line.CoveredLinesPercentage = (int)((float)linesCovered / (float)linesTotal * 100);
            line.CoveredBlocks = blocksCovered;
            line.CoveredBlocksPercentage = (int)((float)blocksCovered / (float)blocksTotal * 100);
        }

        /// <summary>
        /// Creates a list of CodeCoverageLineView containing differences between two lists of CodeCoverageLineModel
        /// </summary>
        private static List<CodeCoverageLineView> InitializeCodeCoverageDifferences(List<CodeCoverageLineModel> codeCoverageTrunk, List<CodeCoverageLineModel> codeCoverageBranche)
        {
            var codeCoverage = new List<CodeCoverageLineView>();

            foreach (var line in codeCoverageBranche)
            {
                var codeCoverageLineView = new CodeCoverageLineView
                {
                    Project = line.Project,
                    Namespace = line.Namespace,
                    Type = line.Type,
                    Member = line.Member,

                    CoveredLinesBranche = line.CoveredLines,
                    CoveredLinesPercentageBranche = line.CoveredLinesPercentage,
                    CoveredBlocksBranche = line.CoveredBlocks,
                    CoveredBlocksPercentageBranche = line.CoveredBlocksPercentage,
                };

                CodeCoverageLineModel sameLine = GetSameLine(line, codeCoverageTrunk);

                if (sameLine != null)
                {
                    codeCoverageLineView.CoveredLinesTrunk = sameLine.CoveredLines;
                    codeCoverageLineView.CoveredLinesPercentageTrunk = sameLine.CoveredLinesPercentage;
                    codeCoverageLineView.CoveredBlocksTrunk = sameLine.CoveredBlocks;
                    codeCoverageLineView.CoveredBlocksPercentageTrunk = sameLine.CoveredBlocksPercentage;

                    codeCoverageLineView.CoveredLinesDifference = sameLine.CoveredLines - line.CoveredLines;
                    codeCoverageLineView.CoveredLinesPercentageDifference = sameLine.CoveredLinesPercentage - line.CoveredLinesPercentage;
                    codeCoverageLineView.CoveredBlocksDifference = sameLine.CoveredBlocks - line.CoveredBlocks;
                    codeCoverageLineView.CoveredBlocksPercentageDifference = sameLine.CoveredBlocksPercentage - line.CoveredBlocksPercentage;
                }

                codeCoverageLineView.Children = InitializeCodeCoverageDifferences(sameLine != null ? sameLine.Children : null, line.Children);
                codeCoverage.Add(codeCoverageLineView);
            }

            return codeCoverage;
        }

        /// <summary>
        /// Get a line from a list with same information
        /// </summary>
        private static CodeCoverageLineModel GetSameLine(CodeCoverageLineModel codeCoverageToFind, List<CodeCoverageLineModel> codeCoverageToExplore)
        {
            if (codeCoverageToExplore != null)
            {
                foreach (var item in codeCoverageToExplore)
                {
                    if (item.Project == codeCoverageToFind.Project
                        && item.Namespace == codeCoverageToFind.Namespace
                        && item.Type == codeCoverageToFind.Type
                        && item.Member == codeCoverageToFind.Member)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
    }
}