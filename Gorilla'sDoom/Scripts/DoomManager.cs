using UnityEngine;
using GorillaLocomotion;
using System.Collections.Generic;

namespace GorillasDoom.Scripts
{
    public class DoomManager
    {
        public List<GameObject> affectedObjects = new List<GameObject>();
        public GameObject tempWatch;
        public GameObject watch;

        public void SetItem(GameObject obj, bool enabled)
        {
            if (!affectedObjects.Contains(obj)) affectedObjects.Add(obj);
            obj.SetActive(enabled);
        }

        public void SetScale(float scale, bool isSmall)
        {
            Player.Instance.TryGetComponent(out SizeManager sizeManager);
            if (isSmall)
            {
                sizeManager.enabled = false;
                Player.Instance.scale = scale; // I know you can go down to 0.03 and still play just fine it's just that I think it's a bit too small
                return;
            }

            sizeManager.enabled = true; // SizeManager will automatically set the Player's scale variable so no need to set it here if it's going to be overwritten
        }

        public void ResetGame()
        {
            for (int objectIndex = 0; objectIndex < affectedObjects.Count; objectIndex++)
            {
                GameObject objectFromIndex = affectedObjects[objectIndex];
                objectFromIndex.SetActive(true);
            }

            EnhanceSpeed(1, 1);
            ManageWatch(true);
            SetScale(1, false);
            ClearPlayer();
        }

        public void ClearPlayer()
        {
            GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.None, 0);
            Player.Instance.disableMovement = false;
        }

        public void SlowPlayer(int minSpeed, int maxSpeed)
        {
            float slowTime = new System.Random().Next(minSpeed, maxSpeed + 1);
            GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, slowTime);
        }

        public void EnhanceSpeed(float speedMulitplier, float jumpMulitplier)
        {
            Player.Instance.maxJumpSpeed *= speedMulitplier;
            Player.Instance.jumpMultiplier *= jumpMulitplier;
        }

        public void ManageWatch(bool link)
        {
            if (tempWatch == null) tempWatch = new GameObject("WatchTemp");
            if (link && watch != null)
            {
                GorillaTagger.Instance.offlineVRRig.huntComputer = watch;
                watch.GetComponentInChildren<GorillaHuntComputer>().enabled = true;
                GorillaHuntComputer localComputer = watch.GetComponentInChildren<GorillaHuntComputer>();
                localComputer.material.gameObject.SetActive(true);
                return;
            }

            watch = GorillaTagger.Instance.offlineVRRig.huntComputer;
            GorillaHuntComputer computer = watch.GetComponentInChildren<GorillaHuntComputer>();
            computer.enabled = false; // Disabling the component so the Update method can't do anything
            computer.hat.gameObject.SetActive(false);
            computer.face.gameObject.SetActive(false);
            computer.badge.gameObject.SetActive(false);
            computer.leftHand.gameObject.SetActive(false);
            computer.rightHand.gameObject.SetActive(false);
            computer.material.gameObject.SetActive(false);
            GorillaTagger.Instance.offlineVRRig.huntComputer = tempWatch;
        }

        public void SetWatchText(string text)
        {
            GorillaHuntComputer computer = watch?.GetComponentInChildren<GorillaHuntComputer>();
            if (computer != null) computer.text.text = text; // So much "text" it's making my brain hurt
        }

        public void FlingPlayer(float flingMulitplier)
        {
            Rigidbody playerBody = Player.Instance.GetComponent<Rigidbody>();   
            playerBody.AddForce(new Vector3(flingMulitplier * 2, Mathf.Abs(flingMulitplier) * 10, flingMulitplier * 2), ForceMode.VelocityChange);
        }
    }
}
