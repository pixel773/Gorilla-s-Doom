using BepInEx;
using System;
using System.ComponentModel;
using UnityEngine;
using Utilla;
using Player = GorillaLocomotion.Player;
using BepInEx.Configuration;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GorillaGoSmallGorillaGoBig
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.7")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [Description("HauntedModMenu")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public bool IsEnabled;
        public bool inRoom = false;
        public static ConfigEntry<float> Size;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        public void SetScale(float scale, bool isSmall) //This code was made by Dev(they/them). and is part of the Gorilla's Doom mod
        {
            Player.Instance.TryGetComponent(out SizeManager sizeManager);
            if (isSmall)
            {
                sizeManager.enabled = false;
                Player.Instance.scale = scale;
            }
            sizeManager.enabled = true;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/
            HarmonyPatches.ApplyHarmonyPatches();
            IsEnabled = true;

            if(inRoom == true)
            {
                SetScale(Size.Value, true);
            }
            else
            {
                SetScale(1, false);
            }
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            IsEnabled = false;

            SetScale(1, false);
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            var SizeChangeFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "GorillaGoSmall.cfg"), true);
            Size = SizeChangeFile.Bind("Configuration", "Size", 0.1f, "What size do you want to be?, 0.1 is the normal small scale");
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;

            if(IsEnabled == true)
            {
                SetScale(Size.Value, true);
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
            SetScale(1, false);
        }
    }
}
