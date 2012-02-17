using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Analytics;

namespace Sitecore.Labs.Rules.Oms.Common.Actions
{ 
    // TODO : Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/FireGoal" when creating FireGoal class. Fix Title field.

    public class FireGoal<T> : RuleAction<T> where T : ConditionalRenderingsRuleContext
    {
        public string Goal { get; set; }

        public override void Apply(T ruleContext)
        {
            if (Goal != null)
            {
                //Tracker.CurrentPage < DMS ?
                AnalyticsTracker.Current.CurrentPage.TriggerEvent(Context.Database.GetItem(Goal).Name, "This is text");
            }
        }
    }
}