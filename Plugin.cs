using System;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;
using Utilla;
namespace Lock
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject PadObj;
        public GameObject LightObj;

        float DistanceThingy = 0.1f;
        public bool active;
        bool inRoom;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            if (inRoom)
            {
                PadObj.SetActive(true);
                LightObj.SetActive(true);
            }

            active = true;

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            PadObj.SetActive(false);
            LightObj.SetActive(false);

            active = false;

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            var assetBundle = LoadAssetBundle("Lock.lock");
            GameObject Pad = assetBundle.LoadAsset<GameObject>("Base");
            GameObject Light = assetBundle.LoadAsset<GameObject>("Light");

            PadObj = Instantiate(Pad);

            LightObj = Instantiate(Light);

            PadObj.SetActive(false);
            LightObj.SetActive(false);
        }

        AssetBundle LoadAssetBundle(string path)
        {
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                stream.Close();
                Debug.Log("[Gorilla Lock] Success loading asset bundle");
                return bundle;
            }
            catch (Exception e)
            {
                Debug.Log("[Gorilla Lock] Error loading asset bundle: " + e.Message);
                throw;
            }
        }

        void Update()
        {
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            if (active)
            {
                PadObj.SetActive(true);
                LightObj.SetActive(true);
            }

            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            PadObj.SetActive(false);
            LightObj.SetActive(false);

            inRoom = false;
        }
    }
}