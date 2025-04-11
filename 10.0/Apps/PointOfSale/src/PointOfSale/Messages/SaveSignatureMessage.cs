using System;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PointOfSale.Messages;

public class SaveSignatureMessage : ValueChangedMessage<int>
{
    public SaveSignatureMessage(int value) : base(value)
    {
    }
}

