using CommunityToolkit.Mvvm.Messaging.Messages;

namespace DP_S_Marketplace.Messages;

public class ConnectionsUpdatedMessage : ValueChangedMessage<bool>
{
    public ConnectionsUpdatedMessage(bool value) : base(value)
    {
    }
}
