using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    private static AudioMgr instance;
    public static AudioMgr Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        instance = this;
    }

    public AudioSource lobbyMusic;
    public AudioSource waitingMusic;
    public AudioSource selectMusic;
    public AudioSource inGameMusic;
}
