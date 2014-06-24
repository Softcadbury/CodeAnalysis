namespace CodeAnalysis.BusinessLogic
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using CodeAnalysis.Models;

    /// <summary>
    /// This class generate averages for code metrics and code coverage
    /// </summary>
    public static class AverageGenerator
    {
        /// <summary>
        /// Adds average for code metrics
        /// </summary>
        public static void AddCodeMetricsAverage(ObservableCollection<CodeMetricsLineView> codeMetricsTree)
        {
            var codeMetricsLineView = new CodeMetricsLineView { Project = "------ Average ------" };

            InitializeCodeMetricsTrunkAverage(codeMetricsTree, codeMetricsLineView);
            InitializeCodeMetricsBrancheAverage(codeMetricsTree, codeMetricsLineView);

            codeMetricsTree.Add(codeMetricsLineView);
        }

        /// <summary>
        /// Initializes CodeMetricsTrunkAverage
        /// </summary>
        private static void InitializeCodeMetricsTrunkAverage(ObservableCollection<CodeMetricsLineView> codeMetricsTree, CodeMetricsLineView codeMetricsLineView)
        {
            double maintainabilityIndexSum = codeMetricsTree.Sum(line => line.MaintainabilityIndexTrunk.HasValue ? line.MaintainabilityIndexTrunk.Value : 0);
            double maintainabilityIndexCount = codeMetricsTree.Count(line => line.MaintainabilityIndexTrunk.HasValue);
            codeMetricsLineView.MaintainabilityIndexTrunk = Math.Round(maintainabilityIndexSum / maintainabilityIndexCount, 2);

            double cyclomaticComplexitySum = codeMetricsTree.Sum(line => line.CyclomaticComplexityTrunk.HasValue ? line.CyclomaticComplexityTrunk.Value : 0);
            double cyclomaticComplexityCount = codeMetricsTree.Count(line => line.CyclomaticComplexityTrunk.HasValue);
            codeMetricsLineView.CyclomaticComplexityTrunk = Math.Round(cyclomaticComplexitySum / cyclomaticComplexityCount, 2);

            double depthOfInheritanceSum = codeMetricsTree.Sum(line => line.DepthOfInheritanceTrunk.HasValue ? line.DepthOfInheritanceTrunk.Value : 0);
            double depthOfInheritanceCount = codeMetricsTree.Count(line => line.DepthOfInheritanceTrunk.HasValue);
            codeMetricsLineView.DepthOfInheritanceTrunk = Math.Round(depthOfInheritanceSum / depthOfInheritanceCount, 2);

            double classCouplingSum = codeMetricsTree.Sum(line => line.ClassCouplingTrunk.HasValue ? line.ClassCouplingTrunk.Value : 0);
            double classCouplingCount = codeMetricsTree.Count(line => line.ClassCouplingTrunk.HasValue);
            codeMetricsLineView.ClassCouplingTrunk = Math.Round(classCouplingSum / classCouplingCount, 2);

            double linesOfCodeSum = codeMetricsTree.Sum(line => line.LinesOfCodeTrunk.HasValue ? line.LinesOfCodeTrunk.Value : 0);
            double linesOfCodeCount = codeMetricsTree.Count(line => line.LinesOfCodeTrunk.HasValue);
            codeMetricsLineView.LinesOfCodeTrunk = Math.Round(linesOfCodeSum / linesOfCodeCount, 2);
        }

        /// <summary>
        /// Initializes CodeMetricsBrancheAverage
        /// </summary>
        private static void InitializeCodeMetricsBrancheAverage(ObservableCollection<CodeMetricsLineView> codeMetricsTree, CodeMetricsLineView codeMetricsLineView)
        {
            double maintainabilityIndexSum = codeMetricsTree.Sum(line => line.MaintainabilityIndexBranche.HasValue ? line.MaintainabilityIndexBranche.Value : 0);
            double maintainabilityIndexCount = codeMetricsTree.Count(line => line.MaintainabilityIndexBranche.HasValue);
            codeMetricsLineView.MaintainabilityIndexBranche = Math.Round(maintainabilityIndexSum / maintainabilityIndexCount, 2);

            double cyclomaticComplexitySum = codeMetricsTree.Sum(line => line.CyclomaticComplexityBranche.HasValue ? line.CyclomaticComplexityBranche.Value : 0);
            double cyclomaticComplexityCount = codeMetricsTree.Count(line => line.CyclomaticComplexityBranche.HasValue);
            codeMetricsLineView.CyclomaticComplexityBranche = Math.Round(cyclomaticComplexitySum / cyclomaticComplexityCount, 2);

            double depthOfInheritanceSum = codeMetricsTree.Sum(line => line.DepthOfInheritanceBranche.HasValue ? line.DepthOfInheritanceBranche.Value : 0);
            double depthOfInheritanceCount = codeMetricsTree.Count(line => line.DepthOfInheritanceBranche.HasValue);
            codeMetricsLineView.DepthOfInheritanceBranche = Math.Round(depthOfInheritanceSum / depthOfInheritanceCount, 2);

            double classCouplingSum = codeMetricsTree.Sum(line => line.ClassCouplingBranche.HasValue ? line.ClassCouplingBranche.Value : 0);
            double classCouplingCount = codeMetricsTree.Count(line => line.ClassCouplingBranche.HasValue);
            codeMetricsLineView.ClassCouplingBranche = Math.Round(classCouplingSum / classCouplingCount, 2);

            double linesOfCodeSum = codeMetricsTree.Sum(line => line.LinesOfCodeBranche.HasValue ? line.LinesOfCodeBranche.Value : 0);
            double linesOfCodeCount = codeMetricsTree.Count(line => line.LinesOfCodeBranche.HasValue);
            codeMetricsLineView.LinesOfCodeBranche = Math.Round(linesOfCodeSum / linesOfCodeCount, 2);
        }
    }
}