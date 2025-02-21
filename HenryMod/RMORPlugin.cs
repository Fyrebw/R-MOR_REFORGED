﻿using BepInEx;
using RMORMod.Content.Shared.Components.Body;
using RMORMod.Content.RMORSurvivor;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

//rename this namespace
namespace RMORMod
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Kingpinush.KingKombatArena", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "SoundAPI",
        "UnlockableAPI",
        "RecalculateStatsAPI",
        "DamageAPI"
    })]

    public class RMORPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.MoriyaLuna.RMORReforged";
        public const string MODNAME = "RMOR Reforged";
        public const string MODVERSION = "1.0.0";

        public const string DEVELOPER_PREFIX = "MORIYA";

        public static RMORPlugin instance;
        public static PluginInfo pluginInfo;

        public static bool ScepterStandaloneLoaded = false;
        public static bool ScepterClassicLoaded = false;
        public static bool EmoteAPILoaded = false;
        public static bool ArenaPluginLoaded = false;
        public static bool ArenaModeActive = false;
        public static bool RiskOfOptionsLoaded = false;

        private void Awake()
        {
            pluginInfo = Info;
            instance = this;

            CheckDependencies();
            Modules.Config.ReadConfig();

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles

            new LanguageTokens();
            // survivor initialization
            //new MyCharacter().Initialize();

            new Content.Shared.SharedContent();
            Content.DamageTypes.Initialize();

            //new HANDSurvivor().Initialize();
            new RMORSurvivor().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            if (EmoteAPILoaded) EmoteAPICompat();
            if (ArenaPluginLoaded)
            {
                Stage.onStageStartGlobal += SetArena;
            }
            RoR2.RoR2Application.onLoad += AddMechanicalBodies;
        }

        private void AddMechanicalBodies()
        {
            BodyIndex sniperClassicIndex = BodyCatalog.FindBodyIndex("SniperClassicBody");
            if (sniperClassicIndex != BodyIndex.None)
            {
                DroneStockController.mechanicalBodies.Add(sniperClassicIndex);
            }
        }

        private void CheckDependencies()
        {
            ScepterStandaloneLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            ScepterClassicLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            EmoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            ArenaPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena");
            RiskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetArena(Stage obj)
        {
            RMORPlugin.ArenaModeActive = NS_KingKombatArena.KingKombatArenaMainPlugin.s_GAME_MODE_ACTIVE;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "HANDOverclockedBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animHANDEmote.prefab");
                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                    }
                }
            };
        }
    }
}