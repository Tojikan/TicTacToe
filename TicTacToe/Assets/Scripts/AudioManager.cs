using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For playing SFX and music
//Kept it pretty simple for now due to size. 
//TODO: a more robust and scalable audio manager as required
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;                                 //keep singleton pattern for simplicity
    public AudioClip click;                                                     //click sound
    public AudioClip victory;                                                   //victory sound             
    public AudioClip draw;                                                      //draw sound
    public AudioClip tileDrop;                                                  //tile drop sound
    private AudioSource soundPlayer;                                            //sFX player

    private void Awake()
    {
        soundPlayer = GetComponent<AudioSource>();

        //make singleton instance
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    //play a click sound when pressing buttons and laying down player pieces.
    public void PlayClick()
    {
        //stop playing sound, if any
        if (soundPlayer.isPlaying)
            soundPlayer.Stop();

        soundPlayer.PlayOneShot(click);
    }

    //victory sound
    public void PlayVictory()
    {
        if (soundPlayer.isPlaying)
            soundPlayer.Stop();

        soundPlayer.PlayOneShot(victory);
    }

    //draw sound
    public void PlayDraw()
    {
        if (soundPlayer.isPlaying)
            soundPlayer.Stop();

        soundPlayer.PlayOneShot(draw);
    }

    //tile drop sound
    public void PlayTileDrop()
    {
        if (soundPlayer.isPlaying)
            soundPlayer.Stop();

        soundPlayer.PlayOneShot(tileDrop);
    }
}
