namespace CodeAnalysis.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using CodeAnalysis.Models;

    /// <summary>
    /// This class compares two code metrics xml files
    /// </summary>
    public static class CodeMetricsGeneratorFromXml
    {
        /// <summary>
        /// Generates a list of CodeMetricsLineView with two code metrics files
        /// </summary>
        public static IEnumerable<CodeMetricsLineView> Generate(StreamReader codeMetricsTrunkFile, StreamReader codeMetricsBrancheFile)
        {
            List<CodeMetricsLineModelFromXml> codeMetricsTrunk = InitializeCodeMetrics(codeMetricsTrunkFile);
            codeMetricsTrunkFile.Close();

            List<CodeMetricsLineModelFromXml> codeMetricsBranche = InitializeCodeMetrics(codeMetricsBrancheFile);
            codeMetricsBrancheFile.Close();

            return InitializeCodeMetricsDifferences(codeMetricsTrunk, codeMetricsBranche);
        }

        /// <summary>
        /// Initializes a list of CodeMetricsLineModel with a Metrics code file
        /// </summary>
        private static List<CodeMetricsLineModelFromXml> InitializeCodeMetrics(StreamReader file)
        {
            var document = XDocument.Parse(file.ReadToEnd());
            return GetModules(document);
        }

        /// <summary>
        /// Gets the modules from a Metrics code file
        /// </summary>
        private static List<CodeMetricsLineModelFromXml> GetModules(XDocument document)
        {
            var codeMetrics = new List<CodeMetricsLineModelFromXml>();

            foreach (var module in document.Descendants("Module"))
            {
                var moduleNameElement = module.Attribute("Name");
                if (moduleNameElement != null)
                {
                    string moduleName = moduleNameElement.Value;

                    var line = new CodeMetricsLineModelFromXml
                    {
                        Project = moduleName,
                        Children = GetNamespaces(module, moduleName)
                    };

                    GetStatistics(module, line);
                    codeMetrics.Add(line);
                }
            }

            return codeMetrics;
        }

        /// <summary>
        /// Gets the namespaces from a Metrics code file
        /// </summary>
        private static List<CodeMetricsLineModelFromXml> GetNamespaces(XElement module, string moduleName)
        {
            var codeMetrics = new List<CodeMetricsLineModelFromXml>();

            foreach (var namespacee in module.Descendants("Namespace"))
            {
                var namespaceNameElement = namespacee.Attribute("Name");
                if (namespaceNameElement != null)
                {
                    string namespaceName = namespaceNameElement.Value;

                    var line = new CodeMetricsLineModelFromXml
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Children = GetTypes(namespacee, moduleName, namespaceName)
                    };

                    GetStatistics(namespacee, line);
                    codeMetrics.Add(line);
                }
            }

            return codeMetrics;
        }

        /// <summary>
        /// Gets the types from a Metrics code file
        /// </summary>
        private static List<CodeMetricsLineModelFromXml> GetTypes(XElement namespacee, string moduleName, string namespaceName)
        {
            var codeMetrics = new List<CodeMetricsLineModelFromXml>();

            foreach (var type in namespacee.Descendants("Type"))
            {
                var typeNameElement = type.Attribute("Name");
                if (typeNameElement != null)
                {
                    string className = typeNameElement.Value;

                    var line = new CodeMetricsLineModelFromXml
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Type = className,
                        Children = GetMethods(type, moduleName, namespaceName, className)
                    };

                    GetStatistics(type, line);
                    codeMetrics.Add(line);
                }
            }

            return codeMetrics;
        }

        /// <summary>
        /// Gets the methods from a Metrics code file
        /// </summary>
        private static List<CodeMetricsLineModelFromXml> GetMethods(XElement type, string moduleName, string namespaceName, string className)
        {
            var codeMetrics = new List<CodeMetricsLineModelFromXml>();

            foreach (var method in type.Descendants("Member"))
            {
                var methodNameElement = method.Attribute("Name");
                if (methodNameElement != null)
                {
                    string methodName = methodNameElement.Value;

                    var line = new CodeMetricsLineModelFromXml
                    {
                        Project = moduleName,
                        Namespace = namespaceName,
                        Type = className,
                        Member = methodName
                    };

                    GetStatistics(method, line);
                    codeMetrics.Add(line);
                }
            }

            return codeMetrics;
        }

        /// <summary>
        /// Gets the statistics from a Metrics code file
        /// </summary>
        private static void GetStatistics(XElement element, CodeMetricsLineModelFromXml line)
        {
            var metrics = element.Descendants("Metrics").First();

            foreach (var metric in metrics.Descendants("Metric"))
            {
                switch (metric.Attribute("Name").Value)
                {
                    case "MaintainabilityIndex":
                        line.MaintainabilityIndex = Convert.ToDouble(metric.Attribute("Value").Value);
                        break;

                    case "CyclomaticComplexity":
                        line.CyclomaticComplexity = Convert.ToDouble(metric.Attribute("Value").Value);
                        break;

                    case "ClassCoupling":
                        line.ClassCoupling = Convert.ToDouble(metric.Attribute("Value").Value);
                        break;

                    case "DepthOfInheritance":
                        line.DepthOfInheritance = Convert.ToDouble(metric.Attribute("Value").Value);
                        break;

                    case "LinesOfCode":
                        line.LinesOfCode = Convert.ToDouble(metric.Attribute("Value").Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a list of CodeMetricsLineView containing differences between two lists of CodeMetricsLineModel
        /// </summary>
        private static List<CodeMetricsLineView> InitializeCodeMetricsDifferences(List<CodeMetricsLineModelFromXml> codeMetricsTrunk, List<CodeMetricsLineModelFromXml> codeMetricsBranche)
        {
            var codeMetrics = new List<CodeMetricsLineView>();

            foreach (CodeMetricsLineModelFromXml line in codeMetricsBranche)
            {
                CodeMetricsLineView codeMetricsLineView = CreateCodeMetricsViewFromBranche(line);

                CodeMetricsLineModelFromXml sameLine = GetSameLine(line, codeMetricsTrunk);
                AddDifferences(codeMetricsLineView, line, sameLine);

                codeMetricsLineView.Children = InitializeCodeMetricsDifferences(sameLine != null ? sameLine.Children : null, line.Children);
                codeMetrics.Add(codeMetricsLineView);
            }

            codeMetrics.AddRange(AddCodeMetricsViewFromTrunk(codeMetricsTrunk, codeMetricsBranche));

            return codeMetrics;
        }

        /// <summary>
        /// Adds lines from trunk but not in branche
        /// </summary>
        private static List<CodeMetricsLineView> AddCodeMetricsViewFromTrunk(List<CodeMetricsLineModelFromXml> codeMetricsTrunk, List<CodeMetricsLineModelFromXml> codeMetricsBranche)
        {
            var codeMetricsToAdd = new List<CodeMetricsLineView>();

            if (codeMetricsTrunk != null)
            {
                foreach (CodeMetricsLineModelFromXml line in codeMetricsTrunk)
                {
                    CodeMetricsLineModelFromXml sameLine = GetSameLine(line, codeMetricsBranche);

                    if (sameLine == null)
                    {
                        codeMetricsToAdd.Add(CreateCodeMetricsViewFromTrunk(line));
                    }
                }
            }

            return codeMetricsToAdd;
        }

        /// <summary>
        /// Creates code Metrics view from branche
        /// </summary>
        private static CodeMetricsLineView CreateCodeMetricsViewFromBranche(CodeMetricsLineModelFromXml line)
        {
            return new CodeMetricsLineView
            {
                Project = line.Project,
                Namespace = line.Namespace,
                Type = line.Type,
                Member = line.Member,

                MaintainabilityIndexBranche = line.MaintainabilityIndex,
                CyclomaticComplexityBranche = line.CyclomaticComplexity,
                ClassCouplingBranche = line.ClassCoupling,
                DepthOfInheritanceBranche = line.DepthOfInheritance,
                LinesOfCodeBranche = line.LinesOfCode
            };
        }

        /// <summary>
        /// Creates code Metrics view from trunk
        /// </summary>
        private static CodeMetricsLineView CreateCodeMetricsViewFromTrunk(CodeMetricsLineModelFromXml line)
        {
            return new CodeMetricsLineView
            {
                Project = line.Project,
                Namespace = line.Namespace,
                Type = line.Type,
                Member = line.Member,

                MaintainabilityIndexTrunk = line.MaintainabilityIndex,
                CyclomaticComplexityTrunk = line.CyclomaticComplexity,
                ClassCouplingTrunk = line.ClassCoupling,
                DepthOfInheritanceTrunk = line.DepthOfInheritance,
                LinesOfCodeTrunk = line.LinesOfCode
            };
        }

        /// <summary>
        /// Gets a line from a list with same informations
        /// </summary>
        private static CodeMetricsLineModelFromXml GetSameLine(CodeMetricsLineModelFromXml codeMetricsToFind, List<CodeMetricsLineModelFromXml> codeMetricsToExplore)
        {
            if (codeMetricsToExplore != null)
            {
                foreach (CodeMetricsLineModelFromXml line in codeMetricsToExplore)
                {
                    if (line.Project == codeMetricsToFind.Project
                        && line.Namespace == codeMetricsToFind.Namespace
                        && line.Type == codeMetricsToFind.Type
                        && line.Member == codeMetricsToFind.Member)
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
        private static void AddDifferences(CodeMetricsLineView codeMetricsLineView, CodeMetricsLineModelFromXml currentLine, CodeMetricsLineModelFromXml sameLine)
        {
            if (sameLine != null)
            {
                codeMetricsLineView.MaintainabilityIndexTrunk = sameLine.MaintainabilityIndex;
                codeMetricsLineView.CyclomaticComplexityTrunk = sameLine.CyclomaticComplexity;
                codeMetricsLineView.ClassCouplingTrunk = sameLine.ClassCoupling;
                codeMetricsLineView.DepthOfInheritanceTrunk = sameLine.DepthOfInheritance;
                codeMetricsLineView.LinesOfCodeTrunk = sameLine.LinesOfCode;

                codeMetricsLineView.MaintainabilityIndexDifference = currentLine.MaintainabilityIndex - sameLine.MaintainabilityIndex;
                codeMetricsLineView.CyclomaticComplexityDifference = currentLine.CyclomaticComplexity - sameLine.CyclomaticComplexity;
                codeMetricsLineView.ClassCouplingDifference = currentLine.ClassCoupling - sameLine.ClassCoupling;
                codeMetricsLineView.DepthOfInheritanceDifference = currentLine.DepthOfInheritance - sameLine.DepthOfInheritance;
                codeMetricsLineView.LinesOfCodeDifference = currentLine.LinesOfCode - sameLine.LinesOfCode;
            }
        }
    }
}