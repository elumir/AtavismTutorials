using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;
// Replaced: Assets/Dragonsan/AtavismObjects/Scripts/LoginController.cs

namespace Atavism
{

    public enum LoginState
    {
        Login,
        Register,
        Authenticating,
        CharacterSelect,
        CharacterCreate
    }

    public class UILoginController : MonoBehaviour
    {
        [Header("Replaced - Assets/Dragonsan/AtavismObjects/Scripts/LoginController.cs")]

        // Atavism Variables
        public GameObject soundMenu;
        public GameObject musicObject;
        public Texture cursorOverride;
        string characterScene = "CharacterSelection";        
        public bool useMd5Encryption = true;
        LoginState loginState;
        bool loaded = false;
        float loginclick;

        // Declare UI Builder Screens
        public VisualElement m_screenLogin;
        public VisualElement m_screenRegister;
        public VisualElement m_screenMessage;
        public VisualElement m_screenError;        
        public VisualElement m_panelButtonError;
        public VisualElement m_panelButtonMessage;
        

        // Declare UI Builder buttons
        public Button m_buttonLogin;
        public Button m_buttonRegister;
        public Button m_buttonRegisterCancel;
        public Button m_buttonRegisterCreate;
        public Button m_buttonMessageClose;
        public Button m_buttonErrorClose;
        public Button m_buttonQuit;
        public TextField m_username;
        public TextField m_password;
        public TextField m_usernameNew; // from Registration
        public TextField m_passwordNew;
        public TextField m_passwordVerify;
        public TextField m_email;
        public TextField m_emailVerify;
        public Toggle m_toggleRemember;
        public Label m_message;
        public Label m_messageError;


        // Use this for initialization
        void Start()
        {
            loginState = LoginState.Login;
            AtavismEventSystem.RegisterEvent("LOGIN_RESPONSE", this);
            AtavismEventSystem.RegisterEvent("REGISTER_RESPONSE", this);
            AtavismEventSystem.RegisterEvent("SETTINGS_LOADED", this);
            // Play music
            SoundSystem.LoadSoundSettings();
            if (musicObject != null)
                SoundSystem.PlayMusic(musicObject.GetComponent<AudioSource>());

            // UI Builder Elements
            var root = GetComponent<UIDocument>().rootVisualElement;
            m_screenLogin = root.Q<VisualElement>("ScreenLogin");
            m_screenRegister = root.Q<VisualElement>("ScreenRegister");
            m_screenError = root.Q<VisualElement>("ScreenError");    
            m_screenMessage = root.Q<VisualElement>("ScreenMessage");    
            m_panelButtonError = root.Q<VisualElement>("ErrorButton"); 
            m_panelButtonMessage = root.Q<VisualElement>("MessageButton"); 

            m_buttonLogin= root.Q<Button>("button-login");
            m_buttonRegister = root.Q<Button>("button-register");
            m_buttonRegisterCancel = root.Q<Button>("button-cancel");
            m_buttonRegisterCreate = root.Q<Button>("button-create");
            m_buttonErrorClose = root.Q<Button>("button-closeError");
            m_buttonMessageClose = root.Q<Button>("button-close");
            m_messageError = root.Q<Label>("varErrorMessage");
            m_message = root.Q<Label>("varMessage");
            m_username = root.Q<TextField>("username");
            m_password = root.Q<TextField>("password");
            m_toggleRemember = root.Q<Toggle>("rememberMe");
            m_usernameNew = root.Q<TextField>("usernameNew");
            m_passwordNew = root.Q<TextField>("passwordNew");
            m_passwordVerify = root.Q<TextField>("passwordVerify");
            m_email = root.Q<TextField>("emailNew");
            m_emailVerify = root.Q<TextField>("emailVerify");

            m_buttonLogin.clicked += Login;
            m_buttonRegister.clicked += ShowRegisterPanel;
            m_buttonRegisterCreate.clicked += Register;
            m_buttonErrorClose.clicked += HideDialog;
            m_buttonMessageClose.clicked += HideDialog;
            m_buttonRegisterCancel.clicked += CancelRegistration;

            m_toggleRemember.RegisterValueChangedCallback(ToggleSave);
        }

        private void OnEnable()
        {            
            if (AtavismSettings.Instance != null)
            {
                if (m_toggleRemember != null)
                    m_toggleRemember.value = AtavismSettings.Instance.GetGeneralSettings().saveCredential ? true : false;
                if (m_username != null)
                    m_username.value = AtavismSettings.Instance.GetCredentials().l;
                if (m_password != null)
                    m_password.value = AtavismSettings.Instance.GetCredentials().p;
            }
        }
        public void ToggleSave(ChangeEvent<bool> evt)
        {
            if (m_toggleRemember != null)
                AtavismSettings.Instance.GetGeneralSettings().saveCredential = evt.newValue;
        }
        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("LOGIN_RESPONSE", this);
            AtavismEventSystem.UnregisterEvent("REGISTER_RESPONSE", this);
            AtavismEventSystem.UnregisterEvent("SETTINGS_LOADED", this);
        }

        void OnGUI()
        {
            if (cursorOverride != null)
            {
                UnityEngine.Cursor.visible = false;
                GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 32, 32), cursorOverride);
            }
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (AtavismSettings.UIHasFocus())
                {
                    if (loginState == LoginState.Login)
                    {
                        Login();
                    }
                }
            }
            if (!loaded)
                if (AtavismSettings.Instance != null)
                {
                    if (m_toggleRemember != null)
                        m_toggleRemember.value = AtavismSettings.Instance.GetGeneralSettings().saveCredential ? true : false;
                    if (AtavismSettings.Instance.GetGeneralSettings().saveCredential)
                    {
                        if (m_username != null)
                            m_username.value = AtavismSettings.Instance.GetCredentials().l;
                        if (m_password != null)
                            m_password.value = AtavismSettings.Instance.GetCredentials().p;                        
                    }
                    loaded = true;
                }

        }

        public void ShowLoginPanel()
        {
            loginState = LoginState.Login;
            m_screenLogin.style.display = DisplayStyle.Flex;
            m_screenRegister.style.display = DisplayStyle.None;
        }

        public void ShowRegisterPanel()
        {
            loginState = LoginState.Register;
            m_screenLogin.style.display = DisplayStyle.None;
            m_screenRegister.style.display = DisplayStyle.Flex;
        }

        public void CancelRegistration()
        {
            ShowLoginPanel();
        }

        public void SetUserName(string username)
        {
            if (loginState == LoginState.Login)
            {
                this.m_username.value = username;
            }
            else
            {
                this.m_usernameNew.value = username;
            }
        }

        public void SetPassword(string password)
        {
            if (loginState == LoginState.Login)
            {
                this.m_password.value = password;
            }
            else
            {
                this.m_passwordNew.value = password;
            }
        }

        public void SetPassword2(string password2)
        {
            this.m_passwordVerify.value = password2;
        }

        public void SetEmail(string email)
        {
            this.m_email.value = email;
        }

        public void SetEmail2(string email2)
        {
            this.m_emailVerify.value = email2;
        }
        // **************
        // LOGIN
        // **************
        public void Login()
        {
            if (AtavismLogger.logLevel <= LogLevel.Debug)
            {
                AtavismLogger.LogDebugMessage("Login pressed");
            }
            if (m_username.value == "")
            {
#if AT_I2LOC_PRESET
   			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Please enter a username"), true);
#else
                ShowDialog("Please enter a username", true);
#endif
                return;
            }
            if (loginclick > Time.time)
                return;
            loginclick = Time.time + 1f;

            //ShowDialog("Logging In...", false);
            Dictionary<string, object> props = new Dictionary<string, object>();

            if (AtavismSettings.Instance.GetGeneralSettings().saveCredential)
            {
                AtavismSettings.Instance.GetCredentials().l = m_username.text;
                AtavismSettings.Instance.GetCredentials().p = m_password.text;
            }
            if (useMd5Encryption)
            {
                AtavismClient.Instance.Login(m_username.text, AtavismEncryption.Md5Sum(m_password.text), props);
            }
            else
            {
                AtavismClient.Instance.Login(m_username.text, m_password.text, props);
            }
        }
        // **************
        // REGISTER
        // **************
        public void Register()
        {
            if (m_usernameNew.value == "")
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Please enter a username"), true);
#else
                ShowDialog("Please enter a username", true);
#endif
                return;
            }
            if (m_usernameNew.value.Length < 4)
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Your username must be at least 4 characters long"), true);
#else
                ShowDialog("Your username must be at least 4 characters long", true);
#endif
                return;
            }
            foreach (char chr in m_usernameNew.value)
            {
                if ((chr < 'a' || chr > 'z') && (chr < 'A' || chr > 'Z') && (chr < '0' || chr > '9'))
                {
#if AT_I2LOC_PRESET
				ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Your username can only contain letters and numbers"), true);
#else
                    ShowDialog("Your username can only contain letters and numbers", true);
#endif
                    return;
                }
            }
            if (m_passwordNew.value == "")
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Please enter a password"), true);
#else
                ShowDialog("Please enter a password", true);
#endif
                return;
            }
            foreach (char chr in m_passwordNew.value)
            {
                if (chr == '*' || chr == '\'' || chr == '"' || chr == '/' || chr == '\\' || chr == ' ')
                {
#if AT_I2LOC_PRESET
				ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Your password cannot contain * \' \" / \\ or spaces"), true);
#else
                    ShowDialog("Your password cannot contain * \' \" / \\ or spaces", true);
#endif
                    return;
                }
            }
            if (m_passwordNew.value.Length < 6)
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Your password must be at least 6 characters long"), true);
#else
                ShowDialog("Your password must be at least 6 characters long", true);
#endif
                return;
            }
            if (m_passwordNew.value != m_passwordVerify.value)
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Your passwords must match"), true);
#else
                ShowDialog("Your passwords must match", true);
#endif
                return;
            }
            if (m_email.value == "")
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Please enter an email address"), true);
#else
                ShowDialog("Please enter an email address", true);
#endif
                return;
            }
            if (!ValidateEmail(m_email.value))
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Please enter a valid email address"), true);
#else
                ShowDialog("Please enter a valid email address", true);
#endif
                return;
            }
            if (m_email.value != m_emailVerify.value)
            {
#if AT_I2LOC_PRESET
			ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Your email addresses must match"), true);
#else
                ShowDialog("Your email addresses must match", true);
#endif
                return;
            }
            if (useMd5Encryption)
            {
                AtavismClient.Instance.CreateAccount(m_usernameNew.text, AtavismEncryption.Md5Sum(m_passwordNew.text), m_email.text);
            }
            else
            {
                AtavismClient.Instance.CreateAccount(m_usernameNew.text, m_passwordNew.text, m_email.text);
            }

        }

        private bool ValidateEmail(string email)
        {
            // Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,5})+)$");
            Match match = regex.Match(email);
            if (match.Success)
                return true;
            else
                return false;
        }


        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "LOGIN_RESPONSE")
            {
                if (eData.eventArgs[0] == "Success")
                {
                    // Debug.Log("Where does it go from here? Arg1: "+eData);
                    //Application.LoadLevel(characterScene);
                }
                else if  (eData.eventArgs[0] == "Resolving World ...")
                {   
                    // Added in to properly show a dialogue for Resolving World ...
                     ShowDialog(eData.eventArgs[0], false, false);
                }
                else
                {
                    string errorType = eData.eventArgs[0];
#if AT_I2LOC_PRESET
				string errorMessage = I2.Loc.LocalizationManager.GetTranslation(errorType);
#else
                    string errorMessage = errorType;
#endif
                    if (errorType == "LoginFailure")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Invalid username or password");
#else
                        errorMessage = "Invalid username or password";
#endif
                    }
                    else if (errorType == "NoAccessFailure")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Your account does not have access to log in");
#else
                        errorMessage = "Your account does not have access to log in";
#endif
                    }
                    else if (errorType == "BannedFailure")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Your account has been banned");
#else
                        errorMessage = "Your account has been banned";
#endif
                    }
                    else if (errorType == "SubscriptionExpiredFailure")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Your account does not have an active subscription");
#else
                        errorMessage = "Your account does not have an active subscription";
#endif
                    }
                    else if (errorType == "ServerMaintanance")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("The server is in maintenance mode, please try again later");
#else
                        errorMessage = "The server is in maintenance mode, please try again later";
#endif
                    }
                    ShowDialog(errorMessage, true);
                }
            }
            else if (eData.eventType == "REGISTER_RESPONSE")
            {
                if (eData.eventArgs[0] == "Success")
                {
                    ShowLoginPanel();
#if AT_I2LOC_PRESET
				ShowDialog(I2.Loc.LocalizationManager.GetTranslation("Account created. You can now log in"), true);
#else
                    ShowDialog("Account created. You can now log in", true, false); // SUCCESS!
#endif
                }
                else
                {
                    string errorType = eData.eventArgs[0];
                    string errorMessage = errorType;
                    if (errorType == "UsernameUsed")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("An account with that username already exists");
#else
                        errorMessage = "An account with that username already exists";
#endif
                    }
                    else if (errorType == "EmailUsed")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("An account with that email address already exists");
#else
                        errorMessage = "An account with that email address already exists";
#endif
                    }
                    else if (errorType == "Unknown")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Unknown error. Please let the Cryptex Hunt team know");
#else
                        errorMessage = "Unknown error. Please let the Cryptex Hunt team know";
#endif
                    }
                    else if (errorType == "MasterTcpConnectFailure")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Unable to connect to the Authentication Server");
#else
                        errorMessage = "Unable to connect to the Authentication Server";
#endif
                    }
                    else if (errorType == "NoAccessFailure")
                    {
#if AT_I2LOC_PRESET
					errorMessage = I2.Loc.LocalizationManager.GetTranslation("Account creation has been disabled on this server");
#else
                        errorMessage = "Account creation has been disabled on this server";
#endif
                    }
                    ShowDialog(errorMessage, true);
                }
            }
            else if (eData.eventType == "SETTINGS_LOADED")
            {
                if (m_toggleRemember != null)
                    m_toggleRemember.value = AtavismSettings.Instance.GetGeneralSettings().saveCredential ? true : false;
                if (m_username.value != null && AtavismSettings.Instance.GetGeneralSettings().saveCredential)
                    m_username.value = AtavismSettings.Instance.GetCredentials().l;
                if (m_password != null && AtavismSettings.Instance.GetGeneralSettings().saveCredential)
                    m_password.value = AtavismSettings.Instance.GetCredentials().p;
            }
        }

        void ShowDialog(string message, bool showButton, bool isError = true)
        {
            // normal background if not error or not showing button
            if (!isError || !showButton) {
                m_message.text = message;
                m_screenError.style.display = DisplayStyle.None; // in case it's visible
                m_screenMessage.style.display = DisplayStyle.Flex;            
                if (showButton)
                    m_panelButtonMessage.style.display = DisplayStyle.Flex;
                else
                    m_panelButtonMessage.style.display = DisplayStyle.None;                   
            }
            else 
            {
                m_messageError.text = message;
                m_screenError.style.display = DisplayStyle.Flex;
                m_screenMessage.style.display = DisplayStyle.None; // in case it's visible
                if (showButton)
                    m_panelButtonError.style.display = DisplayStyle.Flex;
                else
                    m_panelButtonError.style.display = DisplayStyle.None;
            }
        }
        void HideDialog()
        {
            m_screenError.style.display = DisplayStyle.None;
            m_screenMessage.style.display = DisplayStyle.None;
        }

        public string CharacterScene
        {
            get
            {
                return characterScene;
            }
            set
            {
                characterScene = value;
            }
        }
    }
}