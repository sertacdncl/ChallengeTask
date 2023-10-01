using UnityEngine;

namespace TaskTwo.Audio
{
	public class AudioManager : MonoBehaviour
	{
		public AudioSource sfxAudioSource;
		[SerializeField] private AudioClip _noteClip;
		[SerializeField] private AudioClip _failClip;
		[SerializeField] private AudioClip _confettiClip;
		
		public void PlayNote(float pitch)
		{
			sfxAudioSource.pitch = pitch;
			sfxAudioSource.PlayOneShot(_noteClip);
		}
		
		public void PlayFail()
		{
			sfxAudioSource.Stop();
			sfxAudioSource.pitch = 1f;
			sfxAudioSource.PlayOneShot(_failClip);
		}

		public void PlayConfetti()
		{
			sfxAudioSource.pitch = 1f;
			sfxAudioSource.PlayOneShot(_confettiClip);
		}
	}
}