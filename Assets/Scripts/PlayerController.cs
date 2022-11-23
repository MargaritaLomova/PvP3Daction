using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private CharacterController _characterController;
    [SerializeField]
    private Transform _mainCamera;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _model;

    [Space]
    [Header("Variables")]
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private float _rotateSpeed = 5f;

    private float _targetAngle;
    private Vector3 _direction;

    private float _horizontal;
    private float _vertical;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        GetDirection();

        _animator.SetBool("RunBool", _direction.magnitude >= 0.1f);

        if (_direction.magnitude >= 0.1f)
        {
            _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;

            Move();
            Rotate();
        }

        if(Input.GetMouseButton(0))
        {
            _animator.SetBool("RunBool", true);
            _characterController.Move(_model.forward * _speed * Time.fixedDeltaTime * 15f);
        }
    }

    private void GetDirection()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        _direction = new Vector3(_horizontal, 0f, _vertical).normalized;
    }

    private void Move()
    {
        Vector3 moveDirection = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
        _characterController.Move(moveDirection.normalized * _speed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.fixedDeltaTime * _rotateSpeed);
        _model.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _rotateSpeed);
    }
}