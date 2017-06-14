using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum MoveType
{
    XFirst,
    YFirst
}

public enum DirectionType
{
    Right,
    Left,
    Forward,
    Back,
}

public enum FSType
{
    Idel,
    Walk,
    Attack,
    Attacked,
    Magic,
    Evade,
    Defense,
    Defeat
}

public class PlayerController : MonoBehaviour
{
    public Action moveEndCallback;
    public Action<string> overCallback;

    public AnimationGif[] idleAniArray;
    public AnimationGif[] walkAniArray;
    public AnimationGif[] attackedAniArray;
    public AnimationGif[] magicAniArray;

    public Text hpTxt;
    private int _hp = 100;
   
    private RectTransform _playerTransform;
    private Vector2 _destinationPos;

    private bool _canMove = false;

    private float _moveSpeed = 0.2f;
    private MoveType _moveType;
    private DirectionType _currentDirectionType;
    private FSType _currentFSType;
    private AnimationGif _cuerentAniGif;
    private float _currentTime;
    private int _x;
    private int _y;

    public int x
    {
        get { return _x; }
    }

    public int y
    {
        get { return _y; }
    }

    public FSType currentFSType
    {
        get { return _currentFSType; }
    }

    public int HP
    {
        get { return _hp; }
    }

    public DirectionType currentDirectionType
    {
        get { return _currentDirectionType; }
    }

    void Awake()
    {
        hpTxt.gameObject.SetActive(false);
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

                    if (_currentDirectionType != DirectionType.Right)
                        SetAnimation(FSType.Walk, DirectionType.Right);
                }
                else if (_destinationPos.x - _playerTransform.anchoredPosition.x < -AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.x -= AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;

                    if (_currentDirectionType != DirectionType.Left)
                        SetAnimation(FSType.Walk, DirectionType.Left);
                }
                else if (_destinationPos.y - _playerTransform.anchoredPosition.y > AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.y += AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;

                    if (_currentDirectionType != DirectionType.Back)
                        SetAnimation(FSType.Walk, DirectionType.Back);
                }
                else if (_destinationPos.y - _playerTransform.anchoredPosition.y < -AppConstants.CellWidth / 2)
                {
                    Vector2 p = _playerTransform.anchoredPosition;
                    p.y -= AppConstants.CellWidth;
                    _playerTransform.anchoredPosition = p;

                    if (_currentDirectionType != DirectionType.Forward)
                        SetAnimation(FSType.Walk, DirectionType.Forward);
                }
                else
                {
                    _canMove = false;
                    if (moveEndCallback != null)
                        moveEndCallback();

                    if (_currentFSType != FSType.Idel)
                        SetAnimation(FSType.Idel, _currentDirectionType);
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

    public void SetAnimation(FSType sT, DirectionType dT, Action callback = null)
    {
        setAniDefault();
        _currentFSType = sT;
        _currentDirectionType = dT;

        switch (sT)
        {
            case FSType.Idel:
                _cuerentAniGif = idleAniArray[(int)dT];
                break;
            case FSType.Walk:
                _cuerentAniGif = walkAniArray[(int)dT];
                break;
            case FSType.Attacked:
                _cuerentAniGif = attackedAniArray[(int)dT];
                showDamge();
                break;
            case FSType.Magic:
                _cuerentAniGif = magicAniArray[(int)dT];
                break;
            default:
                break;
        }

        _cuerentAniGif.animationEndCallback = callback;
        _cuerentAniGif.SetAnimation(true);
    }

    private void showDamge()
    {
        int damege = UnityEngine.Random.Range(1, 10);
        _hp -= damege;
        hpTxt.text = (-1*damege).ToString();
        hpTxt.gameObject.SetActive(true);
        hpTxt.rectTransform.anchoredPosition = new Vector2(0, 90);
        Tweener t = hpTxt.rectTransform.DOLocalMoveY(116, 1f);
        t.OnComplete(() => { hpTxt.gameObject.SetActive(false); });

        if (_hp <= 0 && overCallback != null)
            overCallback(this.tag);

    }

    private void setAniDefault()
    {
        for(int i=0;i<idleAniArray.Length;i++)
        {
            idleAniArray[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < walkAniArray.Length; i++)
        {
            walkAniArray[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < attackedAniArray.Length; i++)
        {
            attackedAniArray[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < magicAniArray.Length; i++)
        {
            magicAniArray[i].gameObject.SetActive(false);
        }
    }
}
