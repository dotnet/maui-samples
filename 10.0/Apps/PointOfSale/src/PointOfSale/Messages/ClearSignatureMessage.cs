using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PointOfSale.Messages;

public class ClearSignatureMessage : ValueChangedMessage<bool>
{
    public ClearSignatureMessage(bool value) : base(value)
    {
    }
}

