using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Web;

namespace NicamNew.msn.marketing.conditions
{
    using Sitecore.Rules;
    using Sitecore.Rules.Conditions;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Conditions/CampaignSensor" when creating CampaignSensor class. Fix Text field.

           public class CampaignSensor<T> : WhenCondition<T> where T : ConditionalRenderingsRuleContext
        {
            // Methods 
            protected override bool Execute(T ruleContext)
            {
                        Item[] CampaignEvents = Sitecore.Context.Database.SelectItems(
                        "/sitecore/system/Marketing Center/Campaigns//*[@@templatename = 'Campaign Event']");
                List<String> CampaignIds = CampaignEvents.Select(CampaignEvent => CampaignEvent.ID.ToString().Replace("{", "").Replace("}", "").Replace("-", "")).ToList();
                SafeDictionary<string> QueryStrings = WebUtil.ParseQueryString(WebUtil.GetQueryString());
                return (QueryStrings.Any(keyValuePair => CampaignIds.Contains(keyValuePair.Value)));
            }    
       }
}

