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
                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
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
                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
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
                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        Type = classsName,
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
                    codeCoverage.Add(new CodeCoverageLineModel
                    {
                        Project = moduleName,
                        Namespace = namespaceeeName,
                        Type = classsName,
                        Member = methodName
                    });
                }
            }

            return codeCoverage;
        }
    }
}