using System;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;

namespace NicamNew.msn.marketing.conditions
{
    public class DateRangeCondition<T> : OperatorCondition<T> where T : ConditionalRenderingsRuleContext
    {
        // Methods 
  
        public String valueDate { get; set; }

        protected override bool Execute(T ruleContext)
        {

            DateTime StartDate = DateTime.Parse(valueDate);

          
            switch (base.GetOperator())
            {
                case ConditionOperator.Equal:
                    return (StartDate == DateTime.Now);

                case ConditionOperator.GreaterThanOrEqual:
                    return (StartDate >= DateTime.Now);

                case ConditionOperator.GreaterThan:
                    return (StartDate > DateTime.Now);

                case ConditionOperator.LessThanOrEqual:
                    return (StartDate <= DateTime.Now);

                case ConditionOperator.LessThan:
                    return (StartDate < DateTime.Now);

                case ConditionOperator.NotEqual:
                    return (StartDate != DateTime.Now);

            }

            return false;
           
        }
    }
}