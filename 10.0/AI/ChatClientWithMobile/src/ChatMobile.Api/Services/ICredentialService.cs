using ChatMobile.Api.Models;

namespace ChatMobile.Api.Services;

public interface ICredentialService
{
    CredentialResponse GetCredentials();
}