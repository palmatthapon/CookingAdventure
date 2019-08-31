
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TutorialPanel : MonoBehaviour
    {
        GameCore _core;
        public SpriteRenderer _tutorialShow;
        public GameObject _prevBtn, _nextBtn;
        public Sprite _coverSprite;
        public Sprite[] _heroSprite;
        public Sprite[] _mapSprite;
        public Sprite[] _battleSprite;
        public Sprite[] _campSprite;
        public Sprite[] _shopSprite;
        Sprite[] _nextMode;
        int _index;

        private void Awake()
        {
            _core = Camera.main.GetComponent<GameCore>();
        }

        private void OnEnable()
        {
            
            _prevBtn.SetActive(false);
            _nextBtn.SetActive(false);
        }

        public void Hero()
        {
            _prevBtn.SetActive(false);
            if (_heroSprite.Length > 1)
            {
                _nextBtn.SetActive(true);
            }
            else
            {
                _nextBtn.SetActive(false);
            }
            _index = 0;
            _nextMode = _heroSprite;
            _tutorialShow.sprite = _heroSprite[_index];
        }
        public void Map()
        {
            _prevBtn.SetActive(false);
            if (_mapSprite.Length > 1)
            {
                _nextBtn.SetActive(true);
            }
            else
            {
                _nextBtn.SetActive(false);
            }
            _index = 0;
            _nextMode = _mapSprite;
            _tutorialShow.sprite = _mapSprite[_index];
        }
        public void Battle()
        {
            _prevBtn.SetActive(false);
            if (_battleSprite.Length > 1)
            {
                _nextBtn.SetActive(true);
            }
            else
            {
                _nextBtn.SetActive(false);
            }
            _index = 0;
            _nextMode = _battleSprite;
            _tutorialShow.sprite = _battleSprite[_index];
        }
        public void Camp()
        {
            _prevBtn.SetActive(false);
            if (_campSprite.Length > 1)
            {
                _nextBtn.SetActive(true);
            }
            else
            {
                _nextBtn.SetActive(false);
            }
            _index = 0;
            _nextMode = _campSprite;
            _tutorialShow.sprite = _campSprite[_index];
        }
        public void Shop()
        {
            _prevBtn.SetActive(false);
            if (_shopSprite.Length > 1)
            {
                _nextBtn.SetActive(true);
            }
            else
            {
                _nextBtn.SetActive(false);
            }
            _index = 0;
            _nextMode = _shopSprite;
            _tutorialShow.sprite = _shopSprite[_index];
        }

        public void PrevBtn()
        {
            _index--;
            _nextBtn.SetActive(true);
            if (_index == 0) _prevBtn.SetActive(false);
            _tutorialShow.sprite = _nextMode[_index];

        }

        public void NextBtn()
        {
            _index++;
            _prevBtn.SetActive(true);
            if (_index == _nextMode.Length-1) _nextBtn.SetActive(false);
            _tutorialShow.sprite = _nextMode[_index];

        }

        public void Open()
        {
            transform.gameObject.SetActive(true);
            _tutorialShow.sprite = _coverSprite;
        }

        public void Close()
        {
            transform.gameObject.SetActive(false);
        }
    }

}
