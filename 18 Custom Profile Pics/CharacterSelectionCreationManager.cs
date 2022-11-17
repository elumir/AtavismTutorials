using UnityEngine;
using OldUI = UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using HNGamers.Atavism;
using static HNGamers.Atavism.ModularCustomizationManager;
using UnityEngine.UIElements;
using Game.UI; // For color popups
using UnityEngine.Rendering;
using Unity.UI;

/* Modification Notes
 * 20220723 - UV Editing reference https://www.youtube.com/watch?v=GTd8NBg8EZU
 * 20220724 - Updated UI ColorField.cs
 *          - Removed dependence on ModularCharacterColor.cs - should remove script
 *          - Removed dependence on ModularCharacterSlider.cs - should remove script
 *          - Edited ModularCustomizationManagerEditor.cs - removed a lot of things
 *          - Edited AtavismMobAppearance.cs 
 *                  - As I remove Old UI Elements, UGUI scripts need to be changed because of references
 *                    I know these won't be there when I remove them, but I won't remove the panels yet.                            
 *          - Edited UGUICharacterPanel.cs
 *          - Edited UGUIOtherCharacterPanel.cs
 * 20220801 - IconRadio was altered to expose radioValue in inspector
 *          - Edited UGUICharacterRaceSlot.cs 
 *          - Edited UGUICharacterClassSlot.cs
 *          - Edited UGUIAvatarSlot.cs
 *          - Edited UGUICharacterGenderSlot.cs
 * 20220828 - Fixed save, now to update AtavismMobAppearance.cs
 * 20221109 - Edit UGUIAvatarList.cs (Line 39)
 * 20221110 - Edit UGUICharacterClassSlot.cs (line 66)
 *          - Edit UGUICharacterRaceSlot.cs (line 50)
 *          - Edit UGUICharacterGenderSlot.cs (line 43)
*/ 

namespace Atavism
{

    /// <summary>
    /// Handles the selection and creation of the players characters. This script must be added
    /// as a component in the Character Creation / Selection scene.
    /// </summary>
    public class CharacterSelectionCreationManager : MonoBehaviour
    {
        protected static CharacterSelectionCreationManager instance;

      //  public Texture cursorOverride;
        public GameObject characterCamera;
        public Transform spawnPosition;

        protected GameObject character;
        protected GameObject characterDCS;

        protected List<CharacterEntry> characterEntries;
        protected LoginState loginState;
        protected string dialogMessage = "";
        protected string errorMessage = "";

        // Character select fields
        protected CharacterEntry characterSelected = null;
        protected string characterName = "";
        protected int raceId = -1;
        protected int aspectId = -1;
        protected int genderId = -1;

        // Camera fields
        // EEE New Camera Variables
        public Vector3 cameraInLoc = new Vector3(0.4f, 1.6f, -0.7f);
        public Vector3 cameraOutLoc = new Vector3(1.2f, 1f, -2f);
        public Vector3 cameraMidLoc = new Vector3(0f, 1f, -2f);
        protected bool zoomingIn = false;
        protected bool zoomingOut = false;
        protected bool zoomingMid = true; // EEE - Set camera for mid zoom
        public float characterRotationSpeed = 250.0f;
        public float moveRate = 1.0f; // EEE - Moved here to it can be inspector
        public String tabHeadName = "tabThree"; // EEE - this tab triggers zoom to go in
        public String tabAnimalName = "tabOne"; // EEE - this tab holds faces
        public int maxProfiles = 3; // EE max number of profiles
        protected float x = 180;
        protected float y = 0;

        // EEE UI Builder
        public VisualElement m_ScreenSelection; // Holds UI for selecting character
        public IconProfileGroup m_ProfileContainer; // Holds indiviudal profiles for selecting 
        public Label m_labelCharacterName;
        public Button m_buttonDeleteCharacter;
        public Button m_buttonNewCharacter;
        public Button m_buttonPlayGame;
        public VisualElement m_ScreenCustomize; // Holds UI for customizing character
        public IconTabsView m_IconTabsView; // updated to have a reset
        public IconRadioGroup m_animalContainer; // Holds Animals - Changed from sliders
        public IconRadioGroup m_bodyContainer; // Holds bodies - Changed from sliders
        public VisualElement m_SectionContentContainer; // holds create character ui content
        public UQueryBuilder<ColorField> m_allColorFields; // holds all the UI Color fields
        public ColorPopup m_ColorPopup; // same popup window for choosing colours
        public Button m_buttonAcceptCharacter;
        public Button m_buttonCancelCharacter;
        public TextField m_fieldCharacterName;
        public PopupMessage m_ScreenPopup;

        private bool grabbingProfileIcon;
        private string base64ProfileIcon; 
        public int profileIconSize = 512;

        // Use this for initialization
        void Start()
        {
            if (instance != null)
            {
                return;
            }
            instance = this;

            UIInitModularCharacterComponents(); // EEE - UI Builder Component initialization

            AtavismEventSystem.RegisterEvent("World_Error", this);

            StartCharacterSelection();
            if (characterEntries.Count == 0)
            {
                StartCharacterCreation();
            }
            if (characterCamera != null)
                characterCamera.SetActive(true);
        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("World_Error", this);
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "World_Error")
            {
                ShowDialog(eData.eventArgs[0], true);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (character != null)
            {
                if (zoomingIn)
                {
                        characterCamera.transform.position = Vector3.Lerp(characterCamera.transform.position, character.transform.position + cameraInLoc, Time.deltaTime * moveRate);
                }
                else if (zoomingOut)
                {
                    characterCamera.transform.position = Vector3.Lerp(characterCamera.transform.position, character.transform.position + cameraOutLoc, Time.deltaTime * moveRate);
                }
                else if (zoomingMid) // EEE - new zoom position
                {
                    characterCamera.transform.position = Vector3.Lerp(characterCamera.transform.position, character.transform.position + cameraMidLoc, Time.deltaTime * moveRate);
                }
            }
        }

