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

    public NearFightManager nearFightPanel;

    public PlayerController player;
    public PlayerController boss;
    private List<PlayerController> _playerList = new List<PlayerController>();

    private PlayerController _currentPlayer;
    private FSType _currentFSType;

    private int _playerCount = 0;

    private Vector2[,] _posArray;
    private List<CellUI> _usingList = new List<CellUI>();
    private List<CellUI> _unusingList = new List<CellUI>();

    private int _step = 0;

	void Start () 
    {
        resultPanel.SetActive(false);
        nearFightPanel.gameObject.SetActive(false);
        catchPosition();

        player.id = _playerCount++; ;
        player.moveEndCallback = moveEndCallback;
        player.overCallback = this.gameOver;
        player.SetDestination(10, 0, GetPositionByIndex(10, 0), false);
        player.SetAnimation(FSType.Idel, DirectionType.Right);

        boss.id = _playerCount++;
        boss.moveEndCallback = moveEndCallback;
        boss.overCallback = this.gameOver;
        boss.SetDestination(10, 25, GetPositionByIndex(10, 25), false);
        boss.SetAnimation(FSType.Idel, DirectionType.Left);

        _playerList.Add(player);
        _playerList.Add(boss);

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
                else if (xP == x && yP == y)
                {
                    continue;
                }
                else if (getPlayerByXY(xP, yP) != null)
                {
                    continue;
                }
                else
                {
                    CellUI cell = activeCell();
                    cell.SetData(xP, yP, StateType.Normal, GetPositionByIndex(xP, yP));
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
                PlayerController p = getPlayerByXY(x + i, y);
                if (p != null && p.tag.Equals(_currentPlayer.tag))
                    continue;

                CellUI cell = activeCell();
                cell.SetData(x+i, y, StateType.Normal, GetPositionByIndex(x+i, y));
            }
        }

        for (int j = -4; j < 5; j++)
        { 
            if(j != 0 && y + j >=0 && y+j <AppConstants.CellColumnCount)
            {
                PlayerController p = getPlayerByXY(x, y + j);
                if (p != null && p.tag.Equals(_currentPlayer.tag))
                    continue;

                CellUI cell = activeCell();
                cell.SetData( x, y+j, StateType.Normal, GetPositionByIndex(x, y + j));
            }
        }
    }

    /// <summary>
    /// 是否在攻击范围内
    /// </summary>
    /// <param name="me"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private List<PlayerController> checkAttackInRange(PlayerController me, int rangeX, int rangeY)
    {
        int x = me.x;
        int y = me.y;

        List<CellVo> list = new List<CellVo>();
        for (int i = -rangeX; i <= rangeX; i++)
        {
            if (i != 0 && x + i >= 0 && x + i < AppConstants.CellRowCount)
            {
                CellVo v = new CellVo();
                v.x = x + i;
                v.y = y;
                list.Add(v);
            }
        }

        for (int j = -rangeY; j <= rangeY; j++)
        {
            if (j != 0 && y + j >= 0 && y + j < AppConstants.CellColumnCount)
            {
                CellVo v = new CellVo();
                v.x = x;
                v.y = y + j;
                list.Add(v);
            }
        }

        List<PlayerController> pL = new List<PlayerController>();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < _playerList.Count; j++)
            {
                if (list[i].x == _playerList[j].x &&
                    list[i].y == _playerList[j].y &&
                    me.identityType != _playerList[j].identityType)
                {
                    pL.Add(_playerList[j]);
                }
            }
        }

        return pL;
              
    }

    private void nextStep()
    {
        _step++;
        if (_step % 2 > 0)
        {
            _currentPlayer = player;
            showUIPanel(_currentPlayer);
        }
        else
        {
            _currentPlayer = boss;
            bossAI();
        }
    }


    private void bossAI()
    {
        bool inMagic = checkMagicInRange(boss, 2, 2).Count > 0;
        bool inAttack = checkAttackInRange(boss, 4, 4).Count > 0;

        FSType t = FSType.Idel;

        if(!inMagic && !inAttack)
        {
            t = FSType.Walk;
        }
        else if(inMagic && !inAttack)
        {
            int index = Random.Range(0, 6);
            if(index == 0)
            {
                t = FSType.Walk;
            }
            else
            {
                t = FSType.Magic;
            }
        }
        else if(!inMagic && inAttack)
        {
             int index = Random.Range(0, 6);
             if (index == 0)
             {
                 t = FSType.Walk;
             }
             else
             {
                 t = FSType.Attack;
             }
        }
        else
        {
            int index = Random.Range(0, 10);

            if (index <= 5)
                t = FSType.Attack;
            else if (index <= 8)
                t = FSType.Magic;
            else
                t = FSType.Walk;
        }

        switch (t)
        {
            case FSType.Walk:
                 showWakableRange(boss.x, boss.y);
                 int index2 = Random.Range(0, _usingList.Count);

                 boss.SetDestination(_usingList[index2].x, _usingList[index2].y, GetPositionByIndex(_usingList[index2].x, _usingList[index2].y));
                break;
            case FSType.Attack:
                 List<PlayerController> list = checkAttackInRange(boss, 4, 4);
                 int index3 = Random.Range(0, list.Count);
                 PlayerController p = list[index3];

                 if (p != null && !p.tag.Equals(boss.identityType))
                 {
                     nearFightPanel.gameObject.SetActive(true);
                     nearFightPanel.SetFight(boss.identityType, p.SetDamege);

                     Debug.Log(p.name + "  " + p.identityType);
                     catchAllCell();
                     nextStep();
                 }
                break;
            case FSType.Magic:
                boss.SetAnimation(FSType.Magic, boss.currentDirectionType,
                   () =>
                   {
                       Debug.Log(_currentPlayer.name + "施展了魔法");
                       boss.SetAnimation(FSType.Idel, boss.currentDirectionType);
                       List<PlayerController> pList = checkMagicInRange(boss, 2, 2);
                       for (int i = 0; i < pList.Count; i++)
                       {
                           PlayerController p2 = pList[i];
                           p2.SetAnimation(FSType.Attacked, p2.currentDirectionType, () => { p2.SetAnimation(FSType.Idel, p2.currentDirectionType); nextStep(); });
                       }
                   });
                break;
            default:
                break;
        }


    }


    private void showUIPanel(PlayerController p)
    {
        uiPanel.gameObject.SetActive(true);
        uiPanel.anchoredPosition = GetPositionByIndex(p.x, p.y);

        if (checkMagicInRange(p, 2, 2).Count > 0)
            magicBtn.interactable = true;
        else
            magicBtn.interactable = false;
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
            () =>
            {
                Debug.Log(_currentPlayer.name + "施展了魔法");
                _currentPlayer.SetAnimation(FSType.Idel, _currentPlayer.currentDirectionType);
                List<PlayerController> pList = checkMagicInRange(_currentPlayer, 2 ,2);
                for (int i = 0; i < pList.Count; i++)
                {
                    PlayerController p = pList[i];
                    p.SetAnimation(FSType.Attacked, p.currentDirectionType, () => { p.SetAnimation(FSType.Idel, p.currentDirectionType); nextStep(); });
                }
            });
    }

    /// <summary>
    /// 获取魔法攻击范围内的对手
    /// </summary>
    /// <param name="me"></param>
    /// <param name="rangeX"></param>
    /// <param name="rangeY"></param>
    /// <returns></returns>
    private List<PlayerController> checkMagicInRange(PlayerController me, int rangeX, int rangeY)
    {
        int x = me.x;
        int y = me.y;

        List<CellVo> list = new List<CellVo>();
        for (int i = -rangeX; i <= rangeX; i++)
        {
            if (i != 0 && x + i >= 0 && x + i < AppConstants.CellRowCount)
            {
                CellVo v = new CellVo();
                v.x = x + i;
                v.y = y;
                list.Add(v);
            }
        }

        for (int j = -rangeY; j <= rangeY; j++)
        {
            if (j != 0 && y + j >= 0 && y + j < AppConstants.CellColumnCount)
            {
                CellVo v = new CellVo();
                v.x = x;
                v.y = y + j;
                list.Add(v);
            }
        }

        List<PlayerController> pL = new List<PlayerController>();
        for (int i = 0; i < list.Count; i++)
        {
            for(int j =0;j<_playerList.Count;j++)
            {
                if (list[i].x == _playerList[j].x &&
                    list[i].y == _playerList[j].y &&
                    me.tag != _playerList[j].tag)
                {
                    pL.Add(_playerList[j]);
                }
            }
        }

        return pL;
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

    /// <summary>
    /// 根据X,Y来获取Player
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private PlayerController getPlayerByXY (int x, int y)
    {   
        PlayerController p = null;
        for(int i=0;i<_playerList.Count;i++)
        {
            if(_playerList[i].x == x && _playerList[i].y == y)
            {
                p = _playerList[i];
                break;
            }
        }

        return p;
    }

    private void attack(int x, int y)
    {
        PlayerController p = getPlayerByXY(x, y);

        if(p != null && !p.tag.Equals(_currentPlayer))
        {
            nearFightPanel.gameObject.SetActive(true);
            nearFightPanel.SetFight(_currentPlayer.identityType, p.SetDamege);

            Debug.Log(p.name + "  " + p.identityType);
            catchAllCell();
            nextStep();
        }
    }

    private void gameOver(string tag)
    {
        resultPanel.SetActive(true);
        if (tag.Equals("Player"))
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
