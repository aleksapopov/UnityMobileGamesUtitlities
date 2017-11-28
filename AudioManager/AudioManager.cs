using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Simple class for handling background music and sound effects playback.
/// Works as a singleton in all scenes.
/// Using simple pool of 10 soundsources for sound effects playbeck (like shooting or coins collection)
/// To be added as a compoennt to yje GameObject in starting scene,
/// </summary>
public class AudioManager : MonoBehaviour {

	public AudioClip[] EmbientSounds;
	public AudioClip[] SoundEffects;

	public GameObject EmbientAudioSourceGO;
	public GameObject EffectsAudioSource1GO;
	public GameObject EffectsAudioSource2GO;
	public GameObject EffectsAudioSource3GO;
	public GameObject EffectsAudioSource4GO;
	public GameObject EffectsAudioSource5GO;
	public GameObject EffectsAudioSource6GO;
	public GameObject EffectsAudioSource7GO;
	public GameObject EffectsAudioSource8GO;
	public GameObject EffectsAudioSource9GO;
	public GameObject EffectsAudioSource10GO;
	public GameObject EffectsAudioSourceSpecialGO;

	private AudioSource _embientAudioSource;
	private AudioSource _effectsAudioSource1;
	private AudioSource _effectsAudioSource2;
	private AudioSource _effectsAudioSource3;
	private AudioSource _effectsAudioSource4;
	private AudioSource _effectsAudioSource5;
	private AudioSource _effectsAudioSource6;
	private AudioSource _effectsAudioSource7;
	private AudioSource _effectsAudioSource8;
	private AudioSource _effectsAudioSource9;
	private AudioSource _effectsAudioSource10;
	private AudioSource _effectsAudioSourceSpecial;

	private Coroutine _startSoundCoroutine;

	private bool _isEmbientSoundOn = true;
	public bool IsEmbientSoundsOn {
		set { 
			_isEmbientSoundOn = value;
			_embientAudioSource.volume = value ? _defaultEmbientValume : 0f;
		}

		get { return _isEmbientSoundOn; }
	}

	private bool _isSoundEffectsOn = true;
	public bool IsSoundEffectsOn {
		set { 
			_isSoundEffectsOn = value;
			_effectsAudioSource1.volume = value ? _defaultEffectsVolume : 0f;
			_effectsAudioSource2.volume = value ? _defaultEffectsVolume : 0f;
			_effectsAudioSource3.volume = value ? _defaultEffectsVolume : 0f;
		}

		get { return _isSoundEffectsOn; }
	}

	private float _defaultEmbientValume = .75f;
	private float _defaultEffectsVolume = 1f;
	private float _defaultEmbientPitch = 1f;
	private float _defaultEffectsPitch = 1f;

	private float _crossFadeDuration = .4f;
	private float _minCrossFadeVol = .5f;
	private float _maxCrossFadeVol = 1f;

	public static AudioManager Instance;

	void Awake () {
		if (Instance == null) {
			Instance = this;
			_effectsAudioSource1 = EffectsAudioSource1GO.GetComponent<AudioSource> ();
			_effectsAudioSource2 = EffectsAudioSource2GO.GetComponent<AudioSource> ();
			_effectsAudioSource3 = EffectsAudioSource3GO.GetComponent<AudioSource> ();
			_effectsAudioSource4 = EffectsAudioSource4GO.GetComponent<AudioSource> ();
			_effectsAudioSource5 = EffectsAudioSource5GO.GetComponent<AudioSource> ();
			_effectsAudioSource6 = EffectsAudioSource6GO.GetComponent<AudioSource> ();
			_effectsAudioSource7 = EffectsAudioSource7GO.GetComponent<AudioSource> ();
			_effectsAudioSource8 = EffectsAudioSource8GO.GetComponent<AudioSource> ();
			_effectsAudioSource9 = EffectsAudioSource9GO.GetComponent<AudioSource> ();
			_effectsAudioSource10 = EffectsAudioSource10GO.GetComponent<AudioSource> ();
			_effectsAudioSourceSpecial = EffectsAudioSourceSpecialGO.GetComponent<AudioSource> ();
			_embientAudioSource = EmbientAudioSourceGO.GetComponent<AudioSource> ();
		} else if (Instance != this) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		
	}

	public float GetClipDuration (string name) {
		for (int i = 0; i < EmbientSounds.Length; i++) {
			if (name == EmbientSounds [i].name) {
				return EmbientSounds [i].length;
			}
		}

		for (int i = 0; i < SoundEffects.Length; i++) {
			if (name == SoundEffects [i].name) {
				return SoundEffects [i].length;
			}
		}

		return 0f;
	}

	public void PlayStartSound (string soundName1, string soundName2) {
		_startSoundCoroutine = StartCoroutine (playStartSoundCoroutine (soundName1, soundName2));
	}

	private IEnumerator playStartSoundCoroutine (string soundName1, string soundName2) {
		PlaySound (soundName1, false);
		yield return new WaitForSeconds (AudioManager.Instance.GetClipDuration (soundName1));
		PlayEmbientSound (soundName2);
	}

