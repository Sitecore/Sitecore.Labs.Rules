using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace NicamNew.msn.marketing.conditions
{
    public class TimedProfileScores<T> : OperatorCondition<T> where T : RuleContext
    {
        // Fields
        private string profileKeyId;
        private string value;

        // Methods
        public TimedProfileScores()
        {
            profileKeyId = string.Empty;
            value = string.Empty;
        }

        public string ProfileKeyId
        {
            get { return (profileKeyId ?? string.Empty); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                profileKeyId = value;
            }
        }

        public string Value
        {
            get { return (value ?? string.Empty); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.value = value;
            }
        }

        protected override bool Execute(T ruleContext)
        {
            double num2;
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            double profileKeyValue = GetProfileKeyValue();
            if (double.TryParse(Value, out num2))
            {
                switch (base.GetOperator())
                {
                    case ConditionOperator.Equal:
                        return (profileKeyValue == num2);

                    case ConditionOperator.GreaterThanOrEqual:
                        return (profileKeyValue >= num2);

                    case ConditionOperator.GreaterThan:
                        return (profileKeyValue > num2);

                    case ConditionOperator.LessThanOrEqual:
                        return (profileKeyValue <= num2);

                    case ConditionOperator.LessThan:
                        return (profileKeyValue < num2);

                    case ConditionOperator.NotEqual:
                        return (profileKeyValue != num2);
                }
            }
            return false;
        }

        private static ProfileKeyResult GetProfileKeyResult(Guid sessionId, Guid profileKeyDefinitionId)
        {
            var pk = new ProfileKeyResult();


            List<Profile> GetList =
                AnalyticsManager.GetProfilesBySessionId(sessionId).Where(
                    i => AnalyticsManager.GetSessionBySessionId(i.SessionId).Timestamp > DateTime.Now.AddDays(-1)).
                    ToList();


            //List<Profile> GetExternalUserList = Sitecore.Analytics.AnalyticsManager.Get.Where(i => Sitecore.Analytics.AnalyticsManager.GetSessionBySessionId(i.SessionId).Timestamp > DateTime.Now.AddDays(-1)).ToList();

            //int Total =  GetList.Sum(i => i.Total);

            //pk.total = Total;
            //pk.profileId = GetList[0].ProfileId;
            //pk.value = GetList[0].Total;
            //pk.profileType = Sitecore.Analytics.AnalyticsManager.GetProfileDefinitionById(GetList[0].ProfileDefinitionId).ProfileType;
            //pk.count = GetList[0].Count;
            //pk.profileKeyId = Sitecore.Analytics.AnalyticsManager.GetProfileKeysByProfileId(GetList[0].ProfileId)[0].ProfileKeyId;


            if (Context.User.Identity.Name != "extranet\\Anonymous")
            {
                pk =
                    AnalyticsManager.ReadOne(
                        "select {0}ProfileKey{1}.{0}ProfileKeyId{1}, {0}ProfileKey{1}.{0}ProfileId{1}, {0}ProfileKey{1}.{0}Value{1}, \r\n{0}Profile{1}.{0}Count{1}, {0}Profile{1}.{0}Total{1}, {0}ProfileDefinition{1}.{0}ProfileType{1}\r\nfrom {0}ProfileKey{1}, {0}Profile{1}, {0}ProfileDefinition{1}, {0}ProfileKeyDefinition{1}, {0}Session{1}, {0}GlobalSession{1} \r\nwhere {0}ProfileKeyDefinitionId{1} = {2}profileKeyDefinitionId{3}\r\nand {0}ProfileKey{1}.{0}ProfileId{1} = {0}Profile{1}.{0}ProfileId{1}\r\nand {0}Profile{1}.{0}ProfileDefinitionId{1} = {0}ProfileDefinition{1}.{0}ProfileDefinitionId{1}\r\nand {0}GlobalSession{1}.{0}ExternalUser{1} = {2}externalUser{3}\r\nand {0}Session{1}.{0}Timestamp{1} < convert(datetime, {2}timeStamp{3}, 103)",
                        ReadProfileKeyResult, delegate
                                                  {
                                                      return
                                                          new ProfileKeyResult
                                                              ();
                                                  },
                        new object[]
                            {
                                "profileKeyDefinitionId", profileKeyDefinitionId, "externalUser",
                                Context.User.Identity.Name, "timeStamp",
                                DateTime.Now.AddDays(-1).ToLongTimeString()
                            });
            }
            else
            {
                pk =
                    AnalyticsManager.ReadOne(
                        "select {0}ProfileKey{1}.{0}ProfileKeyId{1}, {0}ProfileKey{1}.{0}ProfileId{1}, {0}ProfileKey{1}.{0}Value{1}, \r\n{0}Profile{1}.{0}Count{1}, {0}Profile{1}.{0}Total{1}, {0}ProfileDefinition{1}.{0}ProfileType{1}\r\nfrom {0}ProfileKey{1}, {0}Profile{1}, {0}ProfileDefinition{1}, {0}Session{1}\r\nwhere {0}ProfileKeyDefinitionId{1} = {2}profileKeyDefinitionId{3}\r\nand {0}ProfileKey{1}.{0}ProfileId{1} = {0}Profile{1}.{0}ProfileId{1}\r\nand {0}Profile{1}.{0}ProfileDefinitionId{1} = {0}ProfileDefinition{1}.{0}ProfileDefinitionId{1}\r\nand {0}Profile{1}.{0}SessionId{1} = {2}sessionId{3}",
                        ReadProfileKeyResult, delegate
                                                  {
                                                      return
                                                          new ProfileKeyResult
                                                              ();
                                                  },
                        new object[] {"profileKeyDefinitionId", profileKeyDefinitionId, "sessionId", sessionId});
            }

            return pk;
        }

        private double GetProfileKeyValue()
        {
            Func<ProfileData, bool> predicate = null;
            Func<ProfileKeyData, bool> func2 = null;
            AnalyticsTracker current = AnalyticsTracker.Current;
            if (current == null)
            {
                return 0.0;
            }
            Guid sessionId = current.CurrentSession.GetSessionId();
            if (sessionId == Guid.Empty)
            {
                return 0.0;
            }
            ProfileKeyResult profileKeyResult = GetProfileKeyResult(sessionId, new Guid(ProfileKeyId));
            if ((profileKeyResult.profileId == Guid.Empty) || (profileKeyResult.profileKeyId == Guid.Empty))
            {
                return 0.0;
            }
            if (current.Data != null)
            {
                if (predicate == null)
                {
                    predicate = delegate(ProfileData p) { return p.ProfileId == profileKeyResult.profileId; };
                }
                ProfileData source = current.Data.Profiles.FirstOrDefault(predicate);
                if (source != null)
                {
                    if (func2 == null)
                    {
                        func2 =
                            delegate(ProfileKeyData profileKeyData) { return profileKeyData.ProfileKeyId == profileKeyResult.profileKeyId; };
                    }
                    ProfileKeyData data2 = source.FirstOrDefault(func2);
                    if (data2 != null)
                    {
                        int num = source.Sum(delegate(ProfileKeyData profileKeyData) { return profileKeyData.Value; });
                        bool changed = source.Changed;
                        foreach (ProfileKeyData data3 in source)
                        {
                            changed |= data3.Changed;
                        }
                        profileKeyResult.count = changed ? (source.Count + 1) : source.Count;
                        profileKeyResult.total = num;
                        profileKeyResult.value = data2.Value;
                    }
                }
            }
            string str = profileKeyResult.profileType ?? string.Empty;
            switch (str.ToLower())
            {
                case "sum":
                    return profileKeyResult.value;

                case "percentage":
                    return ((profileKeyResult.value*100.0)/(profileKeyResult.total));

                case "average":
                    return ((profileKeyResult.value)/((double) profileKeyResult.count));
            }
            return profileKeyResult.value;
        }

        private static ProfileKeyResult ReadProfileKeyResult(DataProviderReader reader)
        {
            var result = new ProfileKeyResult();
            result.profileKeyId = AnalyticsManager.GetGuid(0, reader);
            result.profileId = AnalyticsManager.GetGuid(1, reader);
            result.value = AnalyticsManager.GetInt(2, reader);
            result.count = AnalyticsManager.GetInt(3, reader);
            result.total = AnalyticsManager.GetInt(4, reader);
            result.profileType = AnalyticsManager.GetString(5, reader);
            return result;
        }

        // Properties

        // Nested Types

        #region Nested type: ProfileKeyResult

        private class ProfileKeyResult
        {
            // Fields

            // Properties
            public int count { get; set; }

            public Guid profileId { get; set; }

            public Guid profileKeyId { get; set; }

            public string profileType { get; set; }

            public int total { get; set; }

            public int value { get; set; }
        }

        #endregion
    }
}