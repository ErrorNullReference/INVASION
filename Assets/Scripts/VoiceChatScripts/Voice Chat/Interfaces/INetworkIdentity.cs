/// <summary>
/// Interface that represents an object which will be treated as unique on the nerwork
/// </summary>
public interface INetworkIdentity
{
    /// <summary>
    /// Unique network id that identifies this specific object in the network. This value should not change
    /// </summary>
    ///     
    ulong NetworkId { get; set; }
    /// <summary>
    /// True if this object is owned by the local player. This value should not change
    /// </summary>
    bool IsLocalPlayer { get; set; }
}