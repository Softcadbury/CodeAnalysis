namespace CodeAnalysis.BusinessLogic
{
    using System.Collections.Generic;
    using System.IO;

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
            return new List<CodeMetricsLineView>();
        }
    }
}