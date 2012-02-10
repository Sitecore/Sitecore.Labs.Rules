using Sitecore.Rules.Actions;
using Sitecore.Diagnostics;

namespace Sitecore.Labs.Rules.Workflow
{
    public class MoveToStateAction<T> : RuleAction<T> where T : WorkflowRuleContext
    {
        public Data.ID StateID { get; set; }
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            if (Data.ID.IsNullOrEmpty(StateID)) return;
            ruleContext.NextStateID = StateID;
        }
    }
}
