namespace CodeAnalysis.Models
{
    using System.Collections.Generic;

    public class CodeMetricsLineModelFromXml
    {
        public CodeMetricsLineModelFromXml()
        {
            Children = new List<CodeMetricsLineModelFromXml>();
        }

        public string Project { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string Member { get; set; }

        public double? MaintainabilityIndex { get; set; }
        public double? CyclomaticComplexity { get; set; }
        public double? DepthOfInheritance { get; set; }
        public double? ClassCoupling { get; set; }
        public double? LinesOfCode { get; set; }

        public List<CodeMetricsLineModelFromXml> Children { get; set; }
    }
}