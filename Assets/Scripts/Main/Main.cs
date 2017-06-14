using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public Image startImg;
    public GameObject cellPrefab;
    public Transform parent;

    public RectTransform uiPanel;
    public Button attackBtn;
    public Button moveBtn;
    public Button magicBtn;
    public Button skipBtn;

    public GameObject resultPanel;
    public Text resultTxt;
    public Button replayBtn;

    public PlayerController player;
    public PlayerController boss;

    private PlayerController _currentPlayer;
    private FSType _currentFSType;

    private Vector2[,] _posArray;
    private List<CellUI> _usingList = new List<CellUI>();
    private List<CellUI> _unusingList = new List<CellUI>();

    private int _step = 0;

	void Start () 
    {
        resultPanel.SetActive(false);
        catchPosition();

        player.moveEndCallback = moveEndCallback;
        player.overCallback = this.gameOver;
        player.SetDestination(10, 0, GetPositionByIndex(10, 0), false);
        player.SetAnimation(FSType.Idel, DirectionType.Right);

        boss.moveEndCallback = moveEndCallback;
        boss.overCallback = this.gameOver;
        boss.SetDestination(10, 25, GetPositionByIndex(10, 25), false);
        boss.SetAnimation(FSType.Idel, DirectionType.Left);

        nextStep();
        //showWakableRange(10, 0);
        //showAttackRange(10, 10);

        startImg.gameObject.SetActive(true);
        startImg.rectTransform.SetAsLastSibling();

        EventTriggerListener.Get(startImg.gameObject).onClick = this.__onStratClick;
        attackBtn.onClick.AddListener(this.__onAttackClick);
        magicBtn.onClick.AddListener(this.__onMagicClick);
        moveBtn.onClick.AddListener(this.__onMoveClick);
        skipBtn.onClick.AddListener(this.__onSkipClick);

        replayBtn.onClick.AddListener(this.__onReplayClick);
	}

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            if (_usingList.Count > 0 && uiPanel.gameObject.activeSelf == false)
            {
                catchAllCell();
                showUIPanel(_currentPlayer);
            }
        }
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

    private void moveEndCallback()
    {
        catchAllCell();
        nextStep();
    }

    public Vector2 GetPositionByIndex(int x, int y)
    {
        return _posArray[x, y];
    }

    private void showWakableRange(int x, int y)
    {
        _currentFSType = FSType.Walk;

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
                    else if(xP == x && yP == y)
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
        _currentFSType = FSType.Attack;

        for (int i = -4; i < 5; i++)
        {
            if(i != 0 && x + i >=0 && x + i < AppConstants.CellRowCount)
            {
                CellUI cell = activeCell();
                cell.SetData(x+i, y, StateType.Normal, GetPositionByIndex(x+i, y));
            }
        }

        for (int j = -4; j < 5; j++)
        { 
            if(j != 0 && y + j >=0 && y+j <AppConstants.CellColumnCount)
            {
                CellUI cell = activeCell();
                cell.SetData( x, y+j, StateType.Normal, GetPositionByIndex(x, y + j));
            }
        }
    }

    private void nextStep()
    {
        _step++;
        if (_step % 2 > 0)
        {
            _currentPlayer = player;
        }
        else
        {
            _currentPlayer = boss;
        }

        showUIPanel(_currentPlayer);
    }


    private void showUIPanel(PlayerController p)
    {
        uiPanel.gameObject.SetActive(true);
        uiPanel.anchoredPosition = GetPositionByIndex(p.x, p.y);
    }

    private void __onMoveClick()
    {
        showWakableRange(_currentPlayer.x, _currentPlayer.y);
        uiPanel.gameObject.SetActive(false);
    }

    private void __onAttackClick()
    {
        showAttackRange(_currentPlayer.x, _currentPlayer.y);
        uiPanel.gameObject.SetActive(false);
    }

    private void __onMagicClick()
    {
        uiPanel.gameObject.SetActive(false);
        _currentPlayer.SetAnimation(FSType.Magic, _currentPlayer.currentDirectionType, 
            () => { Debug.Log(_currentPlayer.name + "施展了魔法");
            _currentPlayer.SetAnimation(FSType.Idel, _currentPlayer.currentDirectionType);
            if (_step % 2 > 0)
                boss.SetAnimation(FSType.Attacked, boss.currentDirectionType, () => { boss.SetAnimation(FSType.Idel, boss.currentDirectionType); nextStep(); });
            else
                player.SetAnimation(FSType.Attacked, player.currentDirectionType, () => { player.SetAnimation(FSType.Idel, player.currentDirectionType); nextStep(); });
               });
    }

    private void __onSkipClick()
    {
        nextStep();
    }

    /// <summary>
    /// 鼠标点击位置触发事件
    /// </summary>
    /// <param name="vec"></param>
    private void cellClick(int x, int y)
    {
        switch (_currentFSType)
        {
            case FSType.Idel:
                break;
            case FSType.Walk:
                 _currentPlayer.SetDestination(x, y, GetPositionByIndex(x, y));
                break;
            case FSType.Attack:
                attack(x, y);
                break;
            case FSType.Attacked:
                break;
            case FSType.Magic:
                break;
            case FSType.Evade:
                break;
            case FSType.Defense:
                break;
            case FSType.Defeat:
                break;
            default:
                break;
        }
    }

    private void attack(int x, int y)
    {
        PlayerController p = null;
        if (_step % 2 > 0)
            p = boss;
        else
            p = player;

        if(x == p.x && y == p.y)
        {
            Debug.Log("攻击：" + p.name);
        }
    }

    private void gameOver(string tag)
    {
        resultPanel.SetActive(true);
        if (tag == "Player")
            resultTxt.text = "失败";
        else
            resultTxt.text = "胜利";
    }

    private void __onReplayClick()
    {
        SceneManager.LoadScene(0);
    }

	private void __onStratClick(GameObject go)
    {
        startImg.gameObject.SetActive(false);
    }
}
