using System;
using UnityEngine;
using UnityEngine.UIElements;

public class IconRadio : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconRadio, UxmlTraits> { }

    // There is no way to select an image in the Inspector (e.g. UxmlImageAttributeDescription doesn't exist)
    // We'll have to do it in a janky way
    // EDIT: Added to set the value of the radio button
    internal new class UxmlTraits : VisualElement.UxmlTraits{ 
        private readonly UxmlIntAttributeDescription m_radioValue = new UxmlIntAttributeDescription { name = "radioValue" };
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((IconRadio)ve).radioValue = m_radioValue.GetValueFromBag(bag, cc);  
        }        
    }
    
    static readonly string styleName = "IconRadioStyles";
    static readonly string UxmlName = "IconRadio";
    static readonly string s_UssClassName = "icon-radio";
    static readonly string s_UssActiveClassName = s_UssClassName + "--active";

    public event Action<IconRadio> OnSelect;

    public int radioValue { get; set; }

    public IconRadio()
    {
        Init();
    }
    
    private void Init()
    {
        AddToClassList(s_UssClassName);
        styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));

        VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>($"UXML/{UxmlName}");
        visualTree.CloneTree(this);

        this.focusable = true;
        this.pickingMode = PickingMode.Position;

        RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
    }

    public void Select()
    {
        AddToClassList(s_UssActiveClassName);
    }

    public void Deselect()
    {
        RemoveFromClassList(s_UssActiveClassName);
        MarkDirtyRepaint();
    }

    private void OnMouseDownEvent(MouseDownEvent e)
    {
        if (e.button == 0) 
        {
            OnSelect?.Invoke(this);
        }
        e.StopImmediatePropagation();
    }
}
