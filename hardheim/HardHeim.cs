using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using UnityEngine.Rendering;

namespace HardHeim;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
public class HardHeim : BaseUnityPlugin
{
    public const string PluginGUID = "com.valheim.hardheim";
    public const string PluginName = "HardHeim";
    public const string PluginVersion = "0.0.4";

    private ConfigEntry<float> copperOreWeight;
    private ConfigEntry<float> surtlingCoreWeight;
    internal static ConfigEntry<float> cryptSurtlingCoreChance;
    private static Harmony harmony;

    #region Plugin lifecycle and configuration

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

        surtlingCoreWeight = Config.Bind(
            "Ore Weights",
            "SurtlingCore",
            150f,
            new ConfigDescription(
                "Weight of one surtling core.",
                new AcceptableValueRange<float>(0.1f, 1000f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        cryptSurtlingCoreChance = Config.Bind(
            "Crypt Loot",
            "SurtlingCoreChance",
            30f,
            new ConfigDescription(
                "Percent chance (0-100) to find a Surtling Core in a Burial Chamber (0 or 1 total per crypt).",
                new AcceptableValueRange<float>(0f, 100f),
                new ConfigurationManagerAttributes { IsAdminOnly = true }));

        harmony = new Harmony(PluginGUID);
        try
        {
            harmony.PatchAll();
        }
        catch (Exception exception)
        {
            Logger.LogError($"Failed to install Harmony patches: {exception}");
            throw;
        }

        Logger.LogInfo($"{PluginName} started.");
        Jotunn.Logger.LogInfo($"{PluginName} started through Jotunn.");
        ItemManager.OnItemsRegistered += SetItemWeights;
        SynchronizationManager.OnConfigurationSynchronized += OnConfigurationSynchronized;
    }

    private void SetItemWeights()
    {
        var copperOre = PrefabManager.Cache.GetPrefab<ItemDrop>("CopperOre");
        if (copperOre == null)
        {
            Logger.LogError("Could not find the CopperOre prefab.");
            return;
        }

        copperOre.m_itemData.m_shared.m_weight = copperOreWeight.Value;
        Logger.LogInfo($"Copper ore weight set to {copperOreWeight.Value}.");

        var surtlingCore = PrefabManager.Cache.GetPrefab<ItemDrop>("SurtlingCore");
        if (surtlingCore == null)
        {
            Logger.LogError("Could not find the SurtlingCore prefab.");
            return;
        }

        surtlingCore.m_itemData.m_shared.m_weight = surtlingCoreWeight.Value;
        Logger.LogInfo($"Surtling core weight set to {surtlingCoreWeight.Value}.");
    }

    private void OnConfigurationSynchronized(object sender, ConfigurationSynchronizationEventArgs args)
    {
        SetItemWeights();
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
    }

    // Ensures dungeon lighting overrides stay active across interior transitions.
    [HarmonyPatch(typeof(Player), "FixedUpdate")]
    public static class CryptEntryPatch
    {
        private static void Postfix()
        {
            if (EnvMan.instance != null)
            {
                CryptEnvironmentPatch.Refresh(EnvMan.instance);
            }
        }
    }

    #endregion

    #region Crypt loot

    // Reduces Surtling Core stands in Burial Chambers to 0 or 1 total per crypt based on configurable chance.
    [HarmonyPatch]
    public static class CryptLootPatch
    {
        private static bool wasInsideCrypt;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "FixedUpdate")]
        private static void OnPlayerFixedUpdate()
        {
            if (Player.m_localPlayer == null)
            {
                return;
            }

            bool insideCrypt = Player.m_localPlayer.InInterior();
            if (!insideCrypt)
            {
                wasInsideCrypt = false;
                return;
            }

            if (!wasInsideCrypt)
            {
                wasInsideCrypt = true;
                ProcessCurrentCrypt();
            }
        }

        private static void ProcessCurrentCrypt()
        {
            var surtlingCore = PrefabManager.Cache.GetPrefab<ItemDrop>("SurtlingCore");
            if (surtlingCore == null)
            {
                return;
            }

            var playerPos = Player.m_localPlayer.transform.position;

            // Target placed Surtling Core stands within the local dungeon area (80m radius)
            var stands = UnityEngine.Object.FindObjectsByType<Pickable>(FindObjectsSortMode.None)
                .Where(p => p.gameObject.scene.IsValid()
                         && p.m_itemPrefab == surtlingCore.gameObject
                         && Vector3.Distance(p.transform.position, playerPos) <= 80f)
                .ToList();

            if (stands.Count == 0)
            {
                return;
            }

            float roll = UnityEngine.Random.Range(0f, 100f);
            float chance = cryptSurtlingCoreChance != null ? cryptSurtlingCoreChance.Value : 30f;
            bool keepOne = roll < chance;

            // Keep only the first stand if roll succeeded; otherwise destroy all
            var toRemove = keepOne ? stands.Skip(1) : stands;
            foreach (var stand in toRemove)
            {
                var target = stand.gameObject;
                if (ZNetScene.instance != null)
                {
                    ZNetScene.instance.Destroy(target);
                }
                else
                {
                    UnityEngine.Object.Destroy(target);
                }
            }
        }
    }

