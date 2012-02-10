using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Diagnostics;

namespace NicamNew.msn.marketing.conditions
{
    using Sitecore.Rules;
    using Sitecore.Rules.Conditions;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Conditions/IsExitPage" when creating IsExitPage class. Fix Text field.

    public class IsExitPage<T> : WhenCondition<T> where T : RuleContext
    {
        public String pageEvent { get; set; }

        protected override bool Execute(T ruleContext)
        {
            //List<String> PageEvents =
            //    AnalyticsManager.ReadMany(
            //        "\r\nSELECT DISTINCT\r\n{0}Name{1}\r\nFROM {0}Page{1}\r\n inner join {0}PageEvent{1} on \r\n{0}PageEvent{1}.{0}PageId{1} = {0}Page{1}.{0}PageId{1}\r\n inner join {0}PageEventDefinition{1} on \r\n{0}PageEventDefinition{1}.{0}PageEventDefinitionId{1} = {0}PageEvent{1}.{0}PageEventDefinitionId{1}WHERE {0}Page{1}.{0}SessionId{1} = {2}sessionId{3}",
            //        delegate(DataProviderReader reader) { return AnalyticsManager.Provider.GetString(0, reader); },
            //        new object[] { "sessionId", AnalyticsTracker.Current.CurrentSession.SessionId.ToString() });

            List<SmallPage> ExitPages =
                AnalyticsManager.ReadMany("\r\nSELECT TOP 50\r\n{0}Page{1}.{0}ItemId{1}, \r\nCount(*) as Total\r\nFROM {0}Page{1}, {0}Session{1}, {0}GlobalSession{1}, {0}IPOwner{1}, {0}IP{1}WHERE {0}Page{1}.{0}ItemId{1} != {2}itemId{3}GROUP BY {0}Page{1}.{0}ItemId{1} ORDER BY Total DESC", 
            ReadPage,
                    new object[] { "itemId", "00000000-0000-0000-0000-000000000000" });

           //"select top 50 	Page.ItemId, Count(*) from 	Page,	Session,	GlobalSession,	IPOwner,	IP where Page.ItemId != '00000000-0000-0000-0000-000000000000' and	Session.SessionId = Page.SessionId and	Session.IpId = IP.IpId and	IP.IpOwnerId = IPOwner.IpOwnerId and	Session.GlobalSessionId = GlobalSession.GlobalSessionId and Page.PageNo = Session.CurrentPage"

            if (ExitPages != null)
                return ExitPages.FindAll(i => i.Id == pageEvent).Count > 0;
            // return (PageEvents.Contains(Context.Database.GetItem(pageEvent).Name));
            return false;
        }

        private SmallPage ReadPage(DataProviderReader reader)
        {
            Assert.ArgumentNotNull(reader, "reader");
            var page = new SmallPage();
            page.Id = AnalyticsManager.Provider.GetString(0, reader);
            page.Count = AnalyticsManager.Provider.GetInt(1, reader);
        
            return page;
        }
    }



  
}