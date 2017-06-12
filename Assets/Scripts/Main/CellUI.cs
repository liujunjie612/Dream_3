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

    public Action<Vector2> onClickCallback;

    private StateType _currentType;

	void Start () 
    {
        EventTriggerListener.Get(img.gameObject).onEnter = this.__onHover;
        EventTriggerListener.Get(img.gameObject).onExit = this.__onExit;
	}
	
	public void SetData(StateType type)
    {
        _currentType = type;
        img.sprite = sprites[(int)type];
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
        if (onClickCallback != null)
            onClickCallback(img.rectTransform.anchoredPosition);
    }
}


