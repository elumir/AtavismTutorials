using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using HNGamers.Atavism;
using static HNGamers.Atavism.ModularCustomizationManager;


namespace Atavism
{

    [Serializable]
    public class AtavismGender
    {
        public string name;
        public Button genderButton;
        public Text genderText;
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

#if HSVPicker
        public ColorPicker bodyartpicker;
        public ColorPicker skinpicker;
        public ColorPicker eyepicker;
        public ColorPicker hairpicker;
        public ColorPicker beardpicker;
        public ColorPicker eyebrowpicker;
        public ColorPicker stubblepicker;
        public ColorPicker scarpicker;
        public ColorPicker mouthpicker;

        public ColorPicker primarypicker;
        public ColorPicker secondarypicker;
        public ColorPicker metalprimarypicker;
        public ColorPicker metalsecondarypicker;
        public ColorPicker metaldarkpicker;
        public ColorPicker leatherprimarypicker;
        public ColorPicker leathersecondarypicker;
        public ColorPicker leathertertiarypicker;
        public ColorPicker tertiarypicker;
        public AtavismColorPicker atavismColorPicker;
#endif
        [Tooltip("")]
        [SerializeField] public bool allowDifferentHairColors;
        public Button scarbutton;
        public Button tattoobutton;

        
        
      //  public Texture cursorOverride;

        public List<GameObject> createUI;
        public List<GameObject> selectUI;
        public List<UGUICharacterSelectSlot> characterSlots;
        public GameObject enterUI;
        public Button createButton;
        public Text nameUI;
        public TextMeshProUGUI TMPNameUI;
        public Button deleteButton;
        public Text serverNameText;
        public TextMeshProUGUI TMPServerNameText;
        public UGUIServerList serverListUI;
        public GameObject characterCamera;
        public UGUIDialogPopup dialogWindow;
        public Transform spawnPosition;
        public InputField createCaracterName;
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
        public Text raceTitle;
        public TextMeshProUGUI TMPRaceTitle;
        public Text raceDescription;
        public TextMeshProUGUI TMPRaceDescription;

        public Image classIcon;
        public Text classTitle;
        public TextMeshProUGUI TMPClassTitle;
        public Text classDescription;
        public TextMeshProUGUI TMPClassDescription;
        public Color defaultButomTextColor = Color.white;
        public Color selectedButomTextColor = Color.green;
        public GameObject createPanelRace;
        public UGUIAvatarList avatarList;
        public Image avatarIcon;

        // Camera fields
        public Vector3 cameraInLoc = new Vector3(0.193684f, 2.4f, 4.743689f);
        public Vector3 cameraOutLoc = new Vector3(0.4418643f, 1.21f, 6.72f);
        protected bool zoomingIn = false;
        protected bool zoomingOut = false;
        public float characterRotationSpeed = 250.0f;
        protected float x = 180;
        protected float y = 0;

        
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
#if HSVPicker
            if (bodyartpicker)
            {
                bodyartpicker.onValueChanged.AddListener(color =>
            {
                SwitchBodyArtColortoColor(color);
            });
            }

            if (eyepicker)
            {
                eyepicker.onValueChanged.AddListener(color =>
            {
                SwitchEyeColortoColor(color);
            });
            }



            if (hairpicker)
            {
                hairpicker.onValueChanged.AddListener(color =>
            {
                if (allowDifferentHairColors)
                {
                    SwitchHairColortoColor(color);
                }
                else
                {
                    SwitchHairColortoColor(color);
                    SwitchBeardColortoColor(color);
                    SwitchEyebrowColortoColor(color);
                }
            });
            }
            if (stubblepicker)
            {
                stubblepicker.onValueChanged.AddListener(color =>
            {
                SwitchStubbleColortoColor(color);
            });
            }
            if (scarpicker)
            {
                scarpicker.onValueChanged.AddListener(color =>
            {
                SwitchSkinScarColortoColor(color);
            });
            }



            if (mouthpicker)
            {
                mouthpicker.onValueChanged.AddListener(color =>
                {
                    SwitchMouthColortoColor(color);
                });
            }

            if (primarypicker && atavismColorPicker)
            {
                primarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchPrimaryColortoColor(color, atavismColorPicker);
                });
            }

            if (secondarypicker && atavismColorPicker)
            {
                secondarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchSecondaryColortoColor(color, atavismColorPicker);
                });
            }

            if (metalprimarypicker && atavismColorPicker)
            {
                metalprimarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchMetalPrimaryColortoColor(color, atavismColorPicker);
                });
            }

            if (metalsecondarypicker && atavismColorPicker)
            {
                metalsecondarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchMetalSecondaryColortoColor(color, atavismColorPicker);
                });
            }

            if (metaldarkpicker && atavismColorPicker)
            {
                metaldarkpicker.onValueChanged.AddListener(color =>
                {
                    SwitchMetalDarkColortoColor(color, atavismColorPicker);
                });
            }

            if (leatherprimarypicker && atavismColorPicker)
            {
                leatherprimarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchLeatherPrimaryColortoColor(color, atavismColorPicker);
                });
            }

            if (leathertertiarypicker && atavismColorPicker)
            {
                leathertertiarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchLeatherTertiaryColortoColor(color, atavismColorPicker);
                });
            }

            if (tertiarypicker && atavismColorPicker)
            {
                tertiarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchTertiaryColortoColor(color, atavismColorPicker);
                });
            }

            if (leathersecondarypicker && atavismColorPicker)
            {
                leathersecondarypicker.onValueChanged.AddListener(color =>
                {
                    SwitchLeatherSecondaryColortoColor(color, atavismColorPicker);
                });
            }

            if (allowDifferentHairColors)
            {
                if (beardpicker)
                {
                    beardpicker.onValueChanged.AddListener(color =>
                {
                    SwitchBeardColortoColor(color);
                });
                }
                if (eyebrowpicker)
                {
                    eyebrowpicker.onValueChanged.AddListener(color =>
                {
                    SwitchEyebrowColortoColor(color);
                });
                }
            }

            if (skinpicker)
            {
                skinpicker.onValueChanged.AddListener(color =>
                {
                    SwitchSkinColortoColor(color);
                });
            }
