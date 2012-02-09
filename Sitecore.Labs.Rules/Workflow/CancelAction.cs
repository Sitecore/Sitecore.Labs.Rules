using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Rules.Actions;

namespace Sitecore.Labs.Rules.Workflow
{
    public class CancelAction<T> : RuleAction<T> where T : WorkflowRuleContext
    {
        public override void Apply(T ruleContext)
        {
            ruleContext.Cancel();
        }
    }
}
