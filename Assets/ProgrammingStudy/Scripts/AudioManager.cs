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
        // ���۽� �÷���Ÿ�� Ȯ��, Ư�� playTimeRange��ŭ ����� �Ǹ� ����.
    }

    public void SetPlayTime(float playTimeRange)
    {
        StartCoroutine(CheckPlaytime(playTimeRange));
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }


    IEnumerator CheckPlaytime(float playTimeRange)
    {
        while (true)
        {
            if(audioSource.time > playTimeRange)
            {
                audioSource.Stop();
                audioSource.time = 0;
                break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
