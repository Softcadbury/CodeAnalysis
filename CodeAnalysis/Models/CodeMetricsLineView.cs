namespace CodeAnalysis.Models
{
    using System.Collections.Generic;

    public class CodeMetricsLineView
    {
        public CodeMetricsLineView()
        {
            Children = new List<CodeMetricsLineView>();
        }

        public string Scope { get; set; }
        public string Project { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }

        public double? MaintainabilityIndexTrunk { get; set; }
        public double? CyclomaticComplexityTrunk { get; set; }
        public double? DepthOfInheritanceTrunk { get; set; }
        public double? ClassCouplingTrunk { get; set; }
        public double? LinesOfCodeTrunk { get; set; }

        public double? MaintainabilityIndexBranche { get; set; }
        public double? CyclomaticComplexityBranche { get; set; }
        public double? DepthOfInheritanceBranche { get; set; }
        public double? ClassCouplingBranche { get; set; }
        public double? LinesOfCodeBranche { get; set; }

        public double? MaintainabilityIndexDifference { get; set; }
        public double? CyclomaticComplexityDifference { get; set; }
        public double? DepthOfInheritanceDifference { get; set; }
        public double? ClassCouplingDifference { get; set; }

        public List<CodeMetricsLineView> Children { get; set; }
    }
}