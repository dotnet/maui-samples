using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PointOfSale;

public class AddToOrderMessage : ValueChangedMessage<Item>
{
    public AddToOrderMessage(Item product) : base(product)
    {
    }
}
