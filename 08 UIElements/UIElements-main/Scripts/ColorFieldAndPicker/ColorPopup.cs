// https://github.com/plyoung/UIElements
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public class ColorPopup : PopupPanel
	{
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<ColorPopup, UxmlTraits> { }

		public string Heading { get; set; } = "Pick Colour";
		public string ButtonLabel { get; set; } = "Apply";
		public string ResetLabel { get; set; } = "Cancel";  // EEE [A] - label for Reset
		public Color OriginalColor {get; set; } // EEE [A] - Reset colour back to this
		public BodyPartColor CurrentPart {get; set; } // EEE [A] - what part is being updated

		private float H, S, V, A;

		private System.Action<Color> onSubmit;
		public System.Action<Color> onValueChange;

		private Label headingLabel;
		private Button submitButton;
		private Button resetButton; // EEE [A] - Added reset button

		private Slider rSlider;
		private Slider gSlider;
		private Slider bSlider;
//		private Slider aSlider; // EEE - Removed aslider code
		private Slider2D gradientSlider;
		private UnityEngine.UIElements.Slider hueSlider;
		private VisualElement gradientSliderDragger;
		private VisualElement hueSliderDragger;

		private Texture2D gradientTexture;
		private Texture2D hueSliderTexture;

		private const string popupStylesResource = "GameUI/Styles/ColorPopupStyleSheet";
		private const string ussPopupClassName = "color-popup";
		private const string ussHeadingBack = ussPopupClassName + "__heading-area";
		private const string ussContentBack = ussPopupClassName + "__content-area";
		private const string ussHeading = ussPopupClassName + "__heading";
		private const string ussButtonsBar = ussPopupClassName + "__buttons-bar";
		private const string ussSubmitButton = ussPopupClassName + "__submit-button";
		private const string ussResetButton = ussPopupClassName + "__reset-button"; // EEE [A]- Reset Button
		private const string ussRSlider = ussPopupClassName + "__red-slider";
		private const string ussGSlider = ussPopupClassName + "__green-slider";
		private const string ussBSlider = ussPopupClassName + "__blue-slider";
		private const string ussGradientArea = ussPopupClassName + "__gradient-area";
		private const string ussGradientSlider = ussPopupClassName + "__gradient-slider";
		private const string ussHueSlider = ussPopupClassName + "__hue-slider";

		// ------------------------------------------------------------------------------------------------------------

		public ColorPopup()
			: this(null, null)
		{ }

		public ColorPopup(string heading, string buttonLabel)
		{
			if (heading != null) Heading = heading;
			if (buttonLabel != null) ButtonLabel = buttonLabel;

			styleSheets.Add(Resources.Load<StyleSheet>(popupStylesResource));

			// panel
			mainPanel.AddToClassList(ussPopupClassName);

			// heading
			var head = new VisualElement();
			head.AddToClassList(ussHeadingBack);
			mainPanel.Add(head);

			headingLabel = new Label() { text = Heading };
			headingLabel.AddToClassList(ussHeading);
			headingLabel.AddToClassList("font"); // EEE [A] - add base class to font
			head.Add(headingLabel);

			// content area
			var content = new VisualElement();
			content.AddToClassList(ussContentBack);
			mainPanel.Add(content);

			// gradient area
			var gradientArea = new VisualElement();
			gradientArea.AddToClassList(ussGradientArea);
			content.Add(gradientArea);

			// gradient block
			gradientSlider = new Slider2D();
			gradientSliderDragger = gradientSlider.Q("dragger");
			gradientSlider.AddToClassList(ussGradientSlider);
			gradientArea.Add(gradientSlider);

			// hue slider
			hueSlider = new UnityEngine.UIElements.Slider(null, 0f, 360f, SliderDirection.Vertical, 0f);
			hueSliderDragger = hueSlider.Q("unity-dragger");
			hueSlider.AddToClassList(ussHueSlider);
			gradientArea.Add(hueSlider);

			// rgba sliders
			rSlider = new Slider("Red", 0f, 1f, SliderDirection.Horizontal, 0f);
			gSlider = new Slider("Green", 0f, 1f, SliderDirection.Horizontal, 0f);
			bSlider = new Slider("Blue", 0f, 1f, SliderDirection.Horizontal, 0f);

			rSlider.showInputField = true;
			gSlider.showInputField = true;
			bSlider.showInputField = true;

			rSlider.AddToClassList(ussRSlider);
			gSlider.AddToClassList(ussGSlider);
			bSlider.AddToClassList(ussBSlider);

			content.Add(rSlider);
			content.Add(gSlider);
			content.Add(bSlider);

			// button bar
			var buttons = new VisualElement();
			buttons.AddToClassList(ussButtonsBar);
			mainPanel.Add(buttons);

			// EEE - Add reset button
			resetButton = new Button() { text = ResetLabel };
			resetButton.AddToClassList(ussResetButton);
			resetButton.AddToClassList("button"); 
			buttons.Add(resetButton);

			submitButton = new Button() { text = ButtonLabel };
			submitButton.AddToClassList(ussSubmitButton);
			submitButton.AddToClassList("button"); // EEE [A] - add base class to button

			buttons.Add(submitButton);

			submitButton.clicked += OnSubmitButton;
			resetButton.clicked += OnResetButton; // EEE [A]

			rSlider.RegisterValueChangedCallback(ev => SetColorFromRSliders(ev.newValue));
			gSlider.RegisterValueChangedCallback(ev => SetColorFromGSliders(ev.newValue));
			bSlider.RegisterValueChangedCallback(ev => SetColorFromBSliders(ev.newValue));
			hueSlider.RegisterValueChangedCallback(SetColorFromHueSlider);
			gradientSlider.RegisterValueChangedCallback(SetColorFromGradientSlider);
		}

		public void Show(string heading, string buttonLabel, Color color, System.Action<Color> onSubmit)
		{
			if (heading != null) Heading = heading;
			if (buttonLabel != null) ButtonLabel = buttonLabel;
			Show(color, onSubmit);
		}

		public void Show(Color color, System.Action<Color> onSubmit)
		{
			OriginalColor = color; // EEE Get original colour
			Color.RGBToHSV(color, out H, out S, out V);
			A = color.a;
			
			this.onSubmit = onSubmit;

			headingLabel.text = Heading;
			submitButton.text = ButtonLabel;
			
			CreateTextures();
			OnColorChanged(true, true);

			base.Show();
		}

		public override void Hide()
		{
			gradientSlider.style.backgroundImage = null;
			hueSlider.style.backgroundImage = null;

			Object.Destroy(gradientTexture);
			Object.Destroy(hueSliderTexture);

			onSubmit = null;
			base.Hide();
		}

		// ------------------------------------------------------------------------------------------------------------

		private void OnSubmitButton()
		{
			var c = Color.HSVToRGB(H, S, V);
			c.a = A;
			
			onSubmit?.Invoke(c);

			Hide();
		}
		private void OnResetButton() // EEE [A] - Add reset to original colour
		{
			Color.RGBToHSV(OriginalColor, out H, out S, out V);
			OnColorChanged(true, true);
			Hide();				
		}
		private void SetColorFromGradientSlider(ChangeEvent<Vector2> ev)
		{
			S = ev.newValue.x;
			V = ev.newValue.y;
			OnColorChanged(false, false);
		}

		private void SetColorFromHueSlider(ChangeEvent<float> ev)
		{		
			H = ev.newValue / 360f; // hue slider value 0..360
			OnColorChanged(false, true);
		}

		private void SetColorFromRSliders(float value)
		{
			Color c = Color.HSVToRGB(H, S, V);
			c.r = value;
			Color.RGBToHSV(c, out H, out S, out V);
			OnColorChanged(true, true);
		}

		private void SetColorFromGSliders(float value)
		{
			Color c = Color.HSVToRGB(H, S, V);
			c.g = value;
			Color.RGBToHSV(c, out H, out S, out V);
			OnColorChanged(true, true);
		}

		private void SetColorFromBSliders(float value)
		{
			Color c = Color.HSVToRGB(H, S, V);
			c.b = value;
			Color.RGBToHSV(c, out H, out S, out V);
			OnColorChanged(true, true);
		}

		// ------------------------------------------------------------------------------------------------------------

		private void OnColorChanged(bool updateHue, bool updateGradient)
		{
			var c = Color.HSVToRGB(H, S, V);
			hueSliderDragger.style.backgroundColor = Color.HSVToRGB(H, 1f, 1f);
			gradientSliderDragger.style.backgroundColor = c;

			rSlider.SetValueWithoutNotify(Round(c.r, 3));
			gSlider.SetValueWithoutNotify(Round(c.g, 3));
			bSlider.SetValueWithoutNotify(Round(c.b, 3));

			if (updateHue)
			{
				hueSlider.SetValueWithoutNotify(H * 360f);
			}

			if (updateGradient)
			{
				UpdateGradientTexture();
				gradientSlider.SetValueWithoutNotify(new Vector2(S, V));
			}

			onValueChange?.Invoke(c); // EEE [A] - new event for changing colour

		}

		private void CreateTextures()
		{
			gradientTexture = new Texture2D(64, 64, TextureFormat.RGB24, false) { filterMode = FilterMode.Point };
			gradientTexture.hideFlags = HideFlags.HideAndDontSave;
			gradientSlider.style.backgroundImage = gradientTexture;

			hueSliderTexture = new Texture2D(1, 64, TextureFormat.RGB24, false) { filterMode = FilterMode.Point };
			hueSliderTexture.hideFlags = HideFlags.HideAndDontSave;
			hueSlider.style.backgroundImage = hueSliderTexture;
			UpdateHueSliderTexture();
		}

		private void UpdateHueSliderTexture()
		{
			if (hueSliderTexture == null) return;
			for (var i = 0; i < hueSliderTexture.height; i++)
			{
				hueSliderTexture.SetPixel(0, i, Color.HSVToRGB((float)i / (hueSliderTexture.height - 1), 1f, 1f));
			}

			hueSliderTexture.Apply();
			hueSlider.MarkDirtyRepaint();
		}

		private void UpdateGradientTexture()
		{
			if (gradientTexture == null) return;
			var pixels = new Color[gradientTexture.width * gradientTexture.height];

			for (var x = 0; x < gradientTexture.width; x++)
			{
				for (var y = 0; y < gradientTexture.height; y++)
				{
					pixels[x * gradientTexture.width + y] = Color.HSVToRGB(H, (float)y / gradientTexture.height, (float)x / gradientTexture.width);
				}
			}

			gradientTexture.SetPixels(pixels);
			gradientTexture.Apply();
			gradientSlider.MarkDirtyRepaint();
		}

		private static float Round(float value, int digits)
		{
			float mult = Mathf.Pow(10.0f, digits);
			return Mathf.Round(value * mult) / mult;
		}

		// ============================================================================================================
	}
}