using Sitecore.Analytics.Rules.Conditions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Labs.Rules.Oms.Tests.Strategy
{
    public class SmartMultiVariantStrategy : IMultiVariateTestStrategy
    {
        #region IMultiVariateTestStrategy Members

        public Item GetTestVariableItem(Item item, Item multiVariateTest)
        {
            //Run until goal fired.
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(multiVariateTest, "multiVariateTest");

            if (!multiVariateTest.HasChildren)
            {
                return null;
            }

            return multiVariateTest.Children[0];
        }

        #endregion
    }
}