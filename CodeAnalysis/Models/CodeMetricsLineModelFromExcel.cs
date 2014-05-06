namespace CodeAnalysis.Models
{
    public class CodeMetricsLineModelFromExcel
    {
        public string Scope { get; set; }
        public string Project { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }

        public double? MaintainabilityIndex { get; set; }
        public double? CyclomaticComplexity { get; set; }
        public double? DepthOfInheritance { get; set; }
        public double? ClassCoupling { get; set; }
        public double? LinesOfCode { get; set; }
    }
}