    #endregion

    #region Death penalty

    // Replaces Valheim's default skill loss with level-based penalties.
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

            // Use Valheim's default percentage-based penalty below level 50.
            return currentLevel * (1f - factor);
        }
    }

    #endregion

    #region Dungeon darkness

    // Applies black ambient lighting and overrides Valheim's environment lighting indoors.
    [HarmonyPatch(typeof(EnvMan), "UpdateEnvironment")]
    public static class CryptEnvironmentPatch
    {
        public static bool IsDarkDungeon { get; private set; }
        private static bool darknessApplied;
        private static AmbientMode originalAmbientMode;
        private static SphericalHarmonicsL2 originalAmbientProbe;
        private static Color originalAmbientLight;
        private static float originalAmbientIntensity;
        private static float originalReflectionIntensity;
        private static Color originalFogColor;
        private static float originalFogDensity;
        private static EnvSetup darkEnvironment;
        private static Color originalEnvironmentAmbientColorDay;
        private static Color originalEnvironmentAmbientColorNight;
        private static float originalEnvironmentLightIntensityDay;
        private static float originalEnvironmentLightIntensityNight;
        private static Color originalEnvironmentFogColorDay;
        private static Color originalEnvironmentFogColorNight;
        private static float originalEnvironmentFogDensityDay;
        private static float originalEnvironmentFogDensityNight;

        static void Postfix(EnvMan __instance)
        {
            Refresh(__instance);
        }

        public static void Refresh(EnvMan environmentManager)
        {
            if (environmentManager == null || Player.m_localPlayer == null)
            {
                RestoreLighting();
                return;
            }

            if (!Player.m_localPlayer.InInterior())
            {
                RestoreLighting();
                return;
            }

            IsDarkDungeon = true;
            ApplyEnvironmentDarkness(environmentManager.GetCurrentEnvironment());
            ApplyDarkness();
            DungeonLightPatch.DisableDungeonLights();
            PlayerLightPatch.BoostPlayerLights();
        }

        private static void ApplyEnvironmentDarkness(EnvSetup environment)
        {
            if (environment == null || darkEnvironment == environment)
            {
                return;
            }

            RestoreEnvironmentLighting();
            darkEnvironment = environment;
            originalEnvironmentAmbientColorDay = environment.m_ambColorDay;
            originalEnvironmentAmbientColorNight = environment.m_ambColorNight;
            originalEnvironmentLightIntensityDay = environment.m_lightIntensityDay;
            originalEnvironmentLightIntensityNight = environment.m_lightIntensityNight;
            originalEnvironmentFogColorDay = environment.m_fogColorDay;
            originalEnvironmentFogColorNight = environment.m_fogColorNight;
            originalEnvironmentFogDensityDay = environment.m_fogDensityDay;
            originalEnvironmentFogDensityNight = environment.m_fogDensityNight;

            environment.m_ambColorDay = Color.black;
            environment.m_ambColorNight = Color.black;
            environment.m_lightIntensityDay = 0f;
            environment.m_lightIntensityNight = 0f;
            environment.m_fogColorDay = Color.black;
            environment.m_fogColorNight = Color.black;
            environment.m_fogDensityDay = 1f;
            environment.m_fogDensityNight = 1f;
        }

        private static void ApplyDarkness()
        {
            if (!darknessApplied)
            {
                originalAmbientMode = RenderSettings.ambientMode;
                originalAmbientProbe = RenderSettings.ambientProbe;
                originalAmbientLight = RenderSettings.ambientLight;
                originalAmbientIntensity = RenderSettings.ambientIntensity;
                originalReflectionIntensity = RenderSettings.reflectionIntensity;
                originalFogColor = RenderSettings.fogColor;
                originalFogDensity = RenderSettings.fogDensity;
                darknessApplied = true;
            }

            // Remove ambient and reflection light so dungeon walls do not glow.
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.black;
            RenderSettings.ambientIntensity = 0f;
            RenderSettings.ambientProbe = new SphericalHarmonicsL2();
            RenderSettings.reflectionIntensity = 0f;

            // Keep fog from adding a gray glow to the scene.
            RenderSettings.fogColor = Color.black;
            RenderSettings.fogDensity = 0.1f;
        }

        private static void RestoreLighting()
        {
            if (!darknessApplied)
            {
                return;
            }

            RenderSettings.ambientMode = originalAmbientMode;
            RenderSettings.ambientProbe = originalAmbientProbe;
            RenderSettings.ambientLight = originalAmbientLight;
            RenderSettings.ambientIntensity = originalAmbientIntensity;
            RenderSettings.reflectionIntensity = originalReflectionIntensity;
            RenderSettings.fogColor = originalFogColor;
            RenderSettings.fogDensity = originalFogDensity;
            RestoreEnvironmentLighting();
            PlayerLightPatch.RestorePlayerLights();
            darknessApplied = false;
            IsDarkDungeon = false;
        }

        private static void RestoreEnvironmentLighting()
        {
            if (darkEnvironment == null)
            {
                return;
            }

            darkEnvironment.m_ambColorDay = originalEnvironmentAmbientColorDay;
            darkEnvironment.m_ambColorNight = originalEnvironmentAmbientColorNight;
            darkEnvironment.m_lightIntensityDay = originalEnvironmentLightIntensityDay;
            darkEnvironment.m_lightIntensityNight = originalEnvironmentLightIntensityNight;
            darkEnvironment.m_fogColorDay = originalEnvironmentFogColorDay;
            darkEnvironment.m_fogColorNight = originalEnvironmentFogColorNight;
            darkEnvironment.m_fogDensityDay = originalEnvironmentFogDensityDay;
            darkEnvironment.m_fogDensityNight = originalEnvironmentFogDensityNight;
            darkEnvironment = null;
        }
    }

    #endregion

    #region Dungeon light cleanup

    // Disables dungeon light sources and their fire or smoke particles.
    public static class DungeonLightPatch
    {
        public static void DisableDungeonLights()
        {
            foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                DisableDungeonLight(light);
            }
        }

        private static void DisableDungeonLight(Light light)
        {
            if (light == null || !IsDungeonLight(light))
            {
                return;
            }

            light.enabled = false;
            light.intensity = 0f;

            foreach (var particleSystem in light.transform.root.GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Stop();
                particleSystem.Clear();
            }
        }

        public static bool IsDungeonLight(Light light)
        {
            string rootName = light.transform.root.name.ToLowerInvariant();
            if (rootName.Contains("player") || rootName.Contains("character"))
            {
                return false;
            }

            string objectName = light.gameObject.name.ToLowerInvariant();
            return rootName.Contains("torch")
                || rootName.Contains("sconce")
                || rootName.Contains("brazier")
                || rootName.Contains("fire")
                || rootName.Contains("flame")
                || objectName.Contains("torch")
                || objectName.Contains("sconce")
                || objectName.Contains("brazier")
                || objectName.Contains("fire")
                || objectName.Contains("flame");
        }
    }

    #endregion

    #region Player torch boost

    // Makes the local player's torch more useful in the forced darkness.
    public static class PlayerLightPatch
    {
        private const float IntensityMultiplier = 0.25f;
        private const float RangeMultiplier = 0.8f;
        private static readonly Dictionary<Light, LightSettings> originalLights = new();

        public static void BoostPlayerLights()
        {
            if (Player.m_localPlayer == null)
            {
                return;
            }

            foreach (var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None))
            {
                if (DungeonLightPatch.IsDungeonLight(light) || !IsPlayerLight(light))
                {
                    continue;
                }

                if (!originalLights.ContainsKey(light))
                {
                    originalLights[light] = new LightSettings(light.intensity, light.range);
                }

                var original = originalLights[light];
                light.intensity = original.Intensity * IntensityMultiplier;
                light.range = original.Range * RangeMultiplier;
            }
        }

        public static void BoostAttachedLights(Component component)
        {
            if (!CryptEnvironmentPatch.IsDarkDungeon
                || component == null
                || Player.m_localPlayer == null
                || !component.transform.IsChildOf(Player.m_localPlayer.transform))
            {
                return;
            }

            foreach (var light in component.GetComponentsInChildren<Light>(true))
            {
                if (DungeonLightPatch.IsDungeonLight(light))
                {
                    continue;
                }

                BoostLight(light);
            }
        }

        private static void BoostLight(Light light)
        {
            if (!originalLights.ContainsKey(light))
            {
                originalLights[light] = new LightSettings(light.intensity, light.range);
            }

            var original = originalLights[light];
            light.intensity = original.Intensity * IntensityMultiplier;
            light.range = original.Range * RangeMultiplier;
        }

        private static bool IsPlayerLight(Light light)
        {
            return Player.m_localPlayer != null
                && light.transform.IsChildOf(Player.m_localPlayer.transform);
        }

        public static void RestorePlayerLights()
        {
            foreach (var entry in originalLights)
            {
                if (entry.Key == null)
                {
                    continue;
                }

                entry.Key.intensity = entry.Value.Intensity;
                entry.Key.range = entry.Value.Range;
            }

            originalLights.Clear();
        }

        private readonly struct LightSettings
        {
            public LightSettings(float intensity, float range)
            {
                Intensity = intensity;
                Range = range;
            }

            public float Intensity { get; }
            public float Range { get; }
        }
    }

    #endregion

    #region Light reactivation guard

    // Dvergr lanterns expose their light through these components instead of the player hierarchy.
    [HarmonyPatch(typeof(LightFlicker), "Awake")]
    public static class LightFlickerPatch
    {
        static void Postfix(LightFlicker __instance)
        {
            PlayerLightPatch.BoostAttachedLights(__instance);
        }
    }

    [HarmonyPatch(typeof(ParticleIntensityScaler), "Start")]
    public static class ParticleLightPatch
    {
        static void Postfix(ParticleIntensityScaler __instance)
        {
            PlayerLightPatch.BoostAttachedLights(__instance);
        }
    }

    // Prevents Valheim from turning dungeon lights back on after they are disabled.
    [HarmonyPatch(typeof(Behaviour), "set_enabled")]
    public static class DungeonLightEnabledPatch
    {
        static void Prefix(Behaviour __instance, ref bool value)
        {
            if (value && CryptEnvironmentPatch.IsDarkDungeon
                && __instance is Light light
                && DungeonLightPatch.IsDungeonLight(light))
            {
                value = false;
            }
        }
    }

    #endregion

}