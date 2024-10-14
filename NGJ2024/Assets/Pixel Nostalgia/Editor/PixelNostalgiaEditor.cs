using UnityEngine;
using UnityEditor;

namespace DFAEditor
{
    /// <summary>
    /// Custom editor for the Pixel Nostalgia effect.
    /// </summary>
    /// <remarks>
    /// Because this script is a part of the Editor assembly, it should not be referenced by gameplay scripts. It will cause
    /// a build error. Everything can be controlled through <seealso cref="DFA.PixelNostalgia"/>.
    /// </remarks>
    [CustomEditor(typeof(DFA.PixelNostalgia))]
    public sealed class PixelNostalgiaEditor : Editor
    {
        SerializedObject settings;

        // What is used in the rebuild texture function
        SerializedProperty windowSize;

        // The bits per channel
        SerializedProperty rBitDepth;
        SerializedProperty gBitDepth;
        SerializedProperty bBitDepth;
        SerializedProperty grayscale;
        SerializedProperty grayscaleDepth;

        // Booleans
        SerializedProperty doDithering;
        SerializedProperty matchCameraSize;
        SerializedProperty doAscii;
        SerializedProperty doCRT;

        // A second iVec2, specificly set to user input
        SerializedProperty customWindowSize;

        // A multiplier on the camera size
        SerializedProperty scalarSize;
        
        // Used for the enums
        SerializedProperty selectedSizeOption;
        SerializedProperty selectedBitDepthOption;
        SerializedProperty bayerIndex;
        SerializedProperty ditherSeparation;
        SerializedProperty autoSelectSeparation;

        private Vector2[] resolutionValues =
        {
            new Vector2(80, 25),     // DOSASCII,
            new Vector2(160, 192),   // Atari2600,
            new Vector2(240, 160),   // GameboyAdvanced,
            new Vector2(256, 224),   // SuperNES,
            new Vector2(320, 200),   // DOOM,
            new Vector2(320, 224),   // SegaGenesis,
            new Vector2(320, 240),   // N64,
            new Vector2(256, 144),   // r144p,
            new Vector2(426, 240),   // r240p,
            new Vector2(480, 360),   // r360p,
            new Vector2(640, 480),   // r480p,
            new Vector2(1280, 720),  // r720p
        };

        private string[] resolutionStrings =
        {
            "DOS ASCII (80x25)",
            "Atari 2600 (160x192)",
            "Gameboy Advanced (240x160)",
            "Super NES (256x224)",
            "DOOM (320x200)",
            "Sega Genesis (320x224)",
            "N64 (320x240)",
            "144p (256x144)",
            "240p (426x240)",
            "360p (480x360)",
            "480p (640x480)",
            "720p (1280x720)",
        };

        private Vector3[] bitDepthValues =
        {
            new Vector3(3,3,2),
            new Vector3(4,4,4),
            new Vector3(5,5,5),
            new Vector3(5,6,5),
            new Vector3(6,6,6),
            new Vector3(8,8,8),
            new Vector3(8,8,8),
        };

        private string[] bitDepthStrings =
        {
            "BGR233 8bit",
            "RGB444 Direct Color",
            "RGB555 High Color",
            "RGB565 16bit Color",
            "RGB666 18bit Color",
            "RGB888 True Color",
            "Black and White",
            "Custom",
        };

        private string[] ditherStrings =
        {
            "2x2 Dither Matrix",
            "3x3 Dither Matrix",
            "4x4 Dither Matrix",
            "8x8 Dither Matrix",
        };

        private string[] selectStrings =
        {
            "Automatic Separation",
            "Custom Separation"
        };
        
