using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	[SerializeField] private AudioClip OnTapBeganAudioClip;
	[SerializeField] private AudioClip OnTapEndAudioClip;
	[SerializeField] private AudioClip OnTargetSuccessHit;
	[SerializeField]
	private AudioClip OnNewTargetSpawned;
	
	public void PlayTouchBegan()
	{
		PlaySound(OnTapBeganAudioClip);
	}

	public void PlayTouchEnded()
	{
		PlaySound(OnTapEndAudioClip);
	}

	public void PlayTargetSuccessHit()
	{
		PlaySound(OnTargetSuccessHit);
	}

	public void PlayNewTargetSpawned()
	{
		PlaySound(OnNewTargetSpawned);
	}

	private void PlaySound(AudioClip audioClipName)
	{
		AudioSource tempAudioSource;
		tempAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
	
		tempAudioSource.clip = audioClipName;
		tempAudioSource.Play();

		Destroy(tempAudioSource, tempAudioSource.clip.length);
	}
}