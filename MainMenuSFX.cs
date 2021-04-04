using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;


public class MainMenuSFX : MonoBehaviour
{

    public AudioClip audio;
    public Vector3 vec;
    public SoundManager sm;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!Input.anyKeyDown){return;}

        sm.PlaySound(audio, vec, false);
    }
}
