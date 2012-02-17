using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.Labs.Rules.Common.Actions
{
    public class DeviceDetection<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public string Value { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            
            string str = Value;
            
            if (str == null)
            {
                return false;
            }

            var site = Context.Site;
            
            if (site == null)
            {
                return false;
            }
            
            string device = Context.Device.Agent;
            
            if (device == null)
            {
                return false;
            }
            
            return base.Compare(device, str);
        }
    }
}