#endif

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
            float moveRate = 1.0f;
            if (zoomingIn)
            {
                characterCamera.transform.position = Vector3.Lerp(characterCamera.transform.position, character.GetComponent<AtavismMobAppearance>().GetSocketTransform("Head").position + cameraInLoc, Time.deltaTime * moveRate);
            }
            else if (zoomingOut)
            {
                characterCamera.transform.position = Vector3.Lerp(characterCamera.transform.position, character.transform.position + cameraOutLoc, Time.deltaTime * moveRate);
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
        }
        
        
        public virtual void ModularCustomizationManagerCheck()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            /*
            SwitchBodyArtColortoColor(modularCustomizationManager.defaultBodyArtColor);
            SwitchEyeColortoColor(modularCustomizationManager.defaultEyeColor);
            if (allowDifferentHairColors)
            {
                SwitchHairColortoColor(modularCustomizationManager.defaultHairColor);
            }
            else
            {
                SwitchHairColortoColor(modularCustomizationManager.defaultHairColor);
                SwitchBeardColortoColor(modularCustomizationManager.defaultHairColor);
                SwitchEyebrowColortoColor(modularCustomizationManager.defaultHairColor);
            }
            SwitchStubbleColortoColor(modularCustomizationManager.defaultStubbleColor);
            SwitchSkinScarColortoColor(modularCustomizationManager.defaultScarColor);
            SwitchMouthColortoColor(modularCustomizationManager.defaultMouthColor);
            SwitchSkinColortoColor(modularCustomizationManager.defaultSkinColor);*/


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
                if (nameUI != null)
                nameUI.text = (string)entry["characterName"];
            if (TMPNameUI != null)
                TMPNameUI.text = (string)entry["characterName"];

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
            dialogWindow.gameObject.SetActive(true);
#if AT_I2LOC_PRESET
        dialogWindow.ShowDialogOptionPopup(I2.Loc.LocalizationManager.GetTranslation("Do you want to delete character") + ": " + characterSelected["characterName"]);
#else
            dialogWindow.ShowDialogOptionPopup("Do you want to delete character: " + characterSelected["characterName"]);
