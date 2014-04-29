using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class AutoDestroyParticles : MonoBehaviour 
{
	void Update()
	{
		if(particleSystem.isPlaying == false)
			Destroy(gameObject);
	}
}
