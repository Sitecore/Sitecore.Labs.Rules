using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NicamNew.msn.marketing.conditions
{
    class SmallPage
    {

        public String Id { get; set; }
        public int Count { get; set; }
    }

    class AnotherPage
    {

        public String Id { get; set; }
        public Guid GoalId { get; set; }
    }
}
