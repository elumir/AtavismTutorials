using System;
using UnityEngine;
using UnityEngine.UIElements;

public class IconProfile : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconProfile, UxmlTraits> { }

    internal new class UxmlTraits : VisualElement.UxmlTraits{ 
        private readonly UxmlIntAttributeDescription m_profileValue = new UxmlIntAttributeDescription { name = "profileValue" };
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((IconProfile)ve).profileValue = m_profileValue.GetValueFromBag(bag, cc);  
        }        
    }
    
    static readonly string styleName = "IconProfileStyles";
    static readonly string UxmlName = "IconProfile";
    static readonly string s_UssClassName = "icon-profile";
    static readonly string s_UssActiveClassName = s_UssClassName + "--active";

    public event Action<IconProfile> OnSelect;

    public int profileValue { get; set; }

    public VisualElement profileImage;
    public Label m_labelCharacterName;
    public Label m_labelCharacterType;
    public Label m_labelCharacterLevel;

    public IconProfile()
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

        m_labelCharacterName = this.Q<Label>("CharacterName");
        m_labelCharacterType = this.Q<Label>("CharacterType");
        m_labelCharacterLevel = this.Q<Label>("CharacterLevel");

        profileImage = this.Q<VisualElement>("profilePic");
        
        m_labelCharacterType.style.display = DisplayStyle.None;
        m_labelCharacterLevel.style.display = DisplayStyle.None;

        RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
    }

    public void SetName(string characterName)
    {
        m_labelCharacterName.text = characterName;
    }
    public void SetImage(string base64image)
    {
        byte[] byteArray = Convert.FromBase64String(base64image);
        var tempTexture = new Texture2D(96, 96, TextureFormat.ARGB32, false);
        tempTexture.LoadImage(byteArray);
        profileImage.style.backgroundImage = tempTexture;
        // profileImage.style.backgroundImage = new Texture2D(96, 96, TextureFormat.ARGB32, false);
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
