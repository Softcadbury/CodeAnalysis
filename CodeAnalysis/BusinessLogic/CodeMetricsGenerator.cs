namespace CodeAnalysis.BusinessLogic
{
    using CodeAnalysis.Models;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// This class compares two code metrics files
    /// </summary>
    public static class CodeMetricsGenerator
    {
        /// <summary>
        /// Generates a list of CodeMetricsLineView with two code metrics files
        /// </summary>
        public static IEnumerable<CodeMetricsLineView> Generate(StreamReader codeMetricsTrunkFile, StreamReader codeMetricsBrancheFile)
        {
            List<CodeMetricsLineModel> codeMetricsTrunk = InitializeCodeMetrics(codeMetricsTrunkFile);
            codeMetricsTrunkFile.Close();

            List<CodeMetricsLineModel> codeMetricsBranche = InitializeCodeMetrics(codeMetricsBrancheFile);
            codeMetricsBrancheFile.Close();

            IEnumerable<CodeMetricsLineView> codeMetrics = InitializeCodeMetricsDifferences(codeMetricsTrunk, codeMetricsBranche);

            return InitCodeMetricsTree(codeMetrics);
        }

        /// <summary>
        /// Creates a list of CodeMetricsLineModel with information from the excel file
        /// </summary>
        private static List<CodeMetricsLineModel> InitializeCodeMetrics(StreamReader file)
        {
            var codeMetrics = new List<CodeMetricsLineModel>();

            using (var excelPackage = new ExcelPackage(file.BaseStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];

                for (int row = 2; worksheet.Cells[row, 1].Value != null; row++)
                {
                    var line = new CodeMetricsLineModel
                    {
                        Scope = ConvertString(worksheet.Cells[row, 1]),
                        Project = ConvertString(worksheet.Cells[row, 2]),
                        Namespace = ConvertString(worksheet.Cells[row, 3]),
                        Type = ConvertString(worksheet.Cells[row, 4]),
                        Member = ConvertString(worksheet.Cells[row, 5]),
                        MaintainabilityIndex = ConvertDouble(worksheet.Cells[row, 6]),
                        CyclomaticComplexity = ConvertDouble(worksheet.Cells[row, 7]),
                        DepthOfInheritance = ConvertDouble(worksheet.Cells[row, 8]),
                        ClassCoupling = ConvertDouble(worksheet.Cells[row, 9]),
                        LinesOfCode = ConvertDouble(worksheet.Cells[row, 10]),
                    };

                    if (!line.Project.Contains("Test"))
                    {
                        line.Project = line.Project.Replace(" (Debug)", string.Empty);
                        line.Project = line.Project.Split('\\')[line.Project.Split('\\').Length - 1];

                        codeMetrics.Add(line);
                    }
                }
            }

            return codeMetrics;
        }

        /// <summary>
        /// Converts an excel range to a string or null
        /// </summary>
        private static string ConvertString(ExcelRange cell)
        {
            return cell.Value != null ? cell.Value.ToString() : string.Empty;
        }

        /// <summary>
        /// Converts an excel range to a double? or null
        /// </summary>
        private static double? ConvertDouble(ExcelRange cell)
        {
            return cell.Value != null ? (double?)cell.Value : null;
        }

        /// <summary>
        /// Creates a list of CodeMetricsLineView containing difference of metrics between two lists of CodeMetricsLineModel
        /// </summary>
        private static IEnumerable<CodeMetricsLineView> InitializeCodeMetricsDifferences(List<CodeMetricsLineModel> codeMetricsTrunk, List<CodeMetricsLineModel> codeMetricsBranche)
        {
            var codeMetrics = new List<CodeMetricsLineView>();

            foreach (CodeMetricsLineModel line in codeMetricsBranche)
            {
                CodeMetricsLineView codeMetricsLineView = CreateCodeMetricsViewFromBranche(line);

                CodeMetricsLineModel sameLine = GetSameLine(line, codeMetricsTrunk);
                AddDifferences(codeMetricsLineView, line, sameLine);

                codeMetrics.Add(codeMetricsLineView);
            }

            return codeMetrics;
        }

        /// <summary>
        /// Gets a line from a list with same informations
        /// </summary>
        private static CodeMetricsLineModel GetSameLine(CodeMetricsLineModel codeMetricsToFind, List<CodeMetricsLineModel> codeMetricsToExplore)
        {
            foreach (var line in codeMetricsToExplore)
            {
                if (line.Scope == codeMetricsToFind.Scope
                    && line.Project == codeMetricsToFind.Project
                    && line.Namespace == codeMetricsToFind.Namespace
                    && line.Type == codeMetricsToFind.Type
                    && line.Member == codeMetricsToFind.Member)
                {
                    return line;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates code metrics view from branche
        /// </summary>
        private static CodeMetricsLineView CreateCodeMetricsViewFromBranche(CodeMetricsLineModel line)
        {
            return new CodeMetricsLineView
            {
                Scope = line.Scope,
                Project = line.Project,
                Namespace = line.Namespace,
                Type = line.Type,
                Member = line.Member,

                MaintainabilityIndexBranche = line.MaintainabilityIndex,
                CyclomaticComplexityBranche = line.CyclomaticComplexity,
                DepthOfInheritanceBranche = line.DepthOfInheritance,
                ClassCouplingBranche = line.ClassCoupling,
                LinesOfCodeBranche = line.LinesOfCode
            };
        }

        /// <summary>
        /// Adds differences between two lines
        /// </summary>
        private static void AddDifferences(CodeMetricsLineView codeMetricsLineView, CodeMetricsLineModel currentLine, CodeMetricsLineModel sameLine)
        {
            if (sameLine != null)
            {
                codeMetricsLineView.MaintainabilityIndexTrunk = sameLine.MaintainabilityIndex;
                codeMetricsLineView.CyclomaticComplexityTrunk = sameLine.CyclomaticComplexity;
                codeMetricsLineView.DepthOfInheritanceTrunk = sameLine.DepthOfInheritance;
                codeMetricsLineView.ClassCouplingTrunk = sameLine.ClassCoupling;
                codeMetricsLineView.LinesOfCodeTrunk = sameLine.LinesOfCode;

                codeMetricsLineView.MaintainabilityIndexDifference = -(currentLine.MaintainabilityIndex - sameLine.MaintainabilityIndex);
                codeMetricsLineView.CyclomaticComplexityDifference = currentLine.CyclomaticComplexity - sameLine.CyclomaticComplexity;
                codeMetricsLineView.DepthOfInheritanceDifference = currentLine.DepthOfInheritance - sameLine.DepthOfInheritance;
                codeMetricsLineView.ClassCouplingDifference = currentLine.ClassCoupling - sameLine.ClassCoupling;
                codeMetricsLineView.LinesOfCodeDifference = currentLine.LinesOfCode - sameLine.LinesOfCode;
            }
        }

        /// <summary>
        /// Initializes the tree of code metrics
        /// </summary>
        private static IEnumerable<CodeMetricsLineView> InitCodeMetricsTree(IEnumerable<CodeMetricsLineView> codeMetrics)
        {
            var list = new List<CodeMetricsLineView>();

            foreach (CodeMetricsLineView line in codeMetrics)
            {
                switch (line.Scope)
                {
                    case "Project":
                        list.Add(line);
                        break;

                    case "Namespace":
                        list.Last().Children.Add(line);
                        break;

                    case "Type":
                        list.Last().Children.Last().Children.Add(line);
                        break;

                    case "Member":
                        list.Last().Children.Last().Children.Last().Children.Add(line);
                        break;
                }
            }

            return list;
        }
    }
}