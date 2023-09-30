using UnityEngine;

namespace TaskTwo.Audio
{
	public class AudioManager : MonoBehaviour
	{
		public AudioSource sfxAudioSource;
		[SerializeField] private AudioClip _noteClip;
		
		public void PlayNote(float pitch)
		{
			sfxAudioSource.pitch = pitch;
			sfxAudioSource.PlayOneShot(_noteClip);
		}
	}
}