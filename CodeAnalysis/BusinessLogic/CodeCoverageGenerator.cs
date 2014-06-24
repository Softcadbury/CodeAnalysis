namespace CodeAnalysis.BusinessLogic
{
    using CodeAnalysis.Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;

    /// <summary>
    /// This class compares two code coverage files
    /// </summary>
    public static class CodeCoverageGenerator
    {
        /// <summary>
        /// Generates a list of CodeCoverageLineView with two code coverage files
        /// </summary>
        public static IEnumerable<CodeCoverageLineView> Generate(StreamReader codeCoverageTrunkFile, StreamReader codeCoverageBrancheFile)
        {
            List<CodeCoverageLineModel> codeCoverageTrunk = InitializeCodeCoverage(codeCoverageTrunkFile);
            codeCoverageTrunkFile.Close();

            List<CodeCoverageLineModel> codeCoverageBranche = InitializeCodeCoverage(codeCoverageBrancheFile);
            codeCoverageBrancheFile.Close();

            return InitializeCodeCoverageDifferences(codeCoverageTrunk, codeCoverageBranche);
        }

        /// <summary>
        /// Initializes a list of CodeCoverageLineModel with a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> InitializeCodeCoverage(StreamReader file)
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
                        Children = GetTypes(namespacee, moduleName, namespaceName)
                    };

                    GetStatistics(namespacee, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the types from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetTypes(XElement namespacee, string moduleName, string namespaceName)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var type in namespacee.Descendants("Class"))
            {
                var typeNameElement = type.Element("ClassName");
                if (typeNameElement != null)
                {
                    string className = typeNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Type = className,
                        Children = GetMethods(type, moduleName, namespaceName, className)
                    };

                    GetStatistics(type, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the methods from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetMethods(XElement type, string moduleName, string namespaceName, string className)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var method in type.Descendants("Method"))
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
        /// Gets the statistics from a coverage code file
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

            line.CoveredLinesPercentage = (int)((float)linesCovered / (float)linesTotal * 100);
            line.CoveredLines = linesCovered;
            line.CoveredBlocksPercentage = (int)((float)blocksCovered / (float)blocksTotal * 100);
            line.CoveredBlocks = blocksCovered;
        }

        /// <summary>
        /// Creates a list of CodeCoverageLineView containing differences between two lists of CodeCoverageLineModel
        /// </summary>
        private static List<CodeCoverageLineView> InitializeCodeCoverageDifferences(List<CodeCoverageLineModel> codeCoverageTrunk, List<CodeCoverageLineModel> codeCoverageBranche)
        {
            var codeCoverage = new List<CodeCoverageLineView>();

            foreach (CodeCoverageLineModel line in codeCoverageBranche)
            {
                CodeCoverageLineView codeCoverageLineView = CreateCodeCoverageViewFromBranche(line);

                CodeCoverageLineModel sameLine = GetSameLine(line, codeCoverageTrunk);
                AddDifferences(codeCoverageLineView, line, sameLine);

                codeCoverageLineView.Children = InitializeCodeCoverageDifferences(sameLine != null ? sameLine.Children : null, line.Children);
                codeCoverage.Add(codeCoverageLineView);
            }

            codeCoverage.AddRange(AddCodeCoverageViewFromTrunk(codeCoverageTrunk, codeCoverageBranche));

            return codeCoverage;
        }

        /// <summary>
        /// Adds lines from trunk but not in branche
        /// </summary>
        private static List<CodeCoverageLineView> AddCodeCoverageViewFromTrunk(List<CodeCoverageLineModel> codeCoverageTrunk, List<CodeCoverageLineModel> codeCoverageBranche)
        {
            var codeCoverageToAdd = new List<CodeCoverageLineView>();

            if (codeCoverageTrunk != null)
            {
                foreach (CodeCoverageLineModel line in codeCoverageTrunk)
                {
                    CodeCoverageLineModel sameLine = GetSameLine(line, codeCoverageBranche);

                    if (sameLine == null)
                    {
                        codeCoverageToAdd.Add(CreateCodeCoverageViewFromTrunk(line));
                    }
                }
            }

            return codeCoverageToAdd;
        }

        /// <summary>
        /// Creates code coverage view from branche
        /// </summary>
        private static CodeCoverageLineView CreateCodeCoverageViewFromBranche(CodeCoverageLineModel line)
        {
            return new CodeCoverageLineView
            {
                Project = line.Project,
                Namespace = line.Namespace,
                Type = line.Type,
                Member = line.Member,

                CoveredLinesPercentageBranche = line.CoveredLinesPercentage,
                CoveredLinesBranche = line.CoveredLines,
                CoveredBlocksPercentageBranche = line.CoveredBlocksPercentage,
                CoveredBlocksBranche = line.CoveredBlocks
            };
        }

        /// <summary>
        /// Creates code coverage view from trunk
        /// </summary>
        private static CodeCoverageLineView CreateCodeCoverageViewFromTrunk(CodeCoverageLineModel line)
        {
            return new CodeCoverageLineView
            {
                Project = line.Project,
                Namespace = line.Namespace,
                Type = line.Type,
                Member = line.Member,

                CoveredLinesPercentageTrunk = line.CoveredLinesPercentage,
                CoveredLinesTrunk = line.CoveredLines,
                CoveredBlocksPercentageTrunk = line.CoveredBlocksPercentage,
                CoveredBlocksTrunk = line.CoveredBlocks
            };
        }

        /// <summary>
        /// Gets a line from a list with same informations
        /// </summary>
        private static CodeCoverageLineModel GetSameLine(CodeCoverageLineModel codeCoverageToFind, List<CodeCoverageLineModel> codeCoverageToExplore)
        {
            if (codeCoverageToExplore != null)
            {
                foreach (CodeCoverageLineModel line in codeCoverageToExplore)
                {
                    if (line.Project == codeCoverageToFind.Project
                        && line.Namespace == codeCoverageToFind.Namespace
                        && line.Type == codeCoverageToFind.Type
                        && line.Member == codeCoverageToFind.Member)
                    {
                        return line;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Adds differences between two lines
        /// </summary>
        private static void AddDifferences(CodeCoverageLineView codeCoverageLineView, CodeCoverageLineModel currentLine, CodeCoverageLineModel sameLine)
        {
            if (sameLine != null)
            {
                codeCoverageLineView.CoveredLinesPercentageTrunk = sameLine.CoveredLinesPercentage;
                codeCoverageLineView.CoveredLinesTrunk = sameLine.CoveredLines;
                codeCoverageLineView.CoveredBlocksPercentageTrunk = sameLine.CoveredBlocksPercentage;
                codeCoverageLineView.CoveredBlocksTrunk = sameLine.CoveredBlocks;

                codeCoverageLineView.CoveredLinesPercentageDifference = currentLine.CoveredLinesPercentage - sameLine.CoveredLinesPercentage;
                codeCoverageLineView.CoveredBlocksPercentageDifference = currentLine.CoveredBlocksPercentage - sameLine.CoveredBlocksPercentage;
            }
        }
    }
}