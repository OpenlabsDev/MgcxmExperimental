// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;
using Openlabs.Mgcxm.Common;
using Openlabs.Mgcxm.Common.Framework;
using Openlabs.Mgcxm.Common.JsonMapping;
using Openlabs.Mgcxm.Internal;
using Openlabs.Mgcxm.Net;
using Openlabs.Mgcxm.Tests.Assets.Responses;
using Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

namespace Openlabs.Mgcxm.Tests;

public sealed class TestServer : MgcxmHttpListener
{
    public static string PlayerName = "Invalid Name";
    
    public TestServer() : base("localhost:8010")
    {
        SetFramework(new OpenApiFramework(this));
        AddFrameworkEndpoint(new OpenApiUrl("activities/charades", 1, "words"), HttpMethods.GET, HttpGet_Activities_CharadesV1_GetWords);
        
        AvatarController.Initialize().Wait();
        SettingsController.Initialize().Wait();
        MessagesController.Initialize().Wait();
        

        Framework.EndpointResolver.AddSubpartController(new VersionCheckController());
        Framework.EndpointResolver.AddSubpartController(new PlatformLoginController());
        Framework.EndpointResolver.AddSubpartController(new ConfigController());
        Framework.EndpointResolver.AddSubpartController(new PlayersController());
        Framework.EndpointResolver.AddSubpartController(new RelationshipsController());
        Framework.EndpointResolver.AddSubpartController(new AvatarController());
        Framework.EndpointResolver.AddSubpartController(new SettingsController());
        Framework.EndpointResolver.AddSubpartController(new MessagesController());
        Framework.EndpointResolver.ResolveAll();
    }
    
    public void HttpGet_Activities_CharadesV1_GetWords(MgcxmHttpRequest request, MgcxmHttpResponse response)
    {
        response.Status(HttpStatusCodes.OK)
            .Json(new List<object>());
    }
    
