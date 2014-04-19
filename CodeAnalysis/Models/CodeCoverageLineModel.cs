namespace CodeAnalysis.Models
{
    using System.Collections.Generic;

    public class CodeCoverageLineModel
    {
        public CodeCoverageLineModel()
        {
            Children = new List<CodeCoverageLineModel>();
        }

        public string Project { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }

        public int CoveredLines { get; set; }
        public int CoveredLinesPercentage { get; set; }
        public int CoveredBlocks { get; set; }
        public int CoveredBlocksPercentage { get; set; }

        public List<CodeCoverageLineModel> Children { get; set; }
    }
}