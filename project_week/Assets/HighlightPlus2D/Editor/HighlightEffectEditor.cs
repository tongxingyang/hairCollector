using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HighlightPlus2D {
    [CustomEditor(typeof(HighlightEffect2D))]
    [CanEditMultipleObjects]
    public class HighlightEffectEditor : Editor {

        SerializedProperty previewInEditor;
        SerializedProperty pixelSnap, alphaCutOff, pivotPos, polygonPacking, autoSize, center, scale, aspectRatio;
        SerializedProperty glow, glowWidth, glowRenderQueue, glowDithering, glowMagicNumber1, glowMagicNumber2, glowSmooth, glowQuality, glowOnTop, glowPasses;
        Color hitColor = Color.white;
        float hitDuration = 1f;
        float hitMinIntensity = 1f;

        void OnEnable() {
            previewInEditor = serializedObject.FindProperty("previewInEditor");
            polygonPacking = serializedObject.FindProperty("polygonPacking");
            glow = serializedObject.FindProperty("glow");
            glowWidth = serializedObject.FindProperty("glowWidth");
            glowRenderQueue = serializedObject.FindProperty("glowRenderQueue");
            glowDithering = serializedObject.FindProperty("glowDithering");
            glowMagicNumber1 = serializedObject.FindProperty("glowMagicNumber1");
            glowMagicNumber2 = serializedObject.FindProperty("glowMagicNumber2");
            glowSmooth = serializedObject.FindProperty("glowSmooth");
            glowQuality = serializedObject.FindProperty("glowQuality");
            glowOnTop = serializedObject.FindProperty("glowOnTop");
            glowPasses = serializedObject.FindProperty("glowPasses");
            pixelSnap = serializedObject.FindProperty("pixelSnap");
            alphaCutOff = serializedObject.FindProperty("alphaCutOff");
            pivotPos = serializedObject.FindProperty("pivotPos");
            autoSize = serializedObject.FindProperty("autoSize");
            center = serializedObject.FindProperty("center");
            scale = serializedObject.FindProperty("scale");
            aspectRatio = serializedObject.FindProperty("aspectRatio");
        }

        public override void OnInspectorGUI()
        {
            HighlightEffect2D thisEffect = (HighlightEffect2D)target;
            bool isManager = thisEffect.GetComponent<HighlightManager2D>() != null;
            EditorGUILayout.Separator();
            serializedObject.Update();
            if (isManager)
            {
                EditorGUILayout.HelpBox("These are default settings for highlighted objects. If the highlighted object already has a Highlight Effect component, those properties will be used.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.PropertyField(previewInEditor);
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Sprite Options", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(pixelSnap);
            EditorGUILayout.PropertyField(alphaCutOff);
            EditorGUILayout.PropertyField(polygonPacking, new GUIContent("Polygon/SVG"));
            if (!polygonPacking.boolValue)
            {
                EditorGUILayout.LabelField("Sprite Pivot", pivotPos.vector2Value.ToString("F4"));
                EditorGUILayout.PropertyField(autoSize);
                GUI.enabled = !autoSize.boolValue;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(scale);
                EditorGUILayout.PropertyField(aspectRatio);
                EditorGUILayout.PropertyField(center);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Highlight Options", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(glow);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(glowWidth, new GUIContent("Width"));
            EditorGUILayout.PropertyField(glowRenderQueue, new GUIContent("GlowRenderQueue"));
            EditorGUILayout.PropertyField(glowSmooth, new GUIContent("Smooth Edges"));
            EditorGUILayout.PropertyField(glowQuality, new GUIContent("Quality"));
            EditorGUILayout.PropertyField(glowDithering, new GUIContent("Dithering"));
            EditorGUILayout.PropertyField(glowOnTop, new GUIContent("Render On Top"));
            if (glowDithering.boolValue)
            {
                EditorGUILayout.PropertyField(glowMagicNumber1, new GUIContent("Magic Number 1"));
                EditorGUILayout.PropertyField(glowMagicNumber2, new GUIContent("Magic Number 2"));
            }
            EditorGUILayout.PropertyField(glowPasses, true);
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to test this feature. In your code, call effect.HitFX() method to execute this hit effect.", MessageType.Info);
            }
            else
            {
                EditorGUI.indentLevel++;
                hitColor = EditorGUILayout.ColorField(new GUIContent("Color"), hitColor);
                hitDuration = EditorGUILayout.FloatField(new GUIContent("Duration"), hitDuration);
                hitMinIntensity = EditorGUILayout.FloatField(new GUIContent("Min Intensity"), hitMinIntensity);
                if (GUILayout.Button("Execute Hit"))
                {
                    thisEffect.HitFX(hitColor, hitDuration, hitMinIntensity);
                }
                EditorGUI.indentLevel--;
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                foreach (HighlightEffect2D effect in targets)
                {
                    effect.Refresh();
                }
            }
        }


        [MenuItem("GameObject/Effects/Highlight Plus 2D/Create Manager", false, 10)]
        static void CreateManager(MenuCommand menuCommand) {
            HighlightManager2D manager = FindObjectOfType<HighlightManager2D>();
            if (manager == null) {
                GameObject managerGO = new GameObject("HighlightPlus2DManager");
                manager = managerGO.AddComponent<HighlightManager2D>();
                // Register root object for undo.
                Undo.RegisterCreatedObjectUndo(manager, "Create Highlight Plus 2D Manager");
            }
            Selection.activeObject = manager;
        }

    }

}