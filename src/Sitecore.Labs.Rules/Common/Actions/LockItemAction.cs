using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.Diagnostics;
using Sitecore.Security.Accounts;
using Sitecore.Data.Items;

namespace Sitecore.Labs.Rules.Common.Actions
{
    public class LockItemAction<T> : RuleAction<T> where T : RuleContext
    {
        public string UserName { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            if (ruleContext.Item == null)
            {
                Log.Error("LockItemAction: Item is empty", this);
                return;
            }
            if (string.IsNullOrEmpty(UserName))
            {
                Log.Error("LockItemAction: UserName cannot be empty", this);
                return;
            }
            
            if (!User.Exists(UserName))
            {
                UserName = "sitecore\\" + UserName;
                if (!User.Exists(UserName))
                {
                    Log.Error("LockItemAction: Could not find user " + UserName, this);
                    return;
                }
            }
            
            using (new UserSwitcher(UserName, false))
            {
                if (ruleContext.Item.Locking.CanLock())
                {
                    using (new EditContext(ruleContext.Item))
                    {
                        ruleContext.Item.Locking.Lock();
                    }
                   
                }
                else
                {
                    Log.Warn(string.Format("LockItemAction: user {0} cannot lock item {1}", UserName, ruleContext.Item.Paths.FullPath), this);
                }
            }

        }
    }
}
