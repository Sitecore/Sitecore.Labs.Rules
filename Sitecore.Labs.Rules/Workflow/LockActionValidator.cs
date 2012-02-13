using System.Linq;
using Sitecore.Data.Validators;
using Sitecore.Labs.Rules.Common.Actions;
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

            if (field.Item.Parent.Template.Key != "command") return ValidatorResult.Valid;

            var rules = RuleFactory.GetRules<WorkflowRuleContext>(field);
            if (rules.Count == 0) return ValidatorResult.Valid;
            var lockItem =
                rules.Rules.Any(r => r.Actions
                    .Any(a => a.GetType() ==
                        typeof(LockItemAction<WorkflowRuleContext>)));
            if (lockItem)
            {
                Text = GetText("The lock action will not work under a workflow command, place it under a workflow state instead");
                return GetMaxValidatorResult();
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
