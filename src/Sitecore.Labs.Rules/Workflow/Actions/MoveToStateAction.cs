using Sitecore.Rules.Actions;
using Sitecore.Diagnostics;

namespace Sitecore.Labs.Rules.Workflow.Actions
{
    public class MoveToStateAction<T> : RuleAction<T> where T : WorkflowRuleContext
    {
        public Data.ID StateId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            if (Data.ID.IsNullOrEmpty(StateId)) return;

            ruleContext.NextStateId = StateId;
        }
    }
}
