// Copr. (c) Nexus 2023. All rights reserved.

using Openlabs.Mgcxm.Assets;

namespace Openlabs.Mgcxm.Tests.Assets.Responses.DTO;

public class LoginResponseDTO : ObjectDTO
{
    public ulong PlayerId { get; set; }
    public string Token { get; set; }
}