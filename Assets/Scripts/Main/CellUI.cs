using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StateType
{
    Transparent,
    Normal,
    Hover
}
public class CellUI : MonoBehaviour {

    public Image img;
    public Sprite[] sprites;

    public Action<int , int> onClickCallback;

    private StateType _currentType;
    private int _xIndex;
    private int _yindex;

	void Start () 
    {
        EventTriggerListener.Get(img.gameObject).onEnter = this.__onHover;
        EventTriggerListener.Get(img.gameObject).onExit = this.__onExit;
        EventTriggerListener.Get(img.gameObject).onClick = this.__onClick;
	}
	
	public void SetData(int x, int y, StateType type, Vector2 pos)
    {
        _xIndex = x;
        _yindex = y;
        _currentType = type;
        img.sprite = sprites[(int)type];
        img.rectTransform.anchoredPosition = pos;
    }

    private void __onHover(GameObject go)
    {
        if(_currentType == StateType.Normal)
            img.sprite = sprites[2];
    }

    private void __onExit(GameObject go)
    {
        if (_currentType == StateType.Normal)
            img.sprite = sprites[1];
    }

    private void __onClick(GameObject go)
    {
        if (onClickCallback != null && _currentType == StateType.Normal)
        {
            onClickCallback(_xIndex, _yindex);
        }
    }
}


