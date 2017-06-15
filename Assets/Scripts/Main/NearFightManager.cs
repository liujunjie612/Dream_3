using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NearFightManager : MonoBehaviour {

    public AnimationGif wsIdleAttack;
    public AnimationGif wsIdleAttacked;
    public AnimationGif wsAttack_1;
    public AnimationGif wsAttack_2;
    public AnimationGif wsEvade;
    public AnimationGif wsDefense;
    public AnimationGif wsDefeat;

    public AnimationGif bossIdleAttack;
    public AnimationGif bossIdleAttacked;
    public AnimationGif bossAttack_1;
    public AnimationGif bossAttack_2;
    public AnimationGif bossAttack_3;
    public AnimationGif bossEvade;
    public AnimationGif bossDefense;
    public AnimationGif bossDefeat;

    public Button attack1Btn;
    public Button attack2Btn;

    public Button evadeBtn;
    public Button defenseBtn;

    public Text hpTxt;

    private Action<int> _callback;

    void Start()
    {
        attack1Btn.onClick.AddListener(this.__onAttack1);
        attack2Btn.onClick.AddListener(this.__onAttack2);
        evadeBtn.onClick.AddListener(this.__onEvade);
        defenseBtn.onClick.AddListener(this.__onDefense);
    }

    public void SetFight(IdentityType attackType, Action<int> callback)
    {
        _callback = callback;
        catchAll();

        if(attackType == IdentityType.Player)
        {
            wsIdleAttack.gameObject.SetActive(true);
            bossIdleAttacked.gameObject.SetActive(true);
            attack1Btn.gameObject.SetActive(true);
            attack2Btn.gameObject.SetActive(true);
        }
        else
        {
            wsIdleAttacked.gameObject.SetActive(true);
            bossIdleAttack.gameObject.SetActive(true);
            evadeBtn.gameObject.SetActive(true);
            defenseBtn.gameObject.SetActive(true);
        }
        
    }

    private void __onAttack1()
    {
        attack1Btn.gameObject.SetActive(false);
        attack2Btn.gameObject.SetActive(false);

        wsIdleAttack.gameObject.SetActive(false);
        wsAttack_1.gameObject.SetActive(true);
        wsAttack_1.animationEndCallback = () =>
            {
                wsAttack_1.gameObject.SetActive(false);
                wsIdleAttack.gameObject.SetActive(true);
                bossIdleAttacked.gameObject.SetActive(false);
                int hurt = UnityEngine.Random.Range(1, 20);
                if (hurt < 5)
                {
                    bossEvade.gameObject.SetActive(true);
                    bossEvade.animationEndCallback = () =>
                    {
                        bossEvade.gameObject.SetActive(false);
                        bossIdleAttacked.gameObject.SetActive(true);
                    };
                }
                else if (hurt < 15)
                {
                    bossDefense.gameObject.SetActive(true);
                    bossDefense.animationEndCallback = () =>
                    {
                        bossDefense.gameObject.SetActive(false);
                        bossIdleAttacked.gameObject.SetActive(true);
                    };
                }
                else
                    bossDefeat.gameObject.SetActive(true);

                hpTxt.gameObject.SetActive(true);
                hpTxt.rectTransform.anchoredPosition = new Vector2 (600,0);
                hpTxt.text = "-" + hurt;
                Tweener t = hpTxt.rectTransform.DOLocalMoveY(160, 1f);
                t.OnComplete(() =>
                { 
                    this.gameObject.SetActive(false);
                    _callback(hurt);
                });
            };
    }

    private void __onAttack2()
    {
        attack1Btn.gameObject.SetActive(false);
        attack2Btn.gameObject.SetActive(false);

        wsIdleAttack.gameObject.SetActive(false);
        wsAttack_2.gameObject.SetActive(true);
        wsAttack_2.animationEndCallback = () =>
        {
            wsAttack_2.gameObject.SetActive(false);
            wsIdleAttack.gameObject.SetActive(true);
            bossIdleAttacked.gameObject.SetActive(false);
            int hurt = UnityEngine.Random.Range(1, 20);
            if (hurt < 5)
            {
                bossEvade.gameObject.SetActive(true);
                bossEvade.animationEndCallback = () =>
                {
                    bossEvade.gameObject.SetActive(false);
                    bossIdleAttacked.gameObject.SetActive(true);
                };
            }
            else if (hurt < 15)
            {
                bossDefense.gameObject.SetActive(true);
                bossDefense.animationEndCallback = () =>
                {
                    bossDefense.gameObject.SetActive(false);
                    bossIdleAttacked.gameObject.SetActive(true);
                };
            }
            else
                bossDefeat.gameObject.SetActive(true);

            hpTxt.gameObject.SetActive(true);
            hpTxt.rectTransform.anchoredPosition = new Vector2(600, 0);
            hpTxt.text = "-" + hurt;
            Tweener t = hpTxt.rectTransform.DOLocalMoveY(160, 1f);
            t.OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                _callback(hurt);
            });
        };
    }

    private void __onEvade()
    {
        evadeBtn.gameObject.SetActive(false);
        defenseBtn.gameObject.SetActive(false);

        bossIdleAttack.gameObject.SetActive(false);
        bossAttack_1.gameObject.SetActive(true);
        bossAttack_1.animationEndCallback = () =>
        {
            bossAttack_1.gameObject.SetActive(false);
            bossIdleAttack.gameObject.SetActive(true);
            wsIdleAttacked.gameObject.SetActive(false);
            int hurt = UnityEngine.Random.Range(1, 10);
            if (hurt < 5)
            {
                wsEvade.gameObject.SetActive(true);
                wsEvade.animationEndCallback = () =>
                {
                    wsEvade.gameObject.SetActive(false);
                    wsIdleAttacked.gameObject.SetActive(true);
                };
            }
            else
                wsDefeat.gameObject.SetActive(true);

            hpTxt.gameObject.SetActive(true);
            hpTxt.rectTransform.anchoredPosition = new Vector2(600, 0);
            hpTxt.text = "-" + hurt;
            Tweener t = hpTxt.rectTransform.DOLocalMoveY(160, 1f);
            t.OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                _callback(hurt);
            });
        };
    }

    private void __onDefense()
    {
        evadeBtn.gameObject.SetActive(false);
        defenseBtn.gameObject.SetActive(false);

        bossIdleAttack.gameObject.SetActive(false);
        bossAttack_1.gameObject.SetActive(true);
        bossAttack_1.animationEndCallback = () =>
        {
            bossAttack_1.gameObject.SetActive(false);
            bossIdleAttack.gameObject.SetActive(true);
            wsIdleAttacked.gameObject.SetActive(false);
            int hurt = UnityEngine.Random.Range(1, 10);
            if (hurt < 5)
            {
                wsDefense.gameObject.SetActive(true);
                wsDefense.animationEndCallback = () =>
                {
                    wsDefense.gameObject.SetActive(false);
                    wsIdleAttacked.gameObject.SetActive(true);
                };
            }
            else
                wsDefeat.gameObject.SetActive(true);

            hpTxt.gameObject.SetActive(true);
            hpTxt.rectTransform.anchoredPosition = new Vector2(600, 0);
            hpTxt.text = "-" + hurt;
            Tweener t = hpTxt.rectTransform.DOLocalMoveY(160, 1f);
            t.OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                _callback(hurt);
            });
        };
    }

    private void catchAll()
    {
        wsIdleAttack.gameObject.SetActive(false);
        wsIdleAttacked.gameObject.SetActive(false);
        wsAttack_1.gameObject.SetActive(false);
        wsAttack_2.gameObject.SetActive(false);
        wsEvade.gameObject.SetActive(false);
        wsDefense.gameObject.SetActive(false);
        wsDefeat.gameObject.SetActive(false);
        bossIdleAttack.gameObject.SetActive(false);
        bossIdleAttacked.gameObject.SetActive(false);
        bossAttack_1.gameObject.SetActive(false);
        bossAttack_2.gameObject.SetActive(false);
        bossAttack_3.gameObject.SetActive(false);
        bossEvade.gameObject.SetActive(false);
        bossDefense.gameObject.SetActive(false);
        bossDefeat.gameObject.SetActive(false);

        attack1Btn.gameObject.SetActive(false);
        attack2Btn.gameObject.SetActive(false);
        evadeBtn.gameObject.SetActive(false);
        defenseBtn.gameObject.SetActive(false);
    }
}
