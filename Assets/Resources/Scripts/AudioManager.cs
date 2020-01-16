using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] Audios;
    private AudioSource PlayerAudio;

    private void Start()
    {
        PlayerAudio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    public void PlayAudioEffect(int _num, float _val)
    {
        PlayerAudio.PlayOneShot(Audios[_num], _val);
    }
}
