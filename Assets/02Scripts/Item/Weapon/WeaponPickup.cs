using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public RaycastWeapon WeaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
        if(activeWeapon)
        {
            RaycastWeapon newWeapon = Instantiate(WeaponPrefab);
            activeWeapon.Equip(newWeapon);
        }
    }
}
