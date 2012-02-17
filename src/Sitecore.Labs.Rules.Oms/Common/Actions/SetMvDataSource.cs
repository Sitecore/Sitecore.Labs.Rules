using System;
using System.Collections.Generic;
using Sitecore.Analytics;
using Sitecore.Analytics.Rules.Conditions;
using Sitecore.CodeDom.Scripts;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;
using Sitecore.Data.Fields;

namespace Sitecore.Labs.Rules.Oms.Common.Actions
{
    // TODO : Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/SetMVDataSource" when creating SetMVDataSource class. Fix Title field.

    public class SetMvDataSource<T> : SetDataSource<T>, IMultiVariateTestStrategy where T : ConditionalRenderingsRuleContext
    {
        // Fields
        private string _dataSource;

        private SmallPage ReadPage(DataProviderReader reader)
        {
            Assert.ArgumentNotNull(reader, "reader");

            var page = new SmallPage
                           {
                               Id = AnalyticsManager.Provider.GetString(0, reader),
                               Count = AnalyticsManager.Provider.GetInt(1, reader)
                           };

            return page;
        }

        // Methods
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            using (new SecurityDisabler())
            {
                string multiVariateTest = ruleContext.Reference.Settings.MultiVariateTest;

                if (string.IsNullOrEmpty(multiVariateTest)) return;

                var item = ruleContext.Item;

                if (item == null) return;

                var item2 = item.Database.GetItem(multiVariateTest);

                if ((item2 == null) || !item2.HasChildren) return;

                Item currentMostPopularVariant = null;
                int currentLargestCount = 0;

                foreach (Item mvItem in item2.Children)
                {
                    ReferenceField rf = mvItem.Fields["Goal"];

                    if (rf == null) continue;

                    if (rf.TargetItem != null)
                    {
                        List<String> pageEvents =
                            AnalyticsManager.ReadMany(
                                "\r\nSELECT {0}Name{1}\r\nFROM {0}Page{1}\r\n inner join {0}PageEvent{1} on \r\n{0}PageEvent{1}.{0}PageId{1} = {0}Page{1}.{0}PageId{1}\r\n inner join {0}PageEventDefinition{1} on \r\n{0}PageEventDefinition{1}.{0}PageEventDefinitionId{1} = {0}PageEvent{1}.{0}PageEventDefinitionId{1}WHERE {0}PageEventDefinition{1}.{0}Name{1} = {2}pageEvent{3}",
                                reader => AnalyticsManager.Provider.GetString(0, reader),
                                new object[] { "pageEvent", rf.TargetItem.Name });

                        if (currentMostPopularVariant == null)
                        {
                            currentMostPopularVariant = mvItem;
                            if (pageEvents != null) currentLargestCount = pageEvents.Count;
                        }

                        if (pageEvents != null)
                        {
                            if (pageEvents.Count > currentLargestCount)
                            {
                                currentLargestCount = pageEvents.Count;
                                currentMostPopularVariant = mvItem;
                            }
                        }
                    }
                }

                if (currentMostPopularVariant != null)
                {
                    string dataSource = currentMostPopularVariant["Data Source"];
                    base.Apply(ruleContext, dataSource);

                    ruleContext.Reference.Settings.DataSource = dataSource;
                    ruleContext.Reference.Settings.MultiVariateTest = string.Empty;

                    Tracer.Info("Multi Variate Test has permanently changed from \"\" to \"" + dataSource + "\".");
                }
            }
        }

        // Properties
        public string DataSource
        {
            get
            {
                return (this._dataSource ?? string.Empty);
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this._dataSource = value;
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

            var strategy = ItemScripts.CreateObject(item) as IMultiVariateTestStrategy;
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
