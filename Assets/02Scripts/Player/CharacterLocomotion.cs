using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    private CharacterController _characterCtrl;
    private Animator _anim;
    private ActiveWeapon _activeWeapon;
    private ReloadWeapon _reloadWeapon;
    private CharacterAiming _aiming;

    [SerializeField]
    private Animator _rigController;

    [Range(1f, 3f),SerializeField]
    private float _moveSpee = 1.5f;
    [SerializeField]
    private float _jumpHeight = 1.8f;
    [SerializeField]
    private float _gravity = 9.8f;
    [SerializeField]
    private float _stepDown = 0.3f; // 캐릭터 컨트롤러의 stepOffset과 비슷한 개념
    [SerializeField]
    private float _airControl = 2.5f;
    [SerializeField]
    private float _jumpDump = 0.5f;    // 감속
    [SerializeField]
    private float _pushPower = 2;


    private Vector2 _input;
    private Vector3 _rootMotion;
    private Vector3 _velocity;
    private bool _isJumping;

    private int _isSprintingParam = Animator.StringToHash("IsSprinting");


    void Awake()
    {
        _anim = GetComponent<Animator>();
        _characterCtrl = GetComponent<CharacterController>();
        _activeWeapon = GetComponent<ActiveWeapon>();
        _reloadWeapon = GetComponent<ReloadWeapon>();
        _aiming = GetComponent<CharacterAiming>();
    }

    private void FixedUpdate()
    {
        if(_isJumping) //  IsInAir
        {
            UpdateInAir();
        }
        else
        {
            UpdateOnGround();
        }
    }

    private void UpdateOnGround()
    {
        Vector3 stepForwardAmount = _rootMotion * _moveSpee;
        Vector3 stepDownAmount = Vector3.down * _stepDown;

        _characterCtrl.Move(stepForwardAmount + stepDownAmount);
        _rootMotion = Vector3.zero;

        // Fall
        if (!_characterCtrl.isGrounded)
        {
            SetInAir(0);
        }
    }

    private void UpdateInAir()
    {
        _velocity.y -= _gravity * Time.fixedDeltaTime;
        Vector3 displacement = _velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();
        _characterCtrl.Move(displacement);
        _isJumping = !_characterCtrl.isGrounded;
        //공중에 있을때에도 루트모션값 계산이 되어 착지시 순간이동이 되기에
        _rootMotion = Vector3.zero;
        _anim.SetBool("IsJumping", _isJumping);
    }

    void Update()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");

        if(_input.magnitude > 1) _input.Normalize();

        _anim.SetFloat("InputX", _input.x);
        _anim.SetFloat("InputY", _input.y);

        UpdateIsSprinting();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private bool IsSprinting()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isFiring = _activeWeapon.IsFiring();
        bool isReloading = _reloadWeapon.IsReloading;
        bool isSwapWeapon = _activeWeapon.IsSwapWeapon;
        bool isAiming = _aiming.IsAiming;
        return isSprinting && !isFiring && !isReloading && !isSwapWeapon && !isAiming;
    }

    private void UpdateIsSprinting()
    {
        bool isSprinting = IsSprinting();
        _anim.SetBool(_isSprintingParam, isSprinting);
        _rigController.SetBool(_isSprintingParam, isSprinting);
    }

    private void Jump()
    {
        if(!_isJumping)
        {
            float jumpVelocity = Mathf.Sqrt(2 * _gravity * _jumpHeight);
            SetInAir(jumpVelocity);
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        _isJumping = true;
        _velocity = _anim.velocity * _jumpDump * _moveSpee;
        _velocity.y = jumpVelocity;
        _anim.SetBool("IsJumping", true);
    }

    private Vector3 CalculateAirControl()
    {
        return ((transform.forward * _input.y) + (transform.right * _input.x)) * (_airControl / 100f);
    }
    private void OnAnimatorMove()
    {
        _rootMotion += _anim.deltaPosition;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic) return;

        // we dont want to push objects below us
        if (hit.moveDirection.y < -0.3f) return;

        // Caluculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you how fast your character is trying to move.
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.linearVelocity = pushDir *_pushPower;
    }
}