	public void PlayEmbientSound (string name) {
		//StopCoroutine (_startSoundCoroutine);
		for (int i = 0; i < EmbientSounds.Length; i++) {
			if (name == EmbientSounds [i].name) {
				_embientAudioSource.Stop ();
				playSound (EmbientSounds [i], _embientAudioSource, true, _isEmbientSoundOn ? _defaultEmbientValume : 0f, _defaultEmbientPitch);
				return;
			}
		}
	}

	public void PlaySoundEffect (string name, bool makeEmbientSoundFade = false, bool useSpecialAudioSource = false) {
		AudioSource _source;

		_source = _effectsAudioSourceSpecial;

		if (!useSpecialAudioSource) {
			if (!_effectsAudioSource1.isPlaying) {
				_source = _effectsAudioSource1;
			} else if (!_effectsAudioSource2.isPlaying) {
				_source = _effectsAudioSource2;
			} else if (!_effectsAudioSource3.isPlaying) {
				_source = _effectsAudioSource3;
			} else if (!_effectsAudioSource4.isPlaying) {
				_source = _effectsAudioSource4;
			} else if (!_effectsAudioSource5.isPlaying) {
				_source = _effectsAudioSource5;
			} else if (!_effectsAudioSource6.isPlaying) {
				_source = _effectsAudioSource6;
			} else if (!_effectsAudioSource7.isPlaying) {
				_source = _effectsAudioSource7;
			} else if (!_effectsAudioSource8.isPlaying) {
				_source = _effectsAudioSource8;
			} else if (!_effectsAudioSource9.isPlaying) {
				_source = _effectsAudioSource9;
			} else if (!_effectsAudioSource10.isPlaying) {
				_source = _effectsAudioSource10;
			} else {
				return;
			}
		}

		for (int i = 0; i < SoundEffects.Length; i++) {
			if (name == SoundEffects [i].name) {
				playSound (SoundEffects [i], _source, false, _isSoundEffectsOn ? _defaultEffectsVolume : 0f, _defaultEffectsPitch);

				if (makeEmbientSoundFade) {
					makeCrossFade (_crossFadeDuration, _minCrossFadeVol, SoundEffects [i].length);
				}
				return;
			}
		}
	}

	public void PlaySound (string name, bool loop = false, float vol = 1f, float pitch = 1f) {

		for (int i = 0; i < EmbientSounds.Length; i++) {
			if (name == EmbientSounds [i].name) {
				_embientAudioSource.Stop ();
				playSound (EmbientSounds [i], _embientAudioSource, loop, _isEmbientSoundOn ? vol : 0f, pitch);
				return;
			}
		}

		for (int i = 0; i < SoundEffects.Length; i++) {
			AudioSource _source;
			if (!_effectsAudioSource1.isPlaying) {
				_source = _effectsAudioSource1;
			} else if (!_effectsAudioSource2.isPlaying) {
				_source = _effectsAudioSource2;
			} else if (!_effectsAudioSource3.isPlaying) {
				_source = _effectsAudioSource3;
			} else if (!_effectsAudioSource4.isPlaying) {
				_source = _effectsAudioSource4;
			} else if (!_effectsAudioSource5.isPlaying) {
				_source = _effectsAudioSource5;
			} else if (!_effectsAudioSource6.isPlaying) {
				_source = _effectsAudioSource6;
			} else if (!_effectsAudioSource7.isPlaying) {
				_source = _effectsAudioSource7;
			} else if (!_effectsAudioSource8.isPlaying) {
				_source = _effectsAudioSource8;
			} else if (!_effectsAudioSource9.isPlaying) {
				_source = _effectsAudioSource9;
			} else if (!_effectsAudioSource10.isPlaying) {
				_source = _effectsAudioSource10;
			} else {
				return;
			}

			if (name == SoundEffects [i].name) {
				playSound (SoundEffects [i], _source, loop, _isSoundEffectsOn ? vol : 0f, pitch);
				return;
			}
		}
	}

	private IEnumerator makeCrossFade (float duration, float volumeTo, float effectClipDuration) {
		if (_embientAudioSource.volume == 0f) {
			yield return null;
		}

		LeanTween.value (this.gameObject, updateCrossFadeValue, _embientAudioSource.volume, volumeTo, duration);
		yield return new WaitForSeconds (effectClipDuration);

		LeanTween.value (this.gameObject, updateCrossFadeValue, _minCrossFadeVol, _maxCrossFadeVol, duration);
		yield return null;
	}

	private void updateCrossFadeValue (float val) {
		_embientAudioSource.volume = val;
	}  

	private void playSound (AudioClip clip, AudioSource source, bool loop = false, float vol = 1f, float pitch = 1f, float delay = 0f) {
		source.clip = clip;
		source.volume = vol;
		source.pitch = pitch;
		source.loop = loop;

		source.Play ();
	}
}
