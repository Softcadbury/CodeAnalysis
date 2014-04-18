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

        public double? CoveredLinePercentage { get; set; }
        public double? CoveredLine { get; set; }

        public List<CodeCoverageLineModel> Children { get; set; }
    }
}