using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM"),SerializeField]
    private AudioSource _bgm;
    [SerializeField]
    private AudioClip[] _bgmClips;


    [Header("SFX"), SerializeField]
    private AudioSource _sfx;
    [SerializeField]
    private AudioClip[] _reloadSFXs;
    [SerializeField]
    private AudioClip _pickupSFX;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _bgm.loop = true;
        PlayBGM(_bgmClips[0]);
    }

    public void PlayBGM(AudioClip p_clip)
    {
        _bgm.clip = p_clip;
        _bgm.Play();
    }

    public void PlaySFX(AudioClip p_clip)
    {
        _sfx.Stop();
        _sfx.clip = p_clip;
        _sfx.Play();
    }
    public void PlayOneShotSFX(AudioClip p_clip)
    {
        _sfx.PlayOneShot(p_clip);
    }

    public void ReloadDetachMagazine()
    {
        PlayOneShotSFX(_reloadSFXs[0]);
    }

    public void ReloadAttachMagazine()
    {
        PlayOneShotSFX(_reloadSFXs[1]);
    }

    public void PickupSFX()
    {
        PlayOneShotSFX(_pickupSFX);
    }
}
