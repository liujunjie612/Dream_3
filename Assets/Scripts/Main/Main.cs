using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {

    public Image startImg;
    public GameObject cellPrefab;
    public Transform parent;

    private List<CellUI> _celllist = new List<CellUI>();

	void Start () 
    {
        creatCell();

        startImg.rectTransform.SetAsLastSibling();

        EventTriggerListener.Get(startImg.gameObject).onClick = this.__onStratClick;
	}

    private void creatCell()
    {
        for (int i = 0; i < AppConstants.CellRowCount; i++)
        {
            for (int j = 0; j < AppConstants.CellColumnCount; j++)
            {
                CellUI ui = Instantiate(cellPrefab).GetComponent<CellUI>();
                ui.gameObject.transform.SetParent(parent);
                CellVo vo = new CellVo();
                vo.stateType = StateType.Transparent;
                ui.SetData(vo);

                _celllist.Add(ui);
            }
        }
    }
	
	private void __onStratClick(GameObject go)
    {
        startImg.gameObject.SetActive(false);
    }

}
