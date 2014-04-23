namespace CodeAnalysis.Models
{
    using System.Collections.Generic;

    public class CodeCoverageLineView
    {
        public CodeCoverageLineView()
        {
            Children = new List<CodeCoverageLineView>();
        }

        public string Project { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }

        public int? CoveredLinesTrunk { get; set; }
        public int? CoveredLinesPercentageTrunk { get; set; }
        public int? CoveredBlocksTrunk { get; set; }
        public int? CoveredBlocksPercentageTrunk { get; set; }

        public int? CoveredLinesBranche { get; set; }
        public int? CoveredLinesPercentageBranche { get; set; }
        public int? CoveredBlocksBranche { get; set; }
        public int? CoveredBlocksPercentageBranche { get; set; }

        public int? CoveredLinesDifference { get; set; }
        public int? CoveredLinesPercentageDifference { get; set; }
        public int? CoveredBlocksDifference  { get; set; }
        public int? CoveredBlocksPercentageDifference { get; set; }

        public List<CodeCoverageLineView> Children { get; set; }
    }
}