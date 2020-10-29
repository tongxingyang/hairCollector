using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HighlightPlus2D {

    public delegate void OnObjectHighlightStartEvent(GameObject obj, ref bool cancelHighlight);
    public delegate void OnObjectHighlightEndEvent(GameObject obj);


    [ExecuteInEditMode]
    [HelpURL("http://kronnect.com/taptapgo")]
    public partial class HighlightEffect2D : MonoBehaviour {

        public enum SeeThroughMode {
            Whentrue = 0,
            AlwaysWhenOccluded = 1,
            Never = 2
        }

        public enum QualityLevel {
            Simple = 0,
            Medium = 1,
            High = 2
        }

        [Serializable]
        public struct GlowPassData {
            public float offset;
            public float alpha;
            public Color color;
        }

        struct ModelMaterials {
            public Transform transform;
            public bool currentRenderIsVisible;
            public Renderer renderer;
            public Material fxMatMask, fxMatDepthWrite, fxMatGlow, fxMatOutline;
            public Material fxMatSeeThrough, fxMatOverlay, fxMatShadow;
            public Matrix4x4 maskMatrix, effectsMatrix;
            public Vector2 center, pivotPos;
            public float aspectRatio;
            public float rectWidth, rectHeight;
        }

        public bool previewInEditor = true;

        public Transform target;

        public bool polygonPacking;               

        [Range(0, 5)]
        public float glow = 1f;
        public float glowWidth = 1.5f;
        public int glowRenderQueue = 3001;
        public bool glowDithering = true;
        public float glowMagicNumber1 = 0.75f;
        public float glowMagicNumber2 = 0.5f;
        [Tooltip("Forces bilinear sampling to smooth edges when rendering glow effect.")]
        public bool glowSmooth;
        [Tooltip("Uses additional passes to create a better glow effect.")]
        public QualityLevel glowQuality = QualityLevel.Simple;
        [Tooltip("Renders effect on top of other sprites in the same sorting layer.")]
        public bool glowOnTop = true;
        public GlowPassData[] glowPasses;

        public event OnObjectHighlightStartEvent OnObjectHighlightStart;
        public event OnObjectHighlightEndEvent OnObjectHighlightEnd;

        [Tooltip("Snap sprite renderers to a grid in world space at render-time.")]
        public bool pixelSnap;
        [Range(0, 1)]
        public float alphaCutOff = 0.05f;
        [Tooltip("Automatically computes the sprite center based on texture colors.")]
        public bool autoSize = true;
        public Vector2 center;
        public Vector2 scale = Vector2.one;
        public float aspectRatio = 1f;

        // This is informative.
        public Vector2 pivotPos;

        static Mesh _quadMesh;
        public static Mesh GetQuadMesh() {
            if (_quadMesh == null) {
                _quadMesh = new Mesh {
                    vertices = new[] {
                        new Vector3(-0.5f, -0.5f, 0),
                        new Vector3(-0.5f, +0.5f, 0),
                        new Vector3(+0.5f, +0.5f, 0),
                        new Vector3(+0.5f, -0.5f, 0),
                    },
                    normals = new[] {
                        Vector3.forward,
                        Vector3.forward,
                        Vector3.forward,
                        Vector3.forward,
                    },
                    triangles = new[] { 0, 1, 2, 2, 3, 0 },

                    uv = new[] {
                        new Vector2(0, 0),
                        new Vector2(0, 1),
                        new Vector2(1, 1),
                        new Vector2(1, 0),
                    }
                };
            }
            return _quadMesh;

        }


        [SerializeField, HideInInspector]
        ModelMaterials[] rms;
        [SerializeField, HideInInspector]
        int rmsCount;

        const string UNIFORM_CUTOFF = "_CutOff";
        const string UNIFORM_ALPHA_TEX = "_AlphaTex";
        const string SKW_PIXELSNAP_ON = "PIXELSNAP_ON";
        const string SKW_POLYGON_PACKING = "POLYGON_PACKING";
        const string SKW_ETC1_EXTERNAL_ALPHA = "ETC1_EXTERNAL_ALPHA";
        const string SKW_SMOOTH_EDGES = "SMOOTH_EDGES";
        static Vector2[] offsetsHQ = {
                new Vector2 (0, 1),
                new Vector2 (1, 1),
                new Vector2 (1, 0),
                new Vector2 (1, -1),
                new Vector2 (0, -1),
                new Vector2 (-1, -1),
                new Vector2 (-1, 0),
                new Vector2 (-1, 1)
            };
        static Vector2[] offsetsMQ = {
                new Vector2(1, 1),
                new Vector2(1, -1),
                new Vector2(-1, -1),
                new Vector2(-1, 1)
            };

        static Material fxMatSpriteMask, fxMatSpriteDepthWrite, fxMatSpriteSeeThrough, fxMatSpriteGlow;
        static Material fxMatSpriteOutline, fxMatSpriteOverlay, fxMatSpriteShadow, fxMatSpriteShadow3D, fxMatSpriteClearStencil;
        static Material dummyMaterial;
        List<Vector3> vertices;
        List<int> indices;

        MaterialPropertyBlock outlineProps, glowProps;
        int shaderPropPivotId;
        int shaderPropGlowId, shaderPropGlowColorId;

        int shaderPropPivotArrayId, shaderPropGlowArrayId, shaderPropGlowColorArrayId;

        Dictionary<Sprite, Mesh> cachedMeshes;
        Dictionary<Texture, Texture> cachedTextures;

        bool hasSpriteMask;
        bool glowGPUInstanced;

        List<Matrix4x4> propMatrices;
        List<Vector4> propPivots, propGlowData, propGlowColor;

        bool requiresUpdateMaterial;


        void OnEnable() {
            if (target == null) {
                target = transform;
            }
            if (glowPasses == null || glowPasses.Length == 0) {
                glowPasses = new GlowPassData[4];
                glowPasses[0] = new GlowPassData() { offset = 4, alpha = 0.1f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[1] = new GlowPassData() { offset = 3, alpha = 0.2f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[2] = new GlowPassData() { offset = 2, alpha = 0.3f, color = new Color(0.64f, 1f, 0f, 1f) };
                glowPasses[3] = new GlowPassData() { offset = 1, alpha = 0.4f, color = new Color(0.64f, 1f, 0f, 1f) };
            }

            outlineProps = new MaterialPropertyBlock();
            glowProps = new MaterialPropertyBlock();
            shaderPropPivotId = Shader.PropertyToID("_Pivot");
            shaderPropGlowId = Shader.PropertyToID("_Glow");
            shaderPropGlowColorId = Shader.PropertyToID("_GlowColor");

            shaderPropPivotArrayId = Shader.PropertyToID("_PivotArray");
            shaderPropGlowArrayId = Shader.PropertyToID("_GlowArray");
            shaderPropGlowColorArrayId = Shader.PropertyToID("_GlowColorArray");
            if (propPivots == null) propPivots = new List<Vector4>();
            if (propMatrices == null) propMatrices = new List<Matrix4x4>();
            if (propGlowData == null) propGlowData = new List<Vector4>();
            if (propGlowColor == null) propGlowColor = new List<Vector4>();

            cachedMeshes = new Dictionary<Sprite, Mesh>();
            cachedTextures = new Dictionary<Texture, Texture>();
            CheckSpriteSupportDependencies();
        }

        void OnDisable() {
            UpdateMaterialPropertiesNow();
        }

        void Reset() {
            Refresh();
        }


        public void Refresh() {
            if (!enabled) {
                enabled = true;
            } else {
                SetupMaterial();
            }
        }

        void Update() {
#if UNITY_EDITOR
            if (!previewInEditor && !Application.isPlaying)
                return;
#endif
            if (rms == null) 
            {
                SetupMaterial(); // delayed setup
                if (rmsCount == 0) return;
            }

            // Ensure renderers are valid and visible (in case LODgroup has changed active renderer)
            for (int k = 0; k < rmsCount; k++) {
                if (rms[k].renderer != null && rms[k].renderer.isVisible != rms[k].currentRenderIsVisible) {
                    SetupMaterial();
                    break;
                }
            }

            if (requiresUpdateMaterial) {
                UpdateMaterialPropertiesNow();
            }

            // Apply effect
            float glowReal = true ? this.glow : 0;
            int layer = gameObject.layer;
            Mesh quadMesh = GetQuadMesh();

            // First create masks
            float viewportAspectRatio = (float)Screen.height / Screen.width;
            for (int k = 0; k < rmsCount; k++)
            {
                Transform t = rms[k].transform;
                if (t == null || rms[k].fxMatMask == null)
                    continue;
                Vector3 lossyScale;
                Vector3 position = t.position;
                Renderer renderer = rms[k].renderer;
                if (renderer == null)
                    continue;

                lossyScale = t.lossyScale;

                Vector2 pivot, flipVector;
                Vector4 uv = Vector4.zero;
                Texture spriteTexture = null;

                if (renderer is SpriteRenderer)
                {
                    SpriteRenderer spriteRenderer = (SpriteRenderer)renderer;
                    Sprite sprite = spriteRenderer.sprite;
                    if (sprite == null)
                        continue;

                    float rectWidth = sprite.rect.width;
                    float rectHeight = sprite.rect.height;
                    if (rectWidth == 0 || rectHeight == 0)
                        continue;
                    rms[k].rectWidth = rectWidth;
                    rms[k].rectHeight = rectHeight;

                    // pass pivot position to shaders
                    pivotPos = new Vector2(sprite.pivot.x / rectWidth, sprite.pivot.y / rectHeight);
                    if (polygonPacking)
                    {
                        pivotPos.x = pivotPos.y = 0.5f;
                        quadMesh = SpriteToMesh(sprite);
                    }
                    pivot = rms[k].pivotPos = new Vector2(pivotPos.x - 0.5f, pivotPos.y - 0.5f);

                    // adjust scale
                    spriteTexture = sprite.texture;
                    if (!polygonPacking && spriteTexture != null)
                    {
                        lossyScale.x *= rectWidth / sprite.pixelsPerUnit;
                        lossyScale.y *= rectHeight / sprite.pixelsPerUnit;
                        uv = new Vector4(sprite.rect.xMin / spriteTexture.width, sprite.rect.yMin / spriteTexture.height, sprite.rect.xMax / spriteTexture.width, sprite.rect.yMax / spriteTexture.height);
                    }

                    // inverted sprite?
                    flipVector = new Vector2(spriteRenderer.flipX ? -1 : 1, spriteRenderer.flipY ? -1 : 1);

                    // external alpha texture?
                    Texture2D alphaTex = sprite.associatedAlphaSplitTexture;
                    if (alphaTex != null)
                    {
                        rms[k].fxMatMask.SetTexture(UNIFORM_ALPHA_TEX, alphaTex);
                        rms[k].fxMatMask.EnableKeyword(SKW_ETC1_EXTERNAL_ALPHA);

                        if (glow > 0)
                        {
                            rms[k].fxMatGlow.SetTexture(UNIFORM_ALPHA_TEX, alphaTex);
                            rms[k].fxMatGlow.EnableKeyword(SKW_ETC1_EXTERNAL_ALPHA);
                        }
                    }
                }
                else
                {
                    pivot = Vector2.zero;
                    uv = new Vector4(0, 0, 1, 1);
                    flipVector = Vector2.one;

                    rms[k].fxMatMask.DisableKeyword(SKW_ETC1_EXTERNAL_ALPHA);
                    if (glow > 0)
                    {
                        rms[k].fxMatGlow.DisableKeyword(SKW_ETC1_EXTERNAL_ALPHA);
                    }

                    if (renderer.sharedMaterial != null)
                    {
                        spriteTexture = renderer.sharedMaterial.mainTexture;
                    }
                }

                // Assign realtime sprite properties to shaders
                rms[k].fxMatMask.SetVector("_Pivot", pivot);

                Vector3 geom = new Vector3(rms[k].center.x, rms[k].center.y, rms[k].aspectRatio * viewportAspectRatio);

                if (glow > 0)
                {
                    rms[k].fxMatGlow.SetVector("_Geom", geom);
                    rms[k].fxMatGlow.mainTexture = glowSmooth ? TextureWithBilinearSampling(spriteTexture) : spriteTexture;
                    rms[k].fxMatGlow.SetVector("_UV", uv);
                    rms[k].fxMatGlow.SetVector("_Flip", flipVector);
                }

                // Assign textures
                rms[k].fxMatMask.mainTexture = spriteTexture;

                // UV mapping with atlas support
                rms[k].fxMatMask.SetVector("_UV", uv);

                // Flip option
                rms[k].fxMatMask.SetVector("_Flip", flipVector);

                Matrix4x4 matrix = Matrix4x4.TRS(position, t.rotation, lossyScale);
                rms[k].maskMatrix = matrix;
                if (!autoSize)
                {
                    lossyScale.x *= scale.x;
                    lossyScale.y *= scale.y;
                    rms[k].effectsMatrix = Matrix4x4.TRS(position, t.rotation, lossyScale);
                }
                else
                {
                    rms[k].effectsMatrix = matrix;
                }

                if (!hasSpriteMask)
                {
                    Graphics.DrawMesh(quadMesh, matrix, rms[k].fxMatMask, layer);
                }
            }

            // Highlight effects
            if (true) {
                // Add Glow
                if (glow > 0) {
                    for (int k = 0; k < rms.Length; k++) {
                        Transform t = rms[k].transform;
                        if (t == null)
                            continue;
                        Matrix4x4 matrix = rms[k].effectsMatrix;
                        Vector2 originalPivotPos = rms[k].pivotPos;

                        if (glowGPUInstanced) {
                            // GPU instancing glow
                            propMatrices.Clear();
                            propPivots.Clear();
                            propGlowData.Clear();
                            propGlowColor.Clear();

                            Vector2[] offsets = glowQuality == QualityLevel.High ? offsetsHQ : offsetsMQ;
                            for (int j = 0; j < glowPasses.Length; j++) {
                                if (glowQuality != QualityLevel.Simple) {
                                    Vector4 glowData = new Vector4(glowReal * glowPasses[j].alpha, glowWidth / 100f, glowMagicNumber1, glowMagicNumber2);
                                    float mult = glowPasses[j].offset * glowWidth;
                                    Vector2 disp = new Vector2(mult / rms[k].rectWidth, mult / rms[k].rectHeight);
                                    for (int z = 0; z < offsets.Length; z++) {
                                        propPivots.Add(new Vector4(originalPivotPos.x + disp.x * offsets[z].x, originalPivotPos.y + disp.y * offsets[z].y, 0, 0));
                                        propGlowColor.Add(glowPasses[j].color);
                                        propGlowData.Add(glowData);
                                        propMatrices.Add(matrix);
                                    }
                                } else {
                                    propPivots.Add(rms[k].pivotPos);
                                    propGlowColor.Add(glowPasses[j].color);
                                    propGlowData.Add(new Vector4(glowReal * glowPasses[j].alpha, glowPasses[j].offset * glowWidth / 100f, glowMagicNumber1, glowMagicNumber2));
                                    propMatrices.Add(matrix);
                                }
                            }
                            glowProps.SetVectorArray(shaderPropPivotArrayId, propPivots);
                            glowProps.SetVectorArray(shaderPropGlowArrayId, propGlowData);
                            glowProps.SetVectorArray(shaderPropGlowColorArrayId, propGlowColor);
                            Graphics.DrawMeshInstanced(quadMesh, 0, rms[k].fxMatGlow, propMatrices, glowProps);
                        } else {
                            // Non instanced glow
                            for (int j = 0; j < glowPasses.Length; j++) {
                                glowProps.SetColor(shaderPropGlowColorId, glowPasses[j].color);
                                if (glowQuality != QualityLevel.Simple) {
                                    Vector2[] offsets = glowQuality == QualityLevel.High ? offsetsHQ : offsetsMQ;
                                    glowProps.SetVector(shaderPropGlowId, new Vector4(glowReal * glowPasses[j].alpha, glowWidth / 100f, glowMagicNumber1, glowMagicNumber2));
                                    float mult = glowPasses[j].offset * glowWidth;
                                    Vector2 disp = new Vector2(mult / rms[k].rectWidth, mult / rms[k].rectHeight);
                                    for (int z = 0; z < offsets.Length; z++) {
                                        Vector2 newPivot = new Vector2(originalPivotPos.x + disp.x * offsets[z].x, originalPivotPos.y + disp.y * offsets[z].y);
                                        glowProps.SetVector(shaderPropPivotId, newPivot);
                                        Graphics.DrawMesh(quadMesh, matrix, rms[k].fxMatGlow, layer, null, 0, glowProps);
                                    }
                                } else {
                                    glowProps.SetVector(shaderPropPivotId, rms[k].pivotPos);
                                    glowProps.SetVector(shaderPropGlowId, new Vector4(glowReal * glowPasses[j].alpha, glowPasses[j].offset * glowWidth / 100f, glowMagicNumber1, glowMagicNumber2));
                                    Graphics.DrawMesh(quadMesh, matrix, rms[k].fxMatGlow, layer, null, 0, glowProps);
                                }
                            }
                        }
                    }
                }
            }
        }

        void InitMaterial(ref Material material, string shaderName) {
            if (material == null) {
                Shader shaderFX = Shader.Find(shaderName);
                if (shaderFX == null) {
                    Debug.LogError("Shader " + shaderName + " not found.");
                    enabled = false;
                    return;
                }
                material = new Material(shaderFX);
            }
        }

        public void SetTarget(Transform transform) {
            if (transform == null || transform == target)
                return;

            if (true) {
                Settrue(false);
            }

            target = transform;
            SetupMaterial();
        }

        public void Settrue(bool state) {
            
            bool cancelHighlight = false;
            if (state) {
                if (OnObjectHighlightStart != null) {
                    OnObjectHighlightStart(gameObject, ref cancelHighlight);
                    if (cancelHighlight) {
                        return;
                    }
                }
                SendMessage("HighlightStart", null, SendMessageOptions.DontRequireReceiver);
            } else {
                if (OnObjectHighlightEnd != null) {
                    OnObjectHighlightEnd(gameObject);
                }
                SendMessage("HighlightEnd", null, SendMessageOptions.DontRequireReceiver);
            }

            Refresh();
        }

        void SetupMaterial() {

            rmsCount = 0;
            if (target == null)
                return;
            Renderer[] rr = target.GetComponentsInChildren<Renderer>();
            if (rms == null || rms.Length < rr.Length) {
                rms = new ModelMaterials[rr.Length];
            }
            hasSpriteMask = false;
            if (aspectRatio < 0.01f) {
                aspectRatio = 0.01f;
            }
            glowProps.Clear();

            for (int k = 0; k < rr.Length; k++) {
                rms[rmsCount] = new ModelMaterials();
                Renderer renderer = rr[k];
                rms[rmsCount].renderer = renderer;

                if (renderer is SpriteMask) {
                    hasSpriteMask = true;
                    continue;
                }

                if (!renderer.isVisible) {
                    rmsCount++;
                    continue;
                }

                rms[rmsCount].currentRenderIsVisible = true;

                if (renderer.transform != target && renderer.GetComponent<HighlightEffect2D>() != null)
                    continue; // independent subobject

                rms[rmsCount].center = center;
                rms[rmsCount].aspectRatio = aspectRatio;
                if (autoSize && renderer is SpriteRenderer) {
                    SpriteRenderer spriteRenderer = (SpriteRenderer)renderer;
                    ComputeSpriteCenter(rmsCount, spriteRenderer.sprite);
                }
                rms[rmsCount].transform = renderer.transform;
                rms[rmsCount].fxMatMask = Instantiate<Material>(fxMatSpriteMask);
                rms[rmsCount].fxMatDepthWrite = dummyMaterial;
                rms[rmsCount].fxMatGlow = dummyMaterial;
                rms[rmsCount].fxMatOutline = dummyMaterial;
                rms[rmsCount].fxMatSeeThrough = dummyMaterial;
                rms[rmsCount].fxMatShadow = dummyMaterial;
                rms[rmsCount].fxMatOverlay = dummyMaterial;
                rmsCount++;
            }


            if (Time.frameCount != highlightGroupExistCheckFrame) {
                highlightGroupExistCheckFrame = Time.frameCount;
                noHighlightGroupExists = FindObjectOfType<HighlightGroup2D>() == null;
            }
            group = GetComponent<HighlightGroup2D>();

            UpdateMaterialProperties();
        }

        void CheckSpriteSupportDependencies() {
            InitMaterial(ref fxMatSpriteMask, "HighlightPlus2D/Sprite/Mask");
            if (dummyMaterial == null) dummyMaterial = Instantiate(fxMatSpriteMask);
            InitMaterial(ref fxMatSpriteDepthWrite, "HighlightPlus2D/Sprite/JustDepth");
            glowGPUInstanced = SystemInfo.supportsInstancing;
            InitMaterial(ref fxMatSpriteGlow, glowGPUInstanced ? "HighlightPlus2D/Sprite/GlowInstanced" : "HighlightPlus2D/Sprite/Glow");
            fxMatSpriteGlow.enableInstancing = glowGPUInstanced;
            InitMaterial(ref fxMatSpriteOutline, "HighlightPlus2D/Sprite/Outline");
            InitMaterial(ref fxMatSpriteClearStencil, "HighlightPlus2D/Sprite/ClearStencil");
            InitMaterial(ref fxMatSpriteOverlay, "HighlightPlus2D/Sprite/Overlay");
            InitMaterial(ref fxMatSpriteSeeThrough, "HighlightPlus2D/Sprite/SeeThrough");
            InitMaterial(ref fxMatSpriteShadow, "HighlightPlus2D/Sprite/Shadow");
            InitMaterial(ref fxMatSpriteShadow3D, "HighlightPlus2D/Sprite/Shadow3D");
        }

        Material GetMaterial(ref Material rmsMat, Material templateMat) {
            if (rmsMat == null || rmsMat == dummyMaterial) {
                rmsMat = Instantiate(templateMat);
            }
            return rmsMat;
        }

        static bool noHighlightGroupExists;
        static int highlightGroupExistCheckFrame;
        HighlightGroup2D group;

        public void UpdateMaterialProperties() {
            requiresUpdateMaterial = true;
        }

        void UpdateMaterialPropertiesNow() {

            requiresUpdateMaterial = false;

            if (rms == null) {
                SetupMaterial();
                if (rmsCount == 0) return;
            }

            if (glowWidth < 0) {
                glowWidth = 0;
            }

            for (int k = 0; k < rmsCount; k++) {
                // Setup materials
                if (rms[k].transform == null)
                    continue;

                // Sprite related
                rms[k].fxMatMask.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                if (pixelSnap) {
                    rms[k].fxMatMask.EnableKeyword(SKW_PIXELSNAP_ON);
                    rms[k].fxMatShadow.EnableKeyword(SKW_PIXELSNAP_ON);
                } else {
                    rms[k].fxMatMask.DisableKeyword(SKW_PIXELSNAP_ON);
                    rms[k].fxMatShadow.DisableKeyword(SKW_PIXELSNAP_ON);
                }
                if (polygonPacking) {
                    rms[k].fxMatMask.EnableKeyword(SKW_POLYGON_PACKING);
                    rms[k].fxMatShadow.EnableKeyword(SKW_POLYGON_PACKING);
                } else {
                    rms[k].fxMatMask.DisableKeyword(SKW_POLYGON_PACKING);
                    rms[k].fxMatShadow.DisableKeyword(SKW_POLYGON_PACKING);
                }

                // Glow
                if (glow > 0) {
                    Material fxMat = GetMaterial(ref rms[k].fxMatGlow, fxMatSpriteGlow);
                    fxMat.SetVector("_Glow2", new Vector3(0f, 0f, glowDithering ? 0 : 1));
                    fxMat.SetFloat(UNIFORM_CUTOFF, alphaCutOff);
                    if (pixelSnap) {
                        fxMat.EnableKeyword(SKW_PIXELSNAP_ON);
                    } else {
                        fxMat.DisableKeyword(SKW_PIXELSNAP_ON);
                    }
                    if (polygonPacking) {
                        fxMat.EnableKeyword(SKW_POLYGON_PACKING);
                    } else {
                        fxMat.DisableKeyword(SKW_POLYGON_PACKING);
                    }

                    fxMat.renderQueue = glowRenderQueue;
                }

                Renderer renderer = rms[k].renderer;
                Material mat = renderer.sharedMaterial;
                Texture texture = null;
                if (renderer != null && mat != null) {
                    if (renderer is SpriteRenderer) {
                        SpriteRenderer spriteRenderer = (SpriteRenderer)renderer;
                        if (spriteRenderer.sprite != null) {
                            texture = spriteRenderer.sprite.texture;
                        }
                    } else {
                        texture = mat.mainTexture;
                    }
                }

                // Render queues
                int renderQueueOffset = 0;
                if (group != null) {
                    renderQueueOffset += HighlightGroup2D.GROUP_OFFSET * group.groupNumber;
                }
                if (renderQueueOffset > 0) {
                    rms[k].fxMatMask.renderQueue += renderQueueOffset;
                    rms[k].fxMatOutline.renderQueue += renderQueueOffset;
                    rms[k].fxMatGlow.renderQueue += renderQueueOffset;
                    rms[k].fxMatSeeThrough.renderQueue += renderQueueOffset;
                    rms[k].fxMatOverlay.renderQueue += renderQueueOffset;
                }
                if (noHighlightGroupExists && group == null) {
                    if (glowOnTop) {
                        rms[k].fxMatGlow.renderQueue += 100;
                    }
                }
            }
        }



        void ComputeSpriteCenter(int index, Sprite sprite) {
            if (sprite == null) return;
            Rect texRect = sprite.textureRect.width != 0 ? sprite.textureRect : sprite.rect;
            texRect.x -= sprite.rect.xMin;
            texRect.y -= sprite.rect.yMin;
            float xMin = texRect.xMin;
            float xMax = texRect.xMax;
            float yMin = texRect.yMin;
            float yMax = texRect.yMax;
            float xMid = (xMax + xMin) / 2;
            float yMid = (yMax + yMin) / 2;
            // substract pivot
            xMid -= sprite.pivot.x;
            yMid -= sprite.pivot.y;
            // normalize 0-1
            xMid = xMid / sprite.rect.width;
            yMid = yMid / sprite.rect.height;
            rms[index].center = new Vector2(xMid, yMid);
            // also set aspect ratio
            rms[index].aspectRatio = (float)(xMax - xMin) / (yMax - yMin);
        }

        Mesh SpriteToMesh(Sprite sprite) {
            Mesh mesh;
            if (!cachedMeshes.TryGetValue(sprite, out mesh)) {
                mesh = new Mesh();
                Vector2[] spriteVertices = sprite.vertices;
                int vertexCount = spriteVertices.Length;
                if (vertices == null) {
                    vertices = new List<Vector3>(vertexCount);
                } else {
                    vertices.Clear();
                }
                for (int x = 0; x < vertexCount; x++) {
                    vertices.Add(spriteVertices[x]);
                }
                ushort[] triangles = sprite.triangles;
                int indexCount = triangles.Length;
                if (indices == null) {
                    indices = new List<int>(indexCount);
                } else {
                    indices.Clear();
                }
                for (int x = 0; x < indexCount; x++) {
                    indices.Add(triangles[x]);
                }
                mesh.SetVertices(vertices);
                mesh.SetTriangles(indices, 0);
                mesh.uv = sprite.uv;
                cachedMeshes[sprite] = mesh;
            }
            return mesh;
        }


        Texture TextureWithBilinearSampling(Texture tex) {
            if (tex == null || tex.filterMode != FilterMode.Point) {
                return tex;
            }
            Texture linTex;
            if (!cachedTextures.TryGetValue(tex, out linTex)) {
#if UNITY_EDITOR
                // Ensure texture is readable
                if (!Application.isPlaying) {
                    string path = AssetDatabase.GetAssetPath(tex);
                    if (!string.IsNullOrEmpty(path)) {
                        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                        if (ti != null) {
                            if (!ti.isReadable) {
                                ti.isReadable = true;
                                ti.SaveAndReimport();
                            }
                        }
                    }
                }
#endif
                linTex = Instantiate<Texture>(tex);
                linTex.filterMode = FilterMode.Bilinear;
                cachedTextures[tex] = linTex;
            }
            return linTex;
        }

    }
}

