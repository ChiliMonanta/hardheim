using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn;
using Jotunn.Managers;
using Jotunn.Utils;

namespace HardHeim;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
public class HardHeim : BaseUnityPlugin
{
    public const string PluginGUID = "com.valheim.hardheim";
    public const string PluginName = "HardHeim";
    public const string PluginVersion = "0.0.2";

    private ConfigEntry<float> copperOreWeight;
    private static Harmony harmony;

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

        harmony = new Harmony(PluginGUID);
        harmony.PatchAll();

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

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }

    [HarmonyPatch(typeof(Skills), nameof(Skills.LowerAllSkills))]
    public static class DeathPenaltyPatch
    {
        public static bool Prefix(Skills __instance, float factor)
        {
            if (factor <= 0f) return false;

            foreach (var skill in __instance.GetSkillList())
            {
                skill.m_level = CalculatePenaltyLevel(skill.m_level, factor);
                skill.m_accumulator = 0f;
            }

            return false;
        }

        private static float CalculatePenaltyLevel(float currentLevel, float factor)
        {
            if (currentLevel >= 60f)
            {
                return UnityEngine.Mathf.Max(0f, currentLevel - 1f);
            }

            if (currentLevel >= 50f)
            {
                return UnityEngine.Mathf.Max(0f, currentLevel - 2f);
            }

            // Default Valheim penaulty, percentage
            return currentLevel * (1f - factor);
        }
    }
}