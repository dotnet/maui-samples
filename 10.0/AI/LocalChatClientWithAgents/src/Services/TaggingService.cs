using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.AI;

namespace LocalChatClientWithAgents.Services;

public class TaggingService(IChatClient chatClient)
{
	public async Task<List<string>> GenerateTagsAsync(string text, CancellationToken cancellationToken = default)
	{
		var systemPrompt =
			"""
			Your job is to extract the most relevant tags from the input text.
			Return tags as single words or phrases in CamelCase to be used on social media posts.
			""";

		var userPrompt =
			$"""
			Extract relevant tags from this text:
			
			{text}
			""";

		var messages = new List<ChatMessage>
		{
			new(ChatRole.System, systemPrompt),
			new(ChatRole.User, userPrompt)
		};

		var response = await chatClient.GetResponseAsync<TaggingResponse>(messages, cancellationToken: cancellationToken);
		return response.TryGetResult(out var result) ? result?.Tags ?? [] : [];
	}

	public class TaggingResponse
	{
		[Description("Most important topics in the input text.")]
		[Length(5, 5)]
		public List<string> Tags { get; set; } = [];
	}
}
