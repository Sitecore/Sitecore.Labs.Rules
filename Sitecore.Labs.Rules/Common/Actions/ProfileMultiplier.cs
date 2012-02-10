using System;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;

namespace NicamNew.msn.marketing.actions
{
    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/ProfileMultiplier" when creating ProfileMultiplier class. Fix Title field.

    public class ProfileMultiplier<T> : RuleAction<T> where T : ConditionalRenderingsRuleContext
    {
        public Guid ProfileKeyId { get; set; }
        public String Multiplier { get; set; }

        public override void Apply(T ruleContext)
        {
            ProfileKeyDefinition pkd =
                AnalyticsManager.GetProfileKeyDefinitionById(
                    new Guid(Context.Database.GetItem(ProfileKeyId.ToString()).ID.ToString()));

            ProfileDefinition pd = AnalyticsManager.GetProfileDefinitionById(pkd.ProfileDefinitionId);


            if (pd != null)
            {
                ProfileKeyData pkItem = AnalyticsTracker.Current.Data.Profiles.GetProfile(pd.Name).GetProfileKey(pkd.Name);
                if (pkItem != null)
                    AnalyticsManager.UpdateProfileKey(pkItem.ProfileKeyId, pkItem.Value*Int32.Parse(Multiplier));
            }
        }
    }
}