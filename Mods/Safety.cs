using GorillaLocomotion;
using StupidTemplate.Classes;
using StupidTemplate.Notifications;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using static StupidTemplate.Classes.RigManager;
using static StupidTemplate.Menu.Main;

namespace StupidTemplate.Mods
{
    public class Safety
    {
        public static VRRig reportRig;
        public static void AntiReport(System.Action<VRRig, Vector3> onReport)
        {
            if (!NetworkSystem.Instance.InRoom) return;

            if (reportRig != null)
            {
                onReport?.Invoke(reportRig, reportRig.transform.position);
                reportRig = null;
                return;
            }

            foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
            {
                if (line.linePlayer != NetworkSystem.Instance.LocalPlayer)
                    continue;

                Transform report = line.reportButton.transform;

                foreach (VRRig vrrig in UnityEngine.Object.FindObjectsOfType<VRRig>())
                {
                    if (vrrig.isLocal)
                        continue;

                    float D1 = Vector3.Distance(vrrig.rightHandTransform.position, report.position);
                    float D2 = Vector3.Distance(vrrig.leftHandTransform.position, report.position);

                    if (D1 < 0.35f || D2 < 0.35f)
                    {
                        onReport?.Invoke(vrrig, report.position);
                    }
                }
            }
        }

        public static float antiReportDelay;
        public static void AntiReportDisconnect()
        {
            AntiReport((vrrig, position) =>
            {
                NetworkSystem.Instance.ReturnToSinglePlayer();

                if (!(Time.time > antiReportDelay)) return;
                antiReportDelay = Time.time + 1f;
                NotifiLib.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected.");
            });
        }
    }
}