    public sealed class VersionCheckController : EndpointSubpartController
    {
        [HttpEndpoint(HttpMethods.GET, "/api/versioncheck/v1")]
        public void HttpGet_VersionCheckV1(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK).Content(true);
        }
    }
    
    public sealed class PlatformLoginController : EndpointSubpartController
    {
        [HttpEndpoint(HttpMethods.POST, "/api/platformlogin/v2")]
        public void HttpPost_PlatformLoginV2(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            try
            {
                PlayerName = request.Form["Name"];
                response.Status(HttpStatusCodes.OK).Content(new LoginResponseDTO
                {
                    PlayerId = ulong.Parse(request.Form["PlatformId"]),
                    Token = "test"
                });
            }
            catch (Exception ex) { Logger.Exception("Failed to login", ex); }
        }
    }
    
    public sealed class ConfigController : EndpointSubpartController
    {
        [HttpEndpoint(HttpMethods.GET, "/api/config/v2")]
        public void HttpGet_ConfigV2(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            const int MAX_LEVELS = 15;
            
            RecRoomConfigDTO recRoomConfigDto = new RecRoomConfigDTO()
            {
                MessageOfTheDay = "hello",
                CdnBaseUri = "http://localhost:8013/",
                MatchmakingParams = new()
                {
                    PreferFullRoomsFrequency = 0.8f,
                    PreferEmptyRoomsFrequency = 0.2f
                },
                LevelProgressionMaps = ListTools.Create<LevelProgressionIndexDTO>((list, index) => list.Add(new(index)), MAX_LEVELS),
                DailyObjectives = new List<List<ObjectiveDTO>>()
                {
                    ListTools.Create<ObjectiveDTO>((list, index) =>
                    {
                        list.Add(new()
                        {
                            Type = (int)ObjectiveType.Default,
                            Score = 0
                        });
                    }, 3),
                    ListTools.Create<ObjectiveDTO>((list, index) =>
                    {
                        list.Add(new()
                        {
                            Type = (int)ObjectiveType.Default,
                            Score = 0
                        });
                    }, 3),
                    ListTools.Create<ObjectiveDTO>((list, index) =>
                    {
                        list.Add(new()
                        {
                            Type = (int)ObjectiveType.Default,
                            Score = 0
                        });
                    }, 3)
                },
                ConfigTable = new List<SettingDTO>()
                {
                    new("Gift.DropChance", "0.75")
                },
                PhotonConfig = new PhotonConfigDTO()
                {
                    CloudRegion = PhotonRegion.eu.ToString(),
                    CrcCheckEnabled = false
                }
            };
            
            response.Status(HttpStatusCodes.OK)
                    .Json(recRoomConfigDto);
        }
    }
    
    public sealed class PlayersController : EndpointSubpartController
    {
        [HttpEndpoint(HttpMethods.GET, "/api/players/v1/{playerId}")]
        [HttpDynamicUse("{playerId}")]
        public void HttpGet_PlayersV1_PlayerId(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            ulong id = ulong.Parse(request.Uri.Split("/").Last());
            response.Status(HttpStatusCodes.OK)
                    .Json(new ProfileDTO()
                    {
                        Id = id,
                        Username = PlayerName,
                        DisplayName = PlayerName,
                        XP = 0,
                        Level = 1,
                        Reputation = 99,
                        Verified = true,
                        Developer = true,
                        HasEmail = true,
                        CanReceiveInvites = false,
                        ProfileImageName = "DefaultProfileImage"
                    });
        }
    }
    
    public sealed class RelationshipsController : EndpointSubpartController
    {
        [HttpEndpoint(HttpMethods.GET, "/api/relationships/v2/get")]
        public void HttpGet_RelationshipsV2_Get(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                    .Json(new List<object>());
        }
    }
    
    public sealed class AvatarController : EndpointSubpartController
    {
        public static async Task Initialize()
        {
            if (_avatarFile == null)
                _avatarFile = await AssetManager.Load<AvatarJsonFileAsset>("avatar.json");
        }
        
        [HttpEndpoint(HttpMethods.GET, "/api/avatar/v2")]
        public void HttpGet_AvatarV2_Get(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                    .Json(_avatarFile.ReadDto());
        }
        
        [HttpEndpoint(HttpMethods.POST, "/api/avatar/v2/set")]
        public void HttpGet_AvatarV2_Set(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            AvatarDTO avatarPostData = Json.FromJson<AvatarDTO>(request.Body);
            _avatarFile.WriteDto(avatarPostData);
            
            response.Status(HttpStatusCodes.OK)
                    .Json(avatarPostData);
        }
        
        [HttpEndpoint(HttpMethods.GET, "/api/avatar/v3/items")]
        public void HttpGet_AvatarV3_GetItems(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                .Json(new List<object>());
        }
        
        [HttpEndpoint(HttpMethods.GET, "/api/avatar/v2/gifts")]
        public void HttpGet_AvatarV2_GetGifts(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                .Json(new List<object>());
        }

        private static AvatarJsonFileAsset _avatarFile;
    }

    public class SettingsController : EndpointSubpartController
    {
        public static async Task Initialize()
        {
            if (_settingsFile == null)
                _settingsFile = await AssetManager.Load<SettingsJsonFileAsset>("settings.json");
        }
        
        [HttpEndpoint(HttpMethods.GET, "/api/settings/v2")]
        public void HttpGet_SettingsV2_Get(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                    .Json(_settingsFile.ReadDto());
        }
        
        [HttpEndpoint(HttpMethods.POST, "/api/settings/v2/set")]
        public void HttpGet_SettingsV2_Set(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            var settingPostData = Json.FromJson<SettingDTO>(request.Body);
            var settingsList = _settingsFile.ReadDto();
            bool foundSetting = false;
            
            foreach (var setting in settingsList)
            {
                if (setting.Key == settingPostData.Key)
                {
                    foundSetting = true;
                    setting.Value = settingPostData.Value;
                }
            }
            
            if (!foundSetting)
                settingsList.Add(settingPostData);
            
            _settingsFile.WriteDto(settingsList);
            response.Status(HttpStatusCodes.OK)
                    .Json(settingsList);
        }
        
        private static SettingsJsonFileAsset _settingsFile;
    }
    
    public class MessagesController : EndpointSubpartController
    {
        public static async Task Initialize()
        {
            if (_hasClosedWelcomingMessage == null)
                _hasClosedWelcomingMessage = await AssetManager.Load<FileFlagAsset>("hcwm.flag");
        }
        
        [HttpEndpoint(HttpMethods.GET, "/api/messages/v2/get")]
        public void HttpGet_MessagesV2_Get(MgcxmHttpRequest request, MgcxmHttpResponse response)
        {
            response.Status(HttpStatusCodes.OK)
                    .Json(new List<object>());
        }

        private static FileFlagAsset _hasClosedWelcomingMessage = null;
    }
}