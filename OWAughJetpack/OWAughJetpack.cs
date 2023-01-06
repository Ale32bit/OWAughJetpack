using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OWAughJetpack;

[HarmonyPatch]
public class OWAughJetpack : ModBehaviour
{
    public static OWAughJetpack Instance { get; private set; }
    public static OWAudioSource Augh;
    private void Awake()
    {
    }

    private void Start()
    {
        Instance = this;
        Instance.ModHelper.Console.WriteLine($"AUGH JetPack enabled!", MessageType.Success);

        Instance.ModHelper.HarmonyHelper.AddPrefix<JetpackThrusterAudio>("Update", typeof(OWAughJetpack), "Play");

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            if (loadScene != OWScene.SolarSystem) return;

            var playerBody = FindObjectOfType<PlayerBody>();
            GameObject gameObject = FindObjectOfType<PlayerBody>().gameObject;

            Augh = gameObject.AddComponent<OWAudioSource>();
            Augh.clip = ModHelper.Assets.GetAudio("Assets/augh.mp3");
            Augh.SetLocalVolume(1f);
            Augh.playOnAwake = false;
        };
    }

    public static void Play(JetpackThrusterAudio __instance)
    {
        bool flag = ((JetpackThrusterModel)__instance._thrusterModel).IsBoosterFiring();
        bool flag2 = __instance._playerResources.GetFuel() > 0f;
        float num = (flag ? 0f : __instance._thrusterModel.GetThrustFraction());
        float num2 = -__instance._thrusterModel.GetLocalAcceleration().x / __instance._thrusterModel.GetMaxTranslationalThrust() * 0.4f;
        __instance.UpdateTranslationalSource(__instance._translationalSource, num, num2, !__instance._underwater && flag2);
        __instance.UpdateTranslationalSource(__instance._underwaterSource, num, num2, __instance._underwater);
        __instance.UpdateTranslationalSource(__instance._oxygenSource, num, num2, !__instance._underwater && !flag2);
        if (!__instance._wasBoosting && flag)
        {
            Augh.FadeIn(0.1f, false, false, 1f);
        }
        else if (__instance._wasBoosting && !flag)
        {
            Augh.FadeOut(0.3f, OWAudioSource.FadeOutCompleteAction.STOP, 0f);
        }
        __instance._wasBoosting = flag;
        if (!__instance._thrustersFiring && !__instance._translationalSource.isPlaying && !__instance._underwaterSource.isPlaying && !__instance._oxygenSource.isPlaying && !flag && !__instance._wasBoosting)
        {
            __instance.enabled = false;
        }
    }
}