using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OWAughJetpack;

[HarmonyPatch]
public class OWAughJetpack : ModBehaviour
{
    public static OWAughJetpack Instance { get; private set; }

    private void Awake()
    {
    }

    private void Start()
    {
        Instance = this;
        Instance.ModHelper.Console.WriteLine($"AUGH JetPack enabled!", MessageType.Success);
        
        GetClip("Assets/augh.mp3");

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) => 
        {
            StartCoroutine(PatchAudio());
        };
    }

    private IEnumerator PatchAudio() 
    {
        yield return new WaitForSecondsRealtime(1);

        Dictionary<int, AudioLibrary.AudioEntry> dict = ((Dictionary<int, AudioLibrary.AudioEntry>)AccessTools.Field(typeof(AudioManager), "_audioLibraryDict").GetValue(Locator.GetAudioManager()));

        Instance.PatchAudioType(dict, AudioType.PlayerSuitJetpackBoost, "Assets/augh.mp3");        
        
        Instance.ModHelper.Console.WriteLine($"All sounds patched!", MessageType.Success);
    }
    
    
    public Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();
    private AudioClip GetClip(string name) 
    {
        if (Instance.Sounds.ContainsKey(name)) { return Instance.Sounds[name]; }
        AudioClip audioClip = ModHelper.Assets.GetAudio(name);
        Instance.Sounds.Add(name, audioClip);
        return audioClip;
    }

    private void PatchAudioType(Dictionary<int, AudioLibrary.AudioEntry> dict, AudioType type, string name)
    {
        AudioLibrary.AudioEntry entry = new AudioLibrary.AudioEntry(type, GetClip(name), 0.5f);
        try {
            dict[(int)type] = entry;
        } catch {
            dict.Add((int)type, entry);
        }
    }
}
