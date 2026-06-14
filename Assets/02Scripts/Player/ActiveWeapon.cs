using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeapon : MonoBehaviour
{
    public Transform CrossHairTarget;
    public Rig HandIK;
    public Transform WeaponParent;
    public Transform WeaponRightAttach;
    public Transform WeaponLeftAttach;
    public Animator RigController;
    public GameObject RootObj;

    private RaycastWeapon _weapon;
    private Animator _anim;
    private AnimatorOverrideController _overrideAnim;

    void Start()
    {
        //_anim = GetComponent<Animator>();
        //_overrideAnim = _anim.runtimeAnimatorController as AnimatorOverrideController;
        //_overrideAnim = new AnimatorOverrideController(_anim.runtimeAnimatorController);
        //_anim.runtimeAnimatorController = _overrideAnim;
        //_weapon = GetComponentInChildren<RaycastWeapon>();

        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if(existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(_weapon)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _weapon.StartFiring();
            }
            if (_weapon.IsFiring)
            {
                _weapon.UpdateFiring(Time.deltaTime);
            }
            _weapon.UpdateBullets(Time.deltaTime);
            if (Input.GetMouseButtonUp(0))
            {
                _weapon.StopFiring();
            }
        }
        /*else
        {
            HandIK.weight = 0.0f;
            _anim.SetLayerWeight(1, 0.0f);
        }*/
    }

    public void Equip(RaycastWeapon p_newWeapon)
    {
        if(_weapon)
        {
            Destroy(_weapon.gameObject);
        }
        _weapon = p_newWeapon;
        _weapon.RaycastDestination = CrossHairTarget;
        _weapon.transform.parent = WeaponParent;
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.identity;

        /*HandIK.weight = 1.0f;
        _anim.SetLayerWeight(1, 1.0f);
        Invoke(nameof(SetAnimationDelayed), 0.001f);*/
    }

    private void SetAnimationDelayed()
    {
        _overrideAnim["WeaponAnim_Empty"] = _weapon.WeaponAnimClip;
    }

    /*[ContextMenu("Save Weapon Pose")]
    private void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(WeaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(WeaponLeftAttach.gameObject, false);
        recorder.BindComponentsOfType<Transform>(WeaponRightAttach.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        //recorder.TakeSnapshot(1f / 60f);
        recorder.SaveToClip(_weapon.WeaponAnimClip);
    }*/
}
