using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Rules;
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
