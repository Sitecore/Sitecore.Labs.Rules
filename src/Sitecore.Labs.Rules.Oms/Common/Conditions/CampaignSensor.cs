using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Web;
using Sitecore.Rules.Conditions;

namespace Sitecore.Labs.Rules.Oms.Common.Conditions
{
    // TODO : Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Conditions/CampaignSensor" when creating CampaignSensor class. Fix Text field.

    public class CampaignSensor<T> : WhenCondition<T> where T : ConditionalRenderingsRuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            Item[] campaignEvents = Sitecore.Context.Database.SelectItems(
            "/sitecore/system/Marketing Center/Campaigns//*[@@templatename = 'Campaign Event']");
            List<string> campaignIds = campaignEvents.Select(campaignEvent => campaignEvent.ID.ToString().Replace("{", "").Replace("}", "").Replace("-", "")).ToList();
            SafeDictionary<string> queryStrings = WebUtil.ParseQueryString(WebUtil.GetQueryString());

            return (queryStrings.Any(keyValuePair => campaignIds.Contains(keyValuePair.Value)));
        }
    }
}