#endif
        }

        public virtual void DeleteCharacterConfirmed()
        {
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
            if (createCaracterName != null)
                createCaracterName.text = "";
            if (TMPCreateCaracterName != null)
                TMPCreateCaracterName.text = "";
            if (enterUI != null)
                enterUI.SetActive(false);

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
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            if (modularCustomizationManager)
            {
                foreach (var modularCharacterSlider in modularCharacterSliderList)
                {
                    if (modularCharacterSlider != null)
                    {
                        if (modularCharacterSlider.slider)
                        {
                            switch (modularCharacterSlider.dna)
                            {
                                case "Hair":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.hairModels.Count > 0 ? modularCustomizationManager.hairModels.Count - 1 : 0;
                                    break;
                                case "Beard":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.beardModels.Count > 0 ? modularCustomizationManager.beardModels.Count - 1 : 0;
                                    break;
                                case "Face":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.faceModels.Count > 0 ?  modularCustomizationManager.faceModels.Count - 1 : 0;
                                    break;
                                case "Eyebrow":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.eyebrowModels.Count > 0 ? modularCustomizationManager.eyebrowModels.Count - 1 : 0;
                                    break;
                                /* case "Scar":
                                     modularCharacterSlider.slider.onValueChanged.AddListener(id => { Switch((int) id); });
                                     break;*/
                                case "Hands":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.handModels.Count > 0 ? modularCustomizationManager.handModels.Count - 1 : 0;
                                    break;
                                case "LowerArms":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.lowerArmModels.Count > 0 ? modularCustomizationManager.lowerArmModels.Count - 1 : 0;
                                    break;
                                case "UpperArms":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.upperArmModels.Count > 0 ? modularCustomizationManager.upperArmModels.Count - 1 : 0;
                                    break;
                                case "Torso":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.torsoModels.Count > 0 ?  modularCustomizationManager.torsoModels.Count - 1 : 0;
                                    break;
                                case "Hips":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.hipModels.Count > 0 ?  modularCustomizationManager.hipModels.Count - 1 : 0;
                                    break;
                                case "LowerLegs":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.lowerLegModels.Count > 0 ?  modularCustomizationManager.lowerLegModels.Count - 1 : 0;
                                    break;
                                case "Eyes":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.eyeModels.Count > 0 ?  modularCustomizationManager.eyeModels.Count - 1 : 0;
                                    break;
                                case "Mouth":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.mouthModels.Count > 0 ? modularCustomizationManager.mouthModels.Count - 1 : 0;
                                    break;
                                case "Head":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.headModels.Count > 0 ?  modularCustomizationManager.headModels.Count - 1 : 0;
                                    break;
                            }

                        }

                    }
                }

            }
#if HSVPicker
            if (modularCustomizationManager)
            {
                if (bodyartpicker)
                {
                    bodyartpicker.CurrentColor = modularCustomizationManager.defaultBodyArtColor;
                }

                if (eyepicker)
                {
                    eyepicker.CurrentColor = modularCustomizationManager.defaultEyeColor;
                }

                if (hairpicker)
                {
                    hairpicker.CurrentColor = modularCustomizationManager.defaultHairColor;
                }

                if (beardpicker)
                {
                    beardpicker.CurrentColor = modularCustomizationManager.defaultHairColor;
                }

                if (eyebrowpicker)
                {
                    eyebrowpicker.CurrentColor = modularCustomizationManager.defaultHairColor;
                }

                if (stubblepicker)
                {
                    stubblepicker.CurrentColor = modularCustomizationManager.defaultHairColor;
                }

                if (scarpicker)
                {
                    scarpicker.CurrentColor = modularCustomizationManager.defaultScarColor;
                }

                if (mouthpicker)
                {
                    mouthpicker.CurrentColor = modularCustomizationManager.defaultMouthColor;
                }

                if (primarypicker)
                {
                    primarypicker.CurrentColor = modularCustomizationManager.defaultPrimaryColor;
                }

                if (secondarypicker)
                {
                    secondarypicker.CurrentColor = modularCustomizationManager.defaultSecondaryColor;
                }

                if (metalprimarypicker)
                {
                    metalprimarypicker.CurrentColor = modularCustomizationManager.defaultMetalPrimaryColor;
                }

                if (metalsecondarypicker)
                {
                    metalsecondarypicker.CurrentColor = modularCustomizationManager.defaultMetalSecondaryColor;
                }

                if (metaldarkpicker)
                {
                    metaldarkpicker.CurrentColor = modularCustomizationManager.defaultMetalDarkColor;
                }

                if (leatherprimarypicker)
                {
                    leatherprimarypicker.CurrentColor = modularCustomizationManager.defaultLeatherPrimaryColor;
                }

                if (leathersecondarypicker)
                {
                    leathersecondarypicker.CurrentColor = modularCustomizationManager.defaultLeatherSecondaryColor;
                }

                if (leathertertiarypicker)
                {
                    leathertertiarypicker.CurrentColor = modularCustomizationManager.defaultLeatherTertiaryColor;
                }

                if (tertiarypicker)
                {
                    tertiarypicker.CurrentColor = modularCustomizationManager.defaultTertiaryColor;
                }

                if (skinpicker)
                {
                    skinpicker.CurrentColor = modularCustomizationManager.defaultSkinColor;
                }
            }
#endif

        }

        public void ZoomCameraIn()
        {
            zoomingIn = true;
            zoomingOut = false;
        }

        public void ZoomCameraOut()
        {
            zoomingOut = true;
            zoomingIn = false;
        }

        public void ToggleAnim()
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
              
              
         /*   foreach (var cl in AtavismPrefabManager.Instance.GetRaceData()[raceId].classList.Values)
            {
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
            }*/
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

         /*   foreach (var gen in AtavismPrefabManager.Instance.GetRaceData()[raceId].classList[aspectId].genderList.Values)
            {
              //  Debug.LogError(" Gender " + gen.id + " " + gen.name);
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
            }*/
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
         //   Debug.LogError("Race="+raceId+" Class="+aspectId+" Gender="+genderId);

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
        /// Sends the Create Character message to the server with a collection of properties
        /// to save to the new character.
        /// </summary>
        public virtual void CreateCharacter()
        {
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
            ZoomCameraOut();
        }

        void ShowSelectionUI()
        {
            loginState = LoginState.CharacterSelect;
            foreach (GameObject ui in selectUI)
            {
                ui.SetActive(true);
            }
            foreach (GameObject ui in createUI)
            {
                ui.SetActive(false);
            }
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
            foreach (GameObject ui in selectUI)
            {
                ui.SetActive(false);
            }
            foreach (GameObject ui in createUI)
            {
                ui.SetActive(true);
            }

            CloseAvatarList();
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
            character = (GameObject) Instantiate(prefab, spawnPosition.position, spawnPosition.rotation);
            x = 180;
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            if (modularCustomizationManager)
            {
                foreach (var modularCharacterSlider in modularCharacterSliderList)
                {
                    if (modularCharacterSlider != null)
                    {
                        if (modularCharacterSlider.slider)
                        {
                            switch (modularCharacterSlider.dna)
                            {
                                case "Hair":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.hairModels.Count > 0 ? modularCustomizationManager.hairModels.Count - 1 : 0;
                                    break;
                                case "Beard":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.beardModels.Count > 0 ? modularCustomizationManager.beardModels.Count - 1 : 0;
                                    break;
                                case "Face":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.faceModels.Count > 0 ?  modularCustomizationManager.faceModels.Count - 1 : 0;
                                    break;
                                case "Eyebrow":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.eyebrowModels.Count > 0 ? modularCustomizationManager.eyebrowModels.Count - 1 : 0;
                                    break;
                                /* case "Scar":
                                     modularCharacterSlider.slider.onValueChanged.AddListener(id => { Switch((int) id); });
                                     break;*/
                                case "Hands":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.handModels.Count > 0 ? modularCustomizationManager.handModels.Count - 1 : 0;
                                    break;
                                case "LowerArms":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.lowerArmModels.Count > 0 ? modularCustomizationManager.lowerArmModels.Count - 1 : 0;
                                    break;
                                case "UpperArms":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.upperArmModels.Count > 0 ? modularCustomizationManager.upperArmModels.Count - 1 : 0;
                                    break;
                                case "Torso":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.torsoModels.Count > 0 ?  modularCustomizationManager.torsoModels.Count - 1 : 0;
                                    break;
                                case "Hips":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.hipModels.Count > 0 ?  modularCustomizationManager.hipModels.Count - 1 : 0;
                                    break;
                                case "LowerLegs":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.lowerLegModels.Count > 0 ?  modularCustomizationManager.lowerLegModels.Count - 1 : 0;
                                    break;
                                case "Eyes":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.eyeModels.Count > 0 ?  modularCustomizationManager.eyeModels.Count - 1 : 0;
                                    break;
                                case "Mouth":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue =  modularCustomizationManager.mouthModels.Count > 0 ? modularCustomizationManager.mouthModels.Count - 1 : 0;
                                    break;
                                case "Head":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.headModels.Count > 0 ?  modularCustomizationManager.headModels.Count - 1 : 0;
                                    break;
                                case "Feet":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.feetModels.Count > 0 ?  modularCustomizationManager.feetModels.Count - 1 : 0;
                                    break;
                                case "Helmet":
                                    modularCharacterSlider.slider.minValue = 0;
                                    modularCharacterSlider.slider.maxValue = modularCustomizationManager.helmetModels.Count > 0 ? modularCustomizationManager.helmetModels.Count - 1 : 0;
                                    break;
                            }
                        }
                    }
                }

                foreach (var modularCharacterColor in modularCharacterColorPanelList)
                {
                    if (modularCharacterColor != null)
                    {
                       // Debug.LogError(modularCharacterColor.dna+" ");
                        switch (modularCharacterColor.dna)
                        {
                            SwitchHairColortoColor(modularCustomizationManager.defaultHairColor); // EEE - Apply Default to model
                            case "HairColor":
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.hairColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "StubbleColor":
                                SwitchStubbleColortoColor(modularCustomizationManager.defaultStubbleColor); // EEE - Apply Default to model
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.stubbleColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.stubbleColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "SkinColor":
                                SwitchSkinColortoColor(modularCustomizationManager.defaultSkinColor); // EEE - Apply Default to model
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.skinColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.skinColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "EyeColor":
                                SwitchEyeColortoColor(modularCustomizationManager.defaultEyeColor); // EEE - Apply Default to model                           
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.eyeColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.eyeColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "ScarColor":
                                SwitchSkinScarColortoColor(modularCustomizationManager.defaultScarColor);  // EEE - Apply Default to model
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.scarColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.scarColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "BodyArtColor":
                                SwitchBodyArtColortoColor(modularCustomizationManager.defaultBodyArtColor); // EEE - Apply Default to model
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.bodyArtColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.bodyArtColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "BeardColor":
                                SwitchBeardColortoColor(modularCustomizationManager.defaultHairColor); // EEE - Apply Default to model
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.hairColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "EyebrowColor":
                                SwitchEyebrowColortoColor(modularCustomizationManager.defaultHairColor); // EEE - Apply Default to model                       
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.hairColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                            case "MouthColor":
                                SwitchMouthColortoColor(modularCustomizationManager.defaultMouthColor); // EEE - Apply Default to model
                                for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                {
                                    if (modularCustomizationManager.hairColors.Length > i)
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(true);
                                        modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                    }
                                    else
                                    {
                                        modularCharacterColor.colors[i].gameObject.SetActive(false);
                                    }
                                }

                                break;
                       }
                    }
                }
            }

        }

        public void DialogYesClicked()
        {
            DeleteCharacterConfirmed();
            dialogWindow.gameObject.SetActive(false);
        }

        public void DialogNoClicked()
        {
            dialogWindow.gameObject.SetActive(false);
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

        // Temp:
        public void SwitchHair()
        {
            if (character.GetComponent<CustomisedHair>() != null)
            {
                character.GetComponent<CustomisedHair>().SwitchHairForward();
            }
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchHairForward();

        }
        
        public void SwitchHair(int id)
        {
            if (character.GetComponent<CustomisedHair>() != null)
            {
                character.GetComponent<CustomisedHair>().SwitchHairForward();
            }
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();

            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else
                modularCustomizationManager.SwitchHairForward(id);
            

        }
        
        public void SwitchHairBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchHairBack();

        }
        public void SwitchBeard()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchBeardForward();

        }
        public void SwitchBeard(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchBeardForward(id);

        }
        public void SwitchBeardBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchBeardBack();

        }
        public void SwitchEyebrow()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchEyebrowForward();

        }
        public void SwitchEyebrow(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchEyebrowForward(id);

        }
        public void SwitchEyebrowBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchEyebrowBack();

        }

        public void SwitchEye()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchEyeForward();

        }
        public void SwitchEye(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchEyeForward(id);

        }
        public void SwitchEyeBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchEyeBack();

        }


        public void SwitchMouth()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchMouthForward();

        }
        public void SwitchMouth(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchMouthForward(id);

        }
        public void SwitchMouthBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchMouthBack();

        }

        public void SwitchEar()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else      modularCustomizationManager.SwitchEarsForward();

        }
        public void SwitchEar(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else      modularCustomizationManager.SwitchEarsForward(id);

        }
        public void SwitchEarBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchEarsBack();

        }

        public void SwitchTusk()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else     modularCustomizationManager.SwitchTusksForward();

        }
        public void SwitchTusk(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else     modularCustomizationManager.SwitchTusksForward(id);

        }
        public void SwitchTuskBack()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchTusksBack();

        }

        //HNGamers

        public void blahblahRace()
        {
            SwitchSkinColortoColor(Color.black);    
        }

        public void SwitchSkinColortoColor(Color32 color)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else    modularCustomizationManager.SkinColorSet(color);

        }
        public void SwitchSkinColortoColor(Color color)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else      modularCustomizationManager.SkinColorSet(color);

        }

        public void SwitchHairColortoColor(Color32 color)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else      modularCustomizationManager.HairColorSet(color);

        }
        public void SwitchHairColortoColor(Color color)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.HairColorSet(color);

        }

        public void SwitchBodyArtColortoColor(Color32 color)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.BodyArtColorSet(color);

        }
        public void SwitchBodyArtColortoColor(Color color)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.BodyArtColorSet(color);

        }
        
        public void SwitchEyeColortoColor(Color32 color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.EyeColorSet(color);
        }
        
        public void SwitchEyeColortoColor(Color color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.EyeColorSet(color);
        }

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


        public void SwitchPrimaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.PrimaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchSecondaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SecondaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchMetalPrimaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.MetalPrimaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchMetalSecondaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.MetalSecondaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchMetalDarkColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.MetalDarkColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchLeatherPrimaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.LeatherPrimaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchLeatherSecondaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.LeatherSecondaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchLeatherTertiaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.LeatherTertiaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }
        public void SwitchTertiaryColortoColor(Color32 color, AtavismColorPicker atavismColorPicker)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.TertiaryColorSet(color, ReturnBodyType(atavismColorPicker));
        }

        public void SwitchSkinScarColortoColor(Color32 color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.ScarColorSet(color);
        }
        public void SwitchStubbleColortoColor(Color32 color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.StubbleColorSet(color);
        }

        public void SwitchBeardColortoColor(Color32 color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.BeardColorSet(color);
        }

        public void SwitchEyebrowColortoColor(Color32 color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.EyebrowColorSet(color);
        }

        public void SwitchMouthColortoColor(Color32 color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.MouthColorSet(color);
        }

        public void SwitchHands()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchHands();
        }
        
        public void SwitchHands(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.SwitchHands(id);
        }
        
        public void SwitchLowerArms()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.SwitchLowerArm();
        }
        
        public void SwitchLowerArms(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.SwitchLowerArm(id);
        }

        public void SwitchUpperArms()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchUpperArm();
        }
        
        public void SwitchUpperArms(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchUpperArm(id);
        }

        public void SwitchTorsoForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchTorso(true);
        }
        
        public void SwitchTorsoForward(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchTorso(id);
        }

        public void SwitchSkinColor()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>(); 
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SkinColorUP();

        }
        
        public void SwitchSkinColor(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>(); 
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SkinColorUP(id);

        }
        
        public void SwitchHairColor()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.HairColorUP();
        }

        public void SwitchHairColor(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.HairColorUP(id);
        }

        public void SwitchBeardColor()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.BeardColorUP();
        }

        public void SwitchBeardColor(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.BeardColorUP(id);
        }

        public void SwitchEyeBrowColor()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.EyebrowColorUP();
        }
        
        public void SwitchEyeBrowColor(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.EyebrowColorUP(id);
        }

        public void SwitchSkinScarColor()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SkinScarColorUP();
        }

        public void SwitchSkinScarColor(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SkinScarColorUP(id);
        }

        public void SwitchStubbleColor()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.StubbleColorUP();
        }
        
        public void SwitchStubbleColor(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.StubbleColorUP(id);
        }

        public void SwitchBodyArtColor()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.BodyArtColorUP();
        }
        
        public void SwitchBodyArtColor(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if(modularCustomizationManager)
                modularCustomizationManager.BodyArtColorUP(id);
        }

        public void SwitchEyeColor()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.EyeColorUP();
        }
        
        public void SwitchEyeColor(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.EyeColorUP(id);
        }

        public void SwitchAllHairColors()
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SyncHairColorUP();
        }
        public void SwitchAllHairColors(int id)
        {

            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SyncHairColorUP(id );
        }

        public void SwitchTorso()
        {
            SwitchTorsoForward();
        }
        
        public void SwitchTorso(int id)
        {
            SwitchTorsoForward(id);
        }

        public void SwitchTorsoBack()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchTorso(false);
        }

        public void SwitchHips()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchHips();
        }

        public void SwitchHips(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchHips(id);
        }

        public void SwitchLowerLegs()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchLowerLegs();
        }
        
        public void SwitchLowerLegs(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchLowerLegs(id);
        }
        public void SwitchFeet(int id)
        {
            SwitchFeetForward(id);
        }

        public void SwitchFeetForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchFeet( false);
        }
        
        public void SwitchFeetForward(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchFeet( id);
        }

        public void SwitchFeetBackward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchFeet(  true);
        }

        public void SwitchEyeMaterialForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial( BodyType.Eye, false);
        }

        public void SwitchEyeMaterialForward(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial( BodyType.Eye, id);
        }

        public void SwitchEyeMaterialBackward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchMaterial( BodyType.Eye, true);
        }

        public void SwitchSkinMaterialForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial( BodyType.Skin, false);
        }
        public void SwitchSkinMaterialForward(int id )
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial( BodyType.Skin, id);
        }

        public void SwitchSkinMaterialBackward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial( BodyType.Skin, true);
        }

        public void SwitchHairMaterialForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.SwitchMaterial( BodyType.Hair, false);
        }
        public void SwitchHairMaterialForward(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial( BodyType.Hair, id);
        }

        public void SwitchHairMaterialBackward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else modularCustomizationManager.SwitchMaterial( BodyType.Hair, true);
        }


        public void SwitchMouthMaterialForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial(BodyType.Mouth, false);
        }

        public void SwitchMouthMaterialForward(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial(BodyType.Mouth, id);
        }

        public void SwitchMouthMaterialBackward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchMaterial(BodyType.Mouth, true);
        }

        public void SetBlendShapes()
        {
#if IPBRInt
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
             if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SetBlendShapes();
#endif
        }

        public void SwitchBlendshapePreset()
        {
#if IPBRInt
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
             if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchBlendshapePreset();
#endif
        }


        public void SetBlendShapesV3()
        {
#if IPBRInt
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
              if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SetBlendShapes();
#endif
        }

        public void SwitchBlendshapePresetV3()
        {
#if IPBRInt
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
               if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchBlendshapePreset();
#endif
        }
        
        public void SwitchFace()
        {
            SwitchFaceForward();
        }
        
        public void SwitchFace(int id)
        {
            SwitchFaceForward(id);
        }

        public void SwitchFaceForward()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else   modularCustomizationManager.SwitchFace(false);
        }
        
        public void SwitchFaceForward(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else    modularCustomizationManager.SwitchFace(id);
        }


        public void SwitchFaceBack()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
            }else  modularCustomizationManager.SwitchFace(false);
        }

        public void SwitchHead()
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
                return;
            }else modularCustomizationManager.SwitchHead();

            foreach (var ah in modularCustomizationManager.ActiveHead)
            {
                string facevalue = ah.name;
                switch (facevalue)
                {
                    case "Char_Head_Male_00":
                        if (scarbutton)
                        {
                            scarbutton.gameObject.SetActive(true);
                        }

                        if (tattoobutton)
                        {
                            tattoobutton.gameObject.SetActive(false);
                        }

                        break;
                    case "Char_Head_Male_10":
                        if (scarbutton)
                        {
                            scarbutton.gameObject.SetActive(false);
                        }

                        if (tattoobutton)
                        {
                            tattoobutton.gameObject.SetActive(true);
                        }

                        break;
                    case "Char_Head_Male_11":
                        if (scarbutton)
                        {
                            scarbutton.gameObject.SetActive(false);
                        }

                        if (tattoobutton)
                        {
                            tattoobutton.gameObject.SetActive(true);
                        }

                        break;
                }
            }
        }
        public void SwitchHead(int id)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if (modularCustomizationManager == null)
            {
                Debug.LogError("no ModularCustomizationManager Component attached to the model ");
                return;
            }else     modularCustomizationManager.SwitchHead(id);

            foreach (var ah in modularCustomizationManager.ActiveHead)
            {
                string facevalue = ah.name;
                switch (facevalue)
                {
                    case "Char_Head_Male_00":
                        if (scarbutton)
                        {
                            scarbutton.gameObject.SetActive(true);
                        }

                        if (tattoobutton)
                        {
                            tattoobutton.gameObject.SetActive(false);
                        }

                        break;
                    case "Char_Head_Male_10":
                        if (scarbutton)
                        {
                            scarbutton.gameObject.SetActive(false);
                        }

                        if (tattoobutton)
                        {
                            tattoobutton.gameObject.SetActive(true);
                        }

                        break;
                    case "Char_Head_Male_11":
                        if (scarbutton)
                        {
                            scarbutton.gameObject.SetActive(false);
                        }

                        if (tattoobutton)
                        {
                            tattoobutton.gameObject.SetActive(true);
                        }

                        break;
                }
            }
        }

