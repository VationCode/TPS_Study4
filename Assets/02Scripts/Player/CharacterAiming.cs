using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    public float TurnSpeed = 15f;
    public float AimDuration = 0.18f;
    [SerializeField]
    private Rig _animLayer;

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float yawCamera = _mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,yawCamera,0), TurnSpeed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if(_animLayer)
        {
            /*if (Input.GetMouseButton(1))
            {
                _animLayer.weight += Time.deltaTime / AimDuration;
            }
            else
            {
                _animLayer.weight -= Time.deltaTime / AimDuration;
            }*/
            _animLayer.weight = 1.0f;
        }

    }
}
