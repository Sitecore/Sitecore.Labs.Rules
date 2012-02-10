using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Analytics.Rules.Conditions;
using Sitecore.Data.Items;

namespace NicamNew.msn.marketing
{
    public class TimedMVTest : IMultiVariateTestStrategy
    {
        const string _cookieName = "aegeagaegaeWeafea2";

        #region IMultiVariateTestStrategy Members

        public Item GetTestVariableItem(Item item, Item multiVariateTest)
        {
            if (!multiVariateTest.HasChildren) return null;
            int index = Cookie == -1 ? 0 : Cookie;


            if (index >= multiVariateTest.Children.Count) index = 0;

            Cookie = index + 1;
            return multiVariateTest.Children[index];
        }

        #endregion

        private int Cookie
        {
            get
            {
                int i = -1;

                if (HttpContext.Current.Request.Cookies[_cookieName] != null)
                    int.TryParse(HttpContext.Current.Request.Cookies[_cookieName].Value, out i);

                return i;
            }
            set
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(_cookieName, value.ToString()));
            }
        }
    }
}