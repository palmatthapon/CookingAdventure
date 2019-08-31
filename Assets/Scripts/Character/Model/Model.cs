using UnityEngine;
using controller;
using battle;
using System.Linq;
using system;

namespace model
{

    public class Model
    {
        GameCore _core = Camera.main.GetComponent<GameCore>();

        int _slot;

        GameObject _avatar;
        Animator _anim;
        ModelDataSet _data;
        Vector3 _originalSize;
        Material _originalMat;
        PassiveActive PassiveFunction;
        Sprite[] _spriteListloaded = null;
        string _spriteSetLoaded = "";

        GameObject[] loadEffectPlayer;
        GameObject[] loadEffectMonster;

        Vector3 endPosition;
        Vector3 originalPos;
        bool startInjur;
        bool injur;
        float speed = 4f;


        public void setModel(ModelDataSet data)
        {
            _data = data;
        }

        public int getId()
        {
            return _data.id;
        }

        public int getSlot()
        {
            return _slot;
        }

        public void setSlot(int slot)
        {
            _slot = slot;

        }

        public GameCore getCore()
        {
            return _core;
        }
        
        public BattleController getBattCon()
        {
            return _core.getBattCon();
        }
        
        public AttackController getAtkCon()
        {
            return _core.getATKCon();
        }

        public void setAvatar(GameObject avatar)
        {
            _avatar = avatar;
            _anim = _avatar.GetComponent<Animator>();
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
        }

        public void CreateAttackEffect(SkillDataSet skill, _Model target)
        {
            if (target == _Model.MONSTER)
            {
                if (loadEffectPlayer == null)
                    loadEffectPlayer = Resources.LoadAll<GameObject>("Effects/Effect_Player/");

                for (int i = 0; i < loadEffectPlayer.Length; i++)
                {
                    if (skill.effect == loadEffectPlayer[i].name)
                    {
                        getBattCon().CloneAttackEffect(skill, loadEffectPlayer[i], getBattCon().FocusMonster().getAvatarTrans(), target);
                        break;
                    }
                }

            }
            else
            {

                if (loadEffectMonster == null)
                    loadEffectMonster = Resources.LoadAll<GameObject>("Effects/Effect_Monster/");

                for (int i = 0; i < loadEffectMonster.Length; i++)
                {
                    if (skill.effect == loadEffectMonster[i].name)
                    {
                        getBattCon().CloneAttackEffect(skill, loadEffectMonster[i], getBattCon().FocusHero().getAvatarTrans(), target);
                        break;
                    }
                }
            }

        }

        public void RunInjury(Vector3 pos)
        {
            injur = true;
            originalPos = getAvatarPos();

            endPosition = pos;

            setOriginalMat(getAvatarMat());
            setAvatarMat(getBattCon()._injuryMat);

            getAnim().SetTrigger("IsInjury");

            startInjur = true;
        }

        public bool CheckInjury()
        {
            if (getAnim() == null) return false;
            if (startInjur && getAvatarPos() != endPosition)
            {
                setAvatarPos(Vector3.MoveTowards(getAvatarPos(), endPosition, speed * Time.deltaTime));
                return true;
            }
            else if (injur)
            {
                injur = false;
                endPosition = originalPos;
                return true;
            }
            if (startInjur)
            {
                Debug.Log("check injury");
                setAvatarMat(getOriginalMat());
            }
            startInjur = false;
            return false;
        }
        
    }
}
