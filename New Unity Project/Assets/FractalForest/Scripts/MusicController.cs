using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
	// The music clip you want to play. 
	//The [SerializeField] tag specifies that this variable is viewable 
	// in Unity's inspector. I prefer not to use public variables if 
	//I can get away with using private ones.
	public AudioClip audioLoop; 

	// Use this for initialization
	void Start () {

		// Get my AudioSource component and store a reference to it in audio
		// The point of doing this is because GetComponent() is expensive for computer resources
		// So if we can get away with only calling it one time at the start, then let's do that.
		// From this point on, we can refer to our AudioSource through audio, which makes the computer happier than GetComponent.
		//		audio = GetComponent<AudioSource>();

		// We set the audio clip to play as your background music.
		GetComponent<AudioSource>().clip = audioLoop;

	}
	/*********************/
	/* Public Interface */
	/*********************/

	protected void PlayPauseMusic()
	{
		// Check if the music is currently playing.
		if(GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Pause(); // Pause if it is
		else
			GetComponent<AudioSource>().Play(); // Play if it isn't
	}

	public void PlayStop()
	{
		if(GetComponent<AudioSource>().isPlaying)
			GetComponent<AudioSource>().Stop();
		else
			GetComponent<AudioSource>().Play();
	}

	public void PlayMusic()
	{  
		GetComponent<AudioSource>().Play();
	}

	public void StopMusic()
	{
		GetComponent<AudioSource>().Stop();
	}

	public void PauseMusic()
	{
		GetComponent<AudioSource>().Pause();
	}
	// Update is called once per frame
	//	void Update () {
	//		
	//	}
}