using UnityEngine;
using UnityEditor;
using controller;
using skill;
using System.Linq;

namespace model
{

    public class Model
    {
        int _slot;

        GameObject _avatar;
        Animator _anim;
        ModelDataSet _data;
        Vector3 _originalSize;
        Material _originalMat;
        PassiveActive PassiveFunction;
        Sprite[] _spriteListloaded = null;
        string _spriteSetLoaded = "";


        public void setModel(int slot,ModelDataSet data,GameObject avatar)
        {
            _slot = slot;
            _data = data;
            _avatar = avatar;
            _anim = _avatar.GetComponent<Animator>();
        }

        public int getId()
        {
            return _data.id;
        }

        public int getSlot()
        {
            return _slot;
        }
        
        public BattleController getBatCon()
        {
            return GameCore.call()._battleCon;
        }

        public BuffController getbuffCon()
        {
            return GameCore.call()._buffCon;
        }

        public AttackController getAtkCon()
        {
            return GameCore.call()._attackCon;
        }
        
        public Animator getAnim()
        {
            return _anim;
        }

        public Transform getAvatarTrans()
        {
            return _avatar.transform;
        }

        public Vector3 getAvatarPos()
        {
            return getAvatarTrans().position;
        }

        public void setAvatarPos(Vector3 pos)
        {
            getAvatarTrans().position = pos;
        }

        public void setAvatarSprite(Sprite sprite)
        {
            _avatar.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        public void setAvatarActive(bool active)
        {
            _avatar.SetActive(active);
        }

        public Material getAvatarMat()
        {
            return _avatar.GetComponent<SpriteRenderer>().material;
        }

        public void setAvatarMat(Material mat)
        {
            _avatar.GetComponent<SpriteRenderer>().material = mat;
        }
        

        public string getName()
        {
            return _data.name;
        }

        public string getSpriteSet()
        {
            return _data.spriteSet;
        }

        public string getSpriteName()
        {
            return _data.spriteName;
        }

        public void Anim(bool ena)
        {
            getAnim().enabled = ena;
        }
        
        public Vector3 getOriginalSize()
        {
            return _originalSize;
        }

        public void setOriginalSize(Vector3 size)
        {
            _originalSize = size;
        }

        public Material getOriginalMat()
        {
            return _originalMat;
        }

        public void setOriginalMat(Material mat)
        {
            _originalMat = mat;
        }
        

        public void OnPassiveWorking(string passive,_Model model)
        {
            OnPassiveFunction(passive, model);
        }
        
        private void OnPassiveFunction(string methodName, params object[] parameter)
        {
            if (PassiveFunction == null)
                PassiveFunction = new PassiveActive();

            var method = PassiveFunction.GetType().GetMethod(methodName);
            if (method != null)
                method.Invoke(PassiveFunction, parameter);
        }

        public void LoadSprite()
        {
            if (_spriteSetLoaded != getSpriteSet())
            {
                _spriteSetLoaded = getSpriteSet();
                if (_spriteSetLoaded.Contains("monster"))
                {
                    _spriteListloaded = Resources.LoadAll<Sprite>("Sprites/Character/Monster/" + _spriteSetLoaded);
                }
                else
                {
                    _spriteListloaded = Resources.LoadAll<Sprite>("Sprites/Character/Hero/" + _spriteSetLoaded);
                }
            }
            setAvatarSprite(_spriteListloaded.Single(s => s.name == getSpriteName()));
            setAvatarActive(true);
            setOriginalSize(getAvatarTrans().parent.localScale);
            getBatCon()._battleState = _BattleState.Finish;
        }
    }
}
