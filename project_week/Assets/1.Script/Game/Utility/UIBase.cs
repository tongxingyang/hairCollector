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

    [ShowIf(EConditionOperator.And, "m_pkTransforms_chk")]
    public Transform[] m_pkTransforms;
    bool m_pkTransforms_chk { get { return GetEnumTransform() != null; } }

    [ShowIf(EConditionOperator.And, "m_pkTexts_chk")]
    public Text[] m_pkTexts;
    bool m_pkTexts_chk { get { return GetEnumText() != null; } }

    [ShowIf(EConditionOperator.And, "m_pkImages_chk")]
    public Image[] m_pkImages;
    bool m_pkImages_chk { get { return GetEnumImage() != null; } }

    [ShowIf(EConditionOperator.And, "m_pkButton_chk")]
    public Button[] m_pkButton;
    bool m_pkButton_chk { get { return GetEnumButton() != null; } }

    private Enum m_pkEnumGameObject;

    [ShowIf(EConditionOperator.And, "m_pkGameObject_chk")]
    public GameObject[] m_pkGameObject;
    bool m_pkGameObject_chk { get { return GetEnumGameObject() != null; } }

    [ContextMenu("SetContent")]
    public virtual void SetContent()
    {
        if (GetEnumTransform() != null)
        {
            m_pkTransforms = SetComponent<Transform>(GetEnumTransform());
        }

        if (GetEnumText() != null)
        {
            m_pkTexts = SetComponent<Text>(GetEnumText());
        }

        if (GetEnumImage() != null)
        {
            m_pkImages = SetComponent<Image>(GetEnumImage());
        }

        if (GetEnumButton() != null)
        {
            m_pkButton = SetComponent<Button>(GetEnumButton());
        }

        m_pkEnumGameObject = GetEnumGameObject();
        if (m_pkEnumGameObject != null)
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

    /*
    private void SetTransform()
    {
        Enum pkEnum = m_pkpkEnumTransform;

        m_pkpkTransforms = new Transform[(int) Enum.GetNames(pkEnum.GetType()).Length];

        for (int i = 0; i < (int) Enum.GetNames(pkEnum.GetType()).Length; i++)
        {
            string str = Enum.GetName(pkEnum.GetType(), i);

            Transform[] Transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform Transform in Transforms)
            {
                if (Transform.name.Equals(str) == true)
                {
                    if (m_pkpkTransforms[i] != null)
                    {
                        Debug.LogError("SetTransform Error : " + str);
                    }
                    m_pkpkTransforms[i] = Transform;
                }
            }
            if (m_pkpkTransforms[i] == null)
            {
                Debug.LogError("Find not SetTransform Error : " + str);
            }
        }
    }

    private void SetText()
    {
        Enum pkEnum = m_pkpkEnumText;

        m_pkpkTexts = new Text[(int) Enum.GetNames(m_pkpkEnumText.GetType()).Length];

        for (int i = 0; i < (int) Enum.GetNames(pkEnum.GetType()).Length; i++)
        {
            string str = Enum.GetName(pkEnum.GetType(), i);

            Text[] Texts = gameObject.GetComponentsInChildren<Text>();
            foreach (Text Label in Texts)
            {
                if (Label.name.Equals(str) == true)
                {
                    if (m_pkpkTexts[i] != null)
                    {
                        Debug.LogError("SetLabel Error : " + str);
                    }
                    m_pkpkTexts[i] = Label;
                }
            }
            if (m_pkpkTexts[i] == null)
            {
                Debug.LogError("Find not SetLabel Error : " + str);
            }
        }
    }
    */

    /// <summary> 필요한 게임오브젝트 수집 </summary>
    private void SetGameObject()
    {
        Enum pkEnum = m_pkEnumGameObject;

        m_pkGameObject = new GameObject[(int)Enum.GetNames(m_pkEnumGameObject.GetType()).Length];

        for (int i = 0; i < (int)Enum.GetNames(pkEnum.GetType()).Length; i++)
        {
            string str = Enum.GetName(pkEnum.GetType(), i);

            Transform[] trans = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform tran in trans)
            {
                if (tran.name.Equals(str) == true)
                {
                    if (m_pkGameObject[i] != null)
                    {
                        Debug.LogError("SetLabel Error : " + str);
                    }
                    m_pkGameObject[i] = tran.gameObject;
                }
            }
            if (m_pkGameObject[i] == null)
            {
                Debug.LogError("Find not SetLabel Error : " + str);
            }
        }
    }
}
