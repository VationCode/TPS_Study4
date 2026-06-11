using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    private Camera _mainCamera;
    private Ray _ray;
    private RaycastHit _hitInfo;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        _ray.origin = _mainCamera.transform.position;
        _ray.direction = _mainCamera.transform.forward;

        if(Physics.Raycast(_ray, out _hitInfo))
        {
            transform.position = _hitInfo.point;
        }
        else
        {
            transform.position = _ray.origin + _ray.direction * 1000.0f;
        }
    }
}
