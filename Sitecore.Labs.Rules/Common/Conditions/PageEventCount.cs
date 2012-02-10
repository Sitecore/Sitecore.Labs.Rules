using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Diagnostics;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using Sitecore.Rules;

namespace NicamNew.msn.marketing.conditions
{
    public class PageEventCount<T> : OperatorCondition<T> where T : RuleContext
    {

        public String pageEvent { get; set; }
        public int Count { get; set; }

        protected override bool Execute(T ruleContext)
        {
            List<String> PageEvents =
                AnalyticsManager.ReadMany(
                    "\r\nSELECT {0}Name{1}\r\nFROM {0}Page{1}\r\n inner join {0}PageEvent{1} on \r\n{0}PageEvent{1}.{0}PageId{1} = {0}Page{1}.{0}PageId{1}\r\n inner join {0}PageEventDefinition{1} on \r\n{0}PageEventDefinition{1}.{0}PageEventDefinitionId{1} = {0}PageEvent{1}.{0}PageEventDefinitionId{1}WHERE {0}PageEventDefinition{1}.{0}Name{1} = {2}pageEvent{3}",
                    delegate(DataProviderReader reader) { return AnalyticsManager.Provider.GetString(0, reader); },
                    new object[] { "pageEvent", Sitecore.Context.Database.GetItem(pageEvent).Name });


            int CountInner = PageEvents.Count;
            switch (base.GetOperator())
            {
                case ConditionOperator.Equal:
                    return (CountInner == this.Count);

                case ConditionOperator.GreaterThanOrEqual:
                    return (CountInner >= this.Count);

                case ConditionOperator.GreaterThan:
                    return (CountInner > this.Count);

                case ConditionOperator.LessThanOrEqual:
                    return (CountInner <= this.Count);

                case ConditionOperator.LessThan:
                    return (CountInner < this.Count);

                case ConditionOperator.NotEqual:
                    return (CountInner != this.Count);

            }

            return false;


        }



    }
}