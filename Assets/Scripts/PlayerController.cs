using Mirror;
using System.Collections;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Transform _model;
    [SerializeField]
    private SkinnedMeshRenderer _meshRenderer;

    [Space]
    [Header("Variables")]
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private float _rotateSpeed = 5f;
    [SerializeField]
    private float _spurtDistance = 3f;
    [SerializeField]
    private float _hittedTime = 3f;

    [Space]
    [Header("Colors")]
    [SerializeField]
    private Color _idleColor;
    [SerializeField]
    private Color _hittedColor;

    public bool IsSpunt { get; private set; }
    public bool IsHitted { get; private set; }

    private float _targetAngle;
    private float _horizontal;
    private float _vertical;

    private int _countOfHittedPlayers = 0;

    private Vector3 _direction;

    private Transform _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            GetHit();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if(isLocalPlayer)
        {
            GetDirection();

            _animator.SetBool("RunBool", _direction.magnitude >= 0.1f);

            if (_direction.magnitude >= 0.1f)
            {
                _targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;

                Move();
                Rotate();
            }

            if (Input.GetMouseButtonUp(0) && !IsSpunt)
            {
                StartCoroutine(Spunt());
            }
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
        transform.Translate(moveDirection.normalized * _speed * Time.fixedDeltaTime, Space.World);
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_direction), Time.fixedDeltaTime * _rotateSpeed);
        _model.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * _rotateSpeed);
    }

    private void GetHit()
    {
        if (!IsHitted)
        {
            StartCoroutine(ChangeColorToHitted());
        }
    }

    private IEnumerator ChangeColorToHitted()
    {
        IsHitted = true;

        foreach (var material in _meshRenderer.materials)
        {
            material.color = _hittedColor;
        }

        float passedTime = 0f;
        while (passedTime < _hittedTime)
        {
            passedTime += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        foreach (var material in _meshRenderer.materials)
        {
            material.color = _idleColor;
        }

        IsHitted = false;
    }

    private IEnumerator Spunt()
    {
        IsSpunt = true;
        float passedTime = 0f;
        while (passedTime < 0.5f)
        {
            _animator.SetBool("RunBool", true);
            transform.Translate(_model.forward * _speed * Time.fixedDeltaTime * 3f, Space.World);
            passedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _animator.SetBool("RunBool", false);
        IsSpunt = false;
    }
}