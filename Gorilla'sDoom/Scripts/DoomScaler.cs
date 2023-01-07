using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace GorillasDoom.Scripts
{
    public class DoomScaler : MonoBehaviourPunCallbacks
    {
        public VRRig playerRig;

        public void Start()
        {
            if (playerRig == null || playerRig.isOfflineVRRig) Destroy(this);
            Invoke("LateStart", 1);
        }

        public void LateStart()
        {
            if (playerRig.photonView.Owner.CustomProperties.ContainsKey("DoomPlayerSize") && Plugin.Instance.InRoom && Plugin.Instance.inForest)
            {
                playerRig.scaleFactor = (float)playerRig.photonView.Owner.CustomProperties["DoomPlayerSize"];
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer.CustomProperties.ContainsKey("DoomPlayerSize") && Plugin.Instance.InRoom && Plugin.Instance.inForest)
            {
                playerRig.scaleFactor = (float)targetPlayer.CustomProperties["DoomPlayerSize"];
            }
        }
    }
}
