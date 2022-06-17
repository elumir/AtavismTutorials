using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
// https://answers.unity.com/questions/1885778/extend-ui-toolkit-label-control.html

namespace Game.UI
{
	public enum ModularPart // EEE Make new enum for modular parts
	{
        Hair,
        Beard,
        Face,
        Eyebrow,
        Hands,
        LowerArms,
        UpperArms,
        Torso,
        Hips,
        LowerLegs,
        Eyes,
        Mouth,
        Head,
        Feet,
        Helmet,
		Ears
	}

	public class ModCharSlider : SliderInt
	{
		ModularPart m_ModularPart; 
		public ModularPart modularPart
		{
			get { return  m_ModularPart; }
			set
			{
				if ( m_ModularPart != value)
				{
					 m_ModularPart = value;
				}
			}
		}
		[UnityEngine.Scripting.Preserve]
		public new class UxmlFactory : UxmlFactory<ModCharSlider, UxmlTraits> { }

		[UnityEngine.Scripting.Preserve]
		public new class UxmlTraits : BaseFieldTraits<int, UxmlIntAttributeDescription>
		{
			private readonly UxmlEnumAttributeDescription<ModularPart> m_ModularPart = new UxmlEnumAttributeDescription<ModularPart> 
			{
				name = "ModularPart",
				defaultValue=ModularPart.Hair
			};

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				var item = ve as ModCharSlider;
	            item.modularPart = m_ModularPart.GetValueFromBag(bag, cc);
			}
		}
		// ------------------------------------------------------------------------------------------------------------

		static readonly string styleName = "ModCharSliderStyles";
		static readonly string s_UssClassName = "mod-char-slider";

		public ModCharSlider() 
		{ 
			AddToClassList(s_UssClassName);
			styleSheets.Add(Resources.Load<StyleSheet>($"Styles/{styleName}"));
		}

    }	
}