        /// <summary>
        /// Gather all needed variables when enabled from the target class.
        /// </summary>
        public void OnEnable()
        {
            Undo.undoRedoPerformed += ApplyChanges;

            settings = new SerializedObject((target as DFA.PixelNostalgia).settings);
            settings.Update();

            // Find and set the SerializedPropertyOverride values from the PixelNostalgia class
            windowSize = settings.FindProperty("windowSize"); 

            rBitDepth = settings.FindProperty("rBitDepth");
            gBitDepth = settings.FindProperty("gBitDepth");
            bBitDepth = settings.FindProperty("bBitDepth");
            grayscale = settings.FindProperty("grayscale");
            grayscaleDepth = settings.FindProperty("grayscaleDepth");

            doDithering = settings.FindProperty("doDithering");
            matchCameraSize = settings.FindProperty("matchCameraSize");
            doAscii = settings.FindProperty("doAscii");
            doCRT = settings.FindProperty("doCRT");
             
            customWindowSize = settings.FindProperty("customWindowSize");
            
            scalarSize = settings.FindProperty("scalarSize");

            selectedSizeOption = settings.FindProperty("selectedSizeOption");
            selectedBitDepthOption = settings.FindProperty("selectedBitDepthOption");
            bayerIndex = settings.FindProperty("bayerIndex");
            ditherSeparation = settings.FindProperty("ditherSeparation");
            autoSelectSeparation = settings.FindProperty("autoSelectSeparation");
        }

        /// <summary>
        /// Unsubscribe the custom function from the undo/redo listener
        /// </summary>
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= ApplyChanges;
        }

        private bool foldoutA = true, foldoutB = true, foldoutC = true, foldoutD = true;
        private bool crt, asc;

        /// <summary>
        /// Open the manual website
        /// </summary>
        [MenuItem("DFA/Pixel Nostalgia/Manual", false, 0)]
        static void OpenDocumentation()
        {
            Application.OpenURL(@"https://dfa-studios.gitlab.io/pixel-nostalgia/manual/gettingStarted.html");
        }

        /// <summary>
        /// Open the scripting API website
        /// </summary>
        [MenuItem("DFA/Pixel Nostalgia/Scripting API", false, 1)]
        static void OpenAPI()
        {
            Application.OpenURL(@"https://dfa-studios.gitlab.io/pixel-nostalgia/api/DFA.html");
        }
        
        /// <summary>
        /// Open the asset store, to leave a review
        /// </summary>
        [MenuItem("DFA/Pixel Nostalgia/Leave a Review", false, 100)]
        static void OpenReview()
        {
            Application.OpenURL(@"https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/pixel-nostalgia-63909");
        }

        /// <summary>
        /// Open the bug report page.
        /// </summary>
        [MenuItem("DFA/Pixel Nostalgia/Report Bug", false, 101)]
        static void OpenBug()
        {
            Application.OpenURL(@"https://dfa-studios.gitlab.io/pixel-nostalgia/contact/bugs.html");
        }

        /// <summary>
        /// Draw the custom inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            settings.Update();
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            var originalColor = GUI.backgroundColor;

