using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    XFirst,
    YFirst
}

public class PlayerController : MonoBehaviour
{
    public Action moveEndCallback;

    private RectTransform _playerTransform;
    private Vector2 _destinationPos;

    private bool _canMove = false;

    private float _moveSpeed = 0.2f;
    private MoveType _moveType;
    private float _currentTime;
    private int _x;
    private int _y;

    public int x
    {
        get
        {
            return _x;
        }
    }

    public int y
    {
        get
        {
            return _y;
        }
    }

    void Start()
    {
        _playerTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (_canMove)
        {
            if (Time.time - _currentTime > _moveSpeed)
            {
                if (_destinationPos.x - _playerTransform.anchoredPosition.x > AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.x += AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else if (_destinationPos.x - _playerTransform.anchoredPosition.x < -AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.x -= AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else if (_destinationPos.y - _playerTransform.anchoredPosition.y > AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.y += AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else if (_destinationPos.y - _playerTransform.anchoredPosition.y < -AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.y -= AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;
                }
                else
                {
                    _canMove = false;
                    if (moveEndCallback != null)
                        moveEndCallback();
                }

                _currentTime = Time.time;
            }
        }
    }

    public void SetDestination(int x, int y, Vector2 pos, bool move = true)
    {
        if (move)
        {
            _x = x;
            _y = y;

            _destinationPos = pos;
            _canMove = true;
            _currentTime = Time.time;
        }
        else
        {
            _x = x;
            _y = y;

            _playerTransform.anchoredPosition = pos;
        }



    }
}
