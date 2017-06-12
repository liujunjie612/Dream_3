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
public class CellVo
{
    public StateType stateType;
    public Vector2 position;
}
public class CellUI : MonoBehaviour {

    public Image img;
    public Sprite[] sprites;


    public CellVo vo;

	void Start () 
    {
        EventTriggerListener.Get(img.gameObject).onEnter = this.__onHover;
        EventTriggerListener.Get(img.gameObject).onExit = this.__onExit;
	}
	
	public void SetData(CellVo v)
    {
        this.vo = v;
        this.vo.position = img.rectTransform.anchoredPosition;
        img.sprite = sprites[(int)vo.stateType];
        img.rectTransform.anchoredPosition = vo.position;
    }

    private void __onHover(GameObject go)
    {
        Debug.Log(img.rectTransform.anchoredPosition);
        img.sprite = sprites[1];
    }

    private void __onExit(GameObject go)
    {
        img.sprite = sprites[0];
    }
}


