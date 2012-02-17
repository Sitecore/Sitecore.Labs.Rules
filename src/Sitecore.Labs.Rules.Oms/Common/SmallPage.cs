using System;

namespace Sitecore.Labs.Rules.Oms.Common
{
    class SmallPage
    {
        public string Id { get; set; }
        public int Count { get; set; }
    }

    class AnotherPage
    {
        public string Id { get; set; }
        public Guid GoalId { get; set; }
    }
}