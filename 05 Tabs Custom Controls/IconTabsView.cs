using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
// Reference articles
// https://forum.unity.com/threads/confused-about-how-to-pass-values-from-the-outside-world-into-visualelements-code.1002245/
// https://forum.unity.com/threads/ui-builder-and-custom-elements.785129/
// https://github.com/Unity-Technologies/UIToolkitUnityRoyaleRuntimeDemo/tree/master/Assets/Scripts/UI
// https://docs.unity3d.com/2022.1/Documentation/Manual/UIE-create-tabbed-menu-for-runtime.html

public class IconTabsView : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconTabsView, UxmlTraits> { }    
    internal new class UxmlTraits : VisualElement.UxmlTraits {
        private readonly UxmlStringAttributeDescription m_TabsName = new UxmlStringAttributeDescription { name = "tabsName", defaultValue = "tabs-container" };        
        private readonly UxmlStringAttributeDescription m_ContentName = new UxmlStringAttributeDescription { name = "contentName", defaultValue = "content-container" };        
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            base.Init(ve, bag, cc);
            IconTabsView item = ve as IconTabsView;            
            item.TabsName = m_TabsName.GetValueFromBag(bag, cc);              
            item.ContentName = m_ContentName.GetValueFromBag(bag, cc);
        }
    }
    private const string f_styleName = "IconTabsViewStyles";
//    private const string f_UxmlName = "IconTabsView"; // Nothing really in here so not using
    private const string s_UssClassName = "icon-tabs-view";
    private const string s_ContentContainerClassName = "icon-tabs-view__content-container";
    private const string s_TabsContainerClassName = "icon-tabs-view__tabs-container";

    public VisualElement m_TabContent;
    public string TabsName { get; set; }
    public VisualElement m_Content;
    public string ContentName { get; set; }

    private IconTab m_ActiveTab;

    public IconTabsView()
    { 
        AddToClassList(s_UssClassName);
        styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{f_styleName}"));
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);         
    }
    private void OnGeometryChange(GeometryChangedEvent evt)
    {
        // The below visual elements won't exist until now, so they are assigned here
        // so we can still drag in our custom component in the UI Builder
        // WE NEED to make sure to name our element appropriately though. 
        m_TabContent = this.Q(TabsName); // All IconTabs go in here!
        m_Content = this.Q(ContentName); // Content goes here

        // This code goes through tab buttons and initializes them
        for (int i = 0; i < m_TabContent.childCount; ++i)
        {
            VisualElement element = m_TabContent[i];
            if (element is IconTab button)
            {
                if (button.Target == null)
                {
                    string targetId = button.TargetId;
                    button.Target = this.Q(targetId);
                }
                InitTab(button, false);
            }
        }
        // Finally, if we need to, activate this tab...
        if (m_ActiveTab != null)
        {
            SelectTab(m_ActiveTab);
        }
        else if (m_TabContent.childCount > 0)
        {
            m_ActiveTab = (IconTab)m_TabContent[0];

            SelectTab(m_ActiveTab);
        }
        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);        
    }
    public void InitTab(IconTab iconTab, bool activate)
    {
        iconTab.OnSelect += Activate;
        if (activate)
        {
            Activate(iconTab);
        }
    }
    public void Activate(IconTab button)
    {
        if (m_ActiveTab != null)
        {
            DeselectTab(m_ActiveTab);
        }
        m_ActiveTab = button;
        SelectTab(m_ActiveTab);
    }
    private void SelectTab(IconTab iconTab)
    {
        iconTab.Select();
    }

    private void DeselectTab(IconTab iconTab)
    {
        iconTab.Deselect();
    }

}