using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace DFA
{
    /// <summary>
    /// A class for rendering the Pixel Nostalgia effect.
    /// This class doesn't contain settings for the effect. For that, see <seealso cref="DFA.PixelNostalgiaSettings"/>. 
    /// </summary>
    /// <remarks>
    /// The <c>PixelNostalgiaRenderer</c> handles all rendering. Applied as a post-processing effect, Pixel Nostalgia
    /// only modifies what's been drawn on the screen up until a certain point. UI elements in world space or camera space
    /// are modified by the effect, however overlay effects will not be modified.
    /// </remarks>
    [RequireComponent(typeof(Camera)), ExecuteInEditMode, AddComponentMenu("DFA/Pixel Nostalgia"), Serializable]
    public sealed class PixelNostalgia : MonoBehaviour
    {
        /// <summary>
        /// Modify the settings here. This object is read-only.
        /// </summary>
        public PixelNostalgiaSettings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new PixelNostalgiaSettings();
                }
                return _settings;
            }
        }
        [SerializeField]
        private PixelNostalgiaSettings _settings;

        private Vector2Int cameraPixelDim;
        private RenderContext renderContext;

        /// <summary>
        /// A simple context for the renderer
        /// </summary>
        private struct RenderContext
        {
            public Camera camera;
            public bool isSceneView;
            public CommandBuffer command;
            public RenderTargetIdentifier source;
            public RenderTargetIdentifier destination;
        }

        /// <summary>
        /// Initializes, and recreates the effect
        /// </summary>
        private void OnEnable()
        {
            Init();
            UpdateEffectSettings();
        }

        /// <summary>
        /// Remove command buffer and release resources
        /// </summary>
        private void OnDisable()
        {
            if (renderContext.command != null)
            {
                renderContext.camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, renderContext.command);
            }

            settings.ReleaseResources();
        }

        /// <summary>
        /// Call this function after settings have been changed, to update the PixelNostalgia renderer
        /// </summary>
        public void UpdateEffectSettings()
        {
#if UNITY_EDITOR
            renderContext.isSceneView = false;
#else
            renderContext.isSceneView = false;
#endif

            if (settings.asciiLUT == null || PixelNostalgiaSettings.bayerTextures[0] == null)
            {
                // this forces the textures to be loaded, among other potentially non-initialized objects
                Init();
            }

            renderContext.command.Clear();

            const string name = "PixelNostalgiaSourceRT";
            int sourceID = Shader.PropertyToID(name);
            RenderTextureDescriptor desc = new RenderTextureDescriptor(
                renderContext.camera.pixelWidth,
                renderContext.camera.pixelHeight);
            var sourceIdentifier = new RenderTargetIdentifier(sourceID);

            cameraPixelDim = new Vector2Int(renderContext.camera.pixelWidth, renderContext.camera.pixelHeight);

            renderContext.command.GetTemporaryRT(sourceID, desc);
            renderContext.command.Blit(new RenderTargetIdentifier(BuiltinRenderTextureType.CurrentActive), sourceIdentifier);
            renderContext.source = sourceIdentifier;
            renderContext.destination = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            Render(renderContext);
            renderContext.command.ReleaseTemporaryRT(sourceID);
        }

        /// <summary>
        /// Called each frame. Handles aspect ratio changes primarily
        /// </summary>
        private void Update()
        {
            if (PixelNostalgiaSettings.pixelNostalgiaMaterial == null)
            {
                settings.CreateMaterial();
            }

            if (cameraPixelDim != new Vector2Int(renderContext.camera.pixelWidth, renderContext.camera.pixelHeight))
            {
                UpdateEffectSettings();
            }
        }

        /// <summary>
        /// Called when the renderer is created and its associated settings have been set.
        /// </summary>
        private void Init()
        {
            renderContext.camera = GetComponent<Camera>();

            if (renderContext.command != null)
            {
                renderContext.camera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, renderContext.command);
            }

            renderContext.command = new CommandBuffer()
            {
                name = "Pixel Nostalgia",
            };
            renderContext.camera.AddCommandBuffer(CameraEvent.AfterImageEffects, renderContext.command);

            if (PixelNostalgiaSettings.pixelNostalgiaMaterial == null)
            {
                settings.CreateMaterial();
            }

            if (settings.asciiLUT == null)
            {
                settings.asciiLUT = Resources.Load<Texture>("ascii-lut-gradient");
                settings.asciiLUT.filterMode = FilterMode.Point;
            }

            for (int i = 0; i < 4; i++)
            {
                if (PixelNostalgiaSettings.bayerTextures[i] == null)
                {
                    int dim = i == 0 ? 2 : i == 1 ? 3 : i == 2 ? 4 : 8;

                    // TODO: Get a D3d error when using R8 format on certain GPUs
                    PixelNostalgiaSettings.bayerTextures[i] = new Texture2D(dim, dim, TextureFormat.RGBA32, false);
                    PixelNostalgiaSettings.bayerTextures[i].filterMode = FilterMode.Point;
                    PixelNostalgiaSettings.bayerTextures[i].wrapMode = TextureWrapMode.Repeat;

                    byte[] rawData = new byte[dim * dim * 4];

                    int[,] dithers = new int[,]
                    {
                    {   // 2x2
                        0, 2, 3, 1, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                    },
                    {   // 3x3
                        0, 7, 3, 6, 5, 2, 4, 1,
                        8, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                    },
                    {   // 4x4
                        0,  8,  2, 10, 12,  4, 14,  6,
                        3, 11,  1,  9, 15,  7, 13,  5,
                        0,  0,  0,  0,  0,  0,  0,  0,
                        0,  0,  0,  0,  0,  0,  0,  0,
                        0,  0,  0,  0,  0,  0,  0,  0,
                        0,  0,  0,  0,  0,  0,  0,  0,
                        0,  0,  0,  0,  0,  0,  0,  0,
                        0,  0,  0,  0,  0,  0,  0,  0,
                    },
                    {   // 8x8
                        0,  48, 12, 60, 3,  51, 15, 63,
                        32, 16, 44, 28, 35, 19, 47, 31,
                        8,  56, 4,  52, 11, 59,  7, 55,
                        40, 24, 36, 20, 43, 27, 39, 23,
                        2,  50, 14, 62, 1,  49, 13, 61,
                        34, 18, 47, 30, 33, 17, 45, 29,
                        10, 58, 6,  54, 9,  57, 5,  53,
                        42, 26, 38, 22, 41, 25, 37, 21
                    }
                    };

                    for (int x = 0; x < dim; x++)
                    {
                        for (int y = 0; y < dim; y++)
                        {
                            float col = dithers[i, x + y * dim] / (float)(dim * dim - 1);
                            var d = (byte)(col * 255 % 256);

                            rawData[(x + y * dim) * 4 + 0] = d;
                            rawData[(x + y * dim) * 4 + 1] = d;
                            rawData[(x + y * dim) * 4 + 2] = d;
                            rawData[(x + y * dim) * 4 + 3] = d;
                        }
                    }

                    PixelNostalgiaSettings.bayerTextures[i].LoadRawTextureData(rawData);
                    PixelNostalgiaSettings.bayerTextures[i].Apply();
                }
            }
        }

        /// <summary>
        /// This render method is called once only (not each frame), and creates the command buffers needed.
        /// </summary>
        private void Render(RenderContext context)
        {
            Vector4 colorAmounts = new Vector4();

            colorAmounts.x = 256 / Mathf.Pow(2, settings.rBitDepth); // 0 to 256
            colorAmounts.y = 256 / Mathf.Pow(2, settings.gBitDepth); // 0 to 256
            colorAmounts.z = 256 / Mathf.Pow(2, settings.bBitDepth); // 0 to 256

            if (settings.grayscale)
                colorAmounts.w = 256 / Mathf.Pow(2, settings.grayscaleDepth); // 0 to 256
            else
                colorAmounts.w = -1.0f;

            context.command.SetGlobalVector("pn_bitsPerChannel", colorAmounts);
            context.command.SetGlobalTexture("pn_orderedBayer", PixelNostalgiaSettings.bayerTextures[settings.bayerIndex]);
            context.command.SetGlobalFloat("pn_levels", settings.ditherSeparation);

            int ditherDim = settings.bayerIndex == 0 ? 2 : settings.bayerIndex == 1 ? 3 : settings.bayerIndex == 2 ? 4 : 8;

            int endRes = Shader.PropertyToID("EndRes");
            int temp =   Shader.PropertyToID("Temp");

            context.command.GetTemporaryRT(endRes, context.camera.pixelWidth, context.camera.pixelHeight);

            int customWidth, customHeight;

            // Downsample into the pixelated texture
            if (settings.matchCameraSize)
            {
                customWidth = (int)(context.camera.pixelWidth * settings.scalarSize);
                customHeight = (int)(context.camera.pixelHeight * settings.scalarSize);
            }
            else
            {
                customWidth = (int)(settings.windowSize.x);
                customHeight = (int)(settings.windowSize.y);
            }

            context.command.GetTemporaryRT(temp, customWidth, customHeight, 0, FilterMode.Point);
            context.command.SetGlobalVector("pn_screenSize", new Vector3(customWidth, customHeight, ditherDim));
            context.command.SetGlobalFloat("pn_invertFlip", 1.0f);

            context.command.Blit(context.source, temp, PixelNostalgiaSettings.pixelNostalgiaMaterial, settings.doDithering ? 0 : 1);
            context.command.Blit(temp, endRes);
            context.command.ReleaseTemporaryRT(temp);


            // ASCII EFFECT IF APPLICABLE
            if (settings.doAscii)
            {
                context.command.SetGlobalTexture("pn_ascii", settings.asciiLUT);

                context.command.GetTemporaryRT(temp, context.camera.pixelWidth, context.camera.pixelHeight);
                context.command.Blit(endRes, temp, PixelNostalgiaSettings.pixelNostalgiaMaterial, 2);
                context.command.Blit(temp, context.destination);
                context.command.ReleaseTemporaryRT(temp);
            }
            else if (settings.doCRT)
            {
                context.command.GetTemporaryRT(temp, context.camera.pixelWidth, context.camera.pixelHeight);
                context.command.Blit(endRes, temp, PixelNostalgiaSettings.pixelNostalgiaMaterial, 3);
                context.command.Blit(temp, context.destination);
                context.command.ReleaseTemporaryRT(temp);
            }
            else
            {
                context.command.Blit(endRes, context.destination, PixelNostalgiaSettings.pixelNostalgiaMaterial, 4);
            }
            context.command.ReleaseTemporaryRT(endRes);
        }
    }
}