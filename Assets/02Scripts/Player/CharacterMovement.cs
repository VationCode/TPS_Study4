using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;

    public bool UseCharacterForward = false;
    public bool LockToCameraForward = false;
    public float TurnSpeed = 10f;
    public KeyCode SprintJoystick = KeyCode.JoystickButton2;
    public KeyCode SprintKeyboard = KeyCode.LeftShift;

    private float _turnSpeedMultiplier;
    private float _speed = 0f;
    private float _direction = 0f;
    private bool _isSprinting = false;
    private Animator _anim;
    private Vector3 _targetDirection;
    private Vector2 _input;
    private Quaternion _freeRotation;
    private Camera _mainCamera;
    private float _velocity;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        _input.x = Input.GetAxis("Horizontal");
        _input.y = Input.GetAxis("Vertical");
        
        // set speed to both vertical and horizontal inputs
        if (UseCharacterForward)
            _speed = Mathf.Abs(_input.x) + _input.y;
        else
            _speed = Mathf.Abs(_input.x) + Mathf.Abs(_input.y);

        _speed = Mathf.Clamp(_speed, 0f, 1f);
        _speed = Mathf.SmoothDamp(_anim.GetFloat("Speed"), _speed, ref _velocity, 0.1f);
        _anim.SetFloat("Speed", _speed);

        if (_input.y < 0f && UseCharacterForward)
            _direction = _input.y;
        else
            _direction = 0f;

        _anim.SetFloat("Direction", _direction);

        // set sprinting
        _isSprinting = ((Input.GetKey(SprintJoystick) || Input.GetKey(SprintKeyboard)) && _input != Vector2.zero && _direction >= 0f);
        _anim.SetBool("isSprinting",_isSprinting);

        // Update target direction relative to camera view (or not if the Keep Direction option is checked)
        UpdateTargetDirection();
        if(_input != Vector2.zero && _targetDirection.magnitude > 0.1f)
        {
            Vector3 lookDirection = _targetDirection.normalized;
            _freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
            var diferenceRotation = _freeRotation.eulerAngles.y - transform.eulerAngles.y;
            var eulerY = transform.eulerAngles.y;

            if (diferenceRotation < 0 || diferenceRotation > 0) eulerY = _freeRotation.eulerAngles.y;
            var euler = new Vector3(0, eulerY, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), TurnSpeed * _turnSpeedMultiplier * Time.deltaTime);
        }
    }

    public virtual void UpdateTargetDirection()
    {
        if(!UseCharacterForward)
        {
            _turnSpeedMultiplier = 1f;
            var forward = _mainCamera.transform.TransformDirection(Vector3.forward);
            forward.y = 0f;

            // get the right-facing diriection of the referenceTransform
            var right = _mainCamera.transform.TransformDirection(Vector3.right);

            // determine the direction the player will face based on input and the referenceTransform's right and forward directions
            _targetDirection = _input.x * right + _input.y * forward;
        }
        else
        {
            _turnSpeedMultiplier = 0.2f;
            var forward = transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            // get the right-facing direction of the referenceTransform
            var right = transform.TransformDirection(Vector3.right);
            _targetDirection = _input.x * right + Mathf.Abs(_input.y) * forward;
        }
    }

}
