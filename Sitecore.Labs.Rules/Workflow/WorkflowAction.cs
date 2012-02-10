using Sitecore.Workflows.Simple;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Data;

namespace Sitecore.Labs.Rules.Workflow
{
    class WorkflowAction
    {
        public void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.DataItem == null) return;

            var context = new WorkflowRuleContext(args);

            var actionItem = args.ProcessorItem.InnerItem;
            if (actionItem == null) return;

            if (actionItem["execute global rules"] == "1")
            {
                RunGlobalRules(args.DataItem.Database, context);
            }

            var rules = RuleFactory.GetRules<WorkflowRuleContext>(actionItem.Fields["rules"]);
           
                rules.Run(context);
        }

        private static void RunGlobalRules(Database database, WorkflowRuleContext context)
        {
            var rulesFolder = database.GetItem("/sitecore/system/Settings/Rules/Workflow/Rules");
            if (rulesFolder == null) return;
            var rules = RuleFactory.GetRules<WorkflowRuleContext>(rulesFolder, "rules");

            rules.Run(context);
        }
    }
}
