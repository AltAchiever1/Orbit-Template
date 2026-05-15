using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace StupidTemplate.Classes
{
    public class RigManager
    {
        public static VRRig GetVRRigFromPlayer(Player p) =>
            GorillaGameManager.instance.FindPlayerVRRig(p);

        public static VRRig GetRandomVRRig(bool includeSelf)
        {
            VRRig[] rigs = UnityEngine.Object.FindObjectsOfType<VRRig>();

            List<VRRig> validRigs = new List<VRRig>();

            foreach (VRRig rig in rigs)
            {
                if (includeSelf || rig != VRRig.LocalRig)
                    validRigs.Add(rig);
            }

            if (validRigs.Count == 0)
                return null;

            return validRigs[Random.Range(0, validRigs.Count)];
        }

        public static VRRig GetClosestVRRig()
        {
            float minDistance = float.MaxValue;
            VRRig closestRig = null;

            VRRig[] rigs = UnityEngine.Object.FindObjectsOfType<VRRig>();

            foreach (VRRig vrrig in rigs)
            {
                if (vrrig == VRRig.LocalRig)
                    continue;

                float distance = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestRig = vrrig;
                }
            }

            return closestRig;
        }

        public static PhotonView GetPhotonViewFromVRRig(VRRig p) =>
            (PhotonView)Traverse.Create(p).Field("photonView").GetValue();

        public static Player GetRandomPlayer(bool includeSelf)
        {
            if (includeSelf)
                return PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length - 1)];
            else
                return PhotonNetwork.PlayerListOthers[Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)];
        }

        public static Player GetPlayerFromVRRig(VRRig p) =>
            GetPhotonViewFromVRRig(p).Owner;

        public static Player GetPlayerFromID(string id)
        {
            Player found = null;
            foreach (Player target in PhotonNetwork.PlayerList)
            {
                if (target.UserId == id)
                {
                    found = target;
                    break;
                }
            }
            return found;
        }

        public static Color GetPlayerColor(VRRig Player)
        {
            if (Player != null && Player.transform.Find("BodyMesh")?.name.ToLower().Contains("skeleton") == true)
                return Color.green;

            switch (Player.setMatIndex)
            {
                case 1:
                    return Color.red;
                case 2:
                case 11:
                    return new Color32(255, 128, 0, 255);
                case 3:
                case 7:
                    return Color.blue;
                case 12:
                    return Color.green;
                default:
                    return Player.playerColor;
            }
        }
    }
}