using Sitecore.Rules.Actions;

namespace Sitecore.Labs.Rules.Workflow.Actions
{
    public class CancelAction<T> : RuleAction<T> where T : WorkflowRuleContext
    {
        public override void Apply(T ruleContext)
        {
            ruleContext.Cancel();
        }
    }
}
