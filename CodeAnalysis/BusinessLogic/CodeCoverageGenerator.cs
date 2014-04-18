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
        public static IEnumerable<CodeCoverageLineView> Generate(StreamReader codeCoverageTrunkFile, StreamReader codeCoverageBrancheFile)
        {
            List<CodeCoverageLineModel> codeCoverageTrunk = InitCodeCoverage(codeCoverageTrunkFile);
            codeCoverageTrunkFile.Close();

            List<CodeCoverageLineModel> codeCoverageBranche = InitCodeCoverage(codeCoverageBrancheFile);
            codeCoverageBrancheFile.Close();

            return new List<CodeCoverageLineView>();
        }

        /// <summary>
        /// Creates a list of CodeCoverageLineModel with information from the file
        /// </summary>
        private static List<CodeCoverageLineModel> InitCodeCoverage(StreamReader file)
        {
            var document = XDocument.Parse(file.ReadLine());
            return GetModules(document);
        }

        /// <summary>
        ///
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
                    int lineCovered = (int)module.Element("LinesCovered");
                    int lineNotCovered = (int)module.Element("LinesNotCovered");

                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        CoveredLine = lineCovered,
                        CoveredLinePercentage = lineCovered / lineNotCovered * 100,
                        Children = GetNamespaces(module, moduleName)
                    });
                }
            }

            return codeCoverage;
        }

        /// <summary>
        ///
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
                    int lineCovered = (int)namespaceee.Element("LinesCovered");
                    int lineNotCovered = (int)namespaceee.Element("LinesNotCovered");

                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        CoveredLine = lineCovered,
                        CoveredLinePercentage = lineCovered / lineNotCovered * 100,
                        Children = GetClasses(namespaceee, moduleName, namespaceeeName)
                    });
                }
            }

            return codeCoverage;
        }

        /// <summary>
        ///
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
                    int lineCovered = (int)classs.Element("LinesCovered");
                    int lineNotCovered = (int)classs.Element("LinesNotCovered");

                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        Type = classsName,
                        CoveredLine = lineCovered,
                        CoveredLinePercentage = lineCovered / lineNotCovered * 100,
                        Children = GetMethods(classs, moduleName, namespaceeeName, classsName)
                    });
                }
            }

            return codeCoverage;
        }

        /// <summary>
        ///
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
                    int lineCovered = (int)method.Element("LinesCovered");
                    int lineNotCovered = (int)method.Element("LinesNotCovered");

                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        Type = classsName,
                        Member = methodName,
                        CoveredLine = lineCovered,
                        CoveredLinePercentage = lineCovered / lineNotCovered * 100
                    });
                }
            }

            return codeCoverage;
        }
    }
}