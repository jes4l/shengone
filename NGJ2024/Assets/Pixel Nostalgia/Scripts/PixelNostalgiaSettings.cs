using UnityEngine;

namespace DFA
{
    /// <summary>
    /// A class containing settings for Pixel Nostalgia.
    /// This class isn't in charge of Rendering. For that, see <seealso cref="DFA.PixelNostalgia"/>. 
    /// </summary>
    /// <remarks>
    /// To modify Pixel Nostalgia at runtime, requires modifying the fields in this class. 
    /// <code>
    /// 
    /// DFA.PixelNostalgia pixelNostalgia = gameObject.GetComponent<![CDATA[<]]>DFA.PixelNostalgia<![CDATA[>]]>();
    /// 
    /// // To set some settings, see their description for an explanation of what they do
    /// pixelNostalgia.settings.bayerIndex = 1;     // Use a 3x3 dither matrix
    /// pixelNostalgia.settings.grayscale = true;   // Turn on black and white mode
    /// pixelNostalgia.settings.grayscaleDepth = 3; // Use 3 bits per pixel
    ///  
    /// // After you have changed the settings you like, you should apply them:
    /// pixelNostalgia.UpdateEffectSettings();
    /// </code>
    /// </remarks>
    [System.Serializable]
    public class PixelNostalgiaSettings : ScriptableObject
    {
        // These warnings are disabled because, I use variables inside of this script to
        // serialize inspector options. They aren't detected as being used outside of the editor assembly.

    #pragma warning disable 0168 // variable declared but not used.
    #pragma warning disable 0219 // variable assigned but not used.
    #pragma warning disable 0414 // private field assigned but not used.

        /// <summary>
        /// The material used for all of the blit passes the effect performs. Should remain untouched. Is static.
        /// </summary>
        public static Material pixelNostalgiaMaterial;

        /// <summary>
        /// An array of lookup textures used for Ordered Dithering. They contain a Bayer Matrix. Index 0 is a 2x2 Matrix,
        /// Index 1 is a 3x3 Matrix, Index 2 is a 4x4 Matrix, and Index 3 is an 8x8 Matrix. See also <seealso cref="bayerIndex"/> for
        /// the parameter. Should remain untouched. Is static.
        /// </summary>
        public static Texture2D[] bayerTextures = new Texture2D[4];

        /// <summary>
        /// A lookup texture for the ASCII effect. Dense characters are sampled when the luminance of a pixel is low, and vice versa.
        /// If you'd like to modify the characters used, see the <c>ascii-lut-gradient.png</c> texture in <c>PixelNostalgia/Resources</c>.
        /// </summary>
        public Texture asciiLUT;

        /// <summary>
        /// Which index should be used when sampling from <seealso cref="bayerTextures"/>? A value ranging from 0 to 3, where Index 0 is a 
        /// 2x2 Matrix, Index 1 is a 3x3 Matrix, Index 2 is a 4x4 Matrix, and Index 3 is an 8x8 Matrix.
        /// </summary>
        public int bayerIndex = 3;

        /// <summary>
        /// The size of the screen. Stored in a <c>Vector2</c>, however it is rounded down to the nearest integer. Cannot be less than 1x1.
        /// </summary>
        public Vector2 windowSize = new Vector2(256, 240);

        /// <summary>
        /// The amount of precision the red channel has, in bits. 8 bits means 256 shades, and 1 bit means 1 shade (on/off).
        /// See also <seealso cref="gBitDepth"/>, <see cref="bBitDepth"/>, and <seealso cref="grayscaleDepth"/>
        /// </summary>
        [Range(1, 8)] public int rBitDepth = 8;

        /// <summary>
        /// The amount of precision the green channel has, in bits. 8 bits means 256 shades, and 1 bit means 1 shade (on/off).
        /// See also <seealso cref="rBitDepth"/>, <see cref="bBitDepth"/>, and <seealso cref="grayscaleDepth"/>
        /// </summary>
        [Range(1, 8)] public int gBitDepth = 8;

