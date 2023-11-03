using System.Diagnostics;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.RocksDBStore;
using Libplanet.Store;
using Nekoyume;
using Nekoyume.Action.Loader;
using Nekoyume.Blockchain;
using Nekoyume.Blockchain.Policy;
using Nekoyume.Module;
using StateBenchmark;

try
{
    Benchmark();
}
catch (Exception e)
{
    Console.WriteLine($"Unexpected exception {e} occurred during benchmark.");
}

void Benchmark()
{
    var stopwatch = new Stopwatch();
    var address = new Address("0x89d5C90Ef04307b024128e4E6f099d2F2558104D");
    var actionLoader = new NCActionLoader();
    var blockPolicy = new BlockPolicySource(actionLoader).GetPolicy();
    var stagePolicy = new NCStagePolicy(TimeSpan.FromHours(1), 100);
    var store = new RocksDBStore(
        "C:\\Users\\lime_\\planetarium\\local-test\\mainnet-snapshot");
    var stateStore = new TrieStateStore(
        new RocksDBKeyValueStore(
            "C:\\Users\\lime_\\planetarium\\local-test\\mainnet-snapshot\\states"));
    var genesis = store.GetBlock(
        store.IterateIndexes(
                store.GetCanonicalChainId() ??
                throw new Exception("GenesisBlock should not be null."),
                0,
                1)
            .First());
    var actionEvaluator = new ActionEvaluator(
        _ => blockPolicy.BlockAction,
        stateStore,
        new NCActionLoader());
    var blockChain = new BlockChain(
        blockPolicy,
        stagePolicy,
        store,
        stateStore,
        genesis,
        actionEvaluator);
    var worldState = blockChain.GetWorldState();
    var agent = AgentModule.GetAgentState(worldState, address);
    Address avatarAddress = agent?.avatarAddresses.First().Value ??
                            throw new Exception("Avatar should not be null.");
    var benchmark = AvatarHelper.GetAvatarState(worldState, avatarAddress);
    Console.WriteLine(benchmark.ToString());
    benchmark = AvatarHelper.GetAvatarState(worldState, avatarAddress);
    Console.WriteLine(benchmark.ToString());
    benchmark = AvatarHelper.GetAvatarState(worldState, avatarAddress);
    Console.WriteLine(benchmark.ToString());
}