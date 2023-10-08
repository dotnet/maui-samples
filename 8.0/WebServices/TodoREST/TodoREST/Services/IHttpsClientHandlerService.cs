namespace TodoREST.Services
{
	public interface IHttpsClientHandlerService
	{
        HttpMessageHandler GetPlatformMessageHandler();
    }
}

