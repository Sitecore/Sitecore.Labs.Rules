using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace Sitecore.Labs.Rules.Common.Conditions
{
    public class IsAlias<T> : WhenCondition<T> where T : ConditionalRenderingsRuleContext
    {
        // Methods 
        protected override bool Execute(T ruleContext)
        {
            return (Context.Database.Aliases.Exists(Context.RawUrl.TrimStart(new[] {'/'})));
        }
    }
}