using UnityEngine.UIElements;
/* Example: https://github.com/Unity-Technologies/UIToolkitUnityRoyaleRuntimeDemo/blob/master/Assets/Scripts/UI/TitleScreenManager.cs */

public class ScreenManagerLogin : VisualElement
{
    VisualElement m_LoginPanel;
    VisualElement m_ErrorPanel;
    Label m_ErrorMessage;
    
    public new class UxmlFactory: UxmlFactory<ScreenManagerLogin, UxmlTraits> { }
    public new class UxmlTraits : VisualElement.UxmlTraits{ }
       
    public ScreenManagerLogin(){
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }
    void OnGeometryChange(GeometryChangedEvent evt)
    {
        m_LoginPanel = this.Q("PaenlLogin");
        m_ErrorPanel = this.Q("PanelError");

        m_ErrorMessage = this.Q<Label>("varErrorMessage");

        m_LoginPanel?.Q("button-login")?.RegisterCallback<ClickEvent>(ev => OpenErrorPanel());
        m_ErrorPanel?.Q("btn-closeError")?.RegisterCallback<ClickEvent>(ev => CloseErrorPanel());

        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OpenErrorPanel()
    {
        m_ErrorPanel.style.display = DisplayStyle.Flex;
        m_ErrorMessage.text = "You pressed the login button. Good for you!";
    }
    void CloseErrorPanel()
    {
        m_ErrorPanel.style.display = DisplayStyle.None;
    }
}
