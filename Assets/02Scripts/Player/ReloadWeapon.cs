using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator RigController;
    public WeaponAnimationEvents AnimEvents;
    public ActiveWeapon CurrentActiveWeapon;
    public Transform LeftHand;
    public AmmoWidget AmmoUI;

    GameObject _magazineHand;
    private void Awake()
    {
        CurrentActiveWeapon = GetComponent<ActiveWeapon>();
    }
    private void Start()
    {
        AnimEvents.WeaponAnimEvent.AddListener(OnAnimationEvent);
    }

    void Update()
    {
        RaycastWeapon weapon = CurrentActiveWeapon.GetActiveWeapon();
        if(weapon)
        {
            if (Input.GetKeyDown(KeyCode.R) || weapon.AmmoCount <= 0)
            {
                RigController.SetTrigger("Reload");
            }

            if(weapon.IsFiring)
            {
                AmmoUI.Refresh(weapon.AmmoCount);
            }
        }
    }

    private void OnAnimationEvent(string p_eventName)
    {
        switch(p_eventName)
        {
            case "DetachMagazine":
                DetachMagazine();
                break;
            case "DropMagazine":
                DropMagazine();
                break;
            case "RefillMagazine":
                RefillMagazine();
                break;
            case "AttachMagazine":
                AttachMagazine();
                break;
        }
    }
    private void DetachMagazine()
    {
        RaycastWeapon weapon = CurrentActiveWeapon.GetActiveWeapon();
        _magazineHand = Instantiate(weapon.Magazine, LeftHand, true);
        weapon.Magazine.SetActive(false);
    }
    private void DropMagazine()
    {
        GameObject droppedMagazine = Instantiate(_magazineHand, _magazineHand.transform.position, _magazineHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        _magazineHand.SetActive(false);
    }
    private void RefillMagazine()
    {
        _magazineHand.SetActive(true);
    }
    private void AttachMagazine()
    {
        RaycastWeapon weapon = CurrentActiveWeapon.GetActiveWeapon();
        Destroy(_magazineHand);
        weapon.Magazine.SetActive(true);
        weapon.AmmoCount = weapon.MaxAmmoSize;
        RigController.ResetTrigger("Reload");

        AmmoUI.Refresh(weapon.AmmoCount);
    }
}
