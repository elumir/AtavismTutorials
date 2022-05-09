using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UILoginController : MonoBehaviour
{
    public VisualElement loginPanel;
    public VisualElement errorPanel;

    public Button loginButton;
    public Button closeErrorButton;
    public Button registerButton;
    public TextField userField;
    public TextField passField;
    public Label errorMessage;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        loginPanel = root.Q<VisualElement>("PanelLogin");
        errorPanel = root.Q<VisualElement>("PanelError");       

        loginButton = root.Q<Button>("button-login");
        registerButton = root.Q<Button>("button-register");
        closeErrorButton = root.Q<Button>("btn-closeError");
        errorMessage = root.Q<Label>("varErrorMessage");

        loginButton.clicked += LoginButtonPressed;
        closeErrorButton.clicked += CloseErrorButtonPressed;
    }

    private void LoginButtonPressed()
    {
        loginPanel.style.display = DisplayStyle.None;
        errorPanel.style.display = DisplayStyle.Flex;
        errorMessage.text = "You pressed the login button. Good for you!";
    }
    private void CloseErrorButtonPressed()
    {
        loginPanel.style.display = DisplayStyle.Flex;
        errorPanel.style.display = DisplayStyle.None;

    }
}
