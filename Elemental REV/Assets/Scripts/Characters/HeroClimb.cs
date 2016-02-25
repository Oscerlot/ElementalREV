﻿using UnityEngine;
using System.Collections;

public class HeroClimb : MonoBehaviour
{
    public float maxHeightClimb = .5f;

    private HeroAwareness _heroAwareness;
    private PlayerInput _playerInput;
    private Rigidbody _rgBody;
    private Collider _currentWall;
    private Vector3 _currentWallPos;

    private float _distanceToTopOfWall;

    // Use this for initialization
    void Start ()
    {
        _playerInput = GetComponent<PlayerInput>();
	    _heroAwareness = GetComponent<HeroAwareness>();
        _rgBody = GetComponent<Rigidbody>();
	}

    void FixedUpdate()
    {
        if (_currentWall)
        {
            if (GridTools.Instance.PositionIsAccessible(_currentWallPos, new[] {_currentWall.gameObject, gameObject}) &&
                maxHeightClimb > _distanceToTopOfWall)
            {
                if (_playerInput.MovInDirection.magnitude > .02f)
                {
                    _rgBody.AddForce(Vector3.up*.5f, ForceMode.VelocityChange);
                    Debug.Log("Add force dammit");
                }
            }
        }
    }

	// Update is called once per frame
	void Update ()
	{
	    if (!_currentWall && _heroAwareness.CurrentObjectInAwareness && _heroAwareness.CurrentObjectInAwareness.layer.Equals(LayerMask.NameToLayer("Ground")))
	    {
	        if (_heroAwareness.CurrentObjectInAwareness.GetComponent<Collider>())
	        {
	            _currentWall = _heroAwareness.CurrentObjectInAwareness.GetComponent<Collider>();
                _currentWallPos = _currentWall.transform.position;
                _currentWallPos.y = _currentWall.bounds.center.y + _currentWall.bounds.extents.y;
                _distanceToTopOfWall = Mathf.Abs(_currentWallPos.y - gameObject.transform.position.y);
            }
	    }
	    if (_currentWall && !_heroAwareness.CurrentObjectInAwareness)
	    {
	        _currentWall = null;
            
        }

	}

    void OnDrawGizmos()
    {
        if (Application.isPlaying && _currentWall)
        {
            if (GridTools.Instance.PositionIsAccessible(_currentWallPos, new[] {_currentWall.gameObject, gameObject}) && maxHeightClimb > _distanceToTopOfWall)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;
            
            Gizmos.DrawWireCube(GridTools.Instance.SnapVectorToGrid(_currentWallPos), Vector3.one);
            Debug.Log(_distanceToTopOfWall);

        }
    }
}
