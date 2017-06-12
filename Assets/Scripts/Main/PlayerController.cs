using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private RectTransform _playerTransform;
    private Vector2 _destinationPos;

    private bool _canMove = false;

    private float _moveSpeed = 0.5f;
    private float _currentTime;

    void Start()
    {
        _playerTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        if(_canMove)
        {
            if(Time.time - _currentTime > _moveSpeed)
            {
                if(_destinationPos.y > _playerTransform.anchoredPosition.y)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.y += AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else if(_destinationPos.y < _playerTransform.anchoredPosition.y)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.y -= AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else if(_destinationPos.x > _playerTransform.anchoredPosition.x)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.x += AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else if (_destinationPos.x < _playerTransform.anchoredPosition.x)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.x -= AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else
                {
                    _canMove = false;
                }

                _currentTime = Time.time;
            }
        }
    }

    public void SetDestination(Vector2 pos)
    {
        _destinationPos = pos;
        _canMove = true;
        _currentTime = Time.time;
    }
    


}
