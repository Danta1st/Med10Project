using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	[SerializeField]
	private AudioClip OnTapBeganAudioClip;
	[SerializeField]
	private AudioClip OnTapEndAudioClip;
	
	public void PlayTouchBegan()
	{
		PlaySound(OnTapBeganAudioClip);
	}

	public void PlayTouchEnded()
	{
		PlaySound(OnTapEndAudioClip);
	}

	public void PlaySound(AudioClip audioClipName)
	{
		AudioSource tempAudioSource;
		tempAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
	
		tempAudioSource.clip = audioClipName;
		tempAudioSource.Play();

		Destroy(tempAudioSource, tempAudioSource.clip.length);
	}
}