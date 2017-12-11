using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantShaderController : MonoBehaviour {

    private Material originalMaterial; 
    private AudioSource attachedAudio;

    // Use this for initialization
    void Start () {
        originalMaterial = gameObject.GetComponent<Renderer>().material;

        attachedAudio = gameObject.GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!attachedAudio.isPlaying)
        {
            gameObject.GetComponent<Renderer>().material = originalMaterial;
        }
    }
}
