using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using NaughtyAttributes;

public abstract class UIBase : MonoBehaviour
{
    protected virtual Enum GetEnumTransform()
    {
        return null;
    }

    protected virtual Enum GetEnumText()
    {
        return null;
    }

    protected virtual Enum GetEnumImage()
    {
        return null;
    }

    protected virtual Enum GetEnumButton()
    {
        return null;
    }

    protected virtual Enum GetEnumGameObject()
    {
        return null;
    }

    protected virtual void OtherSetContent()
    {
    }

    [ShowIf(EConditionOperator.And, "mTr_chk")]
    public Transform[] mTrs;
    bool mTr_chk { get { return GetEnumTransform() != null; } }

    [ShowIf(EConditionOperator.And, "mTxt_chk")]
    public Text[] mTxts;
    bool mTxt_chk { get { return GetEnumText() != null; } }

    [ShowIf(EConditionOperator.And, "mImg_chk")]
    public Image[] mImgs;
    bool mImg_chk { get { return GetEnumImage() != null; } }

    [ShowIf(EConditionOperator.And, "mBtn_chk")]
    public Button[] mBtns;
    bool mBtn_chk { get { return GetEnumButton() != null; } }

    private Enum mEnumGOs;

    [ShowIf(EConditionOperator.And, "mGO_chk")]
    public GameObject[] mGos;
    bool mGO_chk { get { return GetEnumGameObject() != null; } }

    [ContextMenu("SetContent")]
    public virtual void SetContent()
    {
        if (GetEnumTransform() != null)
        {
            mTrs = SetComponent<Transform>(GetEnumTransform());
        }

        if (GetEnumText() != null)
        {
            mTxts = SetComponent<Text>(GetEnumText());
        }

        if (GetEnumImage() != null)
        {
            mImgs = SetComponent<Image>(GetEnumImage());
        }

        if (GetEnumButton() != null)
        {
            mBtns = SetComponent<Button>(GetEnumButton());
        }

        mEnumGOs = GetEnumGameObject();
        if (mEnumGOs != null)
        {
            SetGameObject();
        }

        OtherSetContent();
    }

    /// <summary> 필요한 컴포넌트 수집 (템플릿) </summary>
    protected T[] SetComponent<T>(Enum m_pkpkEnumComp) where T : Component
    {
        Enum pkEnum = m_pkpkEnumComp;

        T[] m_pkpkComps = new T[(int)Enum.GetNames(pkEnum.GetType()).Length];

        for (int i = 0; i < (int)Enum.GetNames(pkEnum.GetType()).Length; i++)
        {
            string str = Enum.GetName(pkEnum.GetType(), i);

            T[] Comps = gameObject.GetComponentsInChildren<T>();
            foreach (T comp in Comps)
            {
                if (comp.name.Equals(str) == true)
                {
                    if (m_pkpkComps[i] != null)
                    {
                        Debug.LogError("SetTransform Error : " + str);
                    }
                    m_pkpkComps[i] = comp;
                }
            }
            if (m_pkpkComps[i] == null)
            {
                Debug.LogError("Find not SetTransform Error : " + str);
            }
        }

        return m_pkpkComps;
    }

    /// <summary> 필요한 게임오브젝트 수집 </summary>
    private void SetGameObject()
    {
        Enum pkEnum = mEnumGOs;

        mGos = new GameObject[(int)Enum.GetNames(mEnumGOs.GetType()).Length];

        for (int i = 0; i < (int)Enum.GetNames(pkEnum.GetType()).Length; i++)
        {
            string str = Enum.GetName(pkEnum.GetType(), i);

            Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform tran in trans)
            {
                if (tran.name.Equals(str) == true)
                {
                    if (mGos[i] != null)
                    {
                        Debug.LogError("SetLabel Error : " + str);
                    }
                    mGos[i] = tran.gameObject;
                }
            }
            if (mGos[i] == null)
            {
                Debug.LogError("Find not SetLabel Error : " + str);
            }
        }
    }
}
