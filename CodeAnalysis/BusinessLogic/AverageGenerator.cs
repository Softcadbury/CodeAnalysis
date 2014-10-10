namespace CodeAnalysis.BusinessLogic
{
    using System.Collections.Generic;
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
        public static void AddCodeMetricsAverage(List<CodeMetricsLineView> codeMetricsTree)
        {
            var codeMetricsLineView = new CodeMetricsLineView { Project = "------ Average ------" };

            InitializeCodeMetricsTrunkAverage(codeMetricsTree, codeMetricsLineView);
            InitializeCodeMetricsBrancheAverage(codeMetricsTree, codeMetricsLineView);
            InitializeCodeMetricsDifferenceAverage(codeMetricsLineView);

            codeMetricsTree.Add(codeMetricsLineView);
        }

        /// <summary>
        /// Adds average for code coverage
        /// </summary>
        public static void AddCodeCoverageAverage(List<CodeCoverageLineView> codeCoverageTree)
        {
            var codeCoverageLineView = new CodeCoverageLineView { Project = "------ Average ------" };

            InitializeCodeCoverageTrunkAverage(codeCoverageTree, codeCoverageLineView);
            InitializeCodeCoverageBrancheAverage(codeCoverageTree, codeCoverageLineView);
            InitializeCodeCoverageDifferenceAverage(codeCoverageLineView);

            codeCoverageTree.Add(codeCoverageLineView);
        }

        /// <summary>
        /// Initializes CodeMetricsTrunkAverage
        /// </summary>
        private static void InitializeCodeMetricsTrunkAverage(List<CodeMetricsLineView> codeMetricsTree, CodeMetricsLineView codeMetricsLineView)
        {
            double maintainabilityIndexSum = codeMetricsTree.Sum(line => line.MaintainabilityIndexTrunk.HasValue ? line.MaintainabilityIndexTrunk.Value : 0);
            double maintainabilityIndexCount = codeMetricsTree.Count(line => line.MaintainabilityIndexTrunk.HasValue);
            codeMetricsLineView.MaintainabilityIndexTrunk = (int)(maintainabilityIndexSum / maintainabilityIndexCount);

            double cyclomaticComplexitySum = codeMetricsTree.Sum(line => line.CyclomaticComplexityTrunk.HasValue ? line.CyclomaticComplexityTrunk.Value : 0);
            double cyclomaticComplexityCount = codeMetricsTree.Count(line => line.CyclomaticComplexityTrunk.HasValue);
            codeMetricsLineView.CyclomaticComplexityTrunk = (int)(cyclomaticComplexitySum / cyclomaticComplexityCount);

            double depthOfInheritanceSum = codeMetricsTree.Sum(line => line.DepthOfInheritanceTrunk.HasValue ? line.DepthOfInheritanceTrunk.Value : 0);
            double depthOfInheritanceCount = codeMetricsTree.Count(line => line.DepthOfInheritanceTrunk.HasValue);
            codeMetricsLineView.DepthOfInheritanceTrunk = (int)(depthOfInheritanceSum / depthOfInheritanceCount);

            double classCouplingSum = codeMetricsTree.Sum(line => line.ClassCouplingTrunk.HasValue ? line.ClassCouplingTrunk.Value : 0);
            double classCouplingCount = codeMetricsTree.Count(line => line.ClassCouplingTrunk.HasValue);
            codeMetricsLineView.ClassCouplingTrunk = (int)(classCouplingSum / classCouplingCount);

            double linesOfCodeSum = codeMetricsTree.Sum(line => line.LinesOfCodeTrunk.HasValue ? line.LinesOfCodeTrunk.Value : 0);
            double linesOfCodeCount = codeMetricsTree.Count(line => line.LinesOfCodeTrunk.HasValue);
            codeMetricsLineView.LinesOfCodeTrunk = (int)(linesOfCodeSum / linesOfCodeCount);
        }

        /// <summary>
        /// Initializes CodeMetricsBrancheAverage
        /// </summary>
        private static void InitializeCodeMetricsBrancheAverage(List<CodeMetricsLineView> codeMetricsTree, CodeMetricsLineView codeMetricsLineView)
        {
            double maintainabilityIndexSum = codeMetricsTree.Sum(line => line.MaintainabilityIndexBranche.HasValue ? line.MaintainabilityIndexBranche.Value : 0);
            double maintainabilityIndexCount = codeMetricsTree.Count(line => line.MaintainabilityIndexBranche.HasValue);
            codeMetricsLineView.MaintainabilityIndexBranche = (int)(maintainabilityIndexSum / maintainabilityIndexCount);

            double cyclomaticComplexitySum = codeMetricsTree.Sum(line => line.CyclomaticComplexityBranche.HasValue ? line.CyclomaticComplexityBranche.Value : 0);
            double cyclomaticComplexityCount = codeMetricsTree.Count(line => line.CyclomaticComplexityBranche.HasValue);
            codeMetricsLineView.CyclomaticComplexityBranche = (int)(cyclomaticComplexitySum / cyclomaticComplexityCount);

            double depthOfInheritanceSum = codeMetricsTree.Sum(line => line.DepthOfInheritanceBranche.HasValue ? line.DepthOfInheritanceBranche.Value : 0);
            double depthOfInheritanceCount = codeMetricsTree.Count(line => line.DepthOfInheritanceBranche.HasValue);
            codeMetricsLineView.DepthOfInheritanceBranche = (int)(depthOfInheritanceSum / depthOfInheritanceCount);

            double classCouplingSum = codeMetricsTree.Sum(line => line.ClassCouplingBranche.HasValue ? line.ClassCouplingBranche.Value : 0);
            double classCouplingCount = codeMetricsTree.Count(line => line.ClassCouplingBranche.HasValue);
            codeMetricsLineView.ClassCouplingBranche = (int)(classCouplingSum / classCouplingCount);

            double linesOfCodeSum = codeMetricsTree.Sum(line => line.LinesOfCodeBranche.HasValue ? line.LinesOfCodeBranche.Value : 0);
            double linesOfCodeCount = codeMetricsTree.Count(line => line.LinesOfCodeBranche.HasValue);
            codeMetricsLineView.LinesOfCodeBranche = (int)(linesOfCodeSum / linesOfCodeCount);
        }

        /// <summary>
        /// Initializes CodeMetricsDifferenceAverage
        /// </summary>
        private static void InitializeCodeMetricsDifferenceAverage(CodeMetricsLineView codeMetricsLineView)
        {
            codeMetricsLineView.MaintainabilityIndexDifference = codeMetricsLineView.MaintainabilityIndexBranche - codeMetricsLineView.MaintainabilityIndexTrunk;
            codeMetricsLineView.CyclomaticComplexityDifference = -(codeMetricsLineView.CyclomaticComplexityBranche - codeMetricsLineView.CyclomaticComplexityTrunk);
            codeMetricsLineView.DepthOfInheritanceDifference = -(codeMetricsLineView.DepthOfInheritanceBranche - codeMetricsLineView.DepthOfInheritanceTrunk);
            codeMetricsLineView.ClassCouplingDifference = -(codeMetricsLineView.ClassCouplingBranche - codeMetricsLineView.ClassCouplingTrunk);
        }

        /// <summary>
        /// Initializes CodeCoverageTrunkAverage
        /// </summary>
        private static void InitializeCodeCoverageTrunkAverage(List<CodeCoverageLineView> codeCoverageTree, CodeCoverageLineView codeCoverageLineView)
        {
            double coveredLinesSum = codeCoverageTree.Sum(line => line.CoveredLinesTrunk.HasValue ? line.CoveredLinesTrunk.Value : 0);
            int coveredLinesCount = codeCoverageTree.Count(line => line.CoveredLinesTrunk.HasValue);
            codeCoverageLineView.CoveredLinesTrunk = (int)(coveredLinesSum / coveredLinesCount);

            double coveredLinesPercentageSum = codeCoverageTree.Sum(line => line.CoveredLinesPercentageTrunk.HasValue ? line.CoveredLinesPercentageTrunk.Value : 0);
            int coveredLinesPercentageCount = codeCoverageTree.Count(line => line.CoveredLinesPercentageTrunk.HasValue);
            codeCoverageLineView.CoveredLinesPercentageTrunk = (int)(coveredLinesPercentageSum / coveredLinesPercentageCount);

            double coveredBlocksSum = codeCoverageTree.Sum(line => line.CoveredBlocksTrunk.HasValue ? line.CoveredBlocksTrunk.Value : 0);
            int coveredBlocksCount = codeCoverageTree.Count(line => line.CoveredBlocksTrunk.HasValue);
            codeCoverageLineView.CoveredBlocksTrunk = (int)(coveredBlocksSum / coveredBlocksCount);

            double coveredBlocksPercentageSum = codeCoverageTree.Sum(line => line.CoveredBlocksPercentageTrunk.HasValue ? line.CoveredBlocksPercentageTrunk.Value : 0);
            int coveredBlocksPercentageCount = codeCoverageTree.Count(line => line.CoveredBlocksPercentageTrunk.HasValue);
            codeCoverageLineView.CoveredBlocksPercentageTrunk = (int)(coveredBlocksPercentageSum / coveredBlocksPercentageCount);
        }

        /// <summary>
        /// Initializes CodeCoverageBrancheAverage
        /// </summary>
        private static void InitializeCodeCoverageBrancheAverage(List<CodeCoverageLineView> codeCoverageTree, CodeCoverageLineView codeCoverageLineView)
        {
            double coveredLinesSum = codeCoverageTree.Sum(line => line.CoveredLinesBranche.HasValue ? line.CoveredLinesBranche.Value : 0);
            int coveredLinesCount = codeCoverageTree.Count(line => line.CoveredLinesBranche.HasValue);
            codeCoverageLineView.CoveredLinesBranche = (int)(coveredLinesSum / coveredLinesCount);

            double coveredLinesPercentageSum = codeCoverageTree.Sum(line => line.CoveredLinesPercentageBranche.HasValue ? line.CoveredLinesPercentageBranche.Value : 0);
            int coveredLinesPercentageCount = codeCoverageTree.Count(line => line.CoveredLinesPercentageBranche.HasValue);
            codeCoverageLineView.CoveredLinesPercentageBranche = (int)(coveredLinesPercentageSum / coveredLinesPercentageCount);

            double coveredBlocksSum = codeCoverageTree.Sum(line => line.CoveredBlocksBranche.HasValue ? line.CoveredBlocksBranche.Value : 0);
            int coveredBlocksCount = codeCoverageTree.Count(line => line.CoveredBlocksBranche.HasValue);
            codeCoverageLineView.CoveredBlocksBranche = (int)(coveredBlocksSum / coveredBlocksCount);

            double coveredBlocksPercentageSum = codeCoverageTree.Sum(line => line.CoveredBlocksPercentageBranche.HasValue ? line.CoveredBlocksPercentageBranche.Value : 0);
            int coveredBlocksPercentageCount = codeCoverageTree.Count(line => line.CoveredBlocksPercentageBranche.HasValue);
            codeCoverageLineView.CoveredBlocksPercentageBranche = (int)(coveredBlocksPercentageSum / coveredBlocksPercentageCount);
        }

        /// <summary>
        /// Initializes CodeCoverageDifferenceAverage
        /// </summary>
        private static void InitializeCodeCoverageDifferenceAverage(CodeCoverageLineView codeCoverageLineView)
        {
            codeCoverageLineView.CoveredLinesPercentageDifference = codeCoverageLineView.CoveredLinesPercentageBranche - codeCoverageLineView.CoveredLinesPercentageTrunk;
            codeCoverageLineView.CoveredBlocksPercentageDifference = codeCoverageLineView.CoveredBlocksPercentageBranche - codeCoverageLineView.CoveredBlocksPercentageTrunk;
        }
    }
}