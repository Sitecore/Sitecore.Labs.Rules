using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace NicamNew.msn.marketing.conditions
{
    public class PageEventDetected<T> : WhenCondition<T> where T : ConditionalRenderingsRuleContext
    {
        // Methods 

        public String pageEvent { get; set; }

        protected override bool Execute(T ruleContext)
        {
            List<String> PageEvents =
                AnalyticsManager.ReadMany(
                    "\r\nSELECT DISTINCT\r\n{0}Name{1}\r\nFROM {0}Page{1}\r\n inner join {0}PageEvent{1} on \r\n{0}PageEvent{1}.{0}PageId{1} = {0}Page{1}.{0}PageId{1}\r\n inner join {0}PageEventDefinition{1} on \r\n{0}PageEventDefinition{1}.{0}PageEventDefinitionId{1} = {0}PageEvent{1}.{0}PageEventDefinitionId{1}WHERE {0}Page{1}.{0}SessionId{1} = {2}sessionId{3}",
                    delegate(DataProviderReader reader) { return AnalyticsManager.Provider.GetString(0, reader); },
                    new object[] {"sessionId", AnalyticsTracker.Current.CurrentSession.SessionId.ToString()});


            if (PageEvents != null) return (PageEvents.Contains(Context.Database.GetItem(pageEvent).Name));
            return false;
        }
    }
}