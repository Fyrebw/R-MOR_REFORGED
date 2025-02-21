﻿using BepInEx.Configuration;
using RiskOfOptions;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RMORMod.Modules
{
    public static class Config
    {
        public static bool forceUnlock = false;
        public static bool allowPlayerRepair = false;
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteCSS;
        public static ConfigEntry<KeyboardShortcut> KeybindEmote1;
        public static ConfigEntry<KeyboardShortcut> KeybindEmote2;
        public static float sortPosition = 7.51f;

        public static void ReadConfig()
        {
            sortPosition = RMORPlugin.instance.Config.Bind("General", "Survivor Sort Position", 4.51f, "Controls where R-MOR is placed in the character select screen.").Value;
            forceUnlock = RMORPlugin.instance.Config.Bind("General", "Force Unlock", false, "Automatically unlock R-MOR and his skills by default.").Value;

            KeybindEmote1 = RMORPlugin.instance.Config.Bind("Keybinds", "Emote - Sit", new KeyboardShortcut(KeyCode.Alpha1), "Button to play this emote.");
            KeybindEmote2 = RMORPlugin.instance.Config.Bind("Keybinds", "Emote - Malfunction", new KeyboardShortcut(KeyCode.Alpha2), "Button to play this emote.");
            KeybindEmoteCSS = RMORPlugin.instance.Config.Bind("Keybinds", "Emote - Hat Tip", new KeyboardShortcut(KeyCode.Alpha3), "Button to play this emote.");

            if (RMORPlugin.RiskOfOptionsLoaded)
            {
                RiskOfOptionsCompat();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void RiskOfOptionsCompat()
        {
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmote1));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmote2));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmoteCSS));
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return RMORPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}