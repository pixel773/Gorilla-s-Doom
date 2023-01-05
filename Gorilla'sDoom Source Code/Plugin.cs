using BepInEx;
using System;
using UnityEngine;
using Utilla;
using System.Linq;
using Random = UnityEngine.Random;
namespace Gorilla_sDoom
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.7")]
    [ModdedGamemode("GORILLADOOM", "GORILLADOOM", Utilla.Models.BaseGamemode.Infection)]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        public float Timer = 5f;
        public int Event = 0;
        public bool IsEnabled;
        public bool HasSpeed;

        public GameObject Trees;
        public GameObject ForestFloor;
        public GameObject ForestWalls;

        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
            IsEnabled = true;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();

            IsEnabled = false;

            Trees.SetActive(true);
            ForestFloor.SetActive(true);
            ForestWalls.SetActive(true);

            GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
            GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
            GameObject.Find("GorillaPlayer").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            HasSpeed = false;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */

            ForestFloor = GameObject.Find("pit ground");
            ForestWalls = GameObject.Find("pit wall");
            Trees = GameObject.Find("SmallTrees");
        }
        //var TemplateVar = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "TemplateName");

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
            if(inRoom == true)
            {
                Timer -= Time.deltaTime;

                if (Timer < 0)
                {
                    Event = Random.Range(1, 10);
                    Timer = 10f;
                    TimerDone();
                }
            }

            if(HasSpeed == true)
            {
                GorillaLocomotion.Player.Instance.maxJumpSpeed = 10f;
                GorillaLocomotion.Player.Instance.jumpMultiplier = 1.4f;
            }
        }

        public void TimerDone()
        {
            Update();

            if (Event == 1)
            {
                BADEVENT_GetRidOfTrees();
                Debug.Log("Got Rid Of Trees");
            }

            if (Event == 2)
            {
                BADEVENT_RemoveFloor();
                Debug.Log("Removed Floor");
            }

            if (Event == 3)
            {
                BADEVENT_RemoveWalls();
                Debug.Log("Removed Walls");
            }

            if (Event == 4)
            {
                BADEVENT_FreezePlayer();
                Debug.Log("Froze Player");
            }

            if (Event == 5)
            {
                BADEVENT_CanOnlyMoveInOneWay();
                Debug.Log("Haha you can only move in one way");
            }

            if (Event == 6)
            {
                BADEVENT_YeetPlayer();
                Debug.Log("You have been Yeeted");
            }

            if (Event == 7)
            {
                GOODEVENT_GetSpeed();
                Debug.Log("I'm sooooo Fast now :O");
            }

            if (Event == 8)
            {
                GOODEVENT_BringBackTrees();
                Debug.Log("Here are your trees back :)");
            }

            if (Event == 9)
            {
                GOODEVENT_BringBackGround();
                Debug.Log("Here is your ground back :)");
            }

            if (Event == 10)
            {
                GOODEVENT_BringBackWalls();
                Debug.Log("here are your walls back :)");
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            if (gamemode.Contains("GORILLADOOM"))
            {
                inRoom = true;
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            if (gamemode.Contains("GORILLADOOM"))
            {
                inRoom = false;
                LEAVEROOM_FixEverything();
            }
        }








        public void LEAVEROOM_FixEverything()
        {
            Trees.SetActive(true);
            ForestFloor.SetActive(true);
            ForestWalls.SetActive(true);

            GorillaLocomotion.Player.Instance.maxJumpSpeed = 6.5f;
            GorillaLocomotion.Player.Instance.jumpMultiplier = 1.1f;
            GameObject.Find("GorillaPlayer").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            HasSpeed = false;
        }

        public void BADEVENT_GetRidOfTrees()
        {
            if(inRoom == true)
            {
                Trees.SetActive(false);
            }
        }

        public void BADEVENT_CanOnlyMoveInOneWay()
        {
            if (inRoom == true)
            {
                GameObject.Find("GorillaPlayer").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
            }
        }

        public float FreezePlayerTimer;
        public GorillaTagger TaggerHandler;

        public void BADEVENT_FreezePlayer()
        {
            if(inRoom == true)
            {
                TaggerHandler = GameObject.Find("GorillaPlayer").GetComponent<GorillaTagger>();

                FreezePlayerTimer = Random.Range(5, 15);

                TaggerHandler.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, duration:FreezePlayerTimer);
            }
        }

        public void BADEVENT_RemoveFloor()
        {
            if(inRoom == true)
            {
                ForestFloor.SetActive(false);
            }
        }

        public void BADEVENT_RemoveWalls()
        {
            if(inRoom == true)
            {
                ForestWalls.SetActive(false);
            }
        }

        public Vector3 force;

        public void BADEVENT_YeetPlayer()
        {
            if(inRoom == true)
            {
                force.y = Random.Range(10, 15);
                force.z = Random.Range(10, 15);
                force.x = Random.Range(10, 15);

                GameObject.Find("GorillaPlayer").GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange); 
            }
        }

        public void GOODEVENT_GetSpeed()
        {
            if (inRoom == true)
            {
                HasSpeed = true;
            }
        }

        public void GOODEVENT_BringBackTrees()
        {
            if (inRoom == true)
            {
                Trees.SetActive(true);
            }
        }

        public void GOODEVENT_BringBackGround()
        {
            if (inRoom == true)
            {
                ForestFloor.SetActive(true);
            }
        }

        public void GOODEVENT_BringBackWalls()
        {
            if (inRoom == true)
            {
                ForestWalls.SetActive(true);
            }
        }
    }
}
