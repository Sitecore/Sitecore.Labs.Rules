using System;
using System.Globalization;
using System.Web;
using Sitecore.Extensions.StringExtensions;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.Labs.Rules.Oms.Common.Conditions
{
    // TODO : Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Conditions/ReferrerDetection" when creating ReferrerDetection class. Fix Text field.

    public class ReferrerDetection<T> : WhenCondition<T> where T : RuleContext
    {
        public string ReferrerItemId { get; set; }

        private static string Referrer
        {
            get
            {
                if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    return HttpContext.Current.Request.UrlReferrer.AbsoluteUri.ToString(CultureInfo.InvariantCulture);
                }

                return "No Referrer Found";
            }
        }

        private static Uri ReferrerUri
        {
            get
            {
                return HttpContext.Current.Request.UrlReferrer ?? new Uri("http://www.sitecore.net/");
            }
        }

        private string ReferrerDomain
        {
            get
            {
                if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    return HttpContext.Current.Request.UrlReferrer.Host.ToString(CultureInfo.InvariantCulture);
                }

                return "No Referrer Found";
            }
        }

        protected override bool Execute(T ruleContext)
        {
            string query = Referrer;

            if (!string.IsNullOrEmpty(query))
            {
                if (query.StartsWith("?"))
                {
                    query = query.Mid(1);
                }

                int index = query.IndexOf("#", StringComparison.Ordinal);

                if (index >= 0)
                {
                    query = query.Left(index);
                }

                var strArray = query.Split(new char[] { '&' });

                // TODO: MAYBE IMPORTANT TO GET THE QUERY STRINGS AS WELL
                //if (strArray.Length != 0)
                //{
                //    foreach (string str3 in strArray)
                //    {
                //        int length = str3.IndexOf('=');
                //        if ((length >= 0) && !(str3.Left(length) != this.QueryString))
                //        {
                //            string str5 = HttpUtility.UrlDecode(str3.Mid(length + 1)).Trim();
                //            if (!string.IsNullOrEmpty(str5))
                //            {
                //                args.Visit.Keywords = Tracker.Visitor.DataContext.GetKeywords(str5);
                //            }
                //            break;
                //        }
                //    }
                //}


                var itm = Context.Database.GetItem(ReferrerItemId);

                if (itm != null)
                {
                    if (itm.Fields["Domain Urls"].Value.Contains(ReferrerDomain))
                    {
                        return true;
                    }

                    foreach (string s in ReferrerUri.PathAndQuery.Split(new char[] { '/' }))
                    {
                        if (!string.IsNullOrEmpty(s) && itm.Fields["Domain Urls"].Value.Contains(s))
                        {
                            return true;

                        }
                    }
                }
            }

            return false;
        }
    }
}