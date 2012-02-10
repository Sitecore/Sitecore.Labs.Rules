using System;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.Data.Items;

namespace Sitecore.Labs.Rules.Common
{
    public class CreateFolderStructureAction<T> : RuleAction<T> where T : RuleContext
    {
        private string MonthFormat = "MM";
        private string YearFormat = "yyyy";
        private string FolderTemplateID = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}";
        private string DayFormat;
        
        public ID Parent { get; set; }
        public string Field { get; set; }


        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "rulecontext");

            var item = ruleContext.Item;
            if (item == null) return;

            var parentFolder = item.Database.GetItem(Parent);
            if (parentFolder == null) return;


            ReadConfiguration(parentFolder);

            var dateTime = DateTime.Now;

            DateField dField = item.Fields[Field];

            if (dField != null && !string.IsNullOrEmpty(dField.Value))
            {
                dateTime = dField.DateTime;
            }

            var year = GetChildOrCreate(parentFolder, dateTime.ToString(YearFormat));

            var destination = GetChildOrCreate(year, dateTime.ToString(MonthFormat));

            if(!string.IsNullOrEmpty(DayFormat))
            {
                destination = GetChildOrCreate(destination, dateTime.ToString(DayFormat));
            }


            if (item.Parent.ID != destination.ID)
            {
                var oldparent = item.Parent;
                item.MoveTo(destination);
                DeleteFolder(oldparent);
            }
        }

        private void ReadConfiguration(Item configItem)
        {
            YearFormat = StringUtil.GetString(configItem["year format"], defaultValue: YearFormat);
            MonthFormat = StringUtil.GetString(configItem["month format"], defaultValue: MonthFormat);
            FolderTemplateID = StringUtil.GetString(configItem["template for folders"], defaultValue: FolderTemplateID);
            DayFormat = configItem["day format"];
        }

        private void DeleteFolder(Item item)
        {
            if (item.Template.ID.ToString() == FolderTemplateID && !item.HasChildren)
            {
                var parent = item.Parent;
                item.Delete();
                DeleteFolder(parent);
            }
        }

        private Item GetChildOrCreate(Item parent, string name)
        {
            var item = parent.GetChildren()[name];
            return item ?? parent.Add(name, new TemplateID(new ID(FolderTemplateID)));
        }
    }
}
