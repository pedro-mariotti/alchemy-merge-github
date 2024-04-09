using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    static AudioSource[] audioSources;
    public List<AudioClip> audioList;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
    }
    private void Update()
    {
        if (!audioSources[0].isPlaying)
        {
            // audioSources[0].enabled = false;
        }
        if (!audioSources[1].isPlaying)
        {
            audioSources[1].enabled = false;
        }
    }

    public void TriggerMergeSound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[0];
        audioSources[1].Play();
    }
    public void TriggerButtonSound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[1];
        audioSources[1].Play();
    }
    public void TriggerButtonSound2()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[8];
        audioSources[1].Play();
    }
    public void TriggerDealSound()
    {
        audioSources[1].enabled = true;
        Invoke("DealSound", .1f);
    }
    public void DealSound()
    {
        audioSources[1].clip = audioList[2];
        audioSources[1].Play();
    }
    public void TriggerLevelUpSound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[3];
        audioSources[1].Play();
    }

    public void TriggerMoneySound()
    {
        audioSources[1].enabled = true;
        Invoke("MoneySound", .5f);
    }
    public void MoneySound()
    {
        audioSources[1].clip = audioList[4];
        audioSources[1].Play();
    }
    public void TriggerNewElementSound()
    {
        audioSources[0].enabled = true;
        Invoke("NewElementSound", .5f);
    }
    public void NewElementSound()
    {
        audioSources[0].clip = audioList[5];
        audioSources[0].Play();
    }
    public void TriggerOpenWikiSound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[6];
        audioSources[1].Play();
    }
    public void TriggerTrashSound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[7];
        audioSources[1].Play();
    }
    public void TriggerFailMergeSound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[9];
        audioSources[1].Play();
    }
    public void TriggerBuySound()
    {
        audioSources[1].enabled = true;
        audioSources[1].clip = audioList[10];
        audioSources[1].Play();
    }

}
