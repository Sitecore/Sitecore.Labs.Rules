using Sitecore.Analytics.Rules.Conditions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace NicamNew.msn.marketing
{
    public class SmartMultiVariantStatergy : IMultiVariateTestStrategy
    {
        #region IMultiVariateTestStrategy Members

        Item IMultiVariateTestStrategy.GetTestVariableItem(Item item, Item multiVariateTest)
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