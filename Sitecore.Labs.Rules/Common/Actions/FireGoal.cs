using Sitecore;
using Sitecore.Analytics;
using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;

namespace NicamNew.msn.marketing.actions
{
    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/FireGoal" when creating FireGoal class. Fix Title field.

    public class FireGoal<T> : RuleAction<T> where T : ConditionalRenderingsRuleContext
    {
        public string Goal { get; set; }

        public override void Apply(T ruleContext)
        {
            if (Goal != null)
                AnalyticsTracker.Current.CurrentPage.TriggerEvent(Context.Database.GetItem(Goal).Name, "This is text");
        }
    }
}