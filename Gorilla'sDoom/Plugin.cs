using BepInEx;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilla;
using GorillasDoom.Scripts;
using GorillaNetworking;
using GorillaLocomotion;

namespace Gorilla_sDoom
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.7")]
    [ModdedGamemode("gorillasdoom", "DOOM", Utilla.Models.BaseGamemode.Infection)]
    [ModdedGamemode("gorillasdoomcasual", "DOOM CASUAL", Utilla.Models.BaseGamemode.Casual)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public DoomManager Manager { get; private set; }
        public bool InRoom { get; private set; }
        public Player GorillaPlayer;

        // Timers
        private float timeUntilNextTimer = 5f;
        private float timeUntilDecreaseCountdownTimer = 1;

        // Events
        private int currentEvent = 0;
        private int currentCountdown;
        private bool isCountingDown;

        private GameObject trees;
        private GameObject floor;
        private GameObject walls;
        private GameObject campgroundstructure;
        private GameObject SoundPostForest;
        private GameObject slide;
        private GameObject newTreehouse;
        private GameObject SnowMan;
        private GameObject Walljump;
        private GameObject Roof;

        private bool inForest;

        private readonly List<string> eventText = new List<string>()
        {
            "none",
            "hidden trees",
            "hidden floor",
            "hidden walls",
            "stunned",
            "minor fling",
            "major fling",
            "damage",
            "repair",
            "visible trees",
            "visible floor",
            "visible walls",
            "small scale",
            "normal scale",
            "large scale",
        };

        public Dictionary<string, bool> isGood = new Dictionary<string, bool>()
        {
            {"none", true },
            {"hidden trees", false },
            {"hidden floor", false },
            {"hidden walls", false },
            {"stunned", false },
            {"minor fling", false },
            {"major fling", false },
            {"damage", false },
            {"repair", true},
            {"visible trees", true },
            {"visible floor", true },
            {"visible walls", true },
            {"small scale", false },
            {"normal scale", true },
            {"large scale", true },
        };

        internal void Start()
        {
            Manager = new DoomManager();
            Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
            Manager.ResetGame();
        }

        private void OnGameInitialized(object sender, EventArgs e)
        {
            floor = GameObject.Find("pit ground");
            walls = GameObject.Find("pit wall");
            trees = GameObject.Find("SmallTrees");
            campgroundstructure = GameObject.Find("campgroundstructure");
            SoundPostForest = GameObject.Find("SoundPostForest");
            slide = GameObject.Find("slide");
            newTreehouse = GameObject.Find("newTreehouse (1)");
            Walljump = GameObject.Find("wallclimb");
            Roof = GameObject.Find("roof tiles snow");
            SnowMan = GameObject.Find("Snowman_Prefab");
        }

        internal void Update()
        {
            if (InRoom && inForest) // Make sure we're both in a room and in the forest map before handling timers
            {
                timeUntilNextTimer -= Time.deltaTime;

                if (timeUntilNextTimer < 0)
                {
                    currentEvent = new System.Random().Next(1, eventText.Count);
                    timeUntilNextTimer = int.MaxValue;
                    RandomizeMode();
                }

                if (isCountingDown)
                {
                    timeUntilDecreaseCountdownTimer -= Time.deltaTime;
                    if (timeUntilDecreaseCountdownTimer <= 0)
                    {
                        timeUntilDecreaseCountdownTimer = 1;

                        currentCountdown--;
                        Manager.SetWatchText("NEXT EVENT IN " + currentCountdown);
                        GorillaTagger.Instance?.offlineVRRig?.tagSound.PlayOneShot(GorillaTagger.Instance.offlineVRRig.clipToPlay[6]);
                    }
                }
            }
        }

        #region Event methods

        private void RandomizeMode()
        {
            isCountingDown = false;
            timeUntilDecreaseCountdownTimer = int.MaxValue;
            GorillaTagger.Instance?.offlineVRRig?.tagSound.PlayOneShot(GorillaTagger.Instance.offlineVRRig.clipToPlay[7]);
            Invoke("ReadjustTimer", new System.Random().Next(10, 15));
            string tex = $"NEW EVENT:\n<color=#{(!isGood[eventText[currentEvent]] ? "FF0000" : "00FF00")}ff>{eventText[currentEvent].ToUpper()}</color>";
            Manager.SetWatchText(tex);

            switch (currentEvent)
            {
                case 1:
                    Manager.SetItem(trees, false);
                    break;
                case 2:
                    Manager.SetItem(floor, false);
                    break;
                case 3:
                    Manager.SetItem(walls, false);
                    break;
                case 4:
                    Manager.SlowPlayer(10, 15);
                    break;
                case 5:
                    Manager.FlingPlayer(new System.Random().Next(0, 2) == 0 ? 1 : -1);
                    break;
                case 6:
                    Manager.FlingPlayer(new System.Random().Next(0, 2) == 0 ? 2 : -2);
                    break;
                case 7:
                    Manager.SetItem(walls, false);
                    Manager.SetItem(floor, false);
                    Manager.SetItem(trees, false);
                    Manager.SetItem(slide, false);
                    Manager.SetItem(campgroundstructure, false);
                    Manager.SetItem(SoundPostForest, false);
                    Manager.SetItem(newTreehouse, false);
                    Manager.SetItem(SnowMan, false);
                    Manager.SetItem(Walljump, false);
                    Manager.SetItem(Roof, false);

                    break;
                case 8:
                    Manager.SetItem(walls, true);
                    Manager.SetItem(floor, true);
                    Manager.SetItem(trees, true);
                    Manager.SetItem(slide, true);
                    Manager.SetItem(campgroundstructure, true);
                    Manager.SetItem(SoundPostForest, true);
                    Manager.SetItem(newTreehouse, true);
                    Manager.SetItem(SnowMan, true);
                    Manager.SetItem(Walljump, true);
                    Manager.SetItem(Roof, true);
                    break;
                case 9:
                    Manager.SetItem(trees, true);
                    break;
                case 10:
                    Manager.SetItem(floor, true);
                    break;
                case 11:
                    Manager.SetItem(walls, true);
                    break;
                case 12:
                    Manager.SetScale(0.5f, true);
                    break;
                case 13:
                    Manager.SetScale(1, false);
                    break;
                case 14:
                    Manager.SetScale(2, true);
                    break;
            }
        }

        internal void ReadjustTimer()
        {
            timeUntilDecreaseCountdownTimer = 1;
            timeUntilNextTimer = 5;
            currentCountdown = (int)timeUntilNextTimer;
            isCountingDown = true;
            Manager.SetWatchText("NEXT EVENT IN " + currentCountdown);
            GorillaTagger.Instance?.offlineVRRig?.tagSound.PlayOneShot(GorillaTagger.Instance.offlineVRRig.clipToPlay[6]);
        }

        #endregion
        #region Modded room methods

        [ModdedGamemodeJoin]
        internal void OnJoin(string gamemode)
        {
            if ((gamemode.Contains("gorillasdoom") || gamemode.Contains("gorillasdoomcasual")) && Photon.Pun.PhotonNetwork.CurrentRoom.IsVisible)
            {
                InRoom = true;
                Manager.ManageWatch(false);
                Manager.watch.SetActive(true);
                isCountingDown = false;
                inForest = PhotonNetworkController.Instance.currentJoinTrigger.gameModeName == "forest";
                if (inForest) Invoke("ReadjustTimer", 0);
                else Manager.SetWatchText("PLEASE GO IN THE FOREST MAP. OTHER MAPS ARE NOT SUPPORTED YET!");
            }
        }

        [ModdedGamemodeLeave]
        internal void OnLeave(string gamemode)
        {
            InRoom = false;
            Manager.ResetGame();
        }

        #endregion
    }
}
