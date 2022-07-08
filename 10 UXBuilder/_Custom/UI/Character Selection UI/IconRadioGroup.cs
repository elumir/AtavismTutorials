using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class IconRadioGroup : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconRadioGroup, UxmlTraits> { }    
    internal new class UxmlTraits : VisualElement.UxmlTraits { }
    private const string f_styleName = "IconRadioGroupStyles";

    private const string s_UssClassName = "icon-radio-group";
    private const string s_ContentContainerClassName = "icon-radio-group__content";

    public VisualElement m_RadioContent;
    private IconRadio m_ActiveRadio;
    public override VisualElement contentContainer => m_RadioContent;

    public IconRadioGroup()
    { 
       AddToClassList(s_UssClassName);
       styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{f_styleName}"));

        m_RadioContent = new VisualElement();
        m_RadioContent.name = "icon-radio-container";
        m_RadioContent.AddToClassList(s_ContentContainerClassName);
        hierarchy.Add(m_RadioContent);

        RegisterCallback<AttachToPanelEvent>(ProcessEvent);
    }
    private void ProcessEvent(AttachToPanelEvent evt)
    {
        // This code goes through radio buttons and initializes them
        for (int i = 0; i < m_RadioContent.childCount; ++i)
        {
            VisualElement element = m_RadioContent[i];
            if (element is IconRadio radio)
            {
                InitRadio(radio, false);
            }
        }
        // Finally, if we need to, activate this radio button...
        if (m_ActiveRadio != null)
        {
            SelectRadio(m_ActiveRadio);
        }
        else if (m_RadioContent.childCount > 0)
        {
            m_ActiveRadio = (IconRadio)m_RadioContent[0];

            SelectRadio(m_ActiveRadio);
        }   
    }
    public void InitRadio(IconRadio iconRadio, bool activate)
    {
        iconRadio.OnSelect += Activate;
        if (activate)
        {
            Activate(iconRadio);
        }
    }
    public void Activate(IconRadio radio)
    {
        if (m_ActiveRadio != null)
        {
            DeselectRadio(m_ActiveRadio);
        }
        m_ActiveRadio = radio;
        SelectRadio(m_ActiveRadio);
    }
    private void SelectRadio(IconRadio iconRadio)
    {
        iconRadio.Select();
    }

    private void DeselectRadio(IconRadio iconRadio)
    {
        iconRadio.Deselect();
    }

}