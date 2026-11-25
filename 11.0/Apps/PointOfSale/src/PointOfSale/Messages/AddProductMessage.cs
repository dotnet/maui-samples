using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PointOfSale.Messages;

public class AddProductMessage : ValueChangedMessage<bool>
{
    public AddProductMessage(bool value) : base(value)
    {
    }
}

