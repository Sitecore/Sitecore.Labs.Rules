using System;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.Data.Items;

namespace Sitecore.Labs.Rules.Common.Actions
{
    public class CreateFolderStructureAction<T> : RuleAction<T> where T : RuleContext
    {
        private string _monthFormat = "MM";
        private string _yearFormat = "yyyy";
        private string _folderTemplateId = "{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}";
        private string _dayFormat;
        
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

            var year = GetChildOrCreate(parentFolder, dateTime.ToString(_yearFormat));

            var destination = GetChildOrCreate(year, dateTime.ToString(_monthFormat));

            if(!string.IsNullOrEmpty(_dayFormat))
            {
                destination = GetChildOrCreate(destination, dateTime.ToString(_dayFormat));
            }

            if (item.Parent.ID == destination.ID) return;

            var oldparent = item.Parent;
            item.MoveTo(destination);
            DeleteFolder(oldparent);
        }

        private void ReadConfiguration(Item configItem)
        {
            _yearFormat = StringUtil.GetString(configItem["year format"], defaultValue: _yearFormat);
            _monthFormat = StringUtil.GetString(configItem["month format"], defaultValue: _monthFormat);
            _folderTemplateId = StringUtil.GetString(configItem["template for folders"], defaultValue: _folderTemplateId);
            _dayFormat = configItem["day format"];
        }

        private void DeleteFolder(Item item)
        {
            if (item.Template.ID.ToString() != _folderTemplateId || item.HasChildren) return;

            var parent = item.Parent;
            item.Delete();
            DeleteFolder(parent);
        }

        private Item GetChildOrCreate(Item parent, string name)
        {
            var item = parent.GetChildren()[name];
            return item ?? parent.Add(name, new TemplateID(new ID(_folderTemplateId)));
        }
    }
}
