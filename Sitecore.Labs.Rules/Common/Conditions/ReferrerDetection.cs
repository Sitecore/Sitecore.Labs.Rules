using System;
using System.Collections.Generic;
using System.Web;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.Items;
using Sitecore.Extensions.StringExtensions;

namespace NicamNew.msn.marketing.conditions
{
    using Sitecore.Rules;
    using Sitecore.Rules.Conditions;

    // TODO: Created Sitecore Item "/sitecore/system/Settings/Rules/Common/Conditions/ReferrerDetection" when creating ReferrerDetection class. Fix Text field.

    public class ReferrerDetection<T> : WhenCondition<T> where T : RuleContext
    {
        public String ReferrerItemId { get; set; }

        private static String Referrer
        {
            get {
                return HttpContext.Current.Request.UrlReferrer != null
                           ? HttpContext.Current.Request.UrlReferrer.AbsoluteUri.ToString()
                           : "No Referrer Found";
            }
        }

        private static Uri ReferrerUrl
        {
            get {
                return HttpContext.Current.Request.UrlReferrer ?? new Uri("http://www.sitecore");
            }
        }

        private String ReferrerDomain
        {
            get {
                return HttpContext.Current.Request.UrlReferrer != null
                           ? HttpContext.Current.Request.UrlReferrer.Host.ToString()
                           : "No Referrer Found";
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
                int index = query.IndexOf("#");
                if (index >= 0)
                {
                    query = query.Left(index);
                }
                string[] strArray = query.Split(new char[] { '&' });

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


                Item i = Sitecore.Context.Database.GetItem(ReferrerItemId);

                if (i != null)
                {
                    if (i.Fields["Domain Urls"].Value.Contains(ReferrerDomain))
                    {
                        return true;
                    }

               

                    foreach (String s in ReferrerUrl.PathAndQuery.Split(new char[] {'/'}))
                    {
                        if (!String.IsNullOrEmpty(s) && i.Fields["Domain Urls"].Value.Contains(s))
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