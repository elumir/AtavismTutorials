// https://github.com/plyoung/UIElements
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
	public enum BodyPartColor // EEE Make new enum for colors
	{
			BodyArtColor, 
			EyeColor,
			HairColor, 
			StubbleColor, 
			ScarColor, 
			MouthColor, 
			BeardColor, 
			EyebrowColor, 
			SkinColor,
			TattooColor,
			Primary,
			Secondary,
			Tertiary,
			MetalPrimary,
			MetalSecondary,
			MetalDark,
			LeatherPrimary,
			LeatherSecondary,
			LeatherTertiary			
	}

	public class ColorField : BaseField<Color> // EEE - Removed Reset button
	{
		BodyPartColor m_BodyPartColor; // EEE - Add in enums for body part colours
		public BodyPartColor bodyPartColor
		{
			get { return m_BodyPartColor; }
			set
			{
				if (m_BodyPartColor != value)
				{
					m_BodyPartColor = value;
				}
			}
		}		

		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<ColorField, UxmlTraits> { }

		[UnityEngine.Scripting.Preserve]
		public new class UxmlTraits : BaseFieldTraits<Color, UxmlColorAttributeDescription>
		{
			// EEE - Keep track of body color
			private readonly UxmlEnumAttributeDescription<BodyPartColor> m_BodyPartColor = new UxmlEnumAttributeDescription<BodyPartColor> 
			{
				name = "BodyPartColor",
				defaultValue=BodyPartColor.SkinColor
			};			
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				var item = ve as ColorField;
	            item.bodyPartColor = m_BodyPartColor.GetValueFromBag(bag, cc); // EEE - body part colour attribute
			}
		}

		public event System.Action ResetButtonPressed;
		public ColorPopup ColorPopup { get; set; }

		private string resetLabel = null;
		private ColorFieldInput colorFieldInput;
		private Button resetButton;

//		public Color colorValue {get; set; } // EEE - store colour value

		private const string stylesResource = "GameUI/Styles/ColorFieldStyleSheet";
		private const string ussFieldName = "color-field";
		private const string ussFieldLabel = "color-field__label";
		private const string ussFieldResetButton = "color-field__reset-button";

		// ------------------------------------------------------------------------------------------------------------

		public ColorField()
			: this(null, null, new ColorFieldInput())
		{ }

		public ColorField(string label, string resetLabel = null)
			: this(label, resetLabel, new ColorFieldInput())
		{ }

		private ColorField(string label, string resetLabel, ColorFieldInput colorFieldInput)
			: base(label, colorFieldInput)
		{
			this.colorFieldInput = colorFieldInput;
//			this.resetLabel = resetLabel;

			styleSheets.Add(Resources.Load<StyleSheet>(stylesResource));
			AddToClassList(ussFieldName);

			labelElement.AddToClassList(ussFieldLabel);
			labelElement.AddToClassList("font"); // EEE Add our base font style

			colorFieldInput.RegisterCallback<ClickEvent>(OnClickEvent);

//			UpdateResetButton(); - EEE Removed Reset button. moving to popup

			RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
		}

		public override void SetValueWithoutNotify(Color newValue)
		{
			base.SetValueWithoutNotify(newValue);
//			colorValue = newValue;
			colorFieldInput.SetColor(newValue);
		}

		private void UpdateResetButton()
		{
			if (resetLabel == null)
			{
				if (resetButton != null)
				{
					Remove(resetButton);
					resetButton = null;
				}
			}
			else
			{
				if (resetButton == null)
				{
					resetButton = new Button();
					resetButton.AddToClassList(ussFieldResetButton);
					resetButton.clicked += OnResetButton;
					Add(resetButton);
				}
				resetButton.text = resetLabel;
			}
		}

		private void OnGeometryChangedEvent(GeometryChangedEvent ev)
		{
			UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
//			colorValue = value;
			colorFieldInput.SetColor(value);
		}

		private void OnClickEvent(ClickEvent ev)
		{
			ColorPopup.CurrentPart = bodyPartColor; // EEE - Set popup with current body part being coloured
			ColorPopup?.Show(value, c => value = c);
//			Debug.Log("Clicked Field");
		}

		private void OnResetButton()
		{
			ResetButtonPressed?.Invoke();
		}

		// =Removed Alpha Field==========================================================================================
		private class ColorFieldInput : VisualElement
		{
			public VisualElement rgbField;

			private const string ussFieldInput = "color-field__input";
			private const string ussFieldInputRGB = "color-field__input-rgb";

			public ColorFieldInput()
			{
				AddToClassList(ussFieldInput);

				rgbField = new VisualElement();
				rgbField.AddToClassList(ussFieldInputRGB);
				Add(rgbField);

			}

			public void SetColor(Color color)
			{
				// Need to change unity-background-image-tint-color
				rgbField.style.unityBackgroundImageTintColor = new Color(color.r, color.g, color.b, 1f);
//				rgbField.style.tint = new Color(color.r, color.g, color.b, 1f);
			}
		}

		// ============================================================================================================
	}
}