using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class CharacterAiming : MonoBehaviour
{

    private Camera _mainCamera;
    private Animator _anim;
    private ActiveWeapon _currentWeapon;

    public float TurnSpeed = 15f;
    public float AimDuration = 0.18f;
    public Transform CameraLookAt;
    public AxisState XAxis;
    public AxisState YAxis;
    public bool IsAiming;
    //[SerializeField]
    //private Rig _animLayer;


    private int _isAimingParam = Animator.StringToHash("IsAiming");

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _currentWeapon = GetComponent<ActiveWeapon>();
    }

    void Start()
    {
        _mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        XAxis.Update(Time.fixedDeltaTime);
        YAxis.Update(Time.fixedDeltaTime);

        CameraLookAt.eulerAngles = new Vector3(YAxis.Value, XAxis.Value,0);

        float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,yawCamera,0), TurnSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        var weapon = _currentWeapon.GetActiveWeapon();

        if (weapon)
        {
            IsAiming = Input.GetMouseButton(1);
            _anim.SetBool(_isAimingParam, IsAiming);
            // 에임줌에 의한 반종 조절
            weapon.Recoil.RecoilModifier = IsAiming ? 0.3f : 1.0f;
        }
    }
    //private void LateUpdate()
    //{
    /*if(_animLayer)
    {
        /*if (Input.GetMouseButton(1))
        {
            _animLayer.weight += Time.deltaTime / AimDuration;
        }
        else
        {
            _animLayer.weight -= Time.deltaTime / AimDuration;
        }*//*
        _animLayer.weight = 1.0f;
    }*/

    /*if(_weapon)
    {
        if(Input.GetButtonDown("Fire1"))
        {
            _weapon.StartFiring();
        }
        if(_weapon.IsFiring)
        {
            _weapon.UpdateFiring(Time.deltaTime);
        }
        _weapon.UpdateBullets(Time.deltaTime);
        if(Input.GetButtonUp("Fire1"))
        {
            _weapon.StopFiring();
        }
    }*/
    //}
}
