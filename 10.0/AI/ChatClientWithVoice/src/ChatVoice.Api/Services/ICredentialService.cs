using ChatVoice.Api.Models;

namespace ChatVoice.Api.Services;

public interface ICredentialService
{
    CredentialResponse GetCredentials();
}
