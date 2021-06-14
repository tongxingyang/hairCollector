using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace week
{
    public class inquest
    {
        public InQuestKeyList _selectedQuest { get; protected set; }    // 퀘스트 번호
        public string _title{ get; protected set; }                     // 퀘스트 명

        inQuest_goal_key _g_key;                                        // 퀘스트 타입
        string _g_val_type;                                             // 퀘스트 목표(가공전)
        inQuest_goal_valtype _gval_standard;                            // 퀘목 - 일반타입
        gainableTem _gval_tem;                                          // 퀘목 - 템타입

        inQuest_goal_condition _g_condition;                            // 퀘스트 달성 제약 타입
        int _g_condition_val;                                           // 퀘스트 달성 제약 값
        
        public float limit { get; protected set; }                      // 목표값
        public float questVal { get; protected set; }                   // 실제 값

        //===========================================================================================

        public GameScene _gs { get; set; }
        public bool isUsed { get; protected set; }

        public Action<inQuest_goal_key, inQuest_goal_valtype, float> reCheckData;
        public qIconBox IconBox;
        //public Action<float> progressAmount;    // UI 진행률

        public bool _aboutTem 
        {
            get
            {
                if (isUsed)
                {
                    return _g_key == inQuest_goal_key.tem && _g_condition == inQuest_goal_condition.skill;
                }
                return false;
            }
        }
        public string _tem { get => _g_val_type; }

        //===========================================================================================

        public void Init(InQuestKeyList iq)
        {
            isUsed = true;

            _selectedQuest = iq;
            _title = D_inquest.GetEntity(_selectedQuest.ToString()).f_title;

            // 달성 키
            _g_key = D_inquest.GetEntity(_selectedQuest.ToString()).f_goal_Key;
            // 달성 조건
            _g_val_type = D_inquest.GetEntity(_selectedQuest.ToString()).f_goal_val_type;
            if (_g_key == inQuest_goal_key.tem)
            {
                _gval_standard = inQuest_goal_valtype.non;
                _gval_tem = EnumHelper.StringToEnum<gainableTem>(_g_val_type);
            }
            else
            {
                _gval_standard = EnumHelper.StringToEnum<inQuest_goal_valtype>(_g_val_type);
                _gval_tem = gainableTem.non;
            }

            // 추가조건
            _g_condition = D_inquest.GetEntity(_selectedQuest.ToString()).f_goal_Condition;
            _g_condition_val = D_inquest.GetEntity(_selectedQuest.ToString()).f_goal_condition_val;            

            limit = D_inquest.GetEntity(_selectedQuest.ToString()).f_goal_val;
            questVal = 0;
        }

        /// <summary> 들어온 데이터로 퀘스트 처리 </summary>
        public void getData(inQuest_goal_key gk, inQuest_goal_valtype gv, float val)
        {
            // 사용중 체크 / 템 체크(템 모으기 퀘는 다른곳에서)
            if (isUsed == false || _g_key == inQuest_goal_key.tem)
                return;

            extraCheck(val, gk, gv);

            // 키 체크
            if (_g_key != gk)
            {                
                return;
            }

            if (_gval_standard == inQuest_goal_valtype.conti)   // 연속퀘에서 (연속적중 or 안맞고 연속 시간흐름)
            {
                if (val == 0)
                {                                               // 빗나가면
                    questVal = 0;                               // 초기화  
                    IconBox.progressAmount(questVal / limit);
                    return;
                }
            }
            else if (_gval_standard == inQuest_goal_valtype.time)   // 시간퀘에서
            {
                if (val <= _g_condition_val * 0.01f)                // 값이 조건 보다 작으면
                {
                    questVal += Time.deltaTime;                     // 진행
                }
                val = 0;                                            // questVal에 더해져야 하지만 (여기서는 비교용으로 사용)
            }
            else if (_gval_standard != gv)  // 조건이 다르지만 수용될때
            {
                if (_gval_standard == inQuest_goal_valtype.all && gv <= inQuest_goal_valtype.hard)
                { /* all   일때는 모든몹 처치  수용 */ }
                else if (_gval_standard == inQuest_goal_valtype.enemy && (gv == inQuest_goal_valtype.mob || gv == inQuest_goal_valtype.boss))
                { /* enemy 일때는 모든몹데미지 수용 */ }
                else
                { return; }
            }

            if(_gval_standard != inQuest_goal_valtype.time)
                SoundManager.instance.PlaySFX(SFX.inquestup);

            // 진행률 ui
            questVal += val;
            IconBox.progressAmount(questVal / limit);

            // 퀘스트 달성
            if (questVal >= limit)
            {
                completeGoal();
            }
        }

        /// <summary> 들어온 데이터로 퀘스트 처리 [템습득] </summary>
        public void getData(inQuest_goal_key gk, gainableTem tem, float val)
        {
            // Debug.Log(tem + " == " + _gval_tem + " , " + _g_key + " == " + gk);
            if (isUsed == false) return;    // 미사용중 => 퀘스트 없음
            if (_g_key != gk) return;       // 잘못된 키
            
            if (_gval_tem != tem) return;     // 틀린 보조키

            SoundManager.instance.PlaySFX(SFX.inquestup);
            // 진행률 ui
            questVal += val;
            IconBox.progressAmount(questVal / limit);

            // 퀘스트 달성
            if (questVal >= limit)
            {
                completeGoal();
            }
        }

        /// <summary> 내부 값만으로 체크하는 추가 설정 </summary>
        void extraCheck(float val, inQuest_goal_key gk, inQuest_goal_valtype gv)
        {
            //if (_g_key == inQuest_goal_key.skill && gk == inQuest_goal_key.skill &&
            //    _gval_standard == inQuest_goal_valtype.conti && gv == inQuest_goal_valtype.conti && val == 0) // 눈덩이 연속
            //{
            //    questVal = 0;
            //}
            //else 
            if (_g_key == inQuest_goal_key.time && _gval_standard == inQuest_goal_valtype.conti // 안맞고 버틸때
                && gv == inQuest_goal_valtype.take)                                             // 맞으면 초기화
            {
                questVal = 0;
            }
        }

        /// <summary> 달성조건 - 이상/이하 </summary>
        public bool condition_border(float val)
        {
            if (isUsed == false)
                return false;

            if (_g_condition == inQuest_goal_condition.under )
            {
                return val <= _g_condition_val * 0.01f;
            }

            return true;
        }

        /// <summary> 달성조건 - 스킬적중 </summary>
        public bool condition_skill(SkillKeyList sk)
        {
            if (isUsed == false)
                return false;

            return _g_condition.Equals(sk.ToString());
        }

        /// <summary> 퀘스트 완료 </summary>
        public void completeGoal()
        {
            // 보상 구분 키
            inQuest_reward_key _r_key = D_inquest.GetEntity(_selectedQuest.ToString()).f_reward_Key;

            // 보상 구분 키2
            string[] rvt = D_inquest.GetEntity(_selectedQuest.ToString()).f_reward_val_type.Split(',');
            inQuest_reward_valtype[] _r_vt = new inQuest_reward_valtype[rvt.Length];
            for (int i = 0; i < rvt.Length; i++)
            {
                _r_vt[i] = EnumHelper.StringToEnum<inQuest_reward_valtype>(rvt[i]);
            }

            // 보상 내용
            string[] rv = D_inquest.GetEntity(_selectedQuest.ToString()).f_reward_val.Split(',');
            SkillKeyList _skill = SkillKeyList.non;
            float[] r_val;

            // [보상 내용] 구체화
            if (_r_vt[0] == inQuest_reward_valtype.skill)
            {
                _skill = EnumHelper.StringToEnum<SkillKeyList>(rv[0]);
                r_val = new float[] { int.Parse(rv[1]) };
            }
            else
            {
                r_val = new float[rv.Length];
                for (int i = 0; i < rv.Length; i++)
                {
                    r_val[i] = float.Parse(rv[i]);
                }
            }

            // 보상 지급 및 알림 설정
            notiData _data; 
            switch (_r_key)
            {
                case inQuest_reward_key.get_stat:
                    for (int i = 0; i < _r_vt.Length; i++)
                    {
                        SkillKeyList skl = EnumHelper.StringToEnum<SkillKeyList>(_r_vt[i].ToString());
                        _gs.Player.getQuestReward(skl, r_val[i]);

                        _data = new notiData(NotiType.clearQuest);
                        _data._skill = skl;
                        _data._rewardKey = _r_key;
                        _gs.InGameInterface.getNotiUI(_data);
                    }
                    break;
                case inQuest_reward_key.get_coin:
                    {
                        _gs.getCoin(r_val[0]);

                        _data = new notiData(NotiType.clearQuest);
                        _data._rewardKey = _r_key;
                        _gs.InGameInterface.getNotiUI(_data);
                    }
                    break;
                case inQuest_reward_key.get_point:
                    _gs.confirm_skill(_skill, NotiType.non);
                    break;
                case inQuest_reward_key.get_skill:
                    for (int i = 0; i < r_val[0]; i++)
                    {
                        _gs.Player.getSkill(_skill, NotiType.clearQuest);
                    }
                    break;
                case inQuest_reward_key.get_tem:
                    _gs.Player.getTem(EnumHelper.StringToEnum<gainableTem>(rv[0]));
                    break;
            }
            // 퀘 달성            
            _gs.clearQuest();
            SoundManager.instance.PlaySFX(SFX.inquestClear);
            _gs.Player._bffParticle.clearQst();

            // 사용종료
            isUsed = false;

            IconBox.boxOff();
            reCheckData(inQuest_goal_key.kill, inQuest_goal_valtype.quest, 1);
        }

        /// <summary> 리셋 </summary>
        public void reset(bool success)
        {
            if (success)
                reCheckData(inQuest_goal_key.kill, inQuest_goal_valtype.quest, 1);

            isUsed = false;
        }
    }

    public class snowQuestPopup : MonoBehaviour
    {
        [Serializable]
        class suggestQuestBox
        {
            [SerializeField] public Button q_Btn;
            [SerializeField] public Image q_col;
            [SerializeField] public TextMeshProUGUI q_title;
            [SerializeField] public Image q_img;
            [SerializeField] public TextMeshProUGUI q_ex;
            [SerializeField] public TextMeshProUGUI g_txt;
            [SerializeField] public TextMeshProUGUI r_txt;
            [SerializeField] public Image r_img;

            public void setBtn(InQuestKeyList iq)
            {
                q_title.text = D_inquest.GetEntity(iq.ToString()).f_title;

                string[] exs = D_inquest.GetEntity(iq.ToString()).f_explain.Split(',');
                q_ex.text = exs[0];
                for (int i = 1; i < exs.Length; i++)
                {
                    q_ex.text += System.Environment.NewLine + exs[i];
                }

                q_img.sprite = DataManager.QuestSprite[iq];
                
                int val = D_inquest.GetEntity(iq.ToString()).f_goal_val * D_level.GetEntity(BaseManager.userGameData.NowStageLevel.ToString()).f_mobrate;
                exs = D_inquest.GetEntity(iq.ToString()).f_goal_Ex.Replace("n", val.ToString()).Split(',');
                g_txt.text = exs[0];
                for (int i = 1; i < exs.Length; i++)
                {
                    g_txt.text += System.Environment.NewLine + exs[i];
                }

                exs = D_inquest.GetEntity(iq.ToString()).f_reward_Ex.Split(',');
                r_txt.text = exs[0];
                for (int i = 1; i < exs.Length; i++)
                {
                    r_txt.text += System.Environment.NewLine + exs[i];
                }
            }

            public void setColor(Color col)
            {
                Color c = col * 0.75f;
                c.a = 1f;
                r_img.color = c;

                col.a = 0.5f;
                q_col.color = col;
            }
        }

        [SerializeField] GameObject _popup;
        [Header("Quest Btn")]
        [SerializeField] suggestQuestBox[] _suggestQBox;
        [Header("Cancel Btn")]
        [SerializeField] Button _cancel;
        [SerializeField] TextMeshProUGUI _exchange;

        [Space]
        [Header("Quest icon")]
        [SerializeField] GameObject _qIconFab;
        [SerializeField] Transform _icon_parent;

        GameScene _gs;

        //==============================================================================================
        /// <summary> 현재 진행중인 퀘스트 </summary>
        public inquest[] SelectedQuest { get; private set; }

        // 퀘스트 리스트
        public List<InQuestKeyList> QList { get; set; }
        // 제안
        InQuestKeyList[] _suggest;
        int _exchangeCoin;

        //==============================================================================================
        private void Awake()
        {
            Vector3 size = Vector3.one * (((float)Screen.width / Screen.height) == 0.5625f ? 0.85f : 1f);
            _suggestQBox[0].q_Btn.transform.localScale = size;
            _suggestQBox[1].q_Btn.transform.localScale = size;
        }
        /// <summary> 맵보다 먼저 </summary>
        public void Init(GameScene gs)
        {
            _gs = gs;

            SelectedQuest = new inquest[2]; 
            for (int i = 0; i < 2; i++)
            {
                int n = i;
                _suggestQBox[i].q_Btn.onClick.AddListener(()=> { selectQuest(n); });
            }
            
            // 스킬 아이콘
            for (int i = 0; i < SelectedQuest.Length; i++)
            {
                SelectedQuest[i] = new inquest();
                SelectedQuest[i].reCheckData = getData;
                SelectedQuest[i]._gs = _gs;

                SelectedQuest[i].IconBox = Instantiate(_qIconFab).GetComponent<qIconBox>();
                SelectedQuest[i].IconBox.transform.SetParent(_icon_parent);
                SelectedQuest[i].IconBox.Init(SelectedQuest[i]);
                SelectedQuest[i].IconBox.gameObject.SetActive(false);
            }

            QList = new List<InQuestKeyList>();
            for (InQuestKeyList i = 0; i < InQuestKeyList.max; i++)
            {
                if (D_inquest.GetEntity(i.ToString()).f_init)
                {
                    int num = D_inquest.GetEntity(i.ToString()).f_repeat;
                    for (int j = 0; j < num; j++)
                    {
                        QList.Add(i);
                    }
                }
            }

            // (퀘 대신 보상)이벤트 설치
            _cancel.onClick.AddListener(getCoin);

            _popup.SetActive(false);
        }

        /// <summary> 오픈하면서 퀘스트 제안 </summary>
        public void open()
        {
            SoundManager.instance.PlaySFX(SFX.inquest);
            _suggest = new InQuestKeyList[2] { InQuestKeyList.max, InQuestKeyList.max };
            List<InQuestKeyList>  copyQList = new List<InQuestKeyList>(QList);

            for (int i = 0; i < 2; i++)
            {
                int n = UnityEngine.Random.Range(0, copyQList.Count);

                if (_suggest[0] != copyQList[n])   // 중복되지않게
                {
                    _suggest[i] = copyQList[n];
                    _suggestQBox[i].setBtn(copyQList[n]);
                    _suggestQBox[i].setColor(changeToColor(copyQList[n]));
                    _exchangeCoin += D_inquest.GetEntity(_suggest[i].ToString()).f_exchange;

                    copyQList.RemoveAt(n);
                }
                else { i--; }
            }

            _exchangeCoin = (_exchangeCoin + 1) / 2;
            _exchange.text = $"+{_exchangeCoin}";

            _popup.SetActive(true);
        }

        /// <summary> 퀘스트 선택 </summary>
        void selectQuest(int n)
        {
            for (int i = 0; i < 2; i++)
            {
                if (SelectedQuest[i].isUsed == false)
                {
                    InQuestKeyList qst = _suggest[n];
                    QList.Remove(_suggest[n]);

                    notiData no = new notiData(NotiType.takeQuest);
                    no._questType = qst;
                    _gs.InGameInterface.getNotiUI(no);

                    SelectedQuest[i].Init(qst);

                    SelectedQuest[i].IconBox.gameObject.SetActive(true);
                    SelectedQuest[i].IconBox.set_newQuest();
                    SelectedQuest[i].IconBox.transform.SetAsFirstSibling();
                    break;
                }
            }

            _gs.whenResume(true);
            _popup.SetActive(false);
        }

        //qIconBox getQIcon()
        //{
        //    for (int i = 0; i < _maxQst; i++)
        //    {
        //        if (_icons[i].IsUse == false)
        //        {
        //            return _icons[i];
        //        }
        //    }

        //    return null;
        //}

        /// <summary> 포기하고 돈만 받기 </summary>
        void getCoin()
        {
            _gs.getCoin(_exchangeCoin);
            _gs.whenResume(true);
            _popup.SetActive(false);
        }

        public void getData(inQuest_goal_key gk, gainableTem tem, float val)
        {
            SelectedQuest[0].getData(gk, tem, val);
            SelectedQuest[1].getData(gk, tem, val);
        }
        public void getData(inQuest_goal_key gk, inQuest_goal_valtype gv, float val)
        {
            SelectedQuest[0].getData(gk, gv, val);
            SelectedQuest[1].getData(gk, gv, val);
        }

        /// <summary> 상하 </summary>
        public void getData_time(inQuest_goal_key gk, inQuest_goal_valtype gv, float val)
        {
            if (SelectedQuest[0].condition_border(val))
                SelectedQuest[0].getData(gk, gv, Time.deltaTime);
            if (SelectedQuest[1].condition_border(val))
                SelectedQuest[1].getData(gk, gv, Time.deltaTime);
        }

        /// <summary> 스킬 마무리 </summary>
        public void getData_skill(inQuest_goal_key gk, inQuest_goal_valtype gv, SkillKeyList sk, int val)
        {
            if (SelectedQuest[0].condition_skill(sk))
                SelectedQuest[0].getData(gk, gv, val);
            if (SelectedQuest[1].condition_skill(sk))
                SelectedQuest[1].getData(gk, gv, val);
        }

        //==============================================================================================       
        //[SerializeField] float vaval = 1f;
        Color changeToColor(InQuestKeyList n)
        {
            float rate = (float)n / (int)InQuestKeyList.max;
            float tri = 1f / 3f;
            float[] col = new float[3];

            for (int i = 0; i < 3; i++)
            {
                col[i] = 0.5f * (Mathf.Sin(2 * Mathf.PI * (rate + tri * i))) + 0.5f;
            }

            Color nc = new Color(col[0], col[1], col[2]);// * vaval;
            //Color nc = new Color((1f + col[0]) / 2f, (1f + col[1]) / 2f, (1f + col[2]) / 2f);// * vaval;
            
            //nc.a = 1f;
            return nc;
        }
    }
}