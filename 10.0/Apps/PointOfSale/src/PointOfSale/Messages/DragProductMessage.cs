using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PointOfSale.Messages;

public class DragProductMessage : ValueChangedMessage<bool>
{
    public DragProductMessage(bool value) : base(value)
    {
    }
}

