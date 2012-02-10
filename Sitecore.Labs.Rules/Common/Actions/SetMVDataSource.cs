using System;
using System.Collections.Generic;
using NicamNew.msn.marketing.conditions;
using Sitecore.Analytics;
using Sitecore.Analytics.Rules.Conditions;
using Sitecore.CodeDom.Scripts;
using Sitecore.Data;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;

namespace NicamNew.msn.marketing.actions
{
    using Sitecore.Rules;
    using Sitecore.Rules.Actions;
    using Sitecore.Data.Fields;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/SetMVDataSource" when creating SetMVDataSource class. Fix Title field.

    public class SetMVDataSource<T> : SetDataSource<T>, IMultiVariateTestStrategy where T : ConditionalRenderingsRuleContext
    {
        // Fields
        private string dataSource;


        private SmallPage ReadPage(DataProviderReader reader)
        {
            Assert.ArgumentNotNull(reader, "reader");
            var page = new SmallPage();
            page.Id = AnalyticsManager.Provider.GetString(0, reader);
            page.Count = AnalyticsManager.Provider.GetInt(1, reader);

            return page;
        }

        // Methods
        public override void Apply(T ruleContext)
        {




            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            using (new SecurityDisabler())
            {
                string multiVariateTest = ruleContext.Reference.Settings.MultiVariateTest;
                if (!string.IsNullOrEmpty(multiVariateTest))
                {
                    Item item = ruleContext.Item;
                    if (item != null)
                    {
                        Item item2 = item.Database.GetItem(multiVariateTest);
                        if ((item2 != null) && item2.HasChildren)
                        {
                            //IMultiVariateTestStrategy strategy = this.GetStrategy(item2);
                            //Assert.IsNotNull(strategy, "strategy is null");
                            Item currentMostPopularVariant = null;
                            int currentLargestCount = 0;
                            foreach (Item mvItem in item2.Children)
                            {
                                ReferenceField rf = mvItem.Fields["Goal"];

                                if (rf != null)
                                {
                                    if (rf.TargetItem != null)
                                    {
                                        List<String> PageEvents =
                                            AnalyticsManager.ReadMany(
                                                "\r\nSELECT {0}Name{1}\r\nFROM {0}Page{1}\r\n inner join {0}PageEvent{1} on \r\n{0}PageEvent{1}.{0}PageId{1} = {0}Page{1}.{0}PageId{1}\r\n inner join {0}PageEventDefinition{1} on \r\n{0}PageEventDefinition{1}.{0}PageEventDefinitionId{1} = {0}PageEvent{1}.{0}PageEventDefinitionId{1}WHERE {0}PageEventDefinition{1}.{0}Name{1} = {2}pageEvent{3}",
                                                delegate(DataProviderReader reader) { return AnalyticsManager.Provider.GetString(0, reader); },
                                                new object[] { "pageEvent", rf.TargetItem.Name });
                                        if (currentMostPopularVariant == null)
                                        {
                                            currentMostPopularVariant = mvItem;
                                            if (PageEvents != null) currentLargestCount = PageEvents.Count;
                                        }
                                        // Item testVariableItem = strategy.GetTestVariableItem(item, item2);
                                        // if (testVariableItem != null)
                                        // {

                                        if (PageEvents != null)
                                            if (PageEvents.Count > currentLargestCount)
                                            {

                                                currentLargestCount = PageEvents.Count;
                                                currentMostPopularVariant = mvItem;
                                            }
                                    }
                                }


                                //   }
                            }

                            if (currentMostPopularVariant != null)
                            {
                                string dataSource = currentMostPopularVariant["Data Source"];
                                base.Apply(ruleContext, dataSource);
                                ruleContext.Reference.Settings.DataSource = dataSource;
                                ruleContext.Reference.Settings.MultiVariateTest =
                                    "";
                                Tracer.Info("Multi Variate Test has permanently changed from \"\" to \"" +
                                            dataSource + "\".");
                            }
                        }
                    }
                }
            }
        }

        // Properties
        public string DataSource
        {
            get
            {
                return (this.dataSource ?? string.Empty);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.dataSource = value;
            }
        }

        #region IMultiVariateTestStrategy Members

        private IMultiVariateTestStrategy GetStrategy(Item multiVariateTest)
        {
            Assert.ArgumentNotNull(multiVariateTest, "multiVariateTest");
            string str = multiVariateTest["Test Strategy"];
            if (string.IsNullOrEmpty(str))
            {
                return this;
            }
            Item item = multiVariateTest.Database.GetItem(str);
            if (item == null)
            {
                Log.Error("Multivariate Test Strategy with id {0} not found. Using Random strategy instead.".FormatWith(new object[] { str }), base.GetType());
                return this;
            }
            if (!ItemScripts.HasScript(item))
            {
                return this;
            }
            IMultiVariateTestStrategy strategy = ItemScripts.CreateObject(item) as IMultiVariateTestStrategy;
            if (strategy == null)
            {
                Log.Error("Multivariate Test Strategy with id {0} is invalid (returns null). Using Random strategy instead.".FormatWith(new object[] { str }), base.GetType());
                strategy = this;
            }
            return strategy;
        }

        public Item GetTestVariableItem(Item item, Item multiVariateTest)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(multiVariateTest, "multiVariateTest");
            if (!multiVariateTest.HasChildren)
            {
                return null;
            }
            int num = new Random().Next(multiVariateTest.Children.Count);
            return multiVariateTest.Children[num];
        }

        #endregion
    }
}