        /// <summary>
        /// The amount of precision the blue channel has, in bits. 8 bits means 256 shades, and 1 bit means 1 shade (on/off).
        /// See also <seealso cref="rBitDepth"/>, <see cref="gBitDepth"/>, and <seealso cref="grayscaleDepth"/>
        /// </summary>
        [Range(1, 8)] public int bBitDepth = 8;

        /// <summary>
        /// The amount of precision the grayscale image has, in bits. 8 bits means 256 shades, and 1 bit means 1 shade (on/off).
        /// See also <seealso cref="rBitDepth"/>, <see cref="gBitDepth"/>, and <seealso cref="bBitDepth"/>
        /// </summary>
        [Range(1, 8)] public int grayscaleDepth = 8;

        /// <summary>
        /// Should the image be grayscale? Set this value to true/false to swap between both types of images.
        /// </summary>
        public bool grayscale = false;

        /// <summary>
        /// Should the dither separation be automatically chosen? It's recommended to leave this on, but also recommended
        /// to experiment changing <seealso cref="ditherSeparation"/> to see which values work best.
        /// </summary>
        public int autoSelectSeparation = 0;

        /// <summary>
        /// Increases or decreases the range in which a pixel will influence the dither sample chosen. If unsure, make sure 
        /// <seealso cref="autoSelectSeparation"/> is set to <c>true</c>.
        /// </summary>
        [Range(1, 4096)] public int ditherSeparation = 256;

        /// <summary>
        /// Should dithering be performed?
        /// </summary>
        public bool doDithering = true;

        /// <summary>
        /// Should the cameras size be matched? Note that this will remove all pixelation, unless <seealso cref="scalarSize"/> 
        /// is less than <c>1.0f</c>.
        /// </summary>
        public bool matchCameraSize = true;

        /// <summary>
        /// Should the ASCII effect be performed? This requires a low resolution, in order to distinguish the glyphs.
        /// </summary>
        public bool doAscii = false;

        /// <summary>
        /// Should the CRT effect be performed? This performs a blur, fish-eye effect, and a vignette.
        /// </summary>
        public bool doCRT = false;

        /// <summary>
        /// If <seealso cref="matchCameraSize"/> is <c>true</c>, then this is multiplied into the width and height of the camera, 
        /// to dynamically create a resolution in place of <seealso cref="windowSize"/>. Great for keeping square pixels, across
        /// different aspect ratios.
        /// </summary>
        public float scalarSize = 0.25f;

        /// <summary>
        /// Internal use only: Please don't edit.
        /// </summary>
        public Vector2 customWindowSize = new Vector2(256, 240);

        /// <summary>
        /// Internal use only: Please don't edit.
        /// </summary>
        public int selectedSizeOption = 3;

        /// <summary>
        /// Internal use only: Please don't edit.
        /// </summary>
        public int selectedBitDepthOption = 0;

        /// <summary>
        /// Create the static material <seealso cref="pixelNostalgiaMaterial"/>.
        /// </summary>
        public void CreateMaterial()
        {
            Shader shader = Shader.Find("Hidden/PixelNostalgia");
            pixelNostalgiaMaterial = new Material(shader);
        }

        /// <summary>
        /// Called Internally
        /// </summary>
        public void ReleaseResources()
        {
    #if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (bayerTextures[i] != null)
                    {
                        Destroy(bayerTextures[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (bayerTextures[i] != null)
                    {
                        DestroyImmediate(bayerTextures[i]);
                    }
                }
            }
#else
            for (int i = 0; i < 4; i++)
            {
                if (bayerTextures[i] != null)
                {
                    Destroy(bayerTextures[i]);
                }
            }
#endif
        }

#pragma warning restore 0168 // variable declared but not used.
#pragma warning restore 0219 // variable assigned but not used.
#pragma warning restore 0414 // private field assigned but not used.
    }
}