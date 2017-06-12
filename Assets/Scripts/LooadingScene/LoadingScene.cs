using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {

    public Slider progressSlider;
    public Text progressTxt;

    private AsyncOperation _loadingOperation;


	private void loading()
    {
        _loadingOperation =  SceneManager.LoadSceneAsync(AppConstants.MainScene);
    }

    void Start()
    {
        loading();
    }

    void Update()
    {
        if(_loadingOperation != null)
        {
            progressSlider.value = _loadingOperation.progress;
            progressTxt.text = _loadingOperation.progress * 100f + "%";
        }
    }
}
