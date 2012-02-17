using Sitecore.Analytics;
using Sitecore.Rules.Conditions;
using Sitecore.Rules;

namespace Sitecore.Labs.Rules.Oms.Common.Conditions
{
    public class PageEventCount<T> : OperatorCondition<T> where T : RuleContext
    {
        public string PageEvent { get; set; }
        public int Count { get; set; }

        protected override bool Execute(T ruleContext)
        {
            var pageEvents =
                AnalyticsManager.ReadMany(
                    "\r\nSELECT {0}Name{1}\r\nFROM {0}Page{1}\r\n inner join {0}PageEvent{1} on \r\n{0}PageEvent{1}.{0}PageId{1} = {0}Page{1}.{0}PageId{1}\r\n inner join {0}PageEventDefinition{1} on \r\n{0}PageEventDefinition{1}.{0}PageEventDefinitionId{1} = {0}PageEvent{1}.{0}PageEventDefinitionId{1}WHERE {0}PageEventDefinition{1}.{0}Name{1} = {2}pageEvent{3}",
                    reader => AnalyticsManager.Provider.GetString(0, reader),
                    new object[] { "pageEvent", Context.Database.GetItem(PageEvent).Name });

            var countInner = pageEvents.Count;
            switch (base.GetOperator())
            {
                case ConditionOperator.Equal:
                    return (countInner == Count);

                case ConditionOperator.GreaterThanOrEqual:
                    return (countInner >= Count);

                case ConditionOperator.GreaterThan:
                    return (countInner > Count);

                case ConditionOperator.LessThanOrEqual:
                    return (countInner <= Count);

                case ConditionOperator.LessThan:
                    return (countInner < Count);

                case ConditionOperator.NotEqual:
                    return (countInner != Count);
            }

            return false;
        }
    }
}