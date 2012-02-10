using Sitecore.Rules.Actions;
using Sitecore.Rules.InsertOptions;

namespace Sitecore.Labs.Rules.InsertOptions
{
    public class ClearInsertOptionsAction<T> : RuleAction<T> where T: InsertOptionsRuleContext
    {
        public override void Apply(T ruleContext)
        {
            ruleContext.InsertOptions.Clear();
        }
    }
}