            foldoutA = EditorGUILayout.Foldout(foldoutA, "Resolution Settings");
            if (foldoutA)
            {
                EditorGUI.indentLevel++;
        
                GUI.backgroundColor = new Color32(249, 255, 214, 255);
                EditorGUILayout.PropertyField(customWindowSize, new GUIContent() { text = "New Resolution" });
        
                EditorGUI.indentLevel--;
        
                if (GUILayout.Button("Apply"))
                {
                    if (customWindowSize.vector2Value.x >= 1 && customWindowSize.vector2Value.y >= 1 &&
                        customWindowSize.vector2Value.x < 8192 && customWindowSize.vector2Value.y < 8192)
                    {   // Can increase this if you need a larger texture
                        matchCameraSize.boolValue = false;
                        windowSize.vector2Value = customWindowSize.vector2Value;
                    }
                }

                GUI.backgroundColor = new Color32(214, 255, 224, 255);
                if (GUILayout.Button("Match Camera"))
                {
                    matchCameraSize.boolValue = true;
                    scalarSize.floatValue = 1.0f;
                }
        
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("1/8 Cam"))
                    {
                        matchCameraSize.boolValue = true;
                        scalarSize.floatValue = 0.125f;
                    }
                    if (GUILayout.Button("1/4 Cam"))
                    {
                        matchCameraSize.boolValue = true;
                        scalarSize.floatValue = 0.25f;
                    }
                    if (GUILayout.Button("1/2 Cam"))
                    {
                        matchCameraSize.boolValue = true;
                        scalarSize.floatValue = 0.5f;
                    }
                }
                EditorGUILayout.EndHorizontal();
        
                GUI.backgroundColor = new Color32(220, 214, 255, 255);
                selectedSizeOption.intValue = EditorGUILayout.Popup(selectedSizeOption.intValue, resolutionStrings);
                if (GUILayout.Button("Apply " + resolutionStrings[selectedSizeOption.intValue]))
                {
                    matchCameraSize.boolValue = false;
                    windowSize.vector2Value = resolutionValues[selectedSizeOption.intValue];
                }

                string label = string.Format("Resolution: {0} Camera",
                    scalarSize.floatValue == 1.0f ? "Matching" :
                    scalarSize.floatValue == 0.5f ? "1/2" :
                    scalarSize.floatValue == 0.25f ? "1/4" :
                    "1/8"
                );

                if (!matchCameraSize.boolValue)
                {
                    var style = EditorStyles.wordWrappedLabel;
                    style.richText = true;

                    label = string.Format("Resolution: {0}x{1}", windowSize.vector2Value.x, windowSize.vector2Value.y);

                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("<b>Camera aspect ratio may not match texture aspect ratio</b>",
                        style);
                }
                else
                {
                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                }

                EditorGUILayout.Space();

                GUI.backgroundColor = originalColor;
            }
        
            foldoutB = EditorGUILayout.Foldout(foldoutB, "Color Depth Settings");
            if (foldoutB)
            {
                EditorGUI.indentLevel++;

                grayscale.boolValue = selectedBitDepthOption.intValue == bitDepthStrings.Length - 2;

                EditorGUI.BeginDisabledGroup(!grayscale.boolValue);
                {
                    GUI.backgroundColor = Color.white;
                    grayscaleDepth.intValue = EditorGUILayout.IntSlider("B&W", grayscaleDepth.intValue, 1, 8);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(selectedBitDepthOption.intValue != bitDepthStrings.Length - 1 || grayscale.boolValue);
                {
                    GUI.backgroundColor = new Color32(255, 153, 153, 255);
                    rBitDepth.intValue = EditorGUILayout.IntSlider("Red", rBitDepth.intValue, 1, 8);
        
                    GUI.backgroundColor = new Color32(153, 255, 153, 255);
                    gBitDepth.intValue = EditorGUILayout.IntSlider("Green", gBitDepth.intValue, 1, 8);

                    GUI.backgroundColor = new Color32(153, 153, 255, 255);
                    bBitDepth.intValue = EditorGUILayout.IntSlider("Blue", bBitDepth.intValue, 1, 8);
                }
                GUI.backgroundColor = originalColor;
                EditorGUI.EndDisabledGroup();
                
                if (selectedBitDepthOption.intValue < bitDepthStrings.Length - 2)
                {
                    rBitDepth.intValue = (int)bitDepthValues[selectedBitDepthOption.intValue].x;
                    gBitDepth.intValue = (int)bitDepthValues[selectedBitDepthOption.intValue].y;
                    bBitDepth.intValue = (int)bitDepthValues[selectedBitDepthOption.intValue].z;
                }
        
                if (!grayscale.boolValue)
                {
                    string maxColors = (
                        Mathf.Pow(2, rBitDepth.intValue) *
                        Mathf.Pow(2, gBitDepth.intValue) *
                        Mathf.Pow(2, bBitDepth.intValue)).ToString("N0");
        
                    EditorGUILayout.LabelField(maxColors + " possible colors", EditorStyles.boldLabel);
                }
                else
                {
                    string maxColors = (
                        Mathf.Pow(2, grayscaleDepth.intValue)).ToString("N0");

                    EditorGUILayout.LabelField(maxColors + " possible shades", EditorStyles.boldLabel);
                }
        
                selectedBitDepthOption.intValue = EditorGUILayout.Popup(selectedBitDepthOption.intValue, bitDepthStrings);

                if (selectedBitDepthOption.intValue < bitDepthStrings.Length - 2)
                {
                    rBitDepth.intValue = (int)bitDepthValues[selectedBitDepthOption.intValue].x;
                    gBitDepth.intValue = (int)bitDepthValues[selectedBitDepthOption.intValue].y;
                    bBitDepth.intValue = (int)bitDepthValues[selectedBitDepthOption.intValue].z;
                }

                grayscale.boolValue = selectedBitDepthOption.intValue == bitDepthStrings.Length - 2;

                EditorGUI.indentLevel--;
            }
        
            foldoutC = EditorGUILayout.Foldout(foldoutC, "Dithering Settings");
            if (foldoutC)
            {
                EditorGUILayout.PropertyField(doDithering, new GUIContent() { text = "Dithering Enabled" });
        
                EditorGUI.BeginDisabledGroup(!doDithering.boolValue);
                {
                    EditorGUI.indentLevel++;
                    bayerIndex.intValue = EditorGUILayout.Popup(bayerIndex.intValue, ditherStrings);
        
                    EditorGUI.BeginDisabledGroup(autoSelectSeparation.intValue == 0);
                    {
                        EditorGUILayout.PropertyField(ditherSeparation, new GUIContent() { text = "Separation" });
                    }
                    EditorGUI.EndDisabledGroup();
        
                    autoSelectSeparation.intValue = EditorGUILayout.Popup(autoSelectSeparation.intValue, selectStrings);
        
                    if (autoSelectSeparation.intValue == 0)
                    {
                        switch (bayerIndex.intValue)
                        {
                            case 0:     // 2x2
                                ditherSeparation.intValue = 422;
                                break;
                            case 1:     // 3x3
                                ditherSeparation.intValue = 323;
                                break;
                            case 2:     // 4x4
                                ditherSeparation.intValue = 285;
                                break;
                            case 3:     // 8x8
                                ditherSeparation.intValue = 256;
                                break;
                        }
                    }
        
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
            }
        
            foldoutD = EditorGUILayout.Foldout(foldoutD, "Extra Effects");
            if (foldoutD)
            {
                EditorGUI.indentLevel++;

                // Simple radio toggle
                asc = EditorGUILayout.Toggle("ASCII Effect", doAscii.boolValue);
                if (asc != doAscii.boolValue)
                {
                    if (asc) doCRT.boolValue = false;
                    doAscii.boolValue = asc;
                }
                
                crt = EditorGUILayout.Toggle("CRT Effect", doCRT.boolValue);
                if (crt != doCRT.boolValue)
                {
                    if (crt) doAscii.boolValue = false;
                    doCRT.boolValue = crt;
                }

                EditorGUI.indentLevel--;
            }
        
            EditorGUILayout.Space();

            DFA.PixelNostalgia pixelNostalgia = (target as DFA.PixelNostalgia);

            if (EditorGUI.EndChangeCheck())
            {
                settings.ApplyModifiedProperties();
                serializedObject.ApplyModifiedProperties();
                pixelNostalgia.UpdateEffectSettings();
            }
        }

        /// <summary>
        /// For when the undo/redo actions happen, this updates settings.
        /// </summary>
        void ApplyChanges()
        {
            DFA.PixelNostalgia pixelNostalgia = (target as DFA.PixelNostalgia);

            settings.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
            pixelNostalgia.UpdateEffectSettings();
        }
    }
}
