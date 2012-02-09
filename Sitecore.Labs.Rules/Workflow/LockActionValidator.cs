using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Validators;
using Sitecore.Rules;

namespace Sitecore.Labs.Rules.Workflow
{
    public class LockActionValidator : StandardValidator
    {

        protected override ValidatorResult Evaluate()
        {
            var field = this.GetField();
            if (field == null)
                return ValidatorResult.Valid;
            if (field.TypeKey != "rules") return ValidatorResult.Valid;

            var rules = RuleFactory.GetRules<WorkflowRuleContext>(field);

            var lockItem =
                rules.Rules.Any(r => r.Actions
                    .Any(a => a.GetType() ==
                        typeof(Sitecore.Labs.Rules.Common.LockItemAction<WorkflowRuleContext>)));
            if (lockItem)
            {
                if (field.Item.Parent.Template.Key == "command")
                {
                    this.Text = GetText("The lock action will not work under a workflow command, place it under a workflow state instead");
                    return GetMaxValidatorResult();
                }
            }
            return ValidatorResult.Valid;
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return this.GetFailedResult(ValidatorResult.Error);
        }

        public override string Name
        {
            get { return "Lock Action under command"; }
        }
    }
}
