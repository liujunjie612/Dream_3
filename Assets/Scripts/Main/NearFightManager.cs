using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearFightManager : MonoBehaviour {

    public AnimationGif wsAttack_1;
    public AnimationGif wsAttack_2;
    public AnimationGif wsAttack_3;
    public AnimationGif wsAttack_4;

    public AnimationGif bossAttack_1;
    public AnimationGif bossAttack_2;
    public AnimationGif bossAttack_3;
    public AnimationGif bossAttack_4;
    public AnimationGif bossAttack_5;
    public AnimationGif bossDefeat;

    private AnimationGif _startAnimation;
    private AnimationGif _endAnimation;

    void Start()
    {
        SetFight();
    }

    public void SetFight()
    {
        wsAttack_1.SetAnimation(true);
        bossDefeat.SetAnimation(false);
        wsAttack_1.animationEndCallback = () => { bossDefeat.SetAnimation(true); };
    }
}
