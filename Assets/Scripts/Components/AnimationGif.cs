using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Yule.Components
{
    [RequireComponent(typeof (Image))]
    public class AnimationGif : MonoBehaviour
    {
        public Sprite[] animationSprites;
        public float animationSpeed = 0.1f;
        public bool loop = true;

        private Image _img;
        private float _curretnTime;
        private int _index;
        private float _length;
        private bool _continue;

        void Start()
        {
            _continue = true;
            _img = GetComponent<Image>();
            _length = animationSprites.Length;
        }

        private void OnEnable()
        {
            _continue = true;
            _curretnTime = Time.time;
            _index = 0;
        }

        private void Update()
        {
            if (_continue && Time.time - _curretnTime > animationSpeed)
            {
                _curretnTime = Time.time;
                _img.sprite = animationSprites[_index];
                _index++;
                if (_index >= _length)
                {
                    _index = 0;
                    if (!loop)
                        _continue = false;
                }
            }
        }
    }
}