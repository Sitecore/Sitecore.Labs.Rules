using Sitecore;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace NicamNew.msn.marketing.conditions
{
    public class AliasDetection<T> : WhenCondition<T> where T : ConditionalRenderingsRuleContext
    {
        // Methods 
        protected override bool Execute(T ruleContext)
        {
            return (Context.Database.Aliases.Exists(Context.RawUrl.TrimStart(new[] {'/'})));
        }
    }
}