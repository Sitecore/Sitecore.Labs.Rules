using System.Globalization;
using System.Web;
using Sitecore.Analytics.Rules.Conditions;
using Sitecore.Data.Items;

namespace Sitecore.Labs.Rules.Oms.Tests.Strategy
{
    public class MvIterate : IMultiVariateTestStrategy
    {
        const string CookieName = "aegeagaegaeWeafea2";

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

                if (HttpContext.Current.Request.Cookies[CookieName] != null)
                {
                    if (HttpContext.Current != null)
                    {
                        var httpCookie = HttpContext.Current.Request.Cookies[CookieName];

                        if (httpCookie != null)
                        {
                            int.TryParse(httpCookie.Value, out i);
                        }
                    }
                }

                return i;
            }
            set
            {
                HttpContext.Current.Response.Cookies.Add(new HttpCookie(CookieName, value.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}