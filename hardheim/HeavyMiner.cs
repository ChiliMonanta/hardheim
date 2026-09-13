using BepInEx;
using BepInEx.Configuration;
using Jotunn;
using Jotunn.Managers;
using Jotunn.Utils;

namespace HeavyMining;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
public class HeavyMiner : BaseUnityPlugin
{
    public const string PluginGUID = "com.valheim.heavyminer";
    public const string PluginName = "Heavy Miner";
    public const string PluginVersion = "1.0.0";

    private ConfigEntry<float> copperOreWeight;

    private void Awake()
    {
        copperOreWeight = Config.Bind(
            "Ore Weights",
            "CopperOre",
            50f,
            new ConfigDescription(
                "Weight of one copper ore.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        Logger.LogInfo($"{PluginName} started.");
        Jotunn.Logger.LogInfo($"{PluginName} started through Jotunn.");
        ItemManager.OnItemsRegistered += SetCopperOreWeight;
        SynchronizationManager.OnConfigurationSynchronized += OnConfigurationSynchronized;
    }

    private void SetCopperOreWeight()
    {
        var copperOre = PrefabManager.Cache.GetPrefab<ItemDrop>("CopperOre");
        if (copperOre == null)
        {
            Logger.LogError("Could not find the CopperOre prefab.");
            return;
        }

        copperOre.m_itemData.m_shared.m_weight = copperOreWeight.Value;
        Logger.LogInfo($"Copper ore weight set to {copperOreWeight.Value}.");
    }

    private void OnConfigurationSynchronized(object sender, ConfigurationSynchronizationEventArgs args)
    {
        SetCopperOreWeight();
    }
}