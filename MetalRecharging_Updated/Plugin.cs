using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace MetalRecharging_Updated
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MRUPlugin : BaseUnityPlugin
    {
        private const string modGUID = "lynksdev.MetalRecharging_Updated";
        private const string modName = "Metal Recharing Updated";
        private const string modVersion = "1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static MRUPlugin instance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"Plugin {modGUID} is loaded!\n\n\n\n CHARGE YOUR METAL");

            harmony.PatchAll(typeof(MRUPlugin));
            harmony.PatchAll(typeof(ItemChargerPatch));
        }
    }
}
