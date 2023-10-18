//Using
global using BepInEx;
global using BepInEx.IL2CPP;
global using UnityEngine;
global using UnityEngine.UI;
global using UnhollowerRuntimeLib;
global using HarmonyLib;
global using System;
using UnityEngine.UIElements;
using System.Linq;

namespace Exemple
{
    [BepInPlugin("D5C30D3C-EBE2-4A6D-B46C-8A9F216B9924", "NudeMod", "1.0.0")]
    public class Plugin : BasePlugin
    {
        public static bool nudeTrigger = true;
        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<BaseClass>();

            Harmony.CreateAndPatchAll(typeof(Plugin));
        }
        public class BaseClass : MonoBehaviour
        {
            bool init;
            DateTime start = DateTime.Now;
            Rigidbody clientBody = null;

            void Update()
            {
                DateTime end = DateTime.Now;
                if (nudeTrigger && !init)
                {
                    if ((end - start).TotalSeconds > 2)
                    {

                        GameObject[] onlinePlayers = GameObject.FindObjectsOfType<GameObject>().Where(go => go.name == "OnlinePlayer(Clone)").ToArray();

                        foreach (GameObject player in onlinePlayers)
                        {
                            Transform pants = player.transform.Find("PlayerModel/Pants");
                            if (pants != null)
                            {
                                Renderer pantsRenderer = pants.GetComponent<Renderer>();
                                if (pantsRenderer != null)
                                {
                                    pantsRenderer.enabled = false; // Cache l'objet
                                }
                            }
                            Transform sweater = player.transform.Find("PlayerModel/Sweater");
                            if (sweater != null)
                            {
                                Renderer sweaterRenderer = sweater.GetComponent<Renderer>();
                                if (sweaterRenderer != null)
                                {
                                    sweaterRenderer.enabled = false; // Cache l'objet
                                }
                            }
                            init = true;
                        }
                        
                    }
                }
            }
            public static string GetGameStateAsString()
            {
                return GameManager.Instance?.gameMode.modeState.ToString();
            }
            public static Rigidbody GetPlayerBody()
            {
                return GameObject.Find("/Player") == null ? null : GameObject.Find("/Player").GetComponent<Rigidbody>();
            }
        }
        public static GameObject GetPlayerObject()
        {
            return GameObject.Find("/Player");
        }
        public static PlayerManager GetPlayerManager()
        {
            return GetPlayerObject().GetComponent<PlayerManager>();
        }
        public static string GetPlayerUsernameAsString()
        {
            return GetPlayerManager() == null ? "N/A" : GetPlayerManager().username.ToString();
        }

        [HarmonyPatch(typeof(Chatbox), nameof(Chatbox.SendMessage))]
        [HarmonyPostfix]
        static void OnSendMessage(Chatbox __instance, string __0)
        {
            if (__0.ToLower() == "!nude on")
            {
                nudeTrigger = true;
                Chatbox.Instance.ForceMessage("■<color=yellow>Nude Mode <color=blue>ON</color></color>■");
            }
            if (__0.ToLower() == "!nude off")
            {
                nudeTrigger = false;
                Chatbox.Instance.ForceMessage("■<color=yellow>Nude Mode <color=red>OFF</color></color>■");
            }   
        }

        [HarmonyPatch(typeof(GameUI), "Awake")]
        [HarmonyPostfix]
        public static void UIAwakePatch(GameUI __instance)
        {
            GameObject menuObject = new GameObject();
            Text text = menuObject.AddComponent<Text>();

            BaseClass exemple = menuObject.AddComponent<BaseClass>();

            menuObject.transform.SetParent(__instance.transform);
            menuObject.transform.localPosition = new UnityEngine.Vector3(menuObject.transform.localPosition.x, -menuObject.transform.localPosition.y, menuObject.transform.localPosition.z);
            RectTransform rt = menuObject.GetComponent<RectTransform>();
            rt.pivot = new UnityEngine.Vector2(0, 1);
            rt.sizeDelta = new UnityEngine.Vector2(1, 1);
        }

        //Anticheat Bypass 
        [HarmonyPatch(typeof(EffectManager), "Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0")]
        [HarmonyPatch(typeof(LobbyManager), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(MonoBehaviourPublicVesnUnique), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(LobbySettings), "Method_Public_Void_PDM_2")]
        [HarmonyPatch(typeof(MonoBehaviourPublicTeplUnique), "Method_Private_Void_PDM_32")]
        [HarmonyPrefix]
        public static bool Prefix(System.Reflection.MethodBase __originalMethod)
        {
            return false;
        }
    }
}