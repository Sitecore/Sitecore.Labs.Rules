using Sitecore.Rules;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Workflows.Simple;

namespace Sitecore.Labs.Rules.Workflow
{
    public class WorkflowRuleContext : RuleContext
    {
        public WorkflowRuleContext(WorkflowPipelineArgs args)
        {
            WorkflowArgs = args;
            Item = args.DataItem;
        }

        private  WorkflowPipelineArgs WorkflowArgs { get; set; }
        public Item ActionItem
        {
            get { return WorkflowArgs.ProcessorItem.InnerItem; }
        }
        public string Comments
        {
            get { return WorkflowArgs.Comments; }
        }

        public ID NextStateID
        {
            get { return WorkflowArgs.NextStateId; }
            set { WorkflowArgs.NextStateId = value; }

        }
        public void Cancel()
        {
            WorkflowArgs.AbortPipeline();
        }
    }
}
