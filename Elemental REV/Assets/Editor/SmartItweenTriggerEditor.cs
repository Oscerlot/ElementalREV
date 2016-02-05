using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Inspector YEAH ohh YEAH!!
/// </summary>
[CustomEditor(typeof(SmartItweenObject)), CanEditMultipleObjects]
[System.Serializable]
public class SmartItweenTriggerEditor : Editor
{


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var myScript = target as SmartItweenObject;

        if (myScript.triggerType == SmartItweenObject.TriggerType.activatedOnce)
        {
            //Reset to origin options
            myScript.resetToOriginalPos = EditorGUILayout.Toggle("Reset to Origin", myScript.resetToOriginalPos);
            if (myScript.resetToOriginalPos)
            {
                myScript.resetSpeed = EditorGUILayout.Slider("Return Speed", myScript.resetSpeed, 0, 50);
                myScript.originalPosOffset = EditorGUILayout.Vector3Field("Original Position Offset", myScript.originalPosOffset);
                myScript.toOriginEaseType = (SmartItweenObject.EaseType)EditorGUILayout.EnumPopup("Return EaseType", myScript.toOriginEaseType);
            }

            //Unity Event
            myScript.callEventOnFinish = EditorGUILayout.Toggle("CallEventOnFinish", myScript.callEventOnFinish);
            if (myScript.callEventOnFinish)
            {
                SerializedProperty OnPlayOnce = serializedObject.FindProperty("OnPlayOnceFinished");
                EditorGUIUtility.LookLikeControls();
                EditorGUILayout.PropertyField(OnPlayOnce);
            }
        }

        //So that settings stay on play (in case of prefabs)
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
    }
}