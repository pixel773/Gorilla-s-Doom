using BepInEx;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilla;
using GorillasDoom.Scripts;
using GorillaNetworking;

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
        private GameObject campfire;
        private GameObject slide;
        private GameObject christmastree_prefab;
        private GameObject newTreehouse;

        private bool inForest;

        //I wasted 15 mins doing this :O
        private GameObject flatpanel5;
        private GameObject flatpanel9;
        private GameObject flatpanel10;
        private GameObject flatpanel11;
        private GameObject flatpanel12;
        private GameObject flatpanel13;
        private GameObject flatpanel14;
        private GameObject flatpanel15;
        private GameObject flatpanel16;
        private GameObject flatpanel17;

        private readonly List<string> eventText = new List<string>()
        {
            "none",
            "hidden trees",
            "hidden floor",
            "hidden walls",
            "stunned",
            "minor fling",
            "major fling",
            "faster speed",
            "visible trees",
            "visible floor",
            "visible walls",
            "small scale",
            "normal scale",
            "large scale",
            "hidden forest objects",
            "visible forest objects"
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
            {"faster speed", true },
            {"visible trees", true },
            {"visible floor", true },
            {"visible walls", true },
            {"small scale", false },
            {"normal scale", true },
            {"large scale", true },
            {"hidden forest objects", false },
            {"visible forest objects", true }
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
            campfire = GameObject.Find("campfire");
            slide = GameObject.Find("slide");
            christmastree_prefab = GameObject.Find("christmastree_prefab");
            newTreehouse = GameObject.Find("newTreehouse (1)");


            //I wasted 15 mins doing this :O
            flatpanel5 = GameObject.Find("flatpanel(5)");
            flatpanel9 = GameObject.Find("flatpanel(9)");
            flatpanel10 = GameObject.Find("flatpanel(10)");
            flatpanel11 = GameObject.Find("flatpanel(11)");
            flatpanel12 = GameObject.Find("flatpanel(12)");
            flatpanel13 = GameObject.Find("flatpanel(13)");
            flatpanel14 = GameObject.Find("flatpanel(14)");
            flatpanel15 = GameObject.Find("flatpanel(15)");
            flatpanel16 = GameObject.Find("flatpanel(16)");
            flatpanel17 = GameObject.Find("flatpanel(17)");
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
                    Manager.EnhanceSpeed(8, 1.5f);
                    break;
                case 8:
                    Manager.SetItem(trees, true);
                    break;
                case 9:
                    Manager.SetItem(floor, true);
                    break;
                case 10:
                    Manager.SetItem(walls, true);
                    break;
                case 11:
                    Manager.SetScale(0.5f, true);
                    break;
                case 12:
                    Manager.SetScale(1, false);
                    break;
                case 13:
                    Manager.SetScale(2, true);
                    break;
                case 14:
                    Manager.SetItem(slide, false);
                    Manager.SetItem(campfire, false);
                    Manager.SetItem(campgroundstructure, false);
                    Manager.SetItem(SoundPostForest, false);
                    Manager.SetItem(christmastree_prefab, false);
                    Manager.SetItem(newTreehouse, false);
                    Manager.SetItem(flatpanel5, false);
                    Manager.SetItem(flatpanel9, false);
                    Manager.SetItem(flatpanel10, false);
                    Manager.SetItem(flatpanel11, false);
                    Manager.SetItem(flatpanel12, false);
                    Manager.SetItem(flatpanel13, false);
                    Manager.SetItem(flatpanel14, false);
                    Manager.SetItem(flatpanel15, false);
                    Manager.SetItem(flatpanel16, false);
                    Manager.SetItem(flatpanel17, false);
                    break;

                case 15:
                    Manager.SetItem(slide, true);
                    Manager.SetItem(campfire, true);
                    Manager.SetItem(campgroundstructure, true);
                    Manager.SetItem(SoundPostForest, true);
                    Manager.SetItem(christmastree_prefab, true);
                    Manager.SetItem(newTreehouse, true);

                    Manager.SetItem(flatpanel5, true);
                    Manager.SetItem(flatpanel9, true);
                    Manager.SetItem(flatpanel10, true);
                    Manager.SetItem(flatpanel11, true);
                    Manager.SetItem(flatpanel12, true);
                    Manager.SetItem(flatpanel13, true);
                    Manager.SetItem(flatpanel14, true);
                    Manager.SetItem(flatpanel15, true);
                    Manager.SetItem(flatpanel16, true);
                    Manager.SetItem(flatpanel17, true);
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
                else Manager.SetWatchText("PLEASE GO IN THE FOREST MAP.");
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