        /// <summary>
        /// Handles character rotation if the mouse button is down and the player is dragging it.
        /// </summary>
        void LateUpdate()
        {
            //TODO: currently this is artificially limited to only work in the middle 1/3 of the screen, this restriction should be removed
            if (character && Input.GetMouseButton(0) && !AtavismCursor.Instance.IsMouseOverUI() /*(Input.mousePosition.x > Screen.width / 3) && (Input.mousePosition.x < Screen.width / 3 * 2)*/)
            {
                x -= Input.GetAxis("Mouse X") * characterRotationSpeed * 0.02f;

                Quaternion rotation = Quaternion.Euler(y, x, 0);
                //position.y = height;
                character.transform.rotation = rotation;
            }
        }

        #region CHARACTER SELECTION
        public void StartCharacterSelection()
        {

            characterEntries = ClientAPI.GetCharacterEntries();
            // can I sort by characterId? 

            ShowSelectionUI();

            // Research 
            // https://forum.unity.com/threads/querybuilder-usability.797751/

            if (characterEntries.Count > 0)
            {
                m_buttonDeleteCharacter.style.display = DisplayStyle.Flex;
                m_buttonPlayGame.style.display = DisplayStyle.Flex;
                if (characterEntries.Count == maxProfiles)
                {
                    m_buttonNewCharacter.style.display = DisplayStyle.None; // hide new character button if full
                }
                else
                {
                    m_buttonNewCharacter.style.display = DisplayStyle.Flex;
                }

                CharacterSelected(characterEntries[0]); // are we always setting the selected to the first one? 
            }
            else
            {
                if (character != null)
                    Destroy(character);
                characterSelected = null;
                // Should hide play and delete buttons
                m_buttonNewCharacter.style.display = DisplayStyle.Flex;                
                m_buttonDeleteCharacter.style.display = DisplayStyle.None;
                m_buttonPlayGame.style.display = DisplayStyle.None;
            }

            m_ProfileContainer.HideProfiles(); // hide all profiles first
            // Set the slots up
            for (int i = 0; i < m_ProfileContainer.SlotCount(); i++)
            {
                if (characterEntries.Count > i)
                {
                    m_ProfileContainer.InsertProfile(characterEntries[i]);
                }
            }

            m_ProfileContainer.ActivateFirst(); // activate first profile

            loginState = LoginState.CharacterSelect;

            ZoomCameraMid(); // EEE - Added in here - Zoom camera out to character select
        }
        
        public virtual void ModularCustomizationManagerCheck() // Loads colours and body from the DB
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();

            if (character && mcm)
            {
                if (characterSelected.ContainsKey(mcm.eyeColorPropertyName))
                {
                    var item = characterSelected[mcm.eyeColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorEye); // EEE Darn it, need to make everything color32
                }
                if (characterSelected.ContainsKey(mcm.noseColorPropertyName))
                {
                    var item = characterSelected[mcm.noseColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorNose); 
                }
                if (characterSelected.ContainsKey(mcm.eyebrowColorPropertyName))
                {
                    var item = characterSelected[mcm.eyebrowColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorEyebrow); 
                }
                if (characterSelected.ContainsKey(mcm.furLightColorPropertyName))
                {
                    var item = characterSelected[mcm.furLightColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorFurLight); 
                }
                if (characterSelected.ContainsKey(mcm.furDarkColorPropertyName))
                {
                    var item = characterSelected[mcm.furDarkColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorFurDark); 
                }
                if (characterSelected.ContainsKey(mcm.furDarkerColorPropertyName))
                {
                    var item = characterSelected[mcm.furDarkerColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorFurDarker); 
                }
                if (characterSelected.ContainsKey(mcm.headCrestColorPropertyName))
                {
                    var item = characterSelected[mcm.headCrestColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorHeadCrest); 
                }
                if (characterSelected.ContainsKey(mcm.headOtherColorPropertyName))
                {
                    var item = characterSelected[mcm.headOtherColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorHeadOther); 
                }
                if (characterSelected.ContainsKey(mcm.leatherColorPropertyName))
                {
                    var item = characterSelected[mcm.leatherColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorLeather); 
                }
                if (characterSelected.ContainsKey(mcm.leatherDarkColorPropertyName))
                {
                    var item = characterSelected[mcm.leatherDarkColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorLeatherDark); 
                }
                if (characterSelected.ContainsKey(mcm.shirtColorPropertyName))
                {
                    var item = characterSelected[mcm.shirtColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorShirt); 
                }
                if (characterSelected.ContainsKey(mcm.collarColorPropertyName))
                {
                    var item = characterSelected[mcm.collarColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorCollar); 
                }
                if (characterSelected.ContainsKey(mcm.shirtLightColorPropertyName))
                {
                    var item = characterSelected[mcm.shirtLightColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorShirtLight); 
                }
                if (characterSelected.ContainsKey(mcm.shirtDarkColorPropertyName))
                {
                    var item = characterSelected[mcm.shirtDarkColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorShirtDark); 
                }
                if (characterSelected.ContainsKey(mcm.metalColorPropertyName))
                {
                    var item = characterSelected[mcm.metalColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorMetal); 
                }
                if (characterSelected.ContainsKey(mcm.metalLightColorPropertyName))
                {
                    var item = characterSelected[mcm.metalLightColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorMetalLight); 
                }
                if (characterSelected.ContainsKey(mcm.metalDarkColorPropertyName))
                {
                    var item = characterSelected[mcm.metalDarkColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorMetalDark); 
                }
                if (characterSelected.ContainsKey(mcm.metalBlackColorPropertyName))
                {
                    var item = characterSelected[mcm.metalBlackColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorMetalBlack); 
                }
                if (characterSelected.ContainsKey(mcm.gemColorPropertyName))
                {
                    var item = characterSelected[mcm.gemColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorGem); 
                }
                if (characterSelected.ContainsKey(mcm.hemColorPropertyName))
                {
                    var item = characterSelected[mcm.hemColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    mcm.UpdateShaderColor(color32, BodyPartColor.ColorHem); 
                }

                if (characterSelected.ContainsKey(mcm.headPropertyName))
                {
                    mcm.SwitchHead((int)characterSelected[mcm.headPropertyName]);
                }
                if (characterSelected.ContainsKey(mcm.torsoPropertyName))
                {
                    mcm.SwitchTorso((int)characterSelected[mcm.torsoPropertyName]);
                }
            }
        }

        public void StartCharacterSelection(List<CharacterEntry> characterEntries)
        {
            this.characterEntries = characterEntries;
            StartCharacterSelection();
        }

        public virtual void CharacterSelected(CharacterEntry entry)
        {
            characterSelected = entry;
            if (character != null)
                Destroy(character);
            Dictionary<string, object> appearanceProps = new Dictionary<string, object>();
            foreach (string key in entry.Keys)
            {
                if (key.StartsWith("custom:appearanceData:"))
                {
                    appearanceProps.Add(key.Substring(23), entry[key]);
                }
            }
            // Dna settings
            string prefabName = (string)characterSelected["model"];
            if (prefabName.Contains(".prefab"))
            {
                int resourcePathPos = prefabName.IndexOf("Resources/");
                prefabName = prefabName.Substring(resourcePathPos + 10);
                prefabName = prefabName.Remove(prefabName.Length - 7);
            }
            GameObject prefab = (GameObject)Resources.Load(prefabName);
            if (prefab != null)
                character = (GameObject)Instantiate(prefab, spawnPosition.position, spawnPosition.rotation);
            else
            {
                Debug.LogError("prefab = null model: " + prefabName + " Loading ExampleCharacter");
                prefab = (GameObject)Resources.Load("ExampleCharacter");
                if (prefab != null)
                    character = (GameObject)Instantiate(prefab, spawnPosition.position, spawnPosition.rotation);
            }

            // Set equipment
            if (character != null)
            {
                ModularCustomizationManagerCheck();
             //   Debug.LogError(""+characterSelected["aspectId"]+" "+characterSelected["genderId"]+" "+characterSelected["raceId"]);
                character.GetComponent<AtavismMobAppearance>().aspect = (int)characterSelected["aspectId"];
                character.GetComponent<AtavismMobAppearance>().gender = (int)characterSelected["genderId"];
                character.GetComponent<AtavismMobAppearance>().race = (int)characterSelected["raceId"];

                foreach (var key in characterSelected.Keys)
                {
                    if (key.EndsWith("DisplayID"))
                    {
                       // Debug.LogError("Key="+key);
                        string keyv = key.Substring(0,key.IndexOf("DisplayID"));
                       
                    //    Debug.LogError("Keyv="+keyv+" "+characterSelected[keyv+"DisplayVAL"]);
                    if(characterSelected.ContainsKey(keyv+"DisplayVAL"))
                        character.GetComponent<AtavismMobAppearance>().displayVal = (string) characterSelected[keyv+"DisplayVAL"];
                        character.GetComponent<AtavismMobAppearance>().UpdateEquipDisplay(key, (string)characterSelected[key]);
                    }
                }
            }            

            m_labelCharacterName.text = (string)entry["characterName"];

            // Temp
            if (character != null)
            {
                if (character != null && character.GetComponent<CustomisedHair>() != null)
                {
                    CustomisedHair customHair = character.GetComponent<CustomisedHair>();
                    if (characterSelected.ContainsKey(customHair.hairPropertyName))
                    {
                        customHair.UpdateHairModel((string)characterSelected[customHair.hairPropertyName]);
                    }
                }
            }
        }

        private float clicklimit = 0;
        public void Play()
        {
            if (clicklimit > Time.time)
                return;
            clicklimit = Time.time + 1f;
#if AT_I2LOC_PRESET
            dialogMessage = I2.Loc.LocalizationManager.GetTranslation("Entering World...");
#else
            dialogMessage = "Entering World...";
#endif
            AtavismClient.Instance.EnterGameWorld(characterSelected.CharacterId);
            //   Debug.LogError(dialogMessage);
        }

        public void GoToCreation() {
           if (characterEntries.Count < 3) // REPLACE THIS HARD-CODED NUMBER
            {
                StartCharacterCreation();
            }
            if (characterCamera != null)
                characterCamera.SetActive(true);            
        }

        public void DeleteCharacter()
        {
            m_ScreenPopup.Show("Delete Character: "+ characterSelected["characterName"], "Are you sure? Everything about this character will be lost!", 
                "Delete", "Cancel", ButtonDeleteCharacterConfirmed, ButtonCancelOrClose);
        }

        public virtual void ButtonCancelOrClose()
        {
            m_ScreenPopup.Hide();
        }

        public virtual void ButtonDeleteCharacterConfirmed()
        {
            m_ScreenPopup.Hide();
            m_labelCharacterName.text = "";

            Dictionary<string, object> attrs = new Dictionary<string, object>();
            attrs.Add("characterId", characterSelected.CharacterId);
            NetworkAPI.DeleteCharacter(attrs);
            characterSelected = null;
            foreach (CharacterEntry charEntry in characterEntries)
            {
                if (charEntry != characterSelected)
                {
                    CharacterSelected(charEntry);
                    characterSelected = charEntry;
                    m_ProfileContainer.ActivateFirst(); // activate first profile
                    StartCharacterSelection();
                    return;
                }
            }
            StartCharacterCreation();
        }

        public virtual void UIProfileClicked(IconProfile iProfile) // EEE - Profile clicked
        {
            CharacterSelected(characterEntries[iProfile.profileValue]);
        }   
        #endregion CHARACTER SELECTION

        #region CHARACTER CREATION
        public virtual void StartCharacterCreation()
        {
            ShowCreationUI();
            m_labelCharacterName.text = "";
            m_fieldCharacterName.value = "";

            // EEE - I don't have Race, Class, or Gender
            // This was used to prep the old UI but kept in for backwards compatibility, only first ID used.
            // AtavismPrefabManager.Instance.GetRaceData(); Holds Key = ID : value = array of Race Data
            var rlist = AtavismPrefabManager.Instance.GetRaceData().Keys.ToList(); // get list of IDs
            rlist.Sort();
            raceId = rlist[0]; 

            var cllist = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.Keys.ToList();
            cllist.Sort();
            aspectId = cllist[0];

            // Grab genders for this race and class from the DB
            var genderList = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Keys.ToList();
            genderList.Sort();
            genderId = genderList[0];

            if (character != null)
                Destroy(character);
            characterName = "";

            string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
            string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
            string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;

            ResetModel();

            loginState = LoginState.CharacterCreate;

            ZoomCameraOut(); // EEE - Zoom camera out to character create
        }

        public void ZoomCameraIn()
        {
            if (character != null) // EEE - No animation when zoomed in
            {
                Animator anim = character.GetComponentInChildren<Animator>();            
                anim.speed = 0; 
            }            
            zoomingIn = true;
            zoomingOut = false;
            zoomingMid = false;            
        }
        public void ZoomCameraOut()
        {
            if (character != null) // EEE - Start animation when zoomed out
            {
                Animator anim = character.GetComponentInChildren<Animator>();            
                anim.speed = 1; 
            }
            zoomingOut = true;
            zoomingIn = false;
            zoomingMid = false;            
        }
        public void ZoomCameraMid() // EEE - Character Selection zoom position
        {
            if (character != null) // EEE - Start animation when zoomed out
            {
                Animator anim = character.GetComponentInChildren<Animator>();            
                anim.speed = 1; 
            }
            zoomingOut = false;
            zoomingIn = false;
            zoomingMid = true;
        }
        public void ToggleAnim() // Won't use, but placed within Zoom functions
        {
            Animator anim = character.GetComponentInChildren<Animator>();
            if (anim.speed == 0)
                anim.speed = 1;
            else
                anim.speed = 0;
        }

        public void UIAnimalClicked(IconRadio radio) // EEE When New UI Animal is selected
        {
            // 20220801 -   Changed from sliders. Value of radio is set in RadioButton manually
            //              IconRadio was altered to expose radioValue in inspector
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm != null)
            {
                switch (radio.radioValue)
                {
                    case 0: // Badger
                        mcm.defaultEyebrowColor = mcm.defaultBadgerEyebrows;
                        mcm.defaultFurColor = mcm.defaultBadgerFur;
                        mcm.defaultDarkColor = mcm.defaultBadgerDark;
                        break;
                    case 1: // Deer
                        mcm.defaultEyebrowColor = mcm.defaultDeerEyebrows;
                        mcm.defaultFurColor = mcm.defaultDeerFur;
                        mcm.defaultDarkColor = mcm.defaultDeerDark;
                        mcm.defaultCrestColor = mcm.defaultDeerCrest;
                        mcm.defaultDarkerColor = mcm.defaultDeerDarker;
                        break;
                    case 2: // Dog
                        mcm.defaultEyebrowColor = mcm.defaultDogEyebrows;
                        mcm.defaultFurColor = mcm.defaultDogFur;
                        mcm.defaultDarkColor = mcm.defaultDogDark;
                        break;
                    case 3: // Lion
                    case 6: // Rabbit
                        mcm.defaultEyebrowColor = mcm.defaultLionEyebrows;
                        mcm.defaultFurColor = mcm.defaultLionFur;
                        mcm.defaultDarkColor = mcm.defaultLionDark;
                        mcm.defaultCrestColor = mcm.defaultLionCrest;
                        mcm.defaultDarkerColor = mcm.defaultLionDarker;
                        break;
                    case 4: // Lizard
                        mcm.defaultEyebrowColor = mcm.defaultLizardEyebrows;
                        mcm.defaultFurColor = mcm.defaultLizardFur;
                        mcm.defaultDarkColor = mcm.defaultLizardDark;
                        mcm.defaultCrestColor = mcm.defaultLizardCrest;
                        mcm.defaultDarkerColor = mcm.defaultLizardDarker;
                        mcm.defaultOtherColor = mcm.defaultLizardOther;
                        break;
                    case 5: // Owl
                        mcm.defaultEyebrowColor = mcm.defaultOwlEyebrows;
                        mcm.defaultFurColor = mcm.defaultOwlFur;
                        mcm.defaultDarkColor = mcm.defaultOwlDark;
                        mcm.defaultCrestColor = mcm.defaultOwlCrest;
                        mcm.defaultDarkerColor = mcm.defaultOwlDarker;
                        mcm.defaultOtherColor = mcm.defaultOwlOther;
                        break;
                    case 7: // Rat
                        mcm.defaultEyebrowColor = mcm.defaultRatEyebrows;
                        mcm.defaultFurColor = mcm.defaultRatFur;
                        mcm.defaultDarkColor = mcm.defaultRatDark;
                        break;
                }

                ResetColorFieldDefaults();
                mcm.SwitchHead(radio.radioValue); 
            }


        }
        public void UIBodyClicked(IconRadio radio) // EEE When New UI Body is selected
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm != null)
            {
                mcm.SwitchTorso(radio.radioValue); 
            }
        }        
        public void UITabClicked(IconTab iTab) // EEE - Tab clicked - event for zooming
        {
            // Debug.Log("Icontab = " + iTab.name);
            if (iTab.name != tabAnimalName) {
                // Grab profile icon
                grabbingProfileIcon = true; // take a profile pic snapshot when tabs clicked.
                if (iTab.name == tabHeadName)
                {
                    ZoomCameraIn();
                }
                else if (iTab.name != tabAnimalName)
                {
                    ZoomCameraOut();
                }
            }
        }        

        /// <summary>
        /// Sends the Create Character message to the server with a collection of properties
        /// to save to the new character.
        /// </summary>
        public virtual void CreateCharacter()
        {            
            characterName = m_fieldCharacterName.value;
            if (characterName == "")
            {
                m_ScreenPopup.Show("Name Required", "Please enter a name for your character.", "OK");
                return;
            }

            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("characterName", characterName);
            string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
            string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
            string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;
           
            properties.Add("prefab", AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].prefab);
           
            properties.Add("race", raceName);
            properties.Add("aspect", className);
            properties.Add("gender", ganderName);
            properties.Add("genderId", genderId);

            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();

            if (mcm)
            {
                properties.Add("custom:portrait", base64ProfileIcon);

                properties.Add("custom:"+mcm.eyeColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveEyeColor.r, mcm.ActiveEyeColor.g, mcm.ActiveEyeColor.b, mcm.ActiveEyeColor.a)); //modularCustomizationManager.ActiveEyeColor.ToString());
                properties.Add("custom:"+mcm.noseColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveNoseColor.r, mcm.ActiveNoseColor.g, mcm.ActiveNoseColor.b, mcm.ActiveNoseColor.a));
                properties.Add("custom:"+mcm.eyebrowColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveEyebrowColor.r, mcm.ActiveEyebrowColor.g, mcm.ActiveEyebrowColor.b, mcm.ActiveEyebrowColor.a));
                properties.Add("custom:"+mcm.furLightColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveFurLightColor.r, mcm.ActiveFurLightColor.g, mcm.ActiveFurLightColor.b, mcm.ActiveFurLightColor.a)); //modularCustomizationManager.ActiveFurLightColor.ToString());
                properties.Add("custom:"+mcm.furDarkColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveFurDarkColor.r, mcm.ActiveFurDarkColor.g, mcm.ActiveFurDarkColor.b, mcm.ActiveFurDarkColor.a)); //modularCustomizationManager.ActiveFurDarkColor.ToString());
                properties.Add("custom:"+mcm.furDarkerColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveFurDarkerColor.r, mcm.ActiveFurDarkerColor.g, mcm.ActiveFurDarkerColor.b, mcm.ActiveFurDarkerColor.a)); //modularCustomizationManager.ActiveFurDarkerColor.ToString());
                properties.Add("custom:"+mcm.headCrestColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveHeadCrestColor.r, mcm.ActiveHeadCrestColor.g, mcm.ActiveHeadCrestColor.b, mcm.ActiveHeadCrestColor.a)); //modularCustomizationManager.ActiveHeadCrestColor.ToString());
                properties.Add("custom:"+mcm.headOtherColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveHeadOtherColor.r, mcm.ActiveHeadOtherColor.g, mcm.ActiveHeadOtherColor.b, mcm.ActiveHeadOtherColor.a)); //modularCustomizationManager.ActiveHeadOtherColor.ToString());
                properties.Add("custom:"+mcm.leatherColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveLeatherColor.r, mcm.ActiveLeatherColor.g, mcm.ActiveLeatherColor.b, mcm.ActiveLeatherColor.a)); //modularCustomizationManager.ActiveLeatherColor.ToString());
                properties.Add("custom:"+mcm.leatherDarkColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveLeatherDarkColor.r, mcm.ActiveLeatherDarkColor.g, mcm.ActiveLeatherDarkColor.b, mcm.ActiveLeatherDarkColor.a)); //modularCustomizationManager.ActiveLeatherDarkColor.ToString());
                properties.Add("custom:"+mcm.shirtColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveShirtColor.r, mcm.ActiveShirtColor.g, mcm.ActiveShirtColor.b, mcm.ActiveShirtColor.a)); //modularCustomizationManager.ActiveShirtColor.ToString());
                properties.Add("custom:"+mcm.collarColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveCollarColor.r, mcm.ActiveCollarColor.g, mcm.ActiveCollarColor.b, mcm.ActiveCollarColor.a)); //modularCustomizationManager.ActiveCollarColor.ToString());
                properties.Add("custom:"+mcm.shirtLightColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveShirtLightColor.r, mcm.ActiveShirtLightColor.g, mcm.ActiveShirtLightColor.b, mcm.ActiveShirtLightColor.a)); //modularCustomizationManager.ActiveShirtLightColor.ToString());
                properties.Add("custom:"+mcm.shirtDarkColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveShirtDarkColor.r, mcm.ActiveShirtDarkColor.g, mcm.ActiveShirtDarkColor.b, mcm.ActiveShirtDarkColor.a)); //modularCustomizationManager.ActiveShirtDarkColor.ToString());
                properties.Add("custom:"+mcm.metalColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveMetalColor.r, mcm.ActiveMetalColor.g, mcm.ActiveMetalColor.b, mcm.ActiveMetalColor.a)); //modularCustomizationManager.ActiveMetalColor.ToString());
                properties.Add("custom:"+mcm.metalLightColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveMetalLightColor.r, mcm.ActiveMetalLightColor.g, mcm.ActiveMetalLightColor.b, mcm.ActiveMetalLightColor.a)); //modularCustomizationManager.ActiveMetalLightColor.ToString());
                properties.Add("custom:"+mcm.metalDarkColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveMetalDarkColor.r, mcm.ActiveMetalDarkColor.g, mcm.ActiveMetalDarkColor.b, mcm.ActiveMetalDarkColor.a)); //modularCustomizationManager.ActiveMetalDarkColor.ToString());
                properties.Add("custom:"+mcm.metalBlackColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveMetalBlackColor.r, mcm.ActiveMetalBlackColor.g, mcm.ActiveMetalBlackColor.b, mcm.ActiveMetalBlackColor.a)); //modularCustomizationManager.ActiveMetalBlackColor.ToString());
                properties.Add("custom:"+mcm.gemColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveGemColor.r, mcm.ActiveGemColor.g, mcm.ActiveGemColor.b, mcm.ActiveGemColor.a)); //modularCustomizationManager.ActiveGemColor.ToString());
                properties.Add("custom:"+mcm.hemColorPropertyName, String.Format("{0},{1},{2},{3}", mcm.ActiveHemColor.r, mcm.ActiveHemColor.g, mcm.ActiveHemColor.b, mcm.ActiveHemColor.a)); //modularCustomizationManager.ActiveHemColor.ToString());

                properties.Add("custom:"+mcm.headPropertyName, mcm.ActiveAnimalID); // honestly don't know how it worked before
                properties.Add("custom:"+mcm.torsoPropertyName, mcm.ActiveTorsoID);
            }
            
#if AT_I2LOC_PRESET
            dialogMessage = I2.Loc.LocalizationManager.GetTranslation("Please wait...");
#else
            dialogMessage = "Please wait...";
#endif
            errorMessage = "";
            characterSelected = AtavismClient.Instance.NetworkHelper.CreateCharacter(properties);
            if (characterSelected == null)
            {
                errorMessage = "Unknown Error";
            }
            else
            {
                if (!characterSelected.Status)
                {
                    if (characterSelected.ContainsKey("errorMessage"))
                    {
                        errorMessage = (string)characterSelected["errorMessage"];
                    }
                }
            }
            dialogMessage = "";
            // Debug.Log("Error - "+errorMessage);
            if (errorMessage == "")
            {
                StartCharacterSelection();
                // nameUI.text = characterName;
                // Have to rename all the properties. This seems kind of pointless.
                Dictionary<string, object> newProps = new Dictionary<string, object>();
                foreach (string prop in properties.Keys)
                {
                    if (prop.Contains(":"))
                    {
                        string[] newPropParts = prop.Split(':');
                        if (newPropParts.Length > 2 && newPropParts[2] != null)
                        {
                            string newProp = "uma" + newPropParts[2];
                            newProps.Add(newProp, properties[prop]);
                        }
                    }
                }
                foreach (string prop in newProps.Keys)
                {
                    if (!characterSelected.ContainsKey(prop))
                        characterSelected.Add(prop, newProps[prop]);
                }
            }
            else
            {
#if AT_I2LOC_PRESET
            ShowDialog(I2.Loc.LocalizationManager.GetTranslation(errorMessage), true);
#else
                ShowDialog(errorMessage, true);
#endif
            }
        }

        /// <summary>
        /// Cancels character creation and returns back to the selection screen
        /// </summary>
        public virtual void CancelCharacterCreation()
        {
            Destroy(character);
            if (characterSelected != null)
            {
                CharacterSelected(characterSelected);
            }
            StartCharacterSelection();
            ZoomCameraMid();
        }

        void ShowSelectionUI()
        {
            loginState = LoginState.CharacterSelect;

            m_ScreenSelection.style.display = DisplayStyle.Flex;
            m_ScreenCustomize.style.display = DisplayStyle.None;

            // serverNameText.text = AtavismClient.Instance.WorldId;
        }

        void ShowCreationUI()
        {
            m_ScreenSelection.style.display = DisplayStyle.None;
            m_ScreenCustomize.style.display = DisplayStyle.Flex;
        }

        protected void ShowDialog(string message, bool showButton)
        {
            m_ScreenPopup.Show("",message,"OK");
        }

        void ResetModel()
        {
            if (character != null)
                Destroy(character);

            string ganderPrefab = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].prefab;
            int resourcePathPosDamage = ganderPrefab.IndexOf("Resources/");
            if (ganderPrefab.Length > 10)
            {
                ganderPrefab = ganderPrefab.Substring(resourcePathPosDamage + 10);
                ganderPrefab = ganderPrefab.Remove(ganderPrefab.Length - 7);
            }

            GameObject prefab = (GameObject) Resources.Load(ganderPrefab);
            if (prefab == null)
            {
                Debug.LogError("prefab = null model: " + ganderPrefab + " Loading ExampleCharacter");
                prefab = (GameObject)Resources.Load("ExampleCharacter");
            //    return;
            }
            // character = (GameObject) Instantiate(prefab, spawnPosition.position, rotation);
            character = (GameObject) Instantiate(prefab, spawnPosition.position, spawnPosition.rotation);
            x = 180;
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            // set default colours for each colour field in UI
            ResetColorFieldDefaults();

            // reset UI
            m_labelCharacterName.text = "";
            m_fieldCharacterName.value = "";
            m_IconTabsView.ResetTab();
            m_animalContainer.Reset(); // Added Reset to IconRadioGroup to select first radio button
            m_bodyContainer.Reset();
        }

        // Resets the defaults of the colour fields
        public void ResetColorFieldDefaults()
        {
            m_allColorFields.ForEach(SetDefaultColor); 
        }

        private void OnEnable() {
            RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
        }
        private void OnDisable() {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
        }

        private void RenderPipelineManager_endCameraRendering (ScriptableRenderContext arg1, Camera arg2)
        {
            // Takes a picture of the screen for the profile
            if (grabbingProfileIcon) {
                grabbingProfileIcon = false;
                int textureResize = profileIconSize/8;
                Texture2D screenshotTexture = new Texture2D(profileIconSize, profileIconSize,TextureFormat.ARGB32,false);
                Rect rect = new Rect(130,430, profileIconSize, profileIconSize);
                screenshotTexture.ReadPixels(rect, 0,0);
                screenshotTexture.Apply();

                // Resize - https://stackoverflow.com/questions/56949217/how-to-resize-a-texture2d-using-height-and-width
                RenderTexture rt=new RenderTexture(textureResize, textureResize,24);
                RenderTexture.active = rt;
                Graphics.Blit(screenshotTexture,rt);
                Texture2D resizedTexture=new Texture2D(textureResize,textureResize);
                resizedTexture.ReadPixels(new Rect(0,0,textureResize,textureResize),0,0);
                resizedTexture.Apply();

                base64ProfileIcon = Convert.ToBase64String(resizedTexture.EncodeToPNG());
            }
        }

        #endregion CHARACTER CREATION
 
        public void ChangeScene(string sceneName)
        {
            //Application.LoadLevel(sceneName);
            SceneManager.LoadScene(sceneName);
        }
        public void Quit()
        {
            Application.Quit();
        }

        public static CharacterSelectionCreationManager Instance
        {
            get
            {
                return instance;
            }
        }
        public string DialogMessage
        {
            get
            {
                return dialogMessage;
            }
            set
            {
                dialogMessage = value;
            }
        }
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
            }
        }
        public LoginState State
        {
            get
            {
                return loginState;
            }
        }
        public GameObject Character
        {
            get
            {
                return character;
            }
        }
        public GameObject CharacterDCS
        {
            get
            {
                return characterDCS;
            }
        }

        #region UI BUILDER DEFINITIONS
        // EEE Initializes all UI Builder Modular Character Customization components
        public void UIInitModularCharacterComponents()
        {
            int i = 0; // Counter
            var root = GetComponent<UIDocument>().rootVisualElement;
           
            // UI for PopupMessage
            m_ScreenPopup = root.Q<PopupMessage>("ScreenPopup");

            // UI for Character Selection            
            m_ScreenSelection = root.Q<VisualElement>("ScreenSelection"); 
            m_labelCharacterName = root.Q<Label>("label-name");
            m_buttonDeleteCharacter = root.Q<Button>("button-deleteCharacter");
            m_buttonDeleteCharacter.clicked += DeleteCharacter;
            m_buttonNewCharacter = root.Q<Button>("button-newCharacter");
            m_buttonNewCharacter.clicked += GoToCreation;
            m_buttonPlayGame = root.Q<Button>("button-play");
            m_buttonPlayGame.clicked += Play;
            m_ProfileContainer = root.Q<IconProfileGroup>("IconProfileGroup"); 

            UQueryBuilder<IconProfile> allProfiles = m_ProfileContainer.Query<IconProfile>();
            allProfiles.ForEach(e => e.OnSelect += UIProfileClicked); // for each profile, assign function for click

            // UI for Character Creation
            m_ScreenCustomize = root.Q<VisualElement>("ScreenCustomize"); 
            m_SectionContentContainer = root.Q<VisualElement>("content-container"); 

            // Use UQueryBuilder to loop through all children of a specific type: https://docs.unity3d.com/Manual/UIE-UQuery.html
            // Simple Example: https://gist.github.com/polerin/d93feca1f69534e1e64374324bbaf3cc

            // Initialize Tabs
            m_IconTabsView = root.Q<IconTabsView>("IconTabsView"); // needed to call a reset of tabs
            UQueryBuilder<IconTab> allTabs = root.Query<VisualElement>("tabs-container").Children<IconTab>();
            allTabs.ForEach(e => e.OnSelect += UITabClicked); // for each tab, assign function for click

            // Initialize Animal Buttons
            m_animalContainer = root.Q<IconRadioGroup>("animalContainer"); 
            UQueryBuilder<IconRadio> allAnimals = m_animalContainer.Query<IconRadio>();
            allAnimals.ForEach(e => e.OnSelect += UIAnimalClicked); // for each animal, assign function for click

            // Initialize Animal Buttons
            m_bodyContainer = root.Q<IconRadioGroup>("bodyContainer"); 
            UQueryBuilder<IconRadio> allBodies = m_bodyContainer.Query<IconRadio>();
            allBodies.ForEach(e => e.OnSelect += UIBodyClicked); // for each body, assign function for click

            // Initialize colour buttons
            m_ColorPopup = root.Q<ColorPopup>("color-popup"); // Pop up dialogue for color selection
            m_ColorPopup.onValueChange += UIModularCharacterSetColor;

            // Initiate color fields
            m_allColorFields = m_SectionContentContainer.Query<ColorField>();
            m_allColorFields.ForEach(e => e.ColorPopup = m_ColorPopup); // Assign same color popup for each color field

            m_fieldCharacterName = root.Q<TextField>("field-characterName");
            m_buttonAcceptCharacter = root.Q<Button>("button-acceptCharacter");
            m_buttonCancelCharacter = root.Q<Button>("button-cancelCharacter");

            m_buttonCancelCharacter.clicked += CancelCharacterCreation;
            m_buttonAcceptCharacter.clicked += CreateCharacter;

            m_fieldCharacterName.RegisterCallback<KeyDownEvent>(OnKeyPress, TrickleDown.TrickleDown); // restrict certain characters in name

        }

        void OnKeyPress(KeyDownEvent ev)
        {
//            Debug.Log($"Pressed key: {ev.keyCode}, character: {ev.character}, modifiers: {ev.modifiers}");
            if ( Char.IsSymbol(ev.character) || Char.IsNumber(ev.character) || Char.IsPunctuation(ev.character)) 
            {
                ev.PreventDefault(); // Restrict certain characters from being entered. 
            }
        }

        public void UIModularCharacterSetColor(Color32 color) // EEE - Set colour of parts
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm != null)
            {
                mcm.UpdateShaderColor(color,m_ColorPopup.CurrentPart);
            }
        }

        // EEE Initialize default colors for both model and for UI color fields
        public void SetDefaultColor(ColorField colorField) 
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm)
            {
                switch (colorField.bodyPartColor)
                {
                    case BodyPartColor.ColorEye:
                        mcm.UpdateShaderColor(mcm.defaultEyeColor, BodyPartColor.ColorEye);
                        colorField.SetValueWithoutNotify(mcm.defaultEyeColor);
                        break;
                    case BodyPartColor.ColorNose:
                        mcm.UpdateShaderColor(mcm.defaultNoseColor, BodyPartColor.ColorNose);
                        colorField.SetValueWithoutNotify(mcm.defaultNoseColor);
                        break;                        
                    case BodyPartColor.ColorEyebrow:
                        mcm.UpdateShaderColor(mcm.defaultEyebrowColor, BodyPartColor.ColorEyebrow);
                        colorField.SetValueWithoutNotify(mcm.defaultEyebrowColor);
                        break;
                    case BodyPartColor.ColorFurLight:
                        mcm.UpdateShaderColor(mcm.defaultFurColor, BodyPartColor.ColorFurLight);
                        colorField.SetValueWithoutNotify(mcm.defaultFurColor);
                        break;
                    case BodyPartColor.ColorFurDark:
                        mcm.UpdateShaderColor(mcm.defaultDarkColor, BodyPartColor.ColorFurDark);
                        colorField.SetValueWithoutNotify(mcm.defaultDarkColor);
                        break;
                    case BodyPartColor.ColorFurDarker:
                        mcm.UpdateShaderColor(mcm.defaultDarkerColor, BodyPartColor.ColorFurDarker);
                        colorField.SetValueWithoutNotify(mcm.defaultDarkerColor);
                        break;
                    case BodyPartColor.ColorHeadCrest:
                        mcm.UpdateShaderColor(mcm.defaultCrestColor, BodyPartColor.ColorHeadCrest);
                        colorField.SetValueWithoutNotify(mcm.defaultCrestColor);
                        break;
                    case BodyPartColor.ColorHeadOther:
                        mcm.UpdateShaderColor(mcm.defaultOtherColor, BodyPartColor.ColorHeadOther);
                        colorField.SetValueWithoutNotify(mcm.defaultOtherColor);
                        break;
                    case BodyPartColor.ColorLeather:
                        mcm.UpdateShaderColor(mcm.defaultLeatherColor, BodyPartColor.ColorLeather);
                        colorField.SetValueWithoutNotify(mcm.defaultLeatherColor);
                        break;
                    case BodyPartColor.ColorLeatherDark:
                        mcm.UpdateShaderColor(mcm.defaultLeatherDarkColor, BodyPartColor.ColorLeatherDark);
                        colorField.SetValueWithoutNotify(mcm.defaultLeatherDarkColor);
                        break;
                    case BodyPartColor.ColorCollar:
                        mcm.UpdateShaderColor(mcm.defaultCollarColor, BodyPartColor.ColorCollar);
                        colorField.SetValueWithoutNotify(mcm.defaultCollarColor);
                        break;
                    case BodyPartColor.ColorShirt:
                        mcm.UpdateShaderColor(mcm.defaultShirtColor, BodyPartColor.ColorShirt);
                        colorField.SetValueWithoutNotify(mcm.defaultShirtColor);
                        break;
                    case BodyPartColor.ColorShirtLight:
                        mcm.UpdateShaderColor(mcm.defaultShirtLightColor, BodyPartColor.ColorShirtLight);
                        colorField.SetValueWithoutNotify(mcm.defaultShirtLightColor);
                        break;
                    case BodyPartColor.ColorShirtDark:
                        mcm.UpdateShaderColor(mcm.defaultShirtDarkColor, BodyPartColor.ColorShirtDark);
                        colorField.SetValueWithoutNotify(mcm.defaultShirtDarkColor);
                        break;
                    case BodyPartColor.ColorMetal:
                        mcm.UpdateShaderColor(mcm.defaultMetalColor, BodyPartColor.ColorMetal);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalColor);
                        break;
                    case BodyPartColor.ColorMetalLight:
                        mcm.UpdateShaderColor(mcm.defaultMetalLightColor, BodyPartColor.ColorMetalLight);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalLightColor);
                        break;
                    case BodyPartColor.ColorMetalDark:
                        mcm.UpdateShaderColor(mcm.defaultMetalDarkColor, BodyPartColor.ColorMetalDark);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalDarkColor);
                        break;
                    case BodyPartColor.ColorMetalBlack:
                        mcm.UpdateShaderColor(mcm.defaultMetalBlackColor, BodyPartColor.ColorMetalBlack);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalBlackColor);
                        break;
                    case BodyPartColor.ColorGem:
                        mcm.UpdateShaderColor(mcm.defaultGemColor, BodyPartColor.ColorGem);
                        colorField.SetValueWithoutNotify(mcm.defaultGemColor);
                        break;
                    case BodyPartColor.ColorHem:
                        mcm.UpdateShaderColor(mcm.defaultHemColor, BodyPartColor.ColorHem);
                        colorField.SetValueWithoutNotify(mcm.defaultHemColor);
                        break;                        
                }
            }
        }
 
        #endregion UI BUILDER DEFINITIONS
    }
}