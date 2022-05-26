using HNGamers.Atavism;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Atavism
{
    [CustomEditor(typeof(ModularCustomizationManager))]
     class ModularCustomizationManagerEditor : Editor
    {
        ModularCustomizationManager script;
        SerializedObject thisScriptObject;
        GUIContent[] slots;
        SerializedProperty gender;
        SerializedProperty replacementObject;
        SerializedProperty headReplacementObject;
        SerializedProperty torsoReplacementObject;
        SerializedProperty upperArmsReplacementObject;
        SerializedProperty lowerArmsReplacementObject;
        SerializedProperty hipsReplacementObject;
        SerializedProperty lowerLegsReplacementObject;
        SerializedProperty feetReplacementObject;
        SerializedProperty handsReplacementObject;
        SerializedProperty modularModelSwapping;
#if ModularNHance
        SerializedProperty nHanceModelSwapping ;
        SerializedProperty defaultTorso;
        SerializedProperty defaultHands;
        SerializedProperty defaultFeet;
        SerializedProperty defaultLegs;
#endif
        SerializedProperty bodyMat;
        SerializedProperty headMat;
        SerializedProperty hairMat;
        SerializedProperty eyeMat;
        SerializedProperty mouthMat;
        SerializedProperty skinMaterialsToSync;
        SerializedProperty hairMaterialsToSync;
        SerializedProperty bodyMatList;
        SerializedProperty headMatList;
        SerializedProperty hairMatList;
        SerializedProperty eyeMatList;
        SerializedProperty mouthMatList;
        SerializedProperty enableInstancing;
        SerializedProperty bodyShader;
        SerializedProperty headShader;
        SerializedProperty hairShader;
        SerializedProperty eyeShader;
        SerializedProperty mouthShader;
        SerializedProperty directSetHairColor;
        SerializedProperty directSetEyeColor;
        SerializedProperty directSetSkinColor;
        SerializedProperty useBeardColorForColor2;
        SerializedProperty enableMultipleCharacterColors;
        SerializedProperty completelyDisableInstancedMaterials;
        SerializedProperty internalMat;
        SerializedProperty internalHandsMat;
        SerializedProperty internalLowerArmsMat;
        SerializedProperty internalUpperArmsMat;
        SerializedProperty internalTorsoMat;
        SerializedProperty internalHipsMat;
        SerializedProperty internalHelmetMat;
        SerializedProperty internalLowerLegsMat;
        SerializedProperty internalFeetMat;
        SerializedProperty internalHeadMat;
        SerializedProperty internalHairMat;
        SerializedProperty internalMouthMat;
        SerializedProperty internalEyeMat;
        SerializedProperty internalBeardMat;
        SerializedProperty internalEyebrowMat;
        SerializedProperty allowDifferentHairColors;
        SerializedProperty internalHairModels;
        SerializedProperty internalBeardModels;
        SerializedProperty internalEyebrowModels;
        SerializedProperty internalMouthModels;
        SerializedProperty internalEarModels;
        SerializedProperty internalEyeModels;
        SerializedProperty internalTuskModels;

        /*
        SerializedProperty parentJoint;
        SerializedProperty beardJoint;
        SerializedProperty eyeBrowJoint;
        SerializedProperty earJoint;
        SerializedProperty mouthJoint;
        SerializedProperty eyeJoint;
        SerializedProperty rootBone;*/

        SerializedProperty headSlot;
        SerializedProperty headCoveringSlot;

        SerializedProperty torsoSlot;
        SerializedProperty upperarmSlot;
        SerializedProperty lowerarmSlot;
        SerializedProperty handsSlot;

        SerializedProperty hipSlot;
        SerializedProperty upperlegSlot;
        SerializedProperty lowerlegSlot;
        SerializedProperty feetSlot;
        SerializedProperty parentSlot;
        SerializedProperty beardSlot;
        SerializedProperty eyebrowSlot;
        SerializedProperty earSlot;
        SerializedProperty eyeSlot;
        SerializedProperty mouthSlot;
        SerializedProperty rootSlot;


        SerializedProperty headModels;
        SerializedProperty torsoModels;
        SerializedProperty upperArmModels;
        SerializedProperty lowerArmModels;
        SerializedProperty handModels;
        SerializedProperty hipModels;
        SerializedProperty lowerLegModels;
        SerializedProperty feetModels;


        SerializedProperty skinColors;
        SerializedProperty hairColors;
        SerializedProperty eyeColors;
        SerializedProperty stubbleColors;
        SerializedProperty bodyArtColors;
        SerializedProperty scarColors;

#if IPBRInt
        SerializedProperty blendshapePresetObjects;
SerializedProperty infinityPBRIntegration;
        SerializedProperty enableSavingInfinityPBRBlendshapes;
#endif
        SerializedProperty helmetModels;
        SerializedProperty hairModels;
        SerializedProperty faceModels;
        SerializedProperty beardModels;
        SerializedProperty eyebrowModels;
        SerializedProperty earModels;
        SerializedProperty tuskModels;
        SerializedProperty mouthModels;
        SerializedProperty eyeModels;
        SerializedProperty replacementItems;
        SerializedProperty faceTexPropertyName;
        SerializedProperty hairPropertyName;
        SerializedProperty beardPropertyName;
        SerializedProperty eyebrowPropertyName;
        SerializedProperty headPropertyName;
        SerializedProperty handsPropertyName;
        SerializedProperty lowerArmsPropertyName;
        SerializedProperty upperArmsPropertyName;
        SerializedProperty torsoPropertyName;
        SerializedProperty hipsPropertyName;
        SerializedProperty lowerLegsPropertyName;
        SerializedProperty feetPropertyName;
        SerializedProperty mouthPropertyName;
        SerializedProperty eyesPropertyName;
        SerializedProperty tuskPropertyName;
        SerializedProperty earsPropertyName;
        SerializedProperty EyeMaterialPropertyName;
        SerializedProperty HairMaterialPropertyName;
        SerializedProperty SkinMaterialPropertyName;
        SerializedProperty bodyColorPropertyName;
        SerializedProperty scarColorPropertyName;
        SerializedProperty mouthColorPropertyName;
        SerializedProperty hairColorPropertyName;
        SerializedProperty stubbleColorPropertyName;
        SerializedProperty beardColorPropertyName;
        SerializedProperty eyeBrowColorPropertyName;
        SerializedProperty bodyArtColorPropertyName;
        SerializedProperty eyeColorPropertyName;
        SerializedProperty helmetColorPropertyName;
        SerializedProperty torsoColorPropertyName;
        SerializedProperty upperArmsColorPropertyName;
        SerializedProperty lowerArmsColorPropertyName;
        SerializedProperty hipsColorPropertyName;
        SerializedProperty lowerLegsColorPropertyName;
        SerializedProperty handsColorPropertyName;
        SerializedProperty feetColorPropertyName;
        SerializedProperty faithPropertyName;
        SerializedProperty infinityBlendShapes;
        SerializedProperty blendshapePresetValue;
        SerializedProperty hairDirectory;
        SerializedProperty FhairDirectory;
        SerializedProperty beardDirectory;
        SerializedProperty fbeardDirectory;
        SerializedProperty eyebrowDirectory;
        SerializedProperty feyebrowDirectory;
        SerializedProperty defaultHeadName;
        SerializedProperty defaultFaceName;
        SerializedProperty defaultTorsoName;
        SerializedProperty defaultUpperArmName;
        SerializedProperty defaultLowerArmName;
        SerializedProperty defaultHandName;
        SerializedProperty defaultHipsName;
        SerializedProperty defaultLowerLegName;
        SerializedProperty defaultFeetName;
        SerializedProperty defaultMouthColorName;
        SerializedProperty defaultEyeColorName;
        SerializedProperty defaultEarName;
        SerializedProperty defaultTuskName;
        SerializedProperty defaultBeardName;
        SerializedProperty defaultHairName;
        SerializedProperty defaultFemaleHeadName;
        SerializedProperty defaultFemaleFaceName;
        SerializedProperty defaultFemaleTorsoName;
        SerializedProperty defaultFemaleUpperArmName;
        SerializedProperty defaultFemaleLowerArmName;
        SerializedProperty defaultFemaleHandName;
        SerializedProperty defaultFemaleHipsName;
        SerializedProperty defaultFemaleLowerLegName;
        SerializedProperty defaultFemaleFeetName;
        SerializedProperty defaultFemaleMouthName;
        SerializedProperty defaultFemaleEyeName;
        SerializedProperty defaultFemaleEarName;
        SerializedProperty defaultFemaleTuskName;
        SerializedProperty defaultFemaleBeardName;
        SerializedProperty defaultFemaleHairName;
        SerializedProperty defaultFaithName;
        SerializedProperty defaultSkinColor;
        SerializedProperty defaultHairColor;
        SerializedProperty defaultScarColor;
        SerializedProperty defaultStubbleColor;
        SerializedProperty defaultBodyArtColor;
        SerializedProperty defaultEyeColor;
        SerializedProperty defaultMouthColor;
        SerializedProperty defaultPrimaryColor;
        SerializedProperty defaultSecondaryColor;
        SerializedProperty defaultTertiaryColor;
        SerializedProperty defaultLeatherPrimaryColor;
        SerializedProperty defaultLeatherTertiaryColor;
        SerializedProperty defaultMetalPrimaryColor;
        SerializedProperty defaultLeatherSecondaryColor;
        SerializedProperty defaultMetalDarkColor;
        SerializedProperty defaultMetalSecondaryColor;
        SerializedProperty switchHair;
        SerializedProperty switchBeard;
        SerializedProperty switchEyebrow;
        SerializedProperty switchEye;
        SerializedProperty switchMouth;
        SerializedProperty switchTusk;
        SerializedProperty switchEar;
        SerializedProperty eyeMaterialIndex;
        SerializedProperty skinMaterialIndex;
        SerializedProperty hairMaterialIndex;
        SerializedProperty handIndex;
        SerializedProperty lowerArmIndex;
        SerializedProperty upperArmIndex;
        SerializedProperty torsoIndex;
        SerializedProperty hipsIndex;
        SerializedProperty lowerLegIndex;
        SerializedProperty feetIndex;
        SerializedProperty switchHead;
        SerializedProperty switchFace;
        SerializedProperty blendshapePresetObjectsIndex;
        SerializedProperty replacementIndex;
        SerializedProperty defaultCapeName;
        SerializedProperty defaultFemaleCapeName;
        
        SerializedProperty myGUID;

        private void OnEnable()
        {
            script = (ModularCustomizationManager)target;
            thisScriptObject = new SerializedObject(target);


            gender = thisScriptObject.FindProperty("gender");
            replacementObject = thisScriptObject.FindProperty("replacementObject");
            headReplacementObject = thisScriptObject.FindProperty("headReplacementObject");
            torsoReplacementObject = thisScriptObject.FindProperty("torsoReplacementObject");
            upperArmsReplacementObject = thisScriptObject.FindProperty("upperArmsReplacementObject");
            lowerArmsReplacementObject = thisScriptObject.FindProperty("lowerArmsReplacementObject");
            hipsReplacementObject = thisScriptObject.FindProperty("hipsReplacementObject");
            lowerLegsReplacementObject = thisScriptObject.FindProperty("lowerLegsReplacementObject");
            feetReplacementObject = thisScriptObject.FindProperty("feetReplacementObject");
            handsReplacementObject = thisScriptObject.FindProperty("handsReplacementObject");
            modularModelSwapping = thisScriptObject.FindProperty("modularModelSwapping");
#if ModularNHance
            nHanceModelSwapping = thisScriptObject.FindProperty("nHanceModelSwapping");
            defaultTorso = thisScriptObject.FindProperty("defaultTorso");
            defaultHands = thisScriptObject.FindProperty("defaultHands");
            defaultFeet = thisScriptObject.FindProperty("defaultFeet");
            defaultLegs = thisScriptObject.FindProperty("defaultLegs");
#endif
            bodyMat = thisScriptObject.FindProperty("bodyMat");
            headMat = thisScriptObject.FindProperty("headMat");
            hairMat = thisScriptObject.FindProperty("hairMat");
            eyeMat = thisScriptObject.FindProperty("eyeMat");
            mouthMat = thisScriptObject.FindProperty("mouthMat");
            skinMaterialsToSync = thisScriptObject.FindProperty("skinMaterialsToSync");
            hairMaterialsToSync = thisScriptObject.FindProperty("hairMaterialsToSync");
            bodyMatList = thisScriptObject.FindProperty("bodyMatList");
            headMatList = thisScriptObject.FindProperty("headMatList");
            hairMatList = thisScriptObject.FindProperty("hairMatList");
            eyeMatList = thisScriptObject.FindProperty("eyeMatList");
            mouthMatList = thisScriptObject.FindProperty("mouthMatList");
            enableInstancing = thisScriptObject.FindProperty("enableInstancing");
            bodyShader = thisScriptObject.FindProperty("bodyShader");
            headShader = thisScriptObject.FindProperty("headShader");
            hairShader = thisScriptObject.FindProperty("hairShader");
            eyeShader = thisScriptObject.FindProperty("eyeShader");
            mouthShader = thisScriptObject.FindProperty("mouthShader");
            directSetHairColor = thisScriptObject.FindProperty("directSetHairColor");
            directSetEyeColor = thisScriptObject.FindProperty("directSetEyeColor");
            directSetSkinColor = thisScriptObject.FindProperty("directSetSkinColor");
            useBeardColorForColor2 = thisScriptObject.FindProperty("useBeardColorForColor2");
            enableMultipleCharacterColors = thisScriptObject.FindProperty("enableMultipleCharacterColors");
            completelyDisableInstancedMaterials = thisScriptObject.FindProperty("completelyDisableInstancedMaterials");
            internalMat = thisScriptObject.FindProperty("internalMat");
            internalHandsMat = thisScriptObject.FindProperty("internalHandsMat");
            internalLowerArmsMat = thisScriptObject.FindProperty("internalLowerArmsMat");
            internalUpperArmsMat = thisScriptObject.FindProperty("internalUpperArmsMat");
            internalTorsoMat = thisScriptObject.FindProperty("internalTorsoMat");
            internalHipsMat = thisScriptObject.FindProperty("internalHipsMat");
            internalHelmetMat = thisScriptObject.FindProperty("internalHelmetMat");
            internalLowerLegsMat = thisScriptObject.FindProperty("internalLowerLegsMat");
            internalFeetMat = thisScriptObject.FindProperty("internalFeetMat");
            internalHeadMat = thisScriptObject.FindProperty("internalHeadMat");
            internalHairMat = thisScriptObject.FindProperty("internalHairMat");
            internalMouthMat = thisScriptObject.FindProperty("internalMouthMat");
            internalEyeMat = thisScriptObject.FindProperty("internalEyeMat");
            internalBeardMat = thisScriptObject.FindProperty("internalBeardMat");
            internalEyebrowMat = thisScriptObject.FindProperty("internalEyebrowMat");
            allowDifferentHairColors = thisScriptObject.FindProperty("allowDifferentHairColors");
            internalHairModels = thisScriptObject.FindProperty("internalHairModels");
            internalBeardModels = thisScriptObject.FindProperty("internalBeardModels");
            internalEyebrowModels = thisScriptObject.FindProperty("internalEyebrowModels");
            internalMouthModels = thisScriptObject.FindProperty("internalMouthModels");
            internalEarModels = thisScriptObject.FindProperty("internalEarModels");
            internalEyeModels = thisScriptObject.FindProperty("internalEyeModels");
            internalTuskModels = thisScriptObject.FindProperty("internalTuskModels");
            /*
            parentJoint = thisScriptObject.FindProperty("parentJoint");
            beardJoint = thisScriptObject.FindProperty("beardJoint");
            eyeBrowJoint = thisScriptObject.FindProperty("eyeBrowJoint");
            earJoint = thisScriptObject.FindProperty("earJoint");
            mouthJoint = thisScriptObject.FindProperty("mouthJoint");
            eyeJoint = thisScriptObject.FindProperty("eyeJoint");
            rootBone = thisScriptObject.FindProperty("rootBone");*/

            headSlot = thisScriptObject.FindProperty("headSlot");
            headCoveringSlot = thisScriptObject.FindProperty("headCoveringSlot");

            torsoSlot = thisScriptObject.FindProperty("torsoSlot");
            upperarmSlot = thisScriptObject.FindProperty("upperarmSlot");
            lowerarmSlot = thisScriptObject.FindProperty("lowerarmSlot");
            hipSlot = thisScriptObject.FindProperty("hipSlot");
            upperlegSlot = thisScriptObject.FindProperty("upperlegSlot");
            lowerlegSlot = thisScriptObject.FindProperty("lowerlegSlot");
            feetSlot = thisScriptObject.FindProperty("feetSlot");
            parentSlot = thisScriptObject.FindProperty("parentSlot");
            beardSlot = thisScriptObject.FindProperty("beardSlot");
            eyebrowSlot = thisScriptObject.FindProperty("eyebrowSlot");
            earSlot = thisScriptObject.FindProperty("earSlot");
            eyeSlot = thisScriptObject.FindProperty("eyeSlot");
            mouthSlot = thisScriptObject.FindProperty("mouthSlot");
            rootSlot = thisScriptObject.FindProperty("rootSlot");
            handsSlot = thisScriptObject.FindProperty("handsSlot");



            headModels = thisScriptObject.FindProperty("headModels");
            torsoModels = thisScriptObject.FindProperty("torsoModels");
            upperArmModels = thisScriptObject.FindProperty("upperArmModels");
            lowerArmModels = thisScriptObject.FindProperty("lowerArmModels");
            handModels = thisScriptObject.FindProperty("handModels");
            hipModels = thisScriptObject.FindProperty("hipModels");
            lowerLegModels = thisScriptObject.FindProperty("lowerLegModels");
            feetModels = thisScriptObject.FindProperty("feetModels");
            skinColors = thisScriptObject.FindProperty("skinColors");
            hairColors = thisScriptObject.FindProperty("hairColors");
            eyeColors = thisScriptObject.FindProperty("eyeColors");

            stubbleColors = thisScriptObject.FindProperty("stubbleColors");
            bodyArtColors = thisScriptObject.FindProperty("bodyArtColors");
            scarColors = thisScriptObject.FindProperty("scarColors");
#if IPBRInt
        blendshapePresetObjects = thisScriptObject.FindProperty("blendshapePresetObjects");
            infinityPBRIntegration = thisScriptObject.FindProperty("infinityPBRIntegration");
            enableSavingInfinityPBRBlendshapes = thisScriptObject.FindProperty("enableSavingInfinityPBRBlendshapes");
#endif
            helmetModels = thisScriptObject.FindProperty("helmetModels");
            hairModels = thisScriptObject.FindProperty("hairModels");
            faceModels = thisScriptObject.FindProperty("faceModels");
            beardModels = thisScriptObject.FindProperty("beardModels");
            eyebrowModels = thisScriptObject.FindProperty("eyebrowModels");
            earModels = thisScriptObject.FindProperty("earModels");
            tuskModels = thisScriptObject.FindProperty("tuskModels");
            mouthModels = thisScriptObject.FindProperty("mouthModels");
            eyeModels = thisScriptObject.FindProperty("eyeModels");
            replacementItems = thisScriptObject.FindProperty("replacementItems");
            faceTexPropertyName = thisScriptObject.FindProperty("faceTexPropertyName");
            hairPropertyName = thisScriptObject.FindProperty("hairPropertyName");
            beardPropertyName = thisScriptObject.FindProperty("beardPropertyName");
            eyebrowPropertyName = thisScriptObject.FindProperty("eyebrowPropertyName");
            headPropertyName = thisScriptObject.FindProperty("headPropertyName");
            handsPropertyName = thisScriptObject.FindProperty("handsPropertyName");
            lowerArmsPropertyName = thisScriptObject.FindProperty("lowerArmsPropertyName");
            upperArmsPropertyName = thisScriptObject.FindProperty("upperArmsPropertyName");
            torsoPropertyName = thisScriptObject.FindProperty("torsoPropertyName");
            hipsPropertyName = thisScriptObject.FindProperty("hipsPropertyName");
            lowerLegsPropertyName = thisScriptObject.FindProperty("lowerLegsPropertyName");
            feetPropertyName = thisScriptObject.FindProperty("feetPropertyName");
            mouthPropertyName = thisScriptObject.FindProperty("mouthPropertyName");
            eyesPropertyName = thisScriptObject.FindProperty("eyesPropertyName");
            tuskPropertyName = thisScriptObject.FindProperty("tuskPropertyName");
            earsPropertyName = thisScriptObject.FindProperty("earsPropertyName");
            EyeMaterialPropertyName = thisScriptObject.FindProperty("EyeMaterialPropertyName");
            HairMaterialPropertyName = thisScriptObject.FindProperty("HairMaterialPropertyName");
            SkinMaterialPropertyName = thisScriptObject.FindProperty("SkinMaterialPropertyName");
            bodyColorPropertyName = thisScriptObject.FindProperty("bodyColorPropertyName");
            scarColorPropertyName = thisScriptObject.FindProperty("scarColorPropertyName");
            mouthColorPropertyName = thisScriptObject.FindProperty("mouthColorPropertyName");
            hairColorPropertyName = thisScriptObject.FindProperty("hairColorPropertyName");
            stubbleColorPropertyName = thisScriptObject.FindProperty("stubbleColorPropertyName");
            beardColorPropertyName = thisScriptObject.FindProperty("beardColorPropertyName");
            eyeBrowColorPropertyName = thisScriptObject.FindProperty("eyeBrowColorPropertyName");
            bodyArtColorPropertyName = thisScriptObject.FindProperty("bodyArtColorPropertyName");
            eyeColorPropertyName = thisScriptObject.FindProperty("eyeColorPropertyName");
            helmetColorPropertyName = thisScriptObject.FindProperty("helmetColorPropertyName");
            torsoColorPropertyName = thisScriptObject.FindProperty("torsoColorPropertyName");
            upperArmsColorPropertyName = thisScriptObject.FindProperty("upperArmsColorPropertyName");
            lowerArmsColorPropertyName = thisScriptObject.FindProperty("lowerArmsColorPropertyName");
            hipsColorPropertyName = thisScriptObject.FindProperty("hipsColorPropertyName");
            lowerLegsColorPropertyName = thisScriptObject.FindProperty("lowerLegsColorPropertyName");
            handsColorPropertyName = thisScriptObject.FindProperty("handsColorPropertyName");
            feetColorPropertyName = thisScriptObject.FindProperty("feetColorPropertyName");
            faithPropertyName = thisScriptObject.FindProperty("faithPropertyName");
            infinityBlendShapes = thisScriptObject.FindProperty("infinityBlendShapes");
            blendshapePresetValue = thisScriptObject.FindProperty("blendshapePresetValue");
            hairDirectory = thisScriptObject.FindProperty("hairDirectory");
            FhairDirectory = thisScriptObject.FindProperty("FhairDirectory");
            beardDirectory = thisScriptObject.FindProperty("beardDirectory");
            fbeardDirectory = thisScriptObject.FindProperty("fbeardDirectory");
            eyebrowDirectory = thisScriptObject.FindProperty("eyebrowDirectory");
            feyebrowDirectory = thisScriptObject.FindProperty("feyebrowDirectory");
            defaultHeadName = thisScriptObject.FindProperty("defaultHeadName");
            defaultFaceName = thisScriptObject.FindProperty("defaultFaceName");
            defaultTorsoName = thisScriptObject.FindProperty("defaultTorsoName");
            defaultUpperArmName = thisScriptObject.FindProperty("defaultUpperArmName");
            defaultLowerArmName = thisScriptObject.FindProperty("defaultLowerArmName");
            defaultHandName = thisScriptObject.FindProperty("defaultHandName");
            defaultHipsName = thisScriptObject.FindProperty("defaultHipsName");
            defaultLowerLegName = thisScriptObject.FindProperty("defaultLowerLegName");
            defaultFeetName = thisScriptObject.FindProperty("defaultFeetName");
            defaultMouthColorName = thisScriptObject.FindProperty("defaultMouthColorName");
            defaultEyeColorName = thisScriptObject.FindProperty("defaultEyeColorName");
            defaultEarName = thisScriptObject.FindProperty("defaultEarName");
            defaultTuskName = thisScriptObject.FindProperty("defaultTuskName");
            defaultBeardName = thisScriptObject.FindProperty("defaultBeardName");
            defaultHairName = thisScriptObject.FindProperty("defaultHairName");
            defaultFemaleHeadName = thisScriptObject.FindProperty("defaultFemaleHeadName");
            defaultFemaleFaceName = thisScriptObject.FindProperty("defaultFemaleFaceName");
            defaultFemaleTorsoName = thisScriptObject.FindProperty("defaultFemaleTorsoName");
            defaultFemaleUpperArmName = thisScriptObject.FindProperty("defaultFemaleUpperArmName");
            defaultFemaleLowerArmName = thisScriptObject.FindProperty("defaultFemaleLowerArmName");
            defaultFemaleHandName = thisScriptObject.FindProperty("defaultFemaleHandName");
            defaultFemaleHipsName = thisScriptObject.FindProperty("defaultFemaleHipsName");
            defaultFemaleLowerLegName = thisScriptObject.FindProperty("defaultFemaleLowerLegName");
            defaultFemaleFeetName = thisScriptObject.FindProperty("defaultFemaleFeetName");
            defaultFemaleMouthName = thisScriptObject.FindProperty("defaultFemaleMouthName");
            defaultFemaleEyeName = thisScriptObject.FindProperty("defaultFemaleEyeName");
            defaultFemaleEarName = thisScriptObject.FindProperty("defaultFemaleEarName");
            defaultFemaleTuskName = thisScriptObject.FindProperty("defaultFemaleTuskName");
            defaultFemaleBeardName = thisScriptObject.FindProperty("defaultFemaleBeardName");
            defaultFemaleHairName = thisScriptObject.FindProperty("defaultFemaleHairName");
            defaultFaithName = thisScriptObject.FindProperty("defaultFaithName");
            defaultSkinColor = thisScriptObject.FindProperty("defaultSkinColor");
            defaultHairColor = thisScriptObject.FindProperty("defaultHairColor");
            defaultScarColor = thisScriptObject.FindProperty("defaultScarColor");
            defaultStubbleColor = thisScriptObject.FindProperty("defaultStubbleColor");
            defaultBodyArtColor = thisScriptObject.FindProperty("defaultBodyArtColor");
            defaultEyeColor = thisScriptObject.FindProperty("defaultEyeColor");
            defaultMouthColor = thisScriptObject.FindProperty("defaultMouthColor");
            defaultPrimaryColor = thisScriptObject.FindProperty("defaultPrimaryColor");
            defaultSecondaryColor = thisScriptObject.FindProperty("defaultSecondaryColor");
            defaultTertiaryColor = thisScriptObject.FindProperty("defaultTertiaryColor");
            defaultLeatherPrimaryColor = thisScriptObject.FindProperty("defaultLeatherPrimaryColor");
            defaultLeatherTertiaryColor = thisScriptObject.FindProperty("defaultLeatherTertiaryColor");
            defaultMetalPrimaryColor = thisScriptObject.FindProperty("defaultMetalPrimaryColor");
            defaultLeatherSecondaryColor = thisScriptObject.FindProperty("defaultLeatherSecondaryColor");
            defaultMetalDarkColor = thisScriptObject.FindProperty("defaultMetalDarkColor");
            defaultMetalSecondaryColor = thisScriptObject.FindProperty("defaultMetalSecondaryColor");
            switchHair = thisScriptObject.FindProperty("switchHair");
            switchBeard = thisScriptObject.FindProperty("switchBeard");
            switchEyebrow = thisScriptObject.FindProperty("switchEyebrow");
            switchEye = thisScriptObject.FindProperty("switchEye");
            switchMouth = thisScriptObject.FindProperty("switchMouth");
            switchTusk = thisScriptObject.FindProperty("switchTusk");
            switchEar = thisScriptObject.FindProperty("switchEar");
            eyeMaterialIndex = thisScriptObject.FindProperty("eyeMaterialIndex");
            skinMaterialIndex = thisScriptObject.FindProperty("skinMaterialIndex");
            hairMaterialIndex = thisScriptObject.FindProperty("hairMaterialIndex");
            handIndex = thisScriptObject.FindProperty("handIndex");
            lowerArmIndex = thisScriptObject.FindProperty("lowerArmIndex");
            upperArmIndex = thisScriptObject.FindProperty("upperArmIndex");
            torsoIndex = thisScriptObject.FindProperty("torsoIndex");
            hipsIndex = thisScriptObject.FindProperty("hipsIndex");
            lowerLegIndex = thisScriptObject.FindProperty("lowerLegIndex");
            feetIndex = thisScriptObject.FindProperty("feetIndex");
            switchHead = thisScriptObject.FindProperty("switchHead");
            switchFace = thisScriptObject.FindProperty("switchFace");
            blendshapePresetObjectsIndex = thisScriptObject.FindProperty("blendshapePresetObjectsIndex");
            replacementIndex = thisScriptObject.FindProperty("replacementIndex");
            defaultCapeName = thisScriptObject.FindProperty("defaultCapeName");
            defaultFemaleCapeName = thisScriptObject.FindProperty("defaultFemaleCapeName");
        }

        public override void OnInspectorGUI()
        {
            thisScriptObject.Update();
            EditorGUI.BeginChangeCheck();


            script.toolbarTop = GUILayout.Toolbar(script.toolbarTop,
                new string[] { "General", "Parts", "Replacement" });
            switch (script.toolbarTop)
            {
                case 0:
                    script.toolbarBottom = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "General";
                    break;
                case 1:
                    script.toolbarBottom = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "Parts";
                    break;
                case 2:
                    script.toolbarBottom = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "Replacement";
                    break;
            }

            script.toolbarMiddle = GUILayout.Toolbar(script.toolbarMiddle,
    new string[] { "nHance", "InfinityPBR", "Model Parts" });
            switch (script.toolbarMiddle)
            {
                case 0:
                    script.toolbarBottom = 99;
                    script.toolbarTop = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "nHance";
                    break;
                case 1:
                    script.toolbarBottom = 99;
                    script.toolbarTop = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "InfinityPBR";
                    break;
                case 2:
                    script.toolbarBottom = 99;
                    script.toolbarTop = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "Model Parts";
                    break;
            }

            script.toolbarBottom = GUILayout.Toolbar(script.toolbarBottom,
                new string[] { "Directories", "Materials", "Shaders" });


            switch (script.toolbarBottom)
            {
                case 0:
                    script.toolbarTop = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "Directories";
                    break;
                case 1:
                    script.toolbarTop = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "Materials";
                    break;
                case 2:
                    script.toolbarTop = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarSubBottom = 99;

                    script.currentTab = "Shaders";
                    break;
            }


            script.toolbarSubBottom = GUILayout.Toolbar(script.toolbarSubBottom,
                new string[] { "Color Options", "Default Colors", "Color Selections" });


            switch (script.toolbarSubBottom)
            {
                case 0:
                    script.toolbarTop = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarBottom = 99;

                    script.currentTab = "Color Options";
                    break;

                case 1:
                    script.toolbarTop = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarBottom = 99;

                    script.currentTab = "Default Colors";
                    break;
                case 2:
                    script.toolbarTop = 99;
                    script.toolbarMiddle = 99;
                    script.toolbarBottom = 99;

                    script.currentTab = "Color Selections";
                    break;
            }




            if (EditorGUI.EndChangeCheck())
            {
                thisScriptObject.ApplyModifiedProperties();
                GUI.FocusControl(null);
            }
            EditorGUI.BeginChangeCheck();

            switch (script.currentTab)
            {
                case "Model Parts":
                    if (gender.enumValueIndex == 0)
                    {
                        EditorGUILayout.PropertyField(defaultHeadName);
                        EditorGUILayout.PropertyField(defaultFaceName);
                        EditorGUILayout.PropertyField(defaultTorsoName);
                        EditorGUILayout.PropertyField(defaultUpperArmName);
                        EditorGUILayout.PropertyField(defaultLowerArmName);
                        EditorGUILayout.PropertyField(defaultHandName);
                        EditorGUILayout.PropertyField(defaultHipsName);
                        EditorGUILayout.PropertyField(defaultLowerLegName);
                        EditorGUILayout.PropertyField(defaultFeetName);
                        EditorGUILayout.PropertyField(defaultMouthColorName);
                        EditorGUILayout.PropertyField(defaultEyeColorName);
                        EditorGUILayout.PropertyField(defaultEarName);
                        EditorGUILayout.PropertyField(defaultTuskName);
                        EditorGUILayout.PropertyField(defaultBeardName);
                        EditorGUILayout.PropertyField(defaultHairName);
                        EditorGUILayout.PropertyField(defaultCapeName);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(defaultFemaleHeadName);
                        EditorGUILayout.PropertyField(defaultFemaleFaceName);
                        EditorGUILayout.PropertyField(defaultFemaleTorsoName);
                        EditorGUILayout.PropertyField(defaultFemaleUpperArmName);
                        EditorGUILayout.PropertyField(defaultFemaleLowerArmName);
                        EditorGUILayout.PropertyField(defaultFemaleHandName);
                        EditorGUILayout.PropertyField(defaultFemaleHipsName);
                        EditorGUILayout.PropertyField(defaultFemaleLowerLegName);
                        EditorGUILayout.PropertyField(defaultFemaleFeetName);
                        EditorGUILayout.PropertyField(defaultFemaleMouthName);
                        EditorGUILayout.PropertyField(defaultFemaleEyeName);
                        EditorGUILayout.PropertyField(defaultFemaleEarName);
                        EditorGUILayout.PropertyField(defaultFemaleTuskName);
                        EditorGUILayout.PropertyField(defaultFemaleBeardName);
                        EditorGUILayout.PropertyField(defaultFemaleHairName);
                        EditorGUILayout.PropertyField(defaultFemaleCapeName);
                    }
                    break;
                case "Directories":
                    EditorGUILayout.PropertyField(hairDirectory);
                    EditorGUILayout.PropertyField(FhairDirectory);
                    EditorGUILayout.PropertyField(beardDirectory);
                    EditorGUILayout.PropertyField(fbeardDirectory);
                    EditorGUILayout.PropertyField(eyebrowDirectory);
                    EditorGUILayout.PropertyField(feyebrowDirectory);
                    break;
                case "Materials":
                    EditorGUILayout.PropertyField(enableInstancing);

                    EditorGUILayout.PropertyField(bodyMat);
                    EditorGUILayout.PropertyField(headMat);
                    EditorGUILayout.PropertyField(hairMat);
                    EditorGUILayout.PropertyField(eyeMat);
                    EditorGUILayout.PropertyField(mouthMat);

                    EditorGUILayout.PropertyField(skinMaterialsToSync);
                    EditorGUILayout.PropertyField(hairMaterialsToSync);
                    EditorGUILayout.PropertyField(bodyMatList);
                    EditorGUILayout.PropertyField(headMatList);
                    EditorGUILayout.PropertyField(hairMatList);
                    EditorGUILayout.PropertyField(eyeMatList);
                    EditorGUILayout.PropertyField(mouthMatList);
                    break;
                case "Shaders":
                    EditorGUILayout.PropertyField(bodyShader);
                    EditorGUILayout.PropertyField(headShader);
                    EditorGUILayout.PropertyField(hairShader);
                    EditorGUILayout.PropertyField(eyeShader);
                    EditorGUILayout.PropertyField(mouthShader);
                    break;
                case "Default Colors":

                    EditorGUILayout.PropertyField(defaultSkinColor);
                    EditorGUILayout.PropertyField(defaultHairColor);
                    EditorGUILayout.PropertyField(defaultScarColor);
                    EditorGUILayout.PropertyField(defaultStubbleColor);
                    EditorGUILayout.PropertyField(defaultBodyArtColor);
                    EditorGUILayout.PropertyField(defaultEyeColor);
                    EditorGUILayout.PropertyField(defaultMouthColor);
                    EditorGUILayout.PropertyField(defaultPrimaryColor);
                    EditorGUILayout.PropertyField(defaultSecondaryColor);
                    EditorGUILayout.PropertyField(defaultTertiaryColor);
                    EditorGUILayout.PropertyField(defaultLeatherPrimaryColor);
                    EditorGUILayout.PropertyField(defaultLeatherTertiaryColor);
                    EditorGUILayout.PropertyField(defaultMetalPrimaryColor);
                    EditorGUILayout.PropertyField(defaultLeatherSecondaryColor);
                    EditorGUILayout.PropertyField(defaultMetalDarkColor);
                    EditorGUILayout.PropertyField(defaultMetalSecondaryColor);
                    break;
                case "Color Selections":
                    EditorGUILayout.PropertyField(skinColors);
                    EditorGUILayout.PropertyField(hairColors);
                    EditorGUILayout.PropertyField(eyeColors);
                    EditorGUILayout.PropertyField(stubbleColors);
                    EditorGUILayout.PropertyField(bodyArtColors);
                    EditorGUILayout.PropertyField(scarColors);
                    break;
                case "Color Options":


                    EditorGUILayout.PropertyField(allowDifferentHairColors);
                    EditorGUILayout.PropertyField(directSetHairColor);
                    EditorGUILayout.PropertyField(directSetEyeColor);
                    EditorGUILayout.PropertyField(directSetSkinColor);
                    EditorGUILayout.PropertyField(useBeardColorForColor2);
                    EditorGUILayout.PropertyField(enableMultipleCharacterColors);
                    EditorGUILayout.PropertyField(completelyDisableInstancedMaterials);

                    break;
                case "General":
                    EditorGUILayout.PropertyField(gender);
                    EditorGUILayout.PropertyField(defaultFaithName);

                    /*
                    EditorGUILayout.PropertyField(parentJoint, new GUIContent("Hair Root Joint"), true);
                    EditorGUILayout.PropertyField(beardJoint);
                    EditorGUILayout.PropertyField(eyeBrowJoint);
                    EditorGUILayout.PropertyField(earJoint);
                    EditorGUILayout.PropertyField(mouthJoint);
                    EditorGUILayout.PropertyField(eyeJoint);
                    EditorGUILayout.PropertyField(rootBone);*/

                    GUIContent content = new GUIContent("Slot Name");
                    content.tooltip = "Select Slot Name for field";

                    GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                    boxStyle.normal.textColor = Color.cyan;
                    boxStyle.fontStyle = FontStyle.Bold;
                    boxStyle.alignment = TextAnchor.UpperLeft;


                    //GUILayout.BeginVertical("", boxStyle);
                    //  DamageType dt = obj.damageTypeColor[key];


                    content.tooltip = "Select Slot Name for field";
                    if (slots == null)
                    {
                        slots = ServerItems.LoadSlotsOptions();
                    }
                    if (script.slots == null)
                        script.slots = LoadSlotsOptions();

                    content = new GUIContent("Head Slot");
                    int j = -1;
                    if (headSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (headSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        headSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        headSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }


                    content = new GUIContent("Head Covering Slot");
                    j = -1;
                    if (headCoveringSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (headCoveringSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        headCoveringSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        headCoveringSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Torso Slot");
                    j = -1;
                    if (torsoSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (torsoSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        torsoSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        torsoSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Upper Arm Slot");
                    j = -1;
                    if (upperarmSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (upperarmSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        upperarmSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        upperarmSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Lower Arm Slot");
                    j = -1;
                    if (lowerarmSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (lowerarmSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        lowerarmSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        lowerarmSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Hands Slot");
                    j = -1;
                    if (handsSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (handsSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        handsSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        handsSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Hip Slot");
                    j = -1;
                    if (hipSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (hipSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        hipSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        hipSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Upper Leg Slot");
                    j = -1;
                    if (upperlegSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (upperlegSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        upperlegSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        upperlegSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Lower Leg Slot");
                    j = -1;
                    if (lowerlegSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (lowerlegSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        lowerlegSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        lowerlegSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Feet Slot");
                    j = -1;
                    if (feetSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (feetSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        feetSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        feetSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Parent Slot");
                    j = -1;
                    if (parentSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (parentSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        parentSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        parentSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Beard Slot");
                    j = -1;
                    if (beardSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (beardSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        beardSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        beardSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Eyebrow Slot");
                    j = -1;
                    if (eyebrowSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (eyebrowSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        eyebrowSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        eyebrowSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Ear Slot");
                    j = -1;
                    if (earSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (earSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        earSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        earSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Eye Slot");
                    j = -1;
                    if (eyeSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (eyeSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        eyeSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        eyeSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Mouth Slot");
                    j = -1;
                    if (mouthSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (mouthSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        mouthSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        mouthSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    content = new GUIContent("Root Slot");
                    j = -1;
                    if (rootSlot.stringValue != "")
                    {
                        for (int i = 0; i < slots.Length; i++)
                        {
                            if (rootSlot.stringValue == slots[i].text)
                            {
                                j = i;
                                continue;
                            }
                        }
                        rootSlot.stringValue = slots[EditorGUILayout.Popup(content, j, slots)].text;
                    }
                    else
                    {
                        rootSlot.stringValue = slots[EditorGUILayout.Popup(content, 0, slots)].text;
                    }

                    /*
                    //EditorGUILayout.PropertyField(headSlot);
                    EditorGUILayout.PropertyField(torsoSlot);
                    EditorGUILayout.PropertyField(upperarmSlot);
                    EditorGUILayout.PropertyField(lowerarmSlot);
                    EditorGUILayout.PropertyField(hipSlot);
                    EditorGUILayout.PropertyField(upperlegSlot);
                    EditorGUILayout.PropertyField(lowerlegSlot);
                    EditorGUILayout.PropertyField(feetSlot);
                    EditorGUILayout.PropertyField(parentSlot);
                    EditorGUILayout.PropertyField(beardSlot);
                    EditorGUILayout.PropertyField(eyebrowSlot);
                    EditorGUILayout.PropertyField(earSlot);
                    EditorGUILayout.PropertyField(eyeSlot);
                    EditorGUILayout.PropertyField(mouthSlot);
                    EditorGUILayout.PropertyField(rootSlot);*/
                    EditorGUILayout.PropertyField(internalMouthModels);
                    EditorGUILayout.PropertyField(internalHairModels);

                    EditorGUILayout.PropertyField(internalEyebrowModels);
                    EditorGUILayout.PropertyField(internalBeardModels);
                    EditorGUILayout.PropertyField(internalEarModels);
                    EditorGUILayout.PropertyField(internalEyeModels);
                    EditorGUILayout.PropertyField(internalTuskModels);
                    thisScriptObject.ApplyModifiedProperties();

                    break;

                case "Parts":
                    EditorGUILayout.PropertyField(headModels);
                    EditorGUILayout.PropertyField(torsoModels);
                    EditorGUILayout.PropertyField(upperArmModels);
                    EditorGUILayout.PropertyField(lowerArmModels);
                    EditorGUILayout.PropertyField(handModels);
                    EditorGUILayout.PropertyField(hipModels);
                    EditorGUILayout.PropertyField(lowerLegModels);
                    EditorGUILayout.PropertyField(feetModels);
                    EditorGUILayout.PropertyField(helmetModels);
                    EditorGUILayout.PropertyField(hairModels);
                    EditorGUILayout.PropertyField(faceModels);
                    EditorGUILayout.PropertyField(beardModels);
                    EditorGUILayout.PropertyField(eyebrowModels);
                    EditorGUILayout.PropertyField(earModels);
                    EditorGUILayout.PropertyField(tuskModels);
                    EditorGUILayout.PropertyField(mouthModels);
                    EditorGUILayout.PropertyField(eyeModels);

                    break;
                case "Replacement":
                    EditorGUILayout.PropertyField(modularModelSwapping);

                    EditorGUILayout.PropertyField(replacementItems);
                    EditorGUILayout.PropertyField(replacementObject);
                    EditorGUILayout.PropertyField(headReplacementObject);
                    EditorGUILayout.PropertyField(torsoReplacementObject);
                    EditorGUILayout.PropertyField(upperArmsReplacementObject);
                    EditorGUILayout.PropertyField(lowerArmsReplacementObject);
                    EditorGUILayout.PropertyField(hipsReplacementObject);
                    EditorGUILayout.PropertyField(lowerLegsReplacementObject);
                    EditorGUILayout.PropertyField(feetReplacementObject);
                    EditorGUILayout.PropertyField(handsReplacementObject);
                    break;
                case "nHance":
#if ModularNHance
                    EditorGUILayout.PropertyField(nHanceModelSwapping);
                    EditorGUILayout.PropertyField(defaultTorso);
                    EditorGUILayout.PropertyField(defaultHands);
                    EditorGUILayout.PropertyField(defaultFeet);
                    EditorGUILayout.PropertyField(defaultLegs);
#endif

                    break;
                case "InfinityPBR":

#if IPBRInt
                    EditorGUILayout.PropertyField(blendshapePresetObjects);
                    EditorGUILayout.PropertyField(infinityPBRIntegration);
                    EditorGUILayout.PropertyField(enableSavingInfinityPBRBlendshapes);
                    EditorGUILayout.PropertyField(infinityBlendShapes);
                    EditorGUILayout.PropertyField(blendshapePresetValue);
#endif
                    break;
            }

            thisScriptObject.ApplyModifiedProperties(); // EEE - Moved outside of below if statement.

            if (EditorGUI.EndChangeCheck())
            {
                switch (script.currentTab)
                {
                    case "Parts":
                    case "Replacement":
                        GUI.FocusControl(null);
                        break;
                }
            }

        }


        public List<string> LoadSlotsOptions(bool addRoot = false, bool addNone = false)
        {

            List<string> options = new List<string>();
            // Read all entries from the table
            string query = "SELECT name FROM item_slots where isactive = 1";

            List<Dictionary<string, string>> rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);

            // Load data
            rows = DatabasePack.LoadData(DatabasePack.contentDatabasePrefix, query);
            //Debug.Log("#Rows:"+rows.Count);
            // Read all the data
            int optionsId = 0;
            if ((rows != null) && (rows.Count > 0))
            {
                foreach (Dictionary<string, string> data in rows)
                {
                    options.Add(data["name"]);
                    optionsId++;

                }
            }

            return options;
        }

    }
}