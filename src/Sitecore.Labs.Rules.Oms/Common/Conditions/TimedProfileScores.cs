using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Sitecore.Labs.Rules.Oms.Common.Conditions
{
    public class TimedProfileScores<T> : OperatorCondition<T> where T : RuleContext
    {
        private string _profileKeyId;
        private string _value;

        public TimedProfileScores()
        {
            _profileKeyId = string.Empty;
            _value = string.Empty;
        }

        public string ProfileKeyId
        {
            get { return (_profileKeyId ?? string.Empty); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _profileKeyId = value;
            }
        }

        public string Value
        {
            get { return (_value ?? string.Empty); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _value = value;
            }
        }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            double num2;
            double profileKeyValue = GetProfileKeyValue();

            if (double.TryParse(Value, out num2))
            {
                switch (base.GetOperator())
                {
                    case ConditionOperator.Equal:
                        return (Math.Abs(profileKeyValue - num2) < double.Epsilon);

                    case ConditionOperator.GreaterThanOrEqual:
                        return (profileKeyValue >= num2);

                    case ConditionOperator.GreaterThan:
                        return (profileKeyValue > num2);

                    case ConditionOperator.LessThanOrEqual:
                        return (profileKeyValue <= num2);

                    case ConditionOperator.LessThan:
                        return (profileKeyValue < num2);

                    case ConditionOperator.NotEqual:
                        return (Math.Abs(profileKeyValue - num2) > double.Epsilon);
                }
            }
            return false;
        }

        private static ProfileKeyResult GetProfileKeyResult(Guid sessionId, Guid profileKeyDefinitionId)
        {
            var pk = new ProfileKeyResult();

            var getList = AnalyticsManager.GetProfilesBySessionId(sessionId).Where(
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
                        ReadProfileKeyResult, () => new ProfileKeyResult(),
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
                        ReadProfileKeyResult, () => new ProfileKeyResult(),
                        new object[]
                            {
                                "profileKeyDefinitionId", 
                                profileKeyDefinitionId,
                                "sessionId",
                                sessionId
                            });
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

            if ((profileKeyResult.ProfileId == Guid.Empty) || (profileKeyResult.ProfileKeyId == Guid.Empty))
            {
                return 0.0;
            }

            if (current.Data != null)
            {
                if (predicate == null)
                {
                    predicate = p => p.ProfileId == profileKeyResult.ProfileId;
                }
                ProfileData source = current.Data.Profiles.FirstOrDefault(predicate);
                if (source != null)
                {
                    if (func2 == null)
                    {
                        func2 = profileKeyData => profileKeyData.ProfileKeyId == profileKeyResult.ProfileKeyId;
                    }
                    ProfileKeyData data2 = source.FirstOrDefault(func2);
                    if (data2 != null)
                    {
                        int num = source.Sum((ProfileKeyData profileKeyData) => profileKeyData.Value);
                        bool changed = source.Aggregate(source.Changed, (current1, data3) => current1 | data3.Changed);
                        profileKeyResult.Count = changed ? (source.Count + 1) : source.Count;
                        profileKeyResult.Total = num;
                        profileKeyResult.Value = data2.Value;
                    }
                }
            }

            string str = profileKeyResult.ProfileType ?? string.Empty;

            switch (str.ToLower())
            {
                case "sum":
                    return profileKeyResult.Value;

                case "percentage":
                    return ((profileKeyResult.Value * 100.0) / (profileKeyResult.Total));

                case "average":
                    return ((profileKeyResult.Value) / ((double)profileKeyResult.Count));
            }

            return profileKeyResult.Value;
        }

        private static ProfileKeyResult ReadProfileKeyResult(DataProviderReader reader)
        {
            var result = new ProfileKeyResult
                             {
                                 ProfileKeyId = AnalyticsManager.GetGuid(0, reader),
                                 ProfileId = AnalyticsManager.GetGuid(1, reader),
                                 Value = AnalyticsManager.GetInt(2, reader),
                                 Count = AnalyticsManager.GetInt(3, reader),
                                 Total = AnalyticsManager.GetInt(4, reader),
                                 ProfileType = AnalyticsManager.GetString(5, reader)
                             };
            return result;
        }

        #region Nested type: ProfileKeyResult

        private class ProfileKeyResult
        {
            public int Count { get; set; }
            public Guid ProfileId { get; set; }
            public Guid ProfileKeyId { get; set; }
            public string ProfileType { get; set; }
            public int Total { get; set; }
            public int Value { get; set; }
        }

        #endregion
    }
}