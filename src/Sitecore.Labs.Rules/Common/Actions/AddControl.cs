using System;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;

namespace Sitecore.Labs.Rules.Common.Actions
{
    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Actions/Add Control" when creating Add_Control class. Fix Title field.

    public class AddControl<T> : RuleAction<T> where T : ConditionalRenderingsRuleContext
    {
        public string Placeholder { get; set; }
        public string RenderingId { get; set; }

        public override void Apply(T ruleContext)
        {
            if (RenderingId != null)
            {
                Assert.IsNotNullOrEmpty(RenderingId, "RenderingID");
                if (RenderingId != null)
                {
                    var rendering = Context.Database.GetItem(RenderingId);
                    if (rendering != null)
                    {
                        Assert.IsNotNull(rendering, "rendering");

                        if (rendering != null)
                        {
                            var rendRef = new RenderingReference(rendering);

                            if (!String.IsNullOrEmpty(Placeholder) && String.Compare(Placeholder, "specific", StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                rendRef.Placeholder = Placeholder;
                            }

                            if (ruleContext != null)
                            {
                                ruleContext.References.Add(rendRef);
                            }
                        }
                    }
                }
            }
        }
    }
}