#endregion ModularAPI

      /*  public AtavismRaceData GetRaceDataByName(string raceName)
        {
            foreach (UGUICharacterRaceSlot raceSlot in races)
            {
                if (raceSlot != null)
                    if (raceSlot.raceData.raceName == raceName)
                    {
                        return raceSlot.raceData;
                    }
            }
            return null;
        }

        public AtavismClassData GetClassDataByName(string className)
        {
            foreach (UGUICharacterClassSlot classSlot in classes)
            {
                if (classSlot.classData.className == className)
                {
                    return classSlot.classData;
                }
            }
            return null;
        }*/

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

       
        public void RegisterModularCharacterSlider(ModularCharacterSlider modularCharacterSlider)
        {
           
            if(!modularCharacterSliderList.Contains(modularCharacterSlider))
            if (modularCharacterSlider.slider)
            {
                switch (modularCharacterSlider.dna)
                {
                    case "Hair"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchHair((int) id); });
                        break;
                    case "Beard":
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchBeard((int) id); });
                        break;
                    case "Face"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchFace((int) id); });
                        break;
                    case "Eyebrow"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchEyebrow((int) id); });
                        break;
                   /* case "Scar":
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { Switch((int) id); });
                        break;*/
                    case "Hands"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchHands((int) id); });
                        break;
                    case "LowerArms"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchLowerArms((int) id); });
                        break;
                    case "UpperArms"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchUpperArms((int) id); });
                        break;
                    case "Torso"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchTorso((int) id); });
                        break;
                    case "Hips"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchHips((int) id); });
                        break;
                    case "LowerLegs"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchLowerLegs((int) id); });
                        break;
                    case "Eyes"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchEye((int) id); });
                        break;
                    case "Mouth"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchMouth((int) id); });
                        break;
                    case "Head"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchHead((int) id); });
                        break;
                    case "Feet"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchFeet((int) id); });
                        break;
                  /*  case "Helmet"://
                        modularCharacterSlider.slider.onValueChanged.AddListener(id => { SwitchHelmet((int) id); });
                        break;*/

                }
                modularCharacterSliderList.Add(modularCharacterSlider);
                if (character != null)
                {
                    ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
                    if (modularCustomizationManager != null)
                    {
                        switch (modularCharacterSlider.dna)
                        {
                            case "Hair":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.hairModels.Count > 0 ? modularCustomizationManager.hairModels.Count - 1 : 0;
                                break;
                            case "Beard":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.beardModels.Count > 0 ? modularCustomizationManager.beardModels.Count - 1 : 0;
                                break;
                            case "Face":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.faceModels.Count > 0 ? modularCustomizationManager.faceModels.Count - 1 : 0;
                                break;
                            case "Eyebrow":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.eyebrowModels.Count > 0 ? modularCustomizationManager.eyebrowModels.Count - 1 : 0;
                                break;
                            /* case "Scar":
                                 modularCharacterSlider.slider.onValueChanged.AddListener(id => { Switch((int) id); });
                                 break;*/
                            case "Hands":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.handModels.Count > 0 ? modularCustomizationManager.handModels.Count - 1 : 0;
                                break;
                            case "LowerArms":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.lowerArmModels.Count > 0 ? modularCustomizationManager.lowerArmModels.Count - 1 : 0;
                                break;
                            case "UpperArms":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.upperArmModels.Count > 0 ? modularCustomizationManager.upperArmModels.Count - 1 : 0;
                                break;
                            case "Torso":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.torsoModels.Count > 0 ? modularCustomizationManager.torsoModels.Count - 1 : 0;
                                break;
                            case "Hips":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.hipModels.Count > 0 ? modularCustomizationManager.hipModels.Count - 1 : 0;
                                break;
                            case "LowerLegs":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.lowerLegModels.Count > 0 ? modularCustomizationManager.lowerLegModels.Count - 1 : 0;
                                break;
                            case "Eyes":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.eyeModels.Count > 0 ? modularCustomizationManager.eyeModels.Count - 1 : 0;
                                break;
                            case "Mouth":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.mouthModels.Count > 0 ? modularCustomizationManager.mouthModels.Count - 1 : 0;
                                break;
                            case "Head":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.headModels.Count > 0 ? modularCustomizationManager.headModels.Count - 1 : 0;
                                break;
                            case "Feet":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.feetModels.Count > 0 ? modularCustomizationManager.feetModels.Count - 1 : 0;
                                break;
                            case "Helmet":
                                modularCharacterSlider.slider.minValue = 0;
                                modularCharacterSlider.slider.maxValue = modularCustomizationManager.helmetModels.Count > 0 ? modularCustomizationManager.helmetModels.Count - 1 : 0;
                                break;
                        }
                    }
                }
            }
        }

        public void RegisterModularCharacterPicker(ModularCharacterColor modularCharacterColor)
        {
            if(!modularCharacterColorPanelList.Contains(modularCharacterColor))
                if (modularCharacterColor)
                {
                    // EEE - For accessing default colours
                    ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
                    
                   // Debug.LogError("RegisterModularCharacterPicker "+modularCharacterColor.dna);
                    switch (modularCharacterColor.dna)
                    {
                        case "BodyArtColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultBodyArtColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchBodyArtColortoColor(color); });
                            }

                            break;
                        case "EyeColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultEyeColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchEyeColortoColor(color); });
                            }

                            break;
                        case "HairColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultHairColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color =>
                                {
                                    if (allowDifferentHairColors)
                                    {
                                        SwitchHairColortoColor(color);
                                    }
                                    else
                                    {
                                        SwitchHairColortoColor(color);
                                        SwitchBeardColortoColor(color);
                                        SwitchEyebrowColortoColor(color);
                                    }
                                });
                            }

                            break;
                        case "StubbleColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultStubbleColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchStubbleColortoColor(color); });
                            }

                            break;
                        case "ScarColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultStubbleColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchSkinScarColortoColor(color); });
                            }

                            break;
                        case "MouthColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultMouthColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchMouthColortoColor(color); });
                            }

                            break;
                        case "BeardColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultHairColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchBeardColortoColor(color); });
                            }

                            break;
                        case "EyebrowColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultHairColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchEyebrowColortoColor(color); });
                            }

                            break;
                        case "SkinColor":
                            if (modularCharacterColor.picker)
                            {
                                modularCharacterColor.picker.AssignColor(modularCustomizationManager.defaultSkinColor); // EEE Set Default Colour
                                modularCharacterColor.picker.onValueChanged.AddListener(color => { SwitchSkinColortoColor(color); });
                            }

                            break;

                    }

                    /*   if (primarypicker && atavismColorPicker)
                       {
                           primarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchPrimaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (secondarypicker && atavismColorPicker)
                       {
                           secondarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchSecondaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (metalprimarypicker && atavismColorPicker)
                       {
                           metalprimarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchMetalPrimaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (metalsecondarypicker && atavismColorPicker)
                       {
                           metalsecondarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchMetalSecondaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (metaldarkpicker && atavismColorPicker)
                       {
                           metaldarkpicker.onValueChanged.AddListener(color =>
                           {
                               SwitchMetalDarkColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (leatherprimarypicker && atavismColorPicker)
                       {
                           leatherprimarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchLeatherPrimaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (leathertertiarypicker && atavismColorPicker)
                       {
                           leathertertiarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchLeatherTertiaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (tertiarypicker && atavismColorPicker)
                       {
                           tertiarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchTertiaryColortoColor(color, atavismColorPicker);
                           });
                       }
           
                       if (leathersecondarypicker && atavismColorPicker)
                       {
                           leathersecondarypicker.onValueChanged.AddListener(color =>
                           {
                               SwitchLeatherSecondaryColortoColor(color, atavismColorPicker);
                           });
                       }*/

                    modularCharacterColorPanelList.Add(modularCharacterColor);
                }

            if (character != null)
                {
                   // Debug.LogError("RegisterModularCharacterPicker "+modularCharacterColor.dna+" character");

                    ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
                    if (modularCustomizationManager)
                    {
                       // Debug.LogError("RegisterModularCharacterPicker "+modularCharacterColor.dna+" modular");

                        if (modularCharacterColor != null)
                        {
                         //   Debug.LogError(modularCharacterColor.dna + " ");
                            switch (modularCharacterColor.dna)
                            {
                                case "HairColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.hairColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "StubbleColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.stubbleColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.stubbleColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "SkinColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.skinColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.skinColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "EyeColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.eyeColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.eyeColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "ScarColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.scarColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.scarColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "BodyArtColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.bodyArtColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.bodyArtColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "BeardColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.hairColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "EyebrowColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.hairColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                                case "MouthColor":
                                    for (int i = 0; i < modularCharacterColor.colors.Count; i++)
                                    {
                                        if (modularCustomizationManager.hairColors.Length > i)
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(true);
                                            modularCharacterColor.colors[i].image.color = modularCustomizationManager.hairColors[i];
                                        }
                                        else
                                        {
                                            modularCharacterColor.colors[i].gameObject.SetActive(false);
                                        }
                                    }

                                    break;
                            
                        }
                    }
                }
            }
        }

        public void ModularCharacterSetColor(string dna, Color color)
        {
            ModularCustomizationManager modularCustomizationManager = character.GetComponent<ModularCustomizationManager>();
            if(modularCustomizationManager)
                switch (dna)
                {
                    case "HairColor":
                        modularCustomizationManager.HairColorSet(color);
                        break;
                    case "StubbleColor":
                        modularCustomizationManager.StubbleColorSet(color);
                        break;
                    case "SkinColor":
                        modularCustomizationManager.SkinColorSet(color);
                        break;
                    case "EyeColor":
                        modularCustomizationManager.EyeColorSet(color);
                        break;
                    case "ScarColor":
                        modularCustomizationManager.ScarColorSet(color);
                        break;
                    case "BodyArtColor":
                        modularCustomizationManager.BodyArtColorSet(color);
                        break;
                    case "BeardColor":
                        modularCustomizationManager.BeardColorSet(color);
                        break;
                    case "EyebrowColor":
                        modularCustomizationManager.EyebrowColorSet(color);
                        break;
                    case "MouthColor":
                        modularCustomizationManager.MouthColorSet(color);
                        break;
                  /*  case "PrimaryColor":
                        modularCustomizationManager.PrimaryColorSet(color);
                        break;
                    case "SecondaryColor":
                        modularCustomizationManager.SecondaryColorSet(color);
                        break;
                    case "MetalDarkColor":
                        modularCustomizationManager.MetalDarkColorSet(color);
                        break;
                    case "MetalPrimaryColor":
                        modularCustomizationManager.MetalPrimaryColorSet(color);
                        break;
                    case "MetalSecondaryColor":
                        modularCustomizationManager.MetalSecondaryColorSet(color);
                        break;
                    case "LeatherPrimaryColor":
                        modularCustomizationManager.LeatherPrimaryColorSet(color);
                        break;
                    case "TertiaryColor":
                        modularCustomizationManager.TertiaryColorSet(color);
                        break;
                    case "LeatherTertiaryColor":
                        modularCustomizationManager.LeatherTertiaryColorSet(color);
                        break;*/

                }
        }
    }
}