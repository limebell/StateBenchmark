using System.Diagnostics;
using Bencodex.Types;
using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume;
using Nekoyume.Action;
using Nekoyume.Action.Extensions;
using Nekoyume.Model;
using Nekoyume.Model.Item;
using Nekoyume.Model.Quest;
using Nekoyume.Model.State;
using Nekoyume.Module;

namespace StateBenchmark;

public class AvatarHelper
{
    public static BenchmarkStruct GetAvatarState(IWorldState worldState, Address address)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var serializedAvatarRaw = AccountHelper.Resolve(worldState, address, Addresses.Avatar);
        stopWatch.Stop();
        var avatarStateGetState = stopWatch.Elapsed;
        stopWatch.Reset();
        
        AvatarState avatarState = null;
        var avatarStateSerialize = TimeSpan.Zero;
        if (serializedAvatarRaw is Dictionary avatarDict)
        {
            stopWatch.Start();
            Console.WriteLine("AvatarState is Dictionary");
            avatarState = new AvatarState(avatarDict);
            stopWatch.Stop();
            avatarStateSerialize = stopWatch.Elapsed;
            stopWatch.Reset();
        }
        else if (serializedAvatarRaw is List avatarList)
        {
            stopWatch.Start();
            Console.WriteLine("AvatarState is List");
            avatarState = new AvatarState(avatarList);
            stopWatch.Stop();
            avatarStateSerialize = stopWatch.Elapsed;
            stopWatch.Reset();
        }
        else
        {
            throw new Exception();
        }

        var inventoryGetState = TimeSpan.Zero;
        var inventorySerialize = TimeSpan.Zero;
        var worldInformationGetState = TimeSpan.Zero;
        var worldInformationSerialize = TimeSpan.Zero;
        var questListGetState = TimeSpan.Zero;
        var questListSerialize = TimeSpan.Zero;
        // Version 0 contains inventory, worldInformation, questList itself.
        if (avatarState.Version == 1)
        {
            Console.WriteLine("AvatarState is Version 1");
            string[] keys =
            {
                "inventory",
                "worldInformation",
                "questList",
            };
            var addresses = keys.Select(key => address.Derive(key)).ToArray();
            stopWatch.Start();
            var serializedInventory = LegacyModule.GetState(worldState, addresses[0]);
            stopWatch.Stop();
            inventoryGetState = stopWatch.Elapsed;
            stopWatch.Reset();
            stopWatch.Start();
            var serializedWorldInformation = LegacyModule.GetState(worldState, addresses[1]);
            stopWatch.Stop();
            worldInformationGetState = stopWatch.Elapsed;
            stopWatch.Reset();
            stopWatch.Start();
            var serializedQuestList = LegacyModule.GetState(worldState, addresses[2]);
            stopWatch.Stop();
            questListGetState = stopWatch.Elapsed;
            stopWatch.Reset();

            stopWatch.Start();
            avatarState.inventory = new Inventory((List)serializedInventory);
            stopWatch.Stop();
            inventorySerialize = stopWatch.Elapsed;
            stopWatch.Reset();
            stopWatch.Start();
            avatarState.worldInformation =
                new WorldInformation((Dictionary)serializedWorldInformation);
            stopWatch.Stop();
            worldInformationSerialize = stopWatch.Elapsed;
            stopWatch.Reset();
            stopWatch.Start();
            avatarState.questList = new QuestList((Dictionary)serializedQuestList);
            stopWatch.Stop();
            questListSerialize = stopWatch.Elapsed;
            stopWatch.Reset();
        }
        else if (avatarState.Version >= 2)
        {
            Console.WriteLine("AvatarState is Version 2");
            avatarState.inventory = GetInventoryV2(worldState, address);
            avatarState.worldInformation = GetWorldInformationV2(worldState, address);
            avatarState.questList = GetQuestListV2(worldState, address);
        }
        else
        {
            Console.WriteLine("AvatarState is Version 0");
        }

        return new BenchmarkStruct(
            avatarStateGetState,
            avatarStateSerialize,
            inventoryGetState,
            inventorySerialize,
            worldInformationGetState,
            worldInformationSerialize,
            questListGetState,
            questListSerialize);
    }
    
    private static Inventory GetInventoryV2(IWorldState worldState, Address address)
    {
        var serializedInventory = worldState.GetAccount(Addresses.Inventory).GetState(address);
        if (serializedInventory is null || serializedInventory.Equals(Null.Value))
        {
            throw new FailedLoadStateException(
                $"Aborted as the inventory state of the avatar ({address}) was failed to load.");
        }

        return new Inventory((List)serializedInventory);
    }
    
    internal static WorldInformation GetWorldInformationV2(IWorldState worldState, Address address)
    {
        var serializeWorldInfo =
            worldState.GetAccount(Addresses.WorldInformation).GetState(address);
        if (serializeWorldInfo is null || serializeWorldInfo.Equals(Null.Value))
        {
            throw new FailedLoadStateException(
                $"Aborted as the worldInformation state of the avatar ({address}) was failed to load.");
        }

        return new WorldInformation((Dictionary)serializeWorldInfo);
    }
    
    private static QuestList GetQuestListV2(IWorldState worldState, Address address)
    {
        var serializeQuestList = worldState.GetAccount(Addresses.QuestList).GetState(address);
        if (serializeQuestList is null || serializeQuestList.Equals(Null.Value))
        {
            throw new FailedLoadStateException(
                $"Aborted as the questList state of the avatar ({address}) was failed to load.");
        }

        return new QuestList((Dictionary)serializeQuestList);
    }
}