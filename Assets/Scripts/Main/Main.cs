using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public Image startImg;
    public GameObject cellPrefab;
    public Transform parent;

    public PlayerController player;

    private Vector2[,] _posArray;
    private List<CellUI> _usingList = new List<CellUI>();
    private List<CellUI> _unusingList = new List<CellUI>();

	void Start () 
    {
        catchPosition();

        player.moveEndCallback = catchAllCell;
        player.SetDestination(10, 10, GetPositionByIndex(10, 10), false);
        showWakableRange(10, 10);
        //showAttackRange(10, 10);

        startImg.gameObject.SetActive(true);
        startImg.rectTransform.SetAsLastSibling();

        EventTriggerListener.Get(startImg.gameObject).onClick = this.__onStratClick;
	}


    private void catchPosition()
    {
        _posArray = new Vector2[AppConstants.CellRowCount, AppConstants.CellColumnCount];

        for (int i = 0; i < AppConstants.CellRowCount; i++)
        {
            Vector2 p = new Vector2(-(AppConstants.CellColumnCount / 2 + AppConstants.CellColumnCount % 2 * 0.5f) * AppConstants.CellWidth, 
                                     (AppConstants.CellRowCount / 2 + AppConstants.CellRowCount % 2 * 0.5f) * AppConstants.CellWidth - AppConstants.CellWidth * i);
            for (int j = 0; j < AppConstants.CellColumnCount; j++)
            {
                _posArray[i, j] = p;
                p.x += AppConstants.CellWidth;
            }
        }
    }

    private CellUI activeCell()
    {
        CellUI cell = null;
        if(_unusingList.Count > 0)
        {
            cell = _unusingList[0];
            cell.gameObject.SetActive(true);
            _unusingList.RemoveAt(0);
            _usingList.Add(cell);
        }
        else
        {
            cell = Instantiate(cellPrefab).GetComponent<CellUI>();
            cell.transform.SetParent(parent);
            cell.onClickCallback = this.cellClick;
            _usingList.Add(cell);
        }

        return cell;
    }

    private void catchAllCell()
    {
        while(_usingList.Count > 0)
        {
            _usingList[0].gameObject.SetActive(false);
            _unusingList.Add(_usingList[0]);
            _usingList.RemoveAt(0);
        }
    }

    public Vector2 GetPositionByIndex(int x, int y)
    {
        return _posArray[x, y];
    }

    private void showWakableRange(int x, int y)
    {
        int xP = 0;
        int yP = 0;
        for (int i = -4; i < 5; i++)
        {
            for (int j = -4; j < 5; j++)
            {
                if (i + j > 4 || i + j < -4 ||
                    i - j > 4 || i - j < -4)
                    continue;

                xP = x + i;
                yP = y + j;
                if (xP < 0 || xP >= AppConstants.CellRowCount ||
                    yP < 0 || yP >= AppConstants.CellColumnCount)
                {
                    continue;
                }
                else
                {
                    CellUI cell = activeCell();
                    cell.SetData(xP, yP,StateType.Normal, GetPositionByIndex(xP, yP));
                }
            }
        }
    }

    private void showAttackRange(int x, int y)
    {
        for (int i = -4; i < 5; i++)
        {
            if(x + i >=0 && x + i < AppConstants.CellRowCount)
            {
                CellUI cell = activeCell();
                cell.SetData(x+i, y, StateType.Normal, GetPositionByIndex(x+i, y));
            }
        }

        for (int j = -4; j < 5; j++)
        { 
            if(y + j >=0 && y+j <AppConstants.CellColumnCount)
            {
                CellUI cell = activeCell();
                cell.SetData( x, y+j, StateType.Normal, GetPositionByIndex(x, y + j));
            }
        }
    }

    /// <summary>
    /// 鼠标点击位置触发事件
    /// </summary>
    /// <param name="vec"></param>
    private void cellClick(int x, int y)
    {
        player.SetDestination(x, y, GetPositionByIndex(x, y));
    }

	private void __onStratClick(GameObject go)
    {
        startImg.gameObject.SetActive(false);
    }
}
