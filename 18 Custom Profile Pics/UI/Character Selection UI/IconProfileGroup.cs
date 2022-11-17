using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Atavism;

public class IconProfileGroup : VisualElement
{
    internal new class UxmlFactory : UxmlFactory<IconProfileGroup, UxmlTraits> { }    
    internal new class UxmlTraits : VisualElement.UxmlTraits { }
    private const string f_styleName = "IconProfileGroupStyles";

    private const string s_UssClassName = "icon-profile-group";
    private const string s_ContentContainerClassName = "icon-profile-group__content";

    public VisualElement m_ProfileContent;

    private IconProfile m_ActiveProfile;
    public override VisualElement contentContainer => m_ProfileContent;

    public IconProfileGroup()
    { 
        AddToClassList(s_UssClassName);
        styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{f_styleName}"));

        m_ProfileContent = new VisualElement();
        m_ProfileContent.name = "icon-profile-container";
        m_ProfileContent.AddToClassList(s_ContentContainerClassName);
        hierarchy.Add(m_ProfileContent);

        RegisterCallback<AttachToPanelEvent>(ProcessEvent);
    }
    private void ProcessEvent(AttachToPanelEvent evt)
    {
        // This code goes through profile buttons and initializes them
        for (int i = 0; i < m_ProfileContent.childCount; ++i)
        {
            VisualElement element = m_ProfileContent[i];
            if (element is IconProfile profile)
            {
                InitProfile(profile, false);
            }
        }
        // Finally, if we need to, activate this profile button...
        if (m_ActiveProfile != null)
        {
            SelectProfile(m_ActiveProfile);
        }
        else if (m_ProfileContent.childCount > 0)
        {
            m_ActiveProfile = (IconProfile)m_ProfileContent[0];

            SelectProfile(m_ActiveProfile);
        }   
    }
    public void HideProfiles ()
    {
        for (int i = 0; i < m_ProfileContent.childCount; ++i)
        {
            VisualElement element = m_ProfileContent[i];
            if (element is IconProfile profile)
            {
                profile.style.display = DisplayStyle.None;
            }
        }        
    }

    public void InitProfile(IconProfile IconProfile, bool activate)
    {
        IconProfile.OnSelect += Activate;

        IconProfile.style.display = DisplayStyle.None; // initially make invisible

        if (activate)
        {
            Activate(IconProfile);
        }
    }

    // Activate first profile
    public void ActivateFirst()
    {
        if (m_ActiveProfile != null)
        {
            DeselectProfile(m_ActiveProfile);
        }

        // Find an empty profile
        UQueryBuilder<IconProfile> allProfiles = m_ProfileContent.Query<IconProfile>();

        // Find next profile which is invisible                 
        IconProfile firstProfile = allProfiles.Where(x => x.style.display != DisplayStyle.None).First();
        m_ActiveProfile = firstProfile;
        SelectProfile(m_ActiveProfile);        
    }

    public void Activate(IconProfile profile)
    {
        if (m_ActiveProfile != null)
        {
            DeselectProfile(m_ActiveProfile);
        }
        m_ActiveProfile = profile;
        SelectProfile(m_ActiveProfile);
    }
    public void Reset() // Reset selected profile button to first
    {
        if (m_ProfileContent.childCount > 0)
        {
            Activate((IconProfile)m_ProfileContent[0]);
        }
    }
    public int SlotCount()
    {
        return m_ProfileContent.childCount;
    }
    private void SelectProfile(IconProfile IconProfile)
    {
        IconProfile.Select();
    }

    public void InsertProfile(CharacterEntry characterEntry)
    {
        // Very, very loose reference
        // https://gamedev-resources.com/create-an-in-game-inventory-ui-with-ui-toolkit/

        // Find an empty profile
        UQueryBuilder<IconProfile> allProfiles = m_ProfileContent.Query<IconProfile>();

        // Find next profile which is invisible                 
        IconProfile emptyProfile = allProfiles.Where(x => x.style.display== DisplayStyle.None).First();
       
        if (emptyProfile != null)
        {
            emptyProfile.SetName((string)characterEntry["characterName"]);
            emptyProfile.SetImage((string)characterEntry["portrait"]);
            emptyProfile.style.display = DisplayStyle.Flex;
        }  
    }

    private void DeselectProfile(IconProfile IconProfile)
    {
        IconProfile.Deselect();
    }

}