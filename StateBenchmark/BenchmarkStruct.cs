namespace StateBenchmark;

public struct BenchmarkStruct
{
    public BenchmarkStruct(
        TimeSpan avatarStateGetState,
        TimeSpan avatarStateSerialize,
        TimeSpan inventoryGetState,
        TimeSpan inventorySerialize,
        TimeSpan worldInformationGetState,
        TimeSpan worldInformationSerialize,
        TimeSpan questListGetState,
        TimeSpan questListSerialize)
    {
        AvatarStateGetState = avatarStateGetState;
        AvatarStateSerialize = avatarStateSerialize;
        InventoryGetState = inventoryGetState;
        InventorySerialize = inventorySerialize;
        WorldInformationGetState = worldInformationGetState;
        WorldInformationSerialize = worldInformationSerialize;
        QuestListGetState = questListGetState;
        QuestListSerialize = questListSerialize;
    }

    public readonly TimeSpan Total => GetState + Serialize;
    public readonly TimeSpan GetState => AvatarStateGetState + InventoryGetState + WorldInformationGetState + QuestListGetState;
    public readonly TimeSpan Serialize => AvatarStateSerialize + InventorySerialize + WorldInformationSerialize + QuestListSerialize;
    public readonly TimeSpan AvatarState => AvatarStateGetState + AvatarStateSerialize;
    public readonly TimeSpan AvatarStateGetState;
    public readonly TimeSpan AvatarStateSerialize;
    public readonly TimeSpan Inventory => InventoryGetState + InventorySerialize;
    public readonly TimeSpan InventoryGetState;
    public readonly TimeSpan InventorySerialize;
    public readonly TimeSpan WorldInformation => WorldInformationGetState + WorldInformationSerialize;
    public readonly TimeSpan WorldInformationGetState;
    public readonly TimeSpan WorldInformationSerialize;
    public readonly TimeSpan QuestList => QuestListGetState + QuestListSerialize;
    public readonly TimeSpan QuestListGetState;
    public readonly TimeSpan QuestListSerialize;
    
    public override string ToString()
    {
        return
            $"Total: {Total.TotalMilliseconds}ms ({GetState.TotalMilliseconds}ms / {Serialize.TotalMilliseconds}ms), AvatarState: {AvatarState.TotalMilliseconds}ms ({AvatarStateGetState.TotalMilliseconds}ms / {AvatarStateSerialize.TotalMilliseconds}ms), Inventory: {Inventory.TotalMilliseconds}ms ({InventoryGetState.TotalMilliseconds}ms / {InventorySerialize.TotalMilliseconds}ms), WorldInformation: {WorldInformation.TotalMilliseconds}ms ({WorldInformationGetState.TotalMilliseconds}ms / {WorldInformationSerialize.TotalMilliseconds}ms), QuestList: {QuestList.TotalMilliseconds}ms ({QuestListGetState.TotalMilliseconds}ms / {QuestListSerialize.TotalMilliseconds}ms)";
    }
}