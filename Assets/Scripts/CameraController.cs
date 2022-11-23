using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _player;

    private Vector3 _distanceToPlayer;

    private void Start()
    {
        _distanceToPlayer = transform.position - _player.position;
    }

    private void FixedUpdate()
    {
        transform.position = _player.position + _distanceToPlayer;
    }
}