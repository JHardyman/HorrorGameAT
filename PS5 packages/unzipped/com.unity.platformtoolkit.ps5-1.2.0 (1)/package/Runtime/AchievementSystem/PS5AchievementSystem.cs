using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.PlatformToolkit.PS5.Native;
using UnityEngine;
using Unity.PSN.PS5;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.UDS;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5AchievementSystem : AbstractAchievementSystem<AchievementDefinitionWithNativeId<int>>
    {
        private readonly int m_UserID;

        public PS5AchievementSystem(int userID, IReadOnlyList<AchievementDefinitionWithNativeId<int>> achievements, int timeBetweenUpdatesMilliseconds = 100, ILifetimeToken parentLifetimeToken = null) : base(achievements, timeBetweenUpdatesMilliseconds, parentLifetimeToken)
        {
            m_UserID = userID;
        }

        protected override Task InitializeSystem()
        {
            // check if there are easy quick steps to figure out of it will be broken, e.g. no npconfig.zip or achievement data present.
            return Task.CompletedTask;
        }

        protected override async Task FetchNativeAchievementData()
        {
            foreach (var achievement in AchievementDefinitions)
            {
                try
                {
                    var request = CreateTrophyRequest(m_UserID, achievement.NativeId);
                    var result = await AsyncRequestTaskWrapper.AsyncRequestTrophySystem(request);

                    var data = result.Request.TrophyData;
                    var details = result.Request.TrophyDetails;
                    if (data.Unlocked)
                    {
                        SetNativeUnlocked(achievement.Id);
                    }
                    else if (data.IsProgress)
                    {
                        if (!achievement.Progressive)
                        {
                            LogTypeMismatchPtSingleNativeProgress(achievement);
                        }
                        else if (details.TargetValue != achievement.ProgressTarget)
                        {
                            LogProgressTargetMismatch(achievement, details.TargetValue);
                        }
                        var currentProgress = data.ProgressValue > int.MaxValue ? int.MaxValue : (int)data.ProgressValue;
                        SetNativeProgress(achievement.Id, currentProgress, achievement.ProgressTarget);
                    }
                    else if (!data.IsProgress)
                    {
                        if (achievement.Progressive)
                        {
                            LogTypeMismatchPtProgressNativeSingle(achievement);
                        }
                        SetNativeNonProgressive(achievement.Id);
                    }
                }
                catch (PSNException e)
                {
                    if (e.SceErrorCode == PS5Native.SCE_NP_TROPHY2_ERROR_INVALID_TROPHY_ID)
                    {
                        Debug.LogError($"Invalid trophy ID: {achievement.Id}. Please make sure you have set the npconfig.zip file and trophies are correctly configured between Platform Toolkit achievements and PS5 Trophy ID's. Error code: {PS5Native.SCE_NP_TROPHY2_ERROR_INVALID_TROPHY_ID.ToString("x")}");
                        SetNativeInvalid(achievement.Id);
                    }
                    else
                    {
                        Debug.LogError("Error initializing Platform Toolkit Achievement System. Error code: " + e.SceErrorCode.ToString("x"));
                    }
                }
            }
        }

        protected override async Task DoUpdate(AchievementDefinitionWithNativeId<int> achievement, int progress)
        {
            if(achievement.Progressive)
            {
                var request = new UniversalDataSystem.UpdateTrophyProgressRequest
                {
                    TrophyId = achievement.NativeId,
                    UserId = m_UserID,
                    Progress = progress
                };
                try
                {
                    await AsyncRequestTaskWrapper.AsyncRequestUniversalDataSystem(request);
                    SetNativeProgress(achievement.Id, progress, achievement.ProgressTarget);
                }
                catch (PSNException e)
                {
                    Debug.LogException(e);
                }
            }
            else
            {
                var request = new UniversalDataSystem.UnlockTrophyRequest
                {
                    TrophyId = achievement.NativeId,
                    UserId = m_UserID
                };

                try
                {
                    await AsyncRequestTaskWrapper.AsyncRequestUniversalDataSystem(request);
                    SetNativeUnlocked(achievement.Id);
                }
                catch (PSNException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private TrophySystem.GetTrophyInfoRequest CreateTrophyRequest(int userID, int trophyID)
        {
            var request = new TrophySystem.GetTrophyInfoRequest
            {
                UserId = userID,
                TrophyId = trophyID,
                TrophyDetails = new TrophySystem.TrophyDetails(),
                TrophyData = new TrophySystem.TrophyData()
            };
            return request;
        }
    }
}
