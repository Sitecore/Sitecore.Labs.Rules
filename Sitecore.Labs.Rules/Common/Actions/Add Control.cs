using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;

namespace NicamNew.msn.marketing.actions
{
    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/Add Control" when creating Add_Control class. Fix Title field.

    public class Add_Control<T> : RuleAction<T> where T : ConditionalRenderingsRuleContext
    {
        public string Placeholder { get; set; }
        public string RenderingID { get; set; }

        public override void Apply(T ruleContext)
        {
            if (RenderingID != null)
            {
                Assert.IsNotNullOrEmpty(RenderingID, "RenderingID");
                if (RenderingID != null)
                {
                    Item rendering = Context.Database.GetItem(RenderingID);
                    if (rendering != null)
                    {
                        Assert.IsNotNull(rendering, "rendering");
                        if (rendering != null)
                        {
                            var rendRef = new RenderingReference(rendering);

                            if (!String.IsNullOrEmpty(Placeholder) && String.Compare(Placeholder, "specific", true) != 0)
                            {
                                rendRef.Placeholder = Placeholder;
                            }
                            if (ruleContext != null) ruleContext.References.Add(rendRef);
                        }
                    }
                }
            }
        }
    }
}