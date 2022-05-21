using System;
using UnityEngine;
using UnityEngine.UIElements;

public class IconTab : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconTab, UxmlTraits> { }

    // There is no way to select an image in the Inspector (e.g. UxmlImageAttributeDescription doesn't exist)
    // But it's the only UXMLTrait I want to set (for now). We'll have to do it in a janky way
    internal new class UxmlTraits : VisualElement.UxmlTraits{ 
        private readonly UxmlStringAttributeDescription m_TargetID = new UxmlStringAttributeDescription { name = "targetID" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((IconTab)ve).TargetId = m_TargetID.GetValueFromBag(bag, cc);  
        }        
    }

    static readonly string styleName = "IconTabStyles";
    static readonly string UxmlName = "IconTab";
    static readonly string s_UssClassName = "icon-tab";
    static readonly string s_UssActiveClassName = s_UssClassName + "--active";

//    private VisualElement m_Icon;
    public string TargetId { get; set; }
    public VisualElement Target { get; set; }

    public event Action<IconTab> OnSelect;

    public IconTab()
    {
        Init();
    }
    public IconTab(VisualElement target)
    {
        Init();
        Target = target;
    }
    
    private void Init()
    {
        AddToClassList(s_UssClassName);
        styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));

        VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>($"UXML/{UxmlName}");
        visualTree.CloneTree(this);

//        m_Icon = this.Q<VisualElement>("icon");

        this.focusable = true;
        this.pickingMode = PickingMode.Position;

        RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
    }

    public void Select()
    {
        AddToClassList(s_UssActiveClassName);

        if (Target != null)
        {
            Target.style.display = DisplayStyle.Flex;
            Target.style.flexGrow = 1;
        }
    }

    public void Deselect()
    {
        RemoveFromClassList(s_UssActiveClassName);
        MarkDirtyRepaint();

        if (Target != null)
        {
            Target.style.display = DisplayStyle.None;
            Target.style.flexGrow = 0;
        }
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
