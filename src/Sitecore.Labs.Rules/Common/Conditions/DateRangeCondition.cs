using System;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace Sitecore.Labs.Rules.Common.Conditions
{
    public class DateRangeCondition<T> : OperatorCondition<T> where T : ConditionalRenderingsRuleContext
    { 
        public string ValueDate { get; set; }

        protected override bool Execute(T ruleContext)
        {
            var startDate = DateTime.Parse(ValueDate);
     
            switch (GetOperator())
            {
                case ConditionOperator.Equal:
                    return (startDate == DateTime.Now);

                case ConditionOperator.GreaterThanOrEqual:
                    return (startDate >= DateTime.Now);

                case ConditionOperator.GreaterThan:
                    return (startDate > DateTime.Now);

                case ConditionOperator.LessThanOrEqual:
                    return (startDate <= DateTime.Now);

                case ConditionOperator.LessThan:
                    return (startDate < DateTime.Now);

                case ConditionOperator.NotEqual:
                    return (startDate != DateTime.Now);

            }

            return false;
           
        }
    }
}