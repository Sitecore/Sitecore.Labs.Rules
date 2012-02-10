using Sitecore.Rules.Conditions;
using Sitecore.Diagnostics;

namespace Sitecore.Labs.Rules.Workflow
{
    public class CommentCondition<T> : StringOperatorCondition<T> where T : WorkflowRuleContext
    {
        public string Value { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            var comment = ruleContext.Comments ?? string.Empty;
            return Compare(comment, Value);
        }
    }
}
