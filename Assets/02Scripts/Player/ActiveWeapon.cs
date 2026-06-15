using System.Collections;
using Unity.Cinemachine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActiveWeapon : MonoBehaviour
{
    public enum EWeaponSlot
    {
        Primary = 0,
        Secondary = 1
    }

    public Transform CrossHairTarget;
    public Rig HandIK;
    public Transform[] WeaponSlots;
    public Animator RigController;
    public CinemachineFreeLook playerCamera;
    /*public Transform WeaponRightAttach;
    public Transform WeaponLeftAttach;
    public GameObject RootObj;*/

    private RaycastWeapon[] _equippedWeapons = new RaycastWeapon[2];
    private int _activeWeaponIndex;
    private bool _isHolstered = false;
    

    //private Animator _anim;
    //private AnimatorOverrideController _overrideAnim;

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

    RaycastWeapon GetWeapon(int p_index)
    {
        if (p_index < 0 || p_index >= _equippedWeapons.Length) return null;
        return _equippedWeapons[p_index];
    }
    void Update()
    {
        var weapon = GetWeapon(_activeWeaponIndex);
        if(weapon && !_isHolstered)
        {
            weapon.UpdateWeapon(Time.deltaTime);
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleActiveWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon((int)EWeaponSlot.Primary);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon((int)EWeaponSlot.Secondary);
        }

    }

    public void Equip(RaycastWeapon p_newWeapon)
    {
        int weaponSlotIndex = (int)p_newWeapon.WeponSlot;
        var weapon = GetWeapon(weaponSlotIndex);
        if(weapon)
        {
            Destroy(weapon.gameObject);
        }
        weapon = p_newWeapon;
        weapon.RaycastDestination = CrossHairTarget;
        weapon.transform.SetParent(WeaponSlots[weaponSlotIndex],false);
        _equippedWeapons[weaponSlotIndex] = weapon;

        SetActiveWeapon(weaponSlotIndex);
    }

    private void ToggleActiveWeapon()
    {
        bool isHolstered = RigController.GetBool("IsHolster");
        if (isHolstered)
        {
            StartCoroutine(ActivateWeapon(_activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(_activeWeaponIndex));
        }
    }

    private void SetActiveWeapon(int p_weaponSlotIndex)
    {
        // 기존 활성화된무기 홀스터로 만들고 새로 들어온 무기로 활성화
        int holsterIndex = _activeWeaponIndex;
        int activateIndex = p_weaponSlotIndex;

        if(holsterIndex == activateIndex)
        {
            holsterIndex = -1;
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    private IEnumerator SwitchWeapon(int p_holsterIndex,int p_activateIndex)
    {
        yield return StartCoroutine(HolsterWeapon(p_holsterIndex));
        yield return StartCoroutine(ActivateWeapon(p_activateIndex));
        _activeWeaponIndex = p_activateIndex;
    }

    private IEnumerator HolsterWeapon(int p_index)
    {
        _isHolstered = true;
        var weapon = GetWeapon(p_index);
        if(weapon)
        {
            RigController.SetBool("IsHolster", true);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (RigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        }
    }
    private IEnumerator ActivateWeapon(int p_index)
    {
        var weapon = GetWeapon(p_index);
        if (weapon)
        {
            RigController.SetBool("IsHolster", false);
            RigController.Play("Equip_" + weapon.WeaponName);
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (RigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            _isHolstered = false;
        }
    }

    /*private void SetAnimationDelayed()
    {
        _overrideAnim["WeaponAnim_Empty"] = _weapon.WeaponAnimClip;
    }

    [ContextMenu("Save Weapon Pose")]
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
