using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;

namespace Sitecore.Labs.Rules.Common.Actions
{
    public abstract class SetParameterTemplateValue<T> : RuleAction<T> where T : ConditionalRenderingsRuleContext
    {
        protected void Apply(T ruleContext, string paramter, string value)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNull(paramter, "paramter");

            RenderingSettings settings = ruleContext.Reference.Settings;
            RenderingCaching caching = settings.Caching;

            if (caching != null)
            {
                if ((caching.Cacheable && !caching.VaryByData) && (settings.DataSource != paramter))
                {
                    string name;
                    var renderingItem = ruleContext.Reference.RenderingItem;

                    if (renderingItem != null)
                    {
                        name = renderingItem.Name;
                    }
                    else
                    {
                        var renderingId = ruleContext.Reference.RenderingID;
                        {
                            name = renderingId.ToString();
                        }
                    }

                    Log.Warn(string.Format("A rule overwrites the data source of the item {0} which is cachable and not vary by data. Possibly, this value is not be used as the rendering is cached.",name), GetType());
                }
            }
        }
    }
}