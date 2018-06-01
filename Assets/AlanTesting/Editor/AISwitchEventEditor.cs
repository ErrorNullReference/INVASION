//using UnityEditor;

//[CustomEditor(typeof(AISwitchEvent))]
//public class AISwitchEventEditor : Editor
//{
//    private SerializedProperty nextBehaviour;

//    private void OnEnable()
//    {
//        this.nextBehaviour = this.serializedObject.FindProperty("nextBehaviour");
//    }

//    public override void OnInspectorGUI()
//    {
//        this.serializedObject.Update();

//        EditorGUILayout.BeginHorizontal();
//        //base.OnInspectorGUI();
//        EditorGUILayout.PropertyField(this.nextBehaviour);
//        EditorGUILayout.EndHorizontal();

//        this.serializedObject.ApplyModifiedProperties();
//    }
//}