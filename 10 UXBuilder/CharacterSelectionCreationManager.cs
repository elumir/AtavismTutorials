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

namespace Atavism
{
    [Serializable]
    public class AtavismGender
    {
        public string name;
        public OldUI.Button genderButton;
        public OldUI.Text genderText;
        public TextMeshProUGUI TMPGenderText;
    }

    public enum CreationState
    {
        Body,
        Head,
        Face,
        Hair
    }
    
    /// <summary>
    /// Handles the selection and creation of the players characters. This script must be added
    /// as a component in the Character Creation / Selection scene.
    /// </summary>
    public class CharacterSelectionCreationManager : MonoBehaviour
    {
        protected static CharacterSelectionCreationManager instance;
        [Tooltip("")]
        [SerializeField] public bool allowDifferentHairColors;
  
      //  public Texture cursorOverride;
        public List<GameObject> createUI;
        public List<GameObject> selectUI;
        public List<UGUICharacterSelectSlot> characterSlots;
        public GameObject enterUI;
        public OldUI.Button createButton;
        public OldUI.Text nameUI;
        public TextMeshProUGUI TMPNameUI;
        public OldUI.Button deleteButton;
        public OldUI.Text serverNameText;
        public TextMeshProUGUI TMPServerNameText;
        public UGUIServerList serverListUI;
        public GameObject characterCamera;
        public UGUIDialogPopup dialogWindow;
        public Transform spawnPosition;
        public OldUI.InputField createCaracterName;
        public TMP_InputField TMPCreateCaracterName;

        protected GameObject character;
        protected GameObject characterDCS;

        protected List<CharacterEntry> characterEntries;
        protected CreationState creationState;
        protected LoginState loginState;
        protected string dialogMessage = "";
        protected string errorMessage = "";

        // Character select fields
        protected CharacterEntry characterSelected = null;
        protected string characterName = "";
        protected int raceId = -1;
        protected int aspectId = -1;
        protected int genderId = -1;
        public List<UGUICharacterRaceSlot> races;
        public List<UGUICharacterClassSlot> classes;
        public List<UGUICharacterGenderSlot> genders;

        public Image raceIcon;
        public OldUI.Text raceTitle;
        public TextMeshProUGUI TMPRaceTitle;
        public OldUI.Text raceDescription;
        public TextMeshProUGUI TMPRaceDescription;

        public Image classIcon;
        public OldUI.Text classTitle;
        public TextMeshProUGUI TMPClassTitle;
        public OldUI.Text classDescription;
        public TextMeshProUGUI TMPClassDescription;
        public Color defaultButomTextColor = Color.white;
        public Color selectedButomTextColor = Color.green;
        public GameObject createPanelRace;
        public UGUIAvatarList avatarList;
        public Image avatarIcon;

        // Camera fields
        // EEE New Camera Variables
        public Vector3 cameraInLoc = new Vector3(0.4f, 1.6f, -0.7f);
        public Vector3 cameraOutLoc = new Vector3(1.2f, 1f, -2f);
        public Vector3 cameraMidLoc = new Vector3(0f, 1f, -2f);
        protected bool zoomingIn = false;
        protected bool zoomingOut = false;
        protected bool zoomingMid = false; // EEE - Set camera for mid zoom
        public float characterRotationSpeed = 250.0f;
        public float moveRate = 1.0f;
        public String tabHeadName = "tabThree"; // EEE - this tab triggers zoom to go in
        protected float x = 180;
        protected float y = 0;

        // EEE UI Builder
        public VisualElement m_ScreenSelection; // Holds UI for selecting character        
        public Label m_labelCharacterName;
        public Button m_buttonDeleteCharacter;
        public VisualElement m_ScreenCustomize; // Holds UI for customizing character
        public IconTabsView m_IconTabsView; // updated to have a reset
        public IconRadioGroup m_gendersContainer; // Holds genders
        public VisualElement m_SectionContentContainer; // holds create character ui content
        public UQueryBuilder<ModCharSlider> m_allSliders; // holds all the UI Sliders
        public UQueryBuilder<ColorField> m_allColorFields; // holds all the UI Color fields
        public ModCharSlider m_SliderBeard; // EEE to turn off and on depending on gender. Why can't females have beards??
        public ColorField m_ScarColor;
        public ColorField m_TattooColor;
        public ColorPopup m_ColorPopup;
        public Button m_buttonAcceptCharacter;
        public Button m_buttonCancelCharacter;
        public TextField m_fieldCharacterName;
        public PopupMessage m_ScreenPopup;

