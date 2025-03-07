﻿using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace MetalRecharging_Updated
{
    [HarmonyPatch(typeof(ItemCharger))]
    internal class ItemChargerPatch : NetworkBehaviour
    {

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static bool ItemChargerUpdate(ItemCharger __instance, ref float ___updateInterval, ref InteractTrigger ___triggerScript)
        {
            if (NetworkManager.Singleton == null) return false;
            if (___updateInterval > 1f)
            {
                ___updateInterval = 0;
                if (GameNetworkManager.Instance != null && GameNetworkManager.Instance.localPlayerController != null)
                {
                    var heldObject = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer;
                    if (heldObject == null || (!heldObject.itemProperties.isConductiveMetal && !heldObject.itemProperties.requiresBattery))
                    {
                        ___triggerScript.interactable = false;
                        ___triggerScript.disabledHoverTip = "(Requires battery-powered item)";
                    }
                    else
                    {
                        ___triggerScript.interactable = true;
                        // ___triggerScript.hoverTip = "Charge item : [LMB]";
                    }
                    ___triggerScript.twoHandedItemAllowed = true; // uuuuuuh this might break things idk. maybe do this in START or something
                    return false;
                }
            }
            ___updateInterval += Time.deltaTime;
            return false;
        }

        [HarmonyPatch("ChargeItem")]
        [HarmonyPostfix]
        static void ItemChargerCharge(ItemCharger __instance)
        {
            var heldObject = GameNetworkManager.Instance.localPlayerController.currentlyHeldObjectServer;
            if (heldObject != null && !heldObject.itemProperties.requiresBattery && heldObject.itemProperties.isConductiveMetal)
            {
                var localPlayer = GameNetworkManager.Instance.localPlayerController;
                //Landmine.SpawnExplosion(parent.position, effect, killrange, damagerange, 50, 0f, (GameObject)null, false);
                ((MonoBehaviour)__instance).StartCoroutine(KaboomCoroutine(localPlayer.transform.position));
                //LandminePatch.LastExplosionWasLocalPlayer = true;
                //ChatPatch.SendExplosionChat();
            }
        }
        private static IEnumerator KaboomCoroutine(Vector3 boomLocation)
        {
            yield return new WaitForSeconds(0.75f);
            Landmine.SpawnExplosion(boomLocation, true, 1f, 1f, 50, 0f, (GameObject)null, false);
        }
    }
}