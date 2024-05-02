using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ư�� ����� ������ ��, �ش� ��ɿ� �´� audio clip�� ���
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // �̱��� ��ü
    AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // ĳ�� or ���۷���
    }

    public void PlayAudioClip(AudioClip clipName)
    {
        AudioClip clip = audioClips.Find(x => x == clipName);
        audioSource.clip = clip;
        audioSource.Play();
    }
}