        public  List<ModularCharacterSlider> modularCharacterSliderList = new List<ModularCharacterSlider>();
        public  List<ModularCharacterColor> modularCharacterColorPanelList = new List<ModularCharacterColor>();
        

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
//            float moveRate = 1.0f; // made a public variable to set
            if (character != null)
            {
                if (zoomingIn)
                {
                    characterCamera.transform.position = Vector3.Lerp(characterCamera.transform.position, character.GetComponent<AtavismMobAppearance>().GetSocketTransform("Head").position + cameraInLoc, Time.deltaTime * moveRate);
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

        #region Character Selection
        public void StartCharacterSelection()
        {
            characterEntries = ClientAPI.GetCharacterEntries();
            ShowSelectionUI();

            if (characterEntries.Count > 0)
            {
                CharacterSelected(characterEntries[0]);
                if (enterUI != null)
                {
                    enterUI.SetActive(true);
                }
                if (nameUI != null)
                {
                    nameUI.gameObject.SetActive(true);
                }
                if (TMPNameUI != null)
                {
                    TMPNameUI.gameObject.SetActive(true);
                }
                if (deleteButton != null)
                {
                    deleteButton.gameObject.SetActive(true);
                }
            }
            else
            {
                if (character != null)
                    Destroy(character);
                characterSelected = null;
                if (enterUI != null)
                {
                    enterUI.SetActive(false);
                }
                if (nameUI != null)
                {
                    nameUI.gameObject.SetActive(false);
                }
                if (TMPNameUI != null)
                {
                    TMPNameUI.gameObject.SetActive(false);
                }

                if (deleteButton != null)
                {
                    deleteButton.gameObject.SetActive(false);
                }
            }
            if (characterEntries.Count < characterSlots.Count)
            {
                createButton.gameObject.SetActive(true);
            }
            else
            {
                createButton.gameObject.SetActive(false);
            }

            // Set the slots up
            for (int i = 0; i < characterSlots.Count; i++)
            {
                if (characterEntries.Count > i)
                {
                    // Set slot data
                    characterSlots[i].gameObject.SetActive(true);
                    characterSlots[i].SetCharacter(characterEntries[i]);
                    characterSlots[i].CharacterSelected(characterSelected);
                }
                else
                {
                    // Set slot data to null
                    characterSlots[i].gameObject.SetActive(false);
                }
            }
            loginState = LoginState.CharacterSelect;
            ZoomCameraMid(); // EEE - Added in here - Zoom camera out to character select
        }
        
        public virtual void ModularCustomizationManagerCheck()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            if (character && modularCustomizationManager)
            {
                if (characterSelected.ContainsKey(modularCustomizationManager.EyeMaterialPropertyName))
                {
                    modularCustomizationManager.UpdateEyeMaterial((int)characterSelected[modularCustomizationManager.EyeMaterialPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.HairMaterialPropertyName))
                {
                    modularCustomizationManager.UpdateHairMaterial((int)characterSelected[modularCustomizationManager.HairMaterialPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.SkinMaterialPropertyName))
                {
                    modularCustomizationManager.UpdateSkinMaterial((int)characterSelected[modularCustomizationManager.SkinMaterialPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.MouthMaterialPropertyName))
                {
                    modularCustomizationManager.UpdateMouthMaterial((int)characterSelected[modularCustomizationManager.MouthMaterialPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.bodyColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.bodyColorPropertyName].ToString().Split(',');
                    Color32 test = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateBodyColor(test);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.scarColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.scarColorPropertyName].ToString().Split(',');
                    Color32 test = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateBodyScarColor(test);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.hairColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.hairColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateHairColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.mouthColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.mouthColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateMouthColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.beardColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.beardColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateBeardColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.eyeBrowColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.eyeBrowColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateEyebrowColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.stubbleColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.stubbleColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateStubbleColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.bodyArtColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.bodyArtColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateBodyArtColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.eyeColorPropertyName))
                {
                    var item = characterSelected[modularCustomizationManager.eyeColorPropertyName].ToString().Split(',');
                    Color32 color32 = new Color32(Convert.ToByte(item[0]), Convert.ToByte(item[1]), Convert.ToByte(item[2]), Convert.ToByte(item[3]));
                    modularCustomizationManager.UpdateEyeColor(color32);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.helmetColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.helmetColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Head);
                    }
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.torsoColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.torsoColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Torso);
                    }
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.upperArmsColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.upperArmsColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Upperarms);
                    }
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.lowerArmsColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.lowerArmsColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.LowerArms);
                    }
                }
                if (characterSelected.ContainsKey(modularCustomizationManager.hipsColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.hipsColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Hips);
                    }
                }
                if (characterSelected.ContainsKey(modularCustomizationManager.lowerLegsColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.lowerLegsColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.LowerLegs);
                    }
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.feetColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.feetColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Feet);
                    }
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.handsColorPropertyName))
                {
                    var colorProperties = characterSelected[modularCustomizationManager.handsColorPropertyName].ToString().Split('@');
                    foreach (var colorProperty in colorProperties)
                    {
                        var colorPropertyItem = colorProperty.Split(':');
                        var colorslot = colorPropertyItem[0];
                        var coloritem = colorPropertyItem[1].Split(',');
                        Color32 color32 = new Color32(Convert.ToByte(coloritem[0]), Convert.ToByte(coloritem[1]), Convert.ToByte(coloritem[2]), Convert.ToByte(coloritem[3]));
                        modularCustomizationManager.UpdateShaderColor(color32, colorslot, BodyType.Hands);
                    }
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.hairPropertyName))
                {
                    modularCustomizationManager.UpdateHairModel((int)characterSelected[modularCustomizationManager.hairPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.beardPropertyName))
                {
                    modularCustomizationManager.UpdateBeardModel((int)characterSelected[modularCustomizationManager.beardPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.eyebrowPropertyName))
                {

                    modularCustomizationManager.UpdateEyebrowModel((int)characterSelected[modularCustomizationManager.eyebrowPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.headPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.headPropertyName], BodyType.Head);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.faceTexPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.faceTexPropertyName], BodyType.Face);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.handsPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.handsPropertyName], BodyType.Hands);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.lowerArmsPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.lowerArmsPropertyName], BodyType.LowerArms);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.upperArmsPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.upperArmsPropertyName], BodyType.Upperarms);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.torsoPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.torsoPropertyName], BodyType.Torso);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.hipsPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.hipsPropertyName], BodyType.Hips);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.lowerLegsPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.lowerLegsPropertyName], BodyType.LowerLegs);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.feetPropertyName))
                {
                    modularCustomizationManager.UpdateBodyModel((string)characterSelected[modularCustomizationManager.feetPropertyName], BodyType.Feet);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.earsPropertyName))
                {
                    modularCustomizationManager.UpdateEarModel((int)characterSelected[modularCustomizationManager.earsPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.eyesPropertyName))
                {
                    modularCustomizationManager.UpdateEyeModel((int)characterSelected[modularCustomizationManager.eyesPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.tuskPropertyName))
                {
                    modularCustomizationManager.UpdateTuskModel((int)characterSelected[modularCustomizationManager.tuskPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.mouthPropertyName))
                {
                    modularCustomizationManager.UpdateMouthModel((int)characterSelected[modularCustomizationManager.mouthPropertyName]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.faithPropertyName))
                {
                    modularCustomizationManager.SetFaith((string)characterSelected[modularCustomizationManager.faithPropertyName]);
                }
#if IPBRInt
                if (characterSelected.ContainsKey(modularCustomizationManager.blendshapePresetValue) && (!modularCustomizationManager.enableSavingInfinityPBRBlendshapes))
                {
                    modularCustomizationManager.UpdateBlendShapePresets((int)characterSelected[modularCustomizationManager.blendshapePresetValue]);
                }

                if (characterSelected.ContainsKey(modularCustomizationManager.infinityBlendShapes))
                {
                    modularCustomizationManager.UpdateBlendShapes((string)characterSelected[modularCustomizationManager.infinityBlendShapes]);
                }
#endif

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
            foreach (UGUICharacterSelectSlot charSlot in characterSlots)
            {
                charSlot.CharacterSelected(characterSelected);
            }
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

            // Name
            // if (nameUI != null)
            //     nameUI.text = (string)entry["characterName"];
            // if (TMPNameUI != null)
            //     TMPNameUI.text = (string)entry["characterName"];

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

        public void DeleteCharacter()
        {
//             dialogWindow.gameObject.SetActive(true);
// #if AT_I2LOC_PRESET
//         dialogWindow.ShowDialogOptionPopup(I2.Loc.LocalizationManager.GetTranslation("Do you want to delete character") + ": " + characterSelected["characterName"]);
// #else
//             dialogWindow.ShowDialogOptionPopup("Do you want to delete character: " + characterSelected["characterName"]);
// #endif
            m_ScreenPopup.Show("Delete Character: "+ characterSelected["characterName"], "Are you sure? Everything about this character will be lost!", 
                "Delete", "Cancel", DeleteCharacterConfirmed, DeleteCharacterCancelled);
        }

        public virtual void DeleteCharacterCancelled()
        {
            m_ScreenPopup.Hide();
        }

        public virtual void DeleteCharacterConfirmed()
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
                    StartCharacterSelection();
                    return;
                }
            }
            StartCharacterCreation();
        }
        #endregion Character Selection

        #region Character Creation
        public virtual void StartCharacterCreation()
        {
            ShowCreationUI();
            m_labelCharacterName.text = "";
            m_fieldCharacterName.value = "";

            int r = 0;

            var rlist = AtavismPrefabManager.Instance.GetRaceData().Keys.ToList();
            rlist.Sort();
            foreach (var rk in rlist)
            {
                var race = AtavismPrefabManager.Instance.GetRaceData()[rk];
                if (r == 0)
                {
                    raceId = race.id;
                }
                if (races.Count > r)
                {
                    if (!races[r].rootGameObject.activeSelf)
                        races[r].rootGameObject.SetActive(true);
                    races[r].SetRace(race.id);
                }
                r++;
            }

            for (int rr = r; rr < races.Count; rr++)
            {
                if( races[rr]!=null)
                    races[rr].rootGameObject.SetActive(false);
            }

            foreach (UGUICharacterRaceSlot raceSlot in races)
            {
                if( raceSlot!=null)  raceSlot.RaceSelected(raceId);
            }

            int c = 0;
            var cllist = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.Keys.ToList();
            cllist.Sort();
            foreach (var clk in cllist)
            {
                var cl = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[clk];
                if (c == 0 && !AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.ContainsKey(aspectId))
                {
                    aspectId = cl.id;
                }
                if (classes.Count > c)
                {
                    classes[c].SetClass(raceId,cl.id);
                    if( !classes[c].rootGameObject.activeSelf)
                        classes[c].rootGameObject.SetActive(true);
                }
                c++;
            }

            foreach (UGUICharacterClassSlot classSlot in classes)
            {
                classSlot.ClassSelected(aspectId);
            }

            for (int cc = c; cc < classes.Count; cc++)
            {
                if(  classes[cc]!=null)    classes[cc].rootGameObject.SetActive(false);
            }

            UpdateRaceDetails();
            foreach (UGUICharacterClassSlot classSlot in classes)
            {
                if( classSlot!=null)  classSlot.ClassSelected(aspectId);
            }

            // Initialize Gender [EEE]
            int genderRadioIndex = 0;
            // Grab genders for this race and class from the DB
            var genderList = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Keys.ToList();
            genderList.Sort();
            foreach (var genX in genderList)
            {
                var tempGender = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genX];
                if (genderRadioIndex == 0 && !AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.ContainsKey(genderId))
                {
                    genderId = tempGender.id;
                }
                if (m_gendersContainer.childCount > genderRadioIndex)
                {
                    VisualElement element = m_gendersContainer[genderRadioIndex];
                    if (element is IconRadio radioElement)
                    {
                        radioElement.radioValue = tempGender.id; // Assigned Gender ID to radio buttons
                    }
                }
                genderRadioIndex++;
            }

            // Initialize Gender Old
            int g = 0;
            var genlist = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Keys.ToList();
            genlist.Sort();
            foreach (var genk in genlist)
            {
                var gen = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genk];
                if (g == 0 && !AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.ContainsKey(genderId))
                {
                    genderId = gen.id;
                }
                if (genders.Count > g)
                {
                    genders[g].SetGender(raceId,aspectId,gen.id);
                    if( !genders[g].rootGameObject.activeSelf)
                        genders[g].rootGameObject.SetActive(true);
                }
                g++;
            }

            for (int gg = g; gg < genders.Count; gg++)
            {
                genders[gg].rootGameObject.SetActive(false);
            }

            foreach (UGUICharacterGenderSlot genderSlot in genders)
            {
                genderSlot.GenderSelected(genderId);
            }

            if (character != null)
                Destroy(character);
            characterName = "";

            //   Debug.LogError("Race="+raceId+" Class="+aspectId+" Gender="+genderId);
            string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
            string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
            string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;
            if (avatarList != null)
            {
                avatarList.PreparSlots(raceName, ganderName, className);
            }

            // Do this after gender so the icons can be updated 
            UpdateClassDetails();

            ResetModel();

            loginState = LoginState.CharacterCreate;
            creationState = CreationState.Body;

            // EEE - Original Code set min/max values for sliders here. Removed.

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

        public void SetCharacterName(string characterName)
        {
            this.characterName = characterName;
        }

        public void CreateCharacterWithName(string characterName)
        {
            this.characterName = characterName;
            CreateCharacter();
        }

        /// <summary>
        /// Sets the characters race resulting in a new UMA model being generated.
        /// </summary>
        /// <param name="race">Race.</param>
        public virtual void SetCharacterRace(int raceId)
        {
            this.raceId = raceId;
            foreach (UGUICharacterRaceSlot raceSlot in races)
            {
                raceSlot.RaceSelected(raceId);
            }
            UpdateRaceDetails();
            int c = 0;
            var cllist = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.Keys.ToList();
            cllist.Sort();
            foreach (var clk in cllist)
            {
                var cl = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[clk];
                if (c == 0 && !AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.ContainsKey(aspectId))
                {
                    aspectId = cl.id;
                }
                if (classes.Count > c)
                {
                    classes[c].SetClass(raceId,cl.id);
                    if( !classes[c].rootGameObject.activeSelf)
                        classes[c].rootGameObject.SetActive(true);
                }
                c++;
            }

            foreach (UGUICharacterClassSlot classSlot in classes)
            {
                classSlot.ClassSelected(aspectId);
            }
            for (int cc = c; cc < classes.Count; cc++)
            {
                classes[cc].rootGameObject.SetActive(false);
            }
           
            int g = 0;
            
            var genlist = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Keys.ToList();
            genlist.Sort();
            foreach (var genk in genlist)
            {
                var gen = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genk];
                if (g == 0 && !AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.ContainsKey(genderId))
                {
                    genderId = gen.id;
                }
                if (genders.Count > g)
                {
                    genders[g].SetGender(raceId,aspectId,gen.id);
                    if( !genders[g].rootGameObject.activeSelf)
                        genders[g].rootGameObject.SetActive(true);
                }

                g++;
            }
            UpdateClassDetails();
            foreach (UGUICharacterClassSlot classSlot in classes)
            {
                classSlot.ClassSelected(aspectId);
            }

            for (int gg = g; gg < genders.Count; gg++)
            {
                genders[gg].rootGameObject.SetActive(false);
            }
            foreach (UGUICharacterGenderSlot genderSlot in genders)
            {
                genderSlot.GenderSelected(genderId);
            }
            
            string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
            string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
            string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;
            if (avatarList != null)
            {
                avatarList.PreparSlots(raceName, ganderName, className);
            }
            ResetModel();
        }
        
        /// <summary>
        /// Sets the characters race resulting in a new UMA model being generated.
        /// </summary>
        /// <param name="race">Race.</param>
        protected void UpdateRaceDetails()
        {
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId))
            {
                var race  = AtavismPrefabManager.Instance.GetRaceData()[raceId];
                if (raceIcon != null)
                    raceIcon.sprite = race.icon;
                if (raceTitle != null)
                    raceTitle.text = race.name;
                if (raceDescription != null)
                    raceDescription.text = race.description;
                if (TMPRaceTitle != null)
                    TMPRaceTitle.text = race.name;
                if (TMPRaceDescription != null)
                    TMPRaceDescription.text = race.description;
            }
        }

        public virtual void SetCharacterClass(int classId)
        {
            this.aspectId = classId;
            foreach (UGUICharacterClassSlot classSlot in classes)
            {
                if (classSlot != null)
                    classSlot.ClassSelected(classId);
            }
            UpdateClassDetails();
            int g = 0;
            foreach (var gen in AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Values)
            {
              //  Debug.LogError(" Gender " + gen.id + " " + gen.name);
                if (g == 0 && ! AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.ContainsKey(genderId))
                {
                    genderId = gen.id;
                }
                if (genders.Count > g)
                {
                    genders[g].SetGender(raceId,aspectId,gen.id);
                    if( !genders[g].rootGameObject.activeSelf)
                        genders[g].rootGameObject.SetActive(true);
                }
                g++;
            }
            for (int gg = g; gg < genders.Count; gg++)
            {
                genders[gg].rootGameObject.SetActive(false);
            }
            foreach (UGUICharacterGenderSlot genderSlot in genders)
            {
                genderSlot.GenderSelected(genderId);
            }
            
        //    Debug.LogError("Race="+raceId+" Class="+aspectId+" Gender="+genderId);
            string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
            string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
            string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;
            if (avatarList != null)
            {
                avatarList.PreparSlots(raceName, ganderName, className);
            }
            ResetModel();
        }

        protected void UpdateClassDetails()
        {
            if (AtavismPrefabManager.Instance.GetRaceData().ContainsKey(raceId) && AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.ContainsKey(aspectId))
            {
                var clas = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId];
                
                if (classIcon != null)
                {
                   classIcon.sprite = clas.icon;
                }

                if (classTitle != null)
                    classTitle.text = clas.name;
                if (classDescription != null)
                    classDescription.text = clas.description;
                if (TMPClassTitle != null)
                    TMPClassTitle.text = clas.name;
                if (TMPClassDescription != null)
                    TMPClassDescription.text = clas.description;

                string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
                string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
                string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;

                if (avatarList != null)
                    avatarList.PreparSlots(raceName, ganderName, className);
            }
        }

        public virtual void SetCharacterGender(int genderId)
        {
            this.genderId = genderId;
            foreach (UGUICharacterGenderSlot genderSlot in genders)
            {
                if (genderSlot != null)
                    genderSlot.GenderSelected(genderId);
            }
            UpdateClassDetails();
            // Debug.Log("Race="+raceId+" Class="+aspectId+" Gender="+genderId);

            string raceName = AtavismPrefabManager.Instance.GetRaceData()[raceId].name;
            string className = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].name;
            string ganderName = AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList[genderId].name;
            if (avatarList != null)
            {
                avatarList.PreparSlots(raceName, ganderName, className);
            }
            ResetModel();
        }

        public void UIGenderClicked(IconRadio radio) // EEE When New UI gender is selected
        {
            if (radio.radioValue == 414) // 414 is female icon. Don't know how else to quickly do this
            {
                m_SliderBeard.style.display = DisplayStyle.None; // Turn off beard slider when female body
            }
            else 
            {
                m_SliderBeard.style.display = DisplayStyle.Flex;
            }
            SetCharacterGender(radio.radioValue);
        }

        public void UITabClicked(IconTab iTab) // EEE - Tab clicked - event for zooming
        {
            // Debug.Log("Icontab = " + iTab.name);
            if (iTab.name == tabHeadName)
            {
                ZoomCameraIn();
            }
            else 
            {
                ZoomCameraOut();
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
                return;
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
            if (PortraitManager.Instance.portraitType == PortraitType.Custom)
            {
                Sprite[] icons = null;//= { new Sprite() };

                icons = AtavismSettings.Instance.Avatars(raceName, ganderName, className);

                if (icons != null && icons.Length > 0)
                {
                    if (avatarList == null)
                    {
                        Debug.LogError("CharacterSelectionCreationManager avatarList is null", gameObject);
                    }
                    else
                    {
                        if (icons.Length > avatarList.Selected())
                            if (icons[avatarList.Selected()] == null)
                            {
                                Debug.LogError("CharacterSelectionCreationManager icons for " + raceName+" "+ganderName + " "+ className+" is null ; avatarList selected " + avatarList.Selected(), gameObject);
                            }
                            else
                            {
                                properties.Add("custom:portrait", icons[avatarList.Selected()].name);
                                //     properties.Add("portrait", icons[avatarList.Selected()].name);
                            }
                    }
                }
            }

            if (PortraitManager.Instance.portraitType == PortraitType.Class)
            {
                Sprite portraitSprite = PortraitManager.Instance.GetCharacterSelectionPortrait(genderId, raceId, aspectId, PortraitType.Class);
                properties.Add("custom:portrait", portraitSprite.name);
            }

            // If the character has the customisable hair, save the property
            if (character.GetComponent<CustomisedHair>() != null) {
                CustomisedHair customHair = character.GetComponent<CustomisedHair>();
                properties.Add("custom:" + customHair.hairPropertyName, customHair.ActiveHair.name);
            }
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            if (modularCustomizationManager)
            {
                properties.Add("custom:EyeMaterial", modularCustomizationManager.ActiveEyeMaterialId);
                properties.Add("custom:HairMaterial", modularCustomizationManager.ActiveHairMaterialId);
                properties.Add("custom:SkinMaterial", modularCustomizationManager.ActiveSkinMaterialId);
                properties.Add("custom:MouthMaterial", modularCustomizationManager.ActiveMouthMaterialId);

                if (modularCustomizationManager.ActiveHair)
                {
                    properties.Add("custom:HairModel", modularCustomizationManager.ActiveHairId);
                    properties.Add("custom:HairColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveHairColor.r, modularCustomizationManager.ActiveHairColor.g, modularCustomizationManager.ActiveHairColor.b, modularCustomizationManager.ActiveHairColor.a)); //modularCustomizationManager.ActiveHairColor.ToString());
                }
                if (modularCustomizationManager.allowDifferentHairColors)
                {
                    properties.Add("custom:BeardColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveBeardColor.r, modularCustomizationManager.ActiveBeardColor.g, modularCustomizationManager.ActiveBeardColor.b, modularCustomizationManager.ActiveBeardColor.a)); //modularCustomizationManager.ActiveHairColor.ToString());
                    properties.Add("custom:EyeBrowColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveEyebrowColor.r, modularCustomizationManager.ActiveEyebrowColor.g, modularCustomizationManager.ActiveEyebrowColor.b, modularCustomizationManager.ActiveEyebrowColor.a)); //modularCustomizationManager.ActiveHairColor.ToString());
                }

                properties.Add("custom:BodyColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveBodyColor.r, modularCustomizationManager.ActiveBodyColor.g, modularCustomizationManager.ActiveBodyColor.b, modularCustomizationManager.ActiveBodyColor.a));
                properties.Add("custom:ScarColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveScarColor.r, modularCustomizationManager.ActiveScarColor.g, modularCustomizationManager.ActiveScarColor.b, modularCustomizationManager.ActiveScarColor.a));//modularCustomizationManager.ActiveScarColor.ToString());
                properties.Add("custom:StubbleColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveStubbleColor.r, modularCustomizationManager.ActiveStubbleColor.g, modularCustomizationManager.ActiveStubbleColor.b, modularCustomizationManager.ActiveStubbleColor.a)); //modularCustomizationManager.ActiveBeardColor.ToString());
                properties.Add("custom:BodyArtColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveBodyArtColor.r, modularCustomizationManager.ActiveBodyArtColor.g, modularCustomizationManager.ActiveBodyArtColor.b, modularCustomizationManager.ActiveBodyArtColor.a)); //modularCustomizationManager.ActiveBeardColor.ToString());
                properties.Add("custom:EyeColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveEyeColor.r, modularCustomizationManager.ActiveEyeColor.g, modularCustomizationManager.ActiveEyeColor.b, modularCustomizationManager.ActiveEyeColor.a)); //modularCustomizationManager.ActiveEyeColor.ToString());
                properties.Add("custom:MouthColor", String.Format("{0},{1},{2},{3}", modularCustomizationManager.ActiveMouthColor.r, modularCustomizationManager.ActiveMouthColor.g, modularCustomizationManager.ActiveMouthColor.b, modularCustomizationManager.ActiveMouthColor.a)); //modularCustomizationManager.ActiveEyeColor.ToString());

                if (modularCustomizationManager.ActiveHelmetColor != null)
                {
                    properties.Add("custom:HelmetColor", modularCustomizationManager.ActiveHelmetColor);
                }
                if (modularCustomizationManager.ActiveHeadColor != null)
                {
                    properties.Add("custom:HeadColor", modularCustomizationManager.ActiveHeadColor);
                }

                if (modularCustomizationManager.ActiveTorsoColor != null)
                {
                    properties.Add("custom:TorsoColor", modularCustomizationManager.ActiveTorsoColor);
                }
                if (modularCustomizationManager.ActiveUpperArmsColor != null)
                {
                    properties.Add("custom:UpperArmsColor", modularCustomizationManager.ActiveUpperArmsColor);
                }
                if (modularCustomizationManager.ActiveLowerArmsColor != null)
                {
                    properties.Add("custom:LowerArmsColor", modularCustomizationManager.ActiveLowerArmsColor);
                }
                if (modularCustomizationManager.ActiveHipsColor != null)
                {
                    properties.Add("custom:HipsColor", modularCustomizationManager.ActiveHipsColor);
                }
                if (modularCustomizationManager.ActiveLowerLegsColor != null)
                {
                    properties.Add("custom:LowerLegsColor", modularCustomizationManager.ActiveLowerLegsColor);
                }
                if (modularCustomizationManager.ActiveHandsColor != null)
                {
                    properties.Add("custom:HandsColor", modularCustomizationManager.ActiveHandsColor);
                }

                if (modularCustomizationManager.ActiveHands.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveHands)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:HandsModel", data);
                }

                if (modularCustomizationManager.ActiveLowerArms.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveLowerArms)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:LowerArmsModel", data);
                    //   properties.Add("custom:LowerArmsModel", modularCustomizationManager.ActiveLowerArms.name);
                }

                if (modularCustomizationManager.ActiveUpperArms.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveUpperArms)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:UpperArmsModel", data);
                    //  properties.Add("custom:UpperArmsModel", modularCustomizationManager.ActiveUpperArms.name);
                }

                if (modularCustomizationManager.ActiveTorso.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveTorso)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:TorsoModel", data);
                    // properties.Add("custom:TorsoModel", modularCustomizationManager.ActiveTorso.name);
                }

                if (modularCustomizationManager.ActiveHips.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveHips)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:HipsModel", data);
                    //  properties.Add("custom:HipsModel", modularCustomizationManager.ActiveHips.name);
                }

                if (modularCustomizationManager.ActiveLowerLegs.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveLowerLegs)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:LowerLegsModel", data);
                    //   properties.Add("custom:LowerLegsModel", modularCustomizationManager.ActiveLowerLegs.name);
                }

                if (modularCustomizationManager.ActiveFeet.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveFeet)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:FeetModel", data);
                    //   properties.Add("custom:FeetModel", modularCustomizationManager.ActiveFeet.name);
                }

                if (modularCustomizationManager.ActiveHead.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveHead)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:HeadModel", data);
                    //   properties.Add("custom:HeadModel", modularCustomizationManager.ActiveHead.name);
                }

                if (modularCustomizationManager.ActiveBeard)
                {
                    properties.Add("custom:BeardModel", modularCustomizationManager.ActiveBeardId);
                }

                if (modularCustomizationManager.ActiveEyebrow)
                {
                    properties.Add("custom:EyebrowModel", modularCustomizationManager.ActiveEyebrowId);
                }

                if (modularCustomizationManager.ActiveMouth)
                {
                    properties.Add("custom:MouthModel", modularCustomizationManager.ActiveMouthId);
                }

                if (modularCustomizationManager.ActiveEye)
                {
                    properties.Add("custom:EyeModel", modularCustomizationManager.ActiveEyeId);
                }

                if (modularCustomizationManager.ActiveTusk.Count > 0)
                {
                    string data = "";
                    foreach (var go in modularCustomizationManager.ActiveTusk)
                    {
                        data += go.name + "|";
                    }

                    if (data.Length > 0)
                        data = data.Remove(data.Length - 1);
                    properties.Add("custom:TuskModel", data);
                    properties.Add("custom:TuskModel", modularCustomizationManager.ActiveTuskId);
                }

                if (modularCustomizationManager.ActiveEar)
                {
                    properties.Add("custom:EarModel", modularCustomizationManager.ActiveEarId);
                }

                if (modularCustomizationManager.ActiveFaith != null)
                {
                    properties.Add("custom:FaithValue", modularCustomizationManager.ActiveFaith);
                }
#if IPBRInt
                if (modularCustomizationManager.ActiveBlendshapePreset != -1)
                {
                    properties.Add("custom:BlendshapePresetValue", modularCustomizationManager.ActiveBlendshapePreset);
                }

                if (modularCustomizationManager.ActiveBlendshapes != null)
                {
                    properties.Add("custom:BlendshapesValue", modularCustomizationManager.ActiveBlendshapes);
                }
#endif
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
            if (errorMessage == "")
            {
                StartCharacterSelection();
                //nameUI.text = characterName;
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

            if (serverNameText != null)
            {
                serverNameText.text = AtavismClient.Instance.WorldId;
            }
            if (TMPServerNameText != null)
            {
                TMPServerNameText.text = AtavismClient.Instance.WorldId;
            }
        }

        void ShowCreationUI()
        {
            m_ScreenSelection.style.display = DisplayStyle.None;
            m_ScreenCustomize.style.display = DisplayStyle.Flex;
        }

        protected void ShowDialog(string message, bool showButton)
        {
            if (dialogWindow == null)
                return;
            dialogWindow.gameObject.SetActive(true);
            dialogWindow.ShowDialogPopup(message, showButton);
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

            // EEE - Model jumps to this rotation when clicked on, so may as well make it the default start position
            x -= Input.GetAxis("Mouse X") * characterRotationSpeed * 0.02f;
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            character = (GameObject) Instantiate(prefab, spawnPosition.position, rotation);
            x = 180;

            // EEE - Here was original code set min/max values for sliders (again) and initialized colors. Removed.

            // set default colours for each colour field in UI
            m_allColorFields.ForEach(SetDefaultColor); 
            // set max for each slider based on part
            m_allSliders.ForEach(SetSliderMax);

            // reset UI
            m_labelCharacterName.text = "";
            m_fieldCharacterName.value = "";
            m_IconTabsView.ResetTab();
        }

        public void ShowAvatarList()
        {
            avatarList.gameObject.SetActive(true);
            createPanelRace.SetActive(false);

        }
        public void AvatarSelected()
        {
            avatarList.gameObject.SetActive(false);
            createPanelRace.SetActive(true);
            if (avatarIcon != null)
                avatarIcon.sprite = avatarList.icons[avatarList.Selected()];
        }

        public void CloseAvatarList()
        {
            if (avatarList != null)
                avatarList.gameObject.SetActive(false);
            if (createPanelRace != null)
                createPanelRace.SetActive(true);
        }

        #endregion Character Creation
  
        #region ModularAPI
        private BodyType ReturnBodyType(AtavismColorPicker atavismColorPicker) 
        {
            BodyType bodyType = BodyType.None;
            switch (atavismColorPicker.currentBodyPart)
            {
                case 0:
                    bodyType = BodyType.Hips;
                    break;
                case 1:
                    bodyType = BodyType.Torso;
                    break;
                case 2:
                    bodyType = BodyType.Hands;
                    break;
                case 3:
                    bodyType = BodyType.LowerLegs;
                    break;
                case 4:
                    bodyType = BodyType.Upperarms;
                    break;
                case 5:
                    bodyType = BodyType.LowerArms;
                    break;
                case 6:
                    bodyType = BodyType.Feet;
                    break;
            }
            return bodyType;
        }

#endregion ModularAPI

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

        // EEE Initializes the Modular Character Customization components
        // including the gender buttons, colours sliders, and custom sliders
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

            // UI for Character Creation
            m_ScreenCustomize = root.Q<VisualElement>("ScreenCustomize"); 
            m_SectionContentContainer = root.Q<VisualElement>("content-container"); 

            // Use UQueryBuilder to loop through all children of a specific type: https://docs.unity3d.com/Manual/UIE-UQuery.html
            // Simple Example: https://gist.github.com/polerin/d93feca1f69534e1e64374324bbaf3cc

            // Initialize Tabs
            m_IconTabsView = root.Q<IconTabsView>("IconTabsView"); // needed to call a reset of tabs
            UQueryBuilder<IconTab> allTabs = root.Query<VisualElement>("tabs-container").Children<IconTab>();
            allTabs.ForEach(e => e.OnSelect += UITabClicked); // for each tab, assign function for click

            // Initialize Gender Buttons
            m_gendersContainer = root.Q<IconRadioGroup>("gendersContainer"); // m_gendersContainer needed
            UQueryBuilder<IconRadio> allGenders = m_gendersContainer.Query<IconRadio>();
            allGenders.ForEach(e => e.OnSelect += UIGenderClicked); // for each gender, assign function for click

            // Initiate sliders in sections (Head and Body)
            m_allSliders = m_SectionContentContainer.Query<ModCharSlider>();
            m_allSliders.ForEach(e => e.RegisterCallback<ChangeEvent<int>,ModularPart>(UIModularCharacterSetSlider, e.modularPart));

            // these UI controls are named to turn them off and on
            m_SliderBeard = root.Q<ModCharSlider>("sliderBeard"); 
            m_ScarColor = root.Q<ColorField>("scar-color");
            m_TattooColor = root.Q<ColorField>("tattoo-color");

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

            m_fieldCharacterName.RegisterCallback<KeyDownEvent>(OnKeyPress, TrickleDown.TrickleDown);

        }

        void OnKeyPress(KeyDownEvent ev)
        {
//            Debug.Log($"Pressed key: {ev.keyCode}, character: {ev.character}, modifiers: {ev.modifiers}");
            if ( Char.IsSymbol(ev.character) || Char.IsNumber(ev.character) || Char.IsPunctuation(ev.character)) 
            {
                ev.PreventDefault(); // Restrict certain characters from being entered. 
            }
        }

        public void UIModularCharacterSetColor(Color color) // EEE - Set colour of parts
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm != null)
            {
                switch (m_ColorPopup.CurrentPart)
                {
                    case BodyPartColor.SkinColor: 
                        // EEE - combined with stubble. never matches with beard
                        mcm.SkinColorSet(color);
                        mcm.StubbleColorSet(color); 
                        break;
                    case BodyPartColor.StubbleColor:
                        mcm.StubbleColorSet(color);
                        break;
                    case BodyPartColor.HairColor:
                        mcm.HairColorSet(color);
                        mcm.BeardColorSet(color);
                        mcm.EyebrowColorSet(color);
                        break;
                    case BodyPartColor.EyeColor:
                        mcm.EyeColorSet(color);
                        break;
                    case BodyPartColor.ScarColor:
                        mcm.ScarColorSet(color);
                        break;
                    case BodyPartColor.TattooColor: 
                        mcm.BodyArtColorSet(color);
                        break;
                    case BodyPartColor.Primary:
                        mcm.PrimaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.Secondary:
                        mcm.SecondaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.Tertiary: // Doesn't have tertiary, I think
                        mcm.TertiaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.MetalPrimary:
                        mcm.MetalPrimaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.MetalSecondary:
                        mcm.MetalSecondaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.MetalDark:
                        mcm.MetalDarkColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.LeatherPrimary: // Didn't work, fixed case error in modularCustomizationManager
                        mcm.LeatherPrimaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.LeatherSecondary:
                        mcm.LeatherSecondaryColorSet(color, BodyType.None);
                        break;
                    case BodyPartColor.LeatherTertiary:
                        mcm.LeatherTertiaryColorSet(color, BodyType.None);
                        break;
                }
            }
        }

        // EEE - The event set to a slider when changed.
        // Reference: https://docs.unity.cn/Manual/UIE-Events-Handling.html
        public void UIModularCharacterSetSlider(ChangeEvent<int> e, ModularPart modPart)
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm != null)
            {
                switch (modPart)
                {
                    case ModularPart.Hair:
                        mcm.SwitchHairForward(e.newValue);
                        break;
                    case ModularPart.Beard:
                        mcm.SwitchBeardForward(e.newValue);
                        break;
                    case ModularPart.Head:
                        // Note - Female Head 23 included but mesh doesn't exist from HNG. Had to remove
                        mcm.SwitchHead(e.newValue); 
                        if (e.newValue > 0 && e.newValue < 8) // Turn off Scar and Tattoo color fields appropriately
                        {
                            m_ScarColor.style.display = DisplayStyle.Flex;
                            m_TattooColor.style.display = DisplayStyle.None;
                        }
                        else if (e.newValue > 8) 
                        {
                            m_ScarColor.style.display = DisplayStyle.None;
                            m_TattooColor.style.display = DisplayStyle.Flex;
                        }
                        else 
                        {
                            m_ScarColor.style.display = DisplayStyle.None;
                            m_TattooColor.style.display = DisplayStyle.None;
                        }
                        break;
                    case ModularPart.Eyebrow:
                        mcm.SwitchEyebrowForward(e.newValue); // ModularCustomizationManager doesn't activate. Edited code
                        break;                  
                    case ModularPart.Ears:
                        mcm.SwitchEars(e.newValue); // ModularCustomizationManager made new function
                        break;
                    case ModularPart.Torso:
                        mcm.SwitchTorso(e.newValue);
                        break;
                    case ModularPart.UpperArms:
                        mcm.SwitchUpperArm(e.newValue); // Remember to make all sub models active.
                        break;
                    case ModularPart.LowerArms:
                        mcm.SwitchLowerArm(e.newValue);
                        break;
                    case ModularPart.Hands:
                        mcm.SwitchHands(e.newValue);
                        break;
                    case ModularPart.Hips:
                        mcm.SwitchHips(e.newValue);
                        break;
                    case ModularPart.LowerLegs:
                        mcm.SwitchLowerLegs(e.newValue);
                        break;
                }
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
                    case BodyPartColor.SkinColor: 
                        mcm.SkinColorSet(mcm.defaultSkinColor);
                        colorField.SetValueWithoutNotify(mcm.defaultSkinColor);                        
                        break;
                    case BodyPartColor.StubbleColor:
                        mcm.StubbleColorSet(mcm.defaultSkinColor);
                        colorField.SetValueWithoutNotify(mcm.defaultSkinColor);                             
                        break;
                    case BodyPartColor.HairColor:
                        mcm.HairColorSet(mcm.defaultHairColor);
                        mcm.BeardColorSet(mcm.defaultHairColor);
                        mcm.EyebrowColorSet(mcm.defaultHairColor);
                        colorField.SetValueWithoutNotify(mcm.defaultHairColor);
                        break;
                    case BodyPartColor.EyeColor:
                        mcm.EyeColorSet(mcm.defaultEyeColor);
                        colorField.SetValueWithoutNotify(mcm.defaultEyeColor);
                        break;
                    case BodyPartColor.ScarColor:
                        mcm.ScarColorSet(mcm.defaultScarColor);
                        colorField.SetValueWithoutNotify(mcm.defaultScarColor);
                        break;
                    case BodyPartColor.TattooColor: 
                        mcm.BodyArtColorSet(mcm.defaultBodyArtColor);
                        colorField.SetValueWithoutNotify(mcm.defaultBodyArtColor);
                        break;
                    case BodyPartColor.Primary:
                        mcm.PrimaryColorSet(mcm.defaultPrimaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultPrimaryColor);
                        break;
                    case BodyPartColor.Secondary:
                        mcm.SecondaryColorSet(mcm.defaultSecondaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultSecondaryColor);
                        break;
                    case BodyPartColor.Tertiary: // Doesn't have tertiary, I think
                        mcm.TertiaryColorSet(mcm.defaultTertiaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultTertiaryColor);
                        break;
                    case BodyPartColor.MetalPrimary:
                        mcm.MetalPrimaryColorSet(mcm.defaultMetalPrimaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalPrimaryColor);
                        break;
                    case BodyPartColor.MetalSecondary:
                        mcm.MetalSecondaryColorSet(mcm.defaultMetalSecondaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalSecondaryColor);
                        break;
                    case BodyPartColor.MetalDark:
                        mcm.MetalDarkColorSet(mcm.defaultMetalDarkColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultMetalDarkColor);
                        break;
                    case BodyPartColor.LeatherPrimary: // not working for some reason
                        mcm.LeatherPrimaryColorSet(mcm.defaultLeatherPrimaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultLeatherPrimaryColor);
                        break;
                    case BodyPartColor.LeatherSecondary:
                        mcm.LeatherSecondaryColorSet(mcm.defaultLeatherSecondaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultLeatherSecondaryColor);
                        break;
                    case BodyPartColor.LeatherTertiary:
                        mcm.LeatherTertiaryColorSet(mcm.defaultLeatherTertiaryColor, BodyType.None);
                        colorField.SetValueWithoutNotify(mcm.defaultLeatherTertiaryColor);
                        break;
                }
            }
        }
        // EEE Initializes the high value for each given slider
        public virtual void SetSliderMax(ModCharSlider slider)
        {
            ModularCustomizationManager mcm = character.GetComponent<ModularCustomizationManager>();
            if (mcm != null)
            {
                slider.lowValue = 0;
                slider.value = 0;
                switch (slider.modularPart)
                {
                    case ModularPart.Hair:
                        slider.highValue = mcm.hairModels.Count > 0 ? mcm.hairModels.Count - 1 : 0;
                        break;
                    case ModularPart.Beard:
                        slider.highValue = mcm.beardModels.Count > 0 ? mcm.beardModels.Count - 1 : 0;
                        break;
                    case ModularPart.Head:
                        slider.highValue = mcm.headModels.Count > 0 ? mcm.headModels.Count - 1 : 0;
                        break;
                    case ModularPart.Eyebrow:
                        slider.highValue = mcm.eyebrowModels.Count > 0 ? mcm.eyebrowModels.Count - 1 : 0;
                        break;         
                    case ModularPart.Ears:
                        slider.highValue = mcm.earModels.Count > 0 ? mcm.earModels.Count - 1 : 0;
                        break;
                    case ModularPart.Torso:
                        slider.highValue = mcm.torsoModels.Count > 0 ? mcm.torsoModels.Count - 1 : 0;
                        break;
                    case ModularPart.UpperArms:
                        slider.highValue = mcm.upperArmModels.Count > 0 ? mcm.upperArmModels.Count - 1 : 0;
                        break;
                    case ModularPart.LowerArms:
                        slider.highValue = mcm.lowerArmModels.Count > 0 ? mcm.lowerArmModels.Count - 1 : 0;
                        break;
                    case ModularPart.Hands:
                        slider.highValue = mcm.handModels.Count > 0 ? mcm.handModels.Count - 1 : 0;
                        break;
                    case ModularPart.Hips:
                        slider.highValue = mcm.hipModels.Count > 0 ? mcm.hipModels.Count - 1 : 0;
                        break;
                    case ModularPart.LowerLegs:
                        slider.highValue = mcm.lowerLegModels.Count > 0 ? mcm.lowerLegModels.Count - 1 : 0;
                        break;
                }                
            }
        }
        public void RegisterModularCharacterSlider(ModularCharacterSlider modularCharacterSlider)
        {           
            // REMOVED
            Debug.LogError("Function has been removed.");
        }

        public void RegisterModularCharacterPicker(ModularCharacterColor modularCharacterColor)
        {
            // REMOVED
            Debug.LogError("Function has been removed.");
        }

        public void ModularCharacterSetColor(string dna, Color color)
        {
            // REMOVED
            Debug.LogError("Function has been removed.");
        }
    }
}