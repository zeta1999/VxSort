using System.Collections.Generic;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Bench.Utils
{
public abstract class BaselineCustomColumn : IColumn
    {
        public abstract string Id { get; }
        public abstract string ColumnName { get; }

        public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        {
            string logicalGroupKey = summary.GetLogicalGroupKey(benchmarkCase);
            var baseline = summary.GetBaseline(logicalGroupKey);
            bool isBaseline = summary.IsBaseline(benchmarkCase);

            if (ResultsAreInvalid(summary, benchmarkCase, baseline))
                return "?";

            var baselineStat = summary[baseline].ResultStatistics;
            var baselineMetric = summary[baseline].Metrics;
            var currentStat = summary[benchmarkCase].ResultStatistics;
            var currentMetric = summary[benchmarkCase].Metrics;

            return GetValue(summary, benchmarkCase, baselineStat, baselineMetric, currentStat, currentMetric, isBaseline);
        }

        internal abstract string GetValue(Summary summary,        BenchmarkCase benchmarkCase, Statistics baseline,
            IReadOnlyDictionary<string, Metric>   baselineMetric, Statistics    current,
            IReadOnlyDictionary<string, Metric>   currentMetric,  bool          isBaseline);

        public bool IsAvailable(Summary summary) => summary.HasBaselines();
        public bool AlwaysShow => true;
        public ColumnCategory Category => ColumnCategory.Baseline;
        public abstract int PriorityInCategory { get; }
        public abstract bool IsNumeric { get; }
        public abstract UnitType UnitType { get; }
        public abstract string Legend { get; }
        public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);
        public override string ToString() => ColumnName;
        public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

        internal static bool ResultsAreInvalid(Summary summary, BenchmarkCase benchmarkCase, BenchmarkCase baseline)
        {
            return baseline == null ||
                   summary[baseline] == null ||
                   summary[baseline].ResultStatistics == null ||
                   !summary[baseline].ResultStatistics.CanBeInverted() ||
                   summary[benchmarkCase] == null ||
                   summary[benchmarkCase].ResultStatistics == null;
        }
    }
}