using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	#region Editor Publics
	[SerializeField] private AudioClip OnTapBeganAudioClip;
	[SerializeField] private AudioClip OnTapEndAudioClip;
	[SerializeField] private AudioClip OnTargetSuccessHit;
	[SerializeField] private AudioClip OnNewTargetSpawned;
	[SerializeField] private AudioClip OnMiss;
	#endregion
	
	private float hitPitch = 1.0f;
	private float lastHitTime = 0;

	#region Public Methods
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
//		lastHitTime = hitTime;
//		hitTime = Time.time;

		if(Time.time - lastHitTime < 1.0f)
		{
			hitPitch *= 1.05f;
			PlaySound(OnTargetSuccessHit, hitPitch);
		}
		else
		{
			hitPitch = 1.0f;
			PlaySound(OnTargetSuccessHit);
		}

		lastHitTime = Time.time;
	}

	public void PlayNewTargetSpawned()
	{
		PlaySound(OnNewTargetSpawned);
	}

	public void PlayMissed()
	{
		PlaySound(OnMiss);
	}
	#endregion


	private void AdjustPitch()
	{

	}

	private void PlaySound(AudioClip audioClipName)
	{
		AudioSource tempAudioSource;
		tempAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
	
		tempAudioSource.clip = audioClipName;
		tempAudioSource.Play();

		Destroy(tempAudioSource, tempAudioSource.clip.length);
	}

	private void PlaySound(AudioClip audioClipName, float pitch)
	{
		AudioSource tempAudioSource;
		tempAudioSource = gameObject.AddComponent("AudioSource") as AudioSource;
		tempAudioSource.pitch = pitch;
		
		tempAudioSource.clip = audioClipName;
		tempAudioSource.Play();
		
		Destroy(tempAudioSource, tempAudioSource.clip.length);
	}
}