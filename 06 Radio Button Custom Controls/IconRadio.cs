using System;
using UnityEngine;
using UnityEngine.UIElements;

public class IconRadio : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconRadio, UxmlTraits> { }

    // There is no way to select an image in the Inspector (e.g. UxmlImageAttributeDescription doesn't exist)
    // But it's the only UXMLTrait I want to set (for now). We'll have to do it in a janky way
    internal new class UxmlTraits : VisualElement.UxmlTraits{ }

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
