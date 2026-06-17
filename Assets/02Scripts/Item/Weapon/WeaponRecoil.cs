using Unity.Cinemachine;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector]
    public CharacterAiming AimingCtrl;
    // 예전 버전의 시네머신에서 FreeLook의 YAxis에 해당하는 신버전의 컴포넌트
    private CinemachineOrbitalFollow _orbitalFollow;
    [HideInInspector]
    public CinemachineImpulseSource CameraShake;
    [HideInInspector]
    public Animator RigController;

    public Vector2[] RecoilPattern;
    public float Duration;
    public float RecoilModifier = 1.0f;

    private float _verticalRecoil;
    private float _horizontalRecoil;
    private float _time;
    private int _index; 


    private void Awake()
    {
        CameraShake = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        /*if (PlayerCamera != null)
            _orbitalFollow = PlayerCamera.GetComponent<CinemachineOrbitalFollow>();*/
    }

    public void Reset()
    {
        _index = 0;
    }

    void Update()
    {
        if (AimingCtrl != null)
        {
            if (_time > 0)
            {
                AimingCtrl.YAxis.Value -= (((_verticalRecoil/10) * Time.deltaTime) / Duration) * RecoilModifier;
                AimingCtrl.XAxis.Value -= (((_horizontalRecoil/10) * Time.deltaTime) / Duration) * RecoilModifier;
                _time -= Time.deltaTime;
            }
        }
         
    }

    private int NextIndex(int p_index)
    {
        return (p_index + 1) % RecoilPattern.Length;
    }

    public void GenerateRecoil(string p_weaponName)
    {
        _time = Duration;

        CameraShake.GenerateImpulse(Camera.main.transform.forward);

        _horizontalRecoil = RecoilPattern[_index].x;
        _verticalRecoil = RecoilPattern[_index].y;

        _index = NextIndex(_index);

        RigController.Play("WeaponRecoil_" + p_weaponName, 1, 0.0f);
    }

}
