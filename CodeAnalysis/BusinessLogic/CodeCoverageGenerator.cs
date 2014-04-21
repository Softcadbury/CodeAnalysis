namespace CodeAnalysis.BusinessLogic
{
    using CodeAnalysis.Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;

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
        /// Initializes the list of CodeCoverageLineModel with a coverage code file
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

                    InitLine(module, line);
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

            foreach (var namespaceee in module.Descendants("NamespaceTable"))
            {
                var namespaceeeNameElement = namespaceee.Element("NamespaceName");
                if (namespaceeeNameElement != null)
                {
                    string namespaceeeName = namespaceeeNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        Children = GetClasses(namespaceee, moduleName, namespaceeeName)
                    };

                    InitLine(namespaceee, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the classes from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetClasses(XElement namespaceee, string moduleName, string namespaceeeName)
        {
            var codeCoverage = new List<CodeCoverageLineModel>();

            foreach (var classs in namespaceee.Descendants("Class"))
            {
                var classsNameElement = classs.Element("ClassName");
                if (classsNameElement != null)
                {
                    string classsName = classsNameElement.Value;

                    var line = new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        Type = classsName,
                        Children = GetMethods(classs, moduleName, namespaceeeName, classsName)
                    };

                    InitLine(classs, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Gets the methods from a coverage code file
        /// </summary>
        private static List<CodeCoverageLineModel> GetMethods(XElement classs, string moduleName, string namespaceeeName, string classsName)
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
                        Namespace = namespaceeeName,
                        Type = classsName,
                        Member = methodName
                    };

                    InitLine(method, line);
                    codeCoverage.Add(line);
                }
            }

            return codeCoverage;
        }

        /// <summary>
        /// Initializes informations with a coverage code file
        /// </summary>
        private static void InitLine(XElement element, CodeCoverageLineModel line)
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
        /// Creates a list of CodeCoverageLineView containing difference of code coverage between two lists of CodeCoverageLineModel
        /// </summary>
        private static List<CodeCoverageLineView> InitializeCodeCoverageDifferences(List<CodeCoverageLineModel> codeCoverageTrunk, List<CodeCoverageLineModel> codeCoverageBranche)
        {
            var codeCoverageLineViews = new List<CodeCoverageLineView>();

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
                    CoveredBlocksPercentageBranche = line.CoveredBlocksPercentage
                };

                codeCoverageLineView.Children = InitializeCodeCoverageDifferences(null, line.Children);
                codeCoverageLineViews.Add(codeCoverageLineView);
            }

            return codeCoverageLineViews;
        }

        /// <summary>
        ///
        /// </summary>
        private static CodeCoverageLineModel GetSameLine(CodeCoverageLineModel codeCoverageTrunk, CodeCoverageLineModel codeCoverageBranche)
        {
            if (codeCoverageBranche != null)
            {
                foreach (var item in codeCoverageBranche.Children)
                {
                    if (item.Project == codeCoverageTrunk.Project
                        && item.Namespace == codeCoverageTrunk.Namespace
                        && item.Type == codeCoverageTrunk.Type
                        && item.Member == codeCoverageTrunk.Member)
                    {
                        return item;
                    }
                }
            }

            return null;
        }
    }
}