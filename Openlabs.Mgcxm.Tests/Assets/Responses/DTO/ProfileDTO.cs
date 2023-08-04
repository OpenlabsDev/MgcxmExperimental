// Copr. (c) Nexus 2023. All rights reserved.

namespace Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

public class ProfileDTO
{
    public ulong Id { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public int XP { get; set; }
    public int Level { get; set; }
    public int Reputation { get; set; }
    public bool Verified { get; set; }
    public bool Developer { get; set; }
    public bool HasEmail { get; set; }
    public bool CanReceiveInvites { get; set; }
    public string ProfileImageName { get; set; }
}