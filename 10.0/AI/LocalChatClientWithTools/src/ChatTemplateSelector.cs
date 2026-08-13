using LocalChatClientWithTools.ViewModels;

namespace LocalChatClientWithTools;

public class ChatTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? ToolCallTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => item switch
    {
        ToolCallMessageViewModel => ToolCallTemplate!,
        TextMessageViewModel => TextTemplate!,
        _ => TextTemplate!
    };
}
