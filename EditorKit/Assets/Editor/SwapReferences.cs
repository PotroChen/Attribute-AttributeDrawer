using System.IO;
using UnityEditor;
using UnityEngine;

public class SwapReferences : EditorWindow
{
    private Object firstObject;
    private Object secondObject;

    [MenuItem("EditorKit/ͨ�ù���/��������", priority = 1)]
    static void Init()
    {
        SwapReferences window = (SwapReferences)EditorWindow.GetWindow(typeof(SwapReferences));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("ѡ��������Դ�������ǵ�����", EditorStyles.boldLabel);

        firstObject = EditorGUILayout.ObjectField("��Դ1", firstObject, typeof(Object), false);
        secondObject = EditorGUILayout.ObjectField("��Դ2", secondObject, typeof(Object), false);

        if (GUILayout.Button("��������"))
        {
            if (firstObject == null || secondObject == null)
                return;

            if (firstObject.GetType() != secondObject.GetType())
                return;

            if (firstObject is GameObject)
                SwapGUIDsForObjects_GameObject(firstObject, secondObject);
            else
                SwapGUIDsForObjects_Asset(firstObject, secondObject);
        }
    }

    /*
     * ֻ֧����ͨԤ���岻֧��ModelԤ����
     */
    static void SwapGUIDsForObjects_GameObject(Object firstObject, Object secondObject)
    {
        string tempPath = "Assets/TempObj_SwapGuid.asset";

        string firstPath = AssetDatabase.GetAssetPath(firstObject);

        string secondPath = AssetDatabase.GetAssetPath(secondObject);

        Object tempObjA = Object.Instantiate(firstObject);
        Object tempObjB = Object.Instantiate(secondObject);

        //��B������->A(meta����)
        //��A������->B(meta����)
        PrefabUtility.SaveAsPrefabAsset((GameObject)tempObjB, firstPath);
        PrefabUtility.SaveAsPrefabAsset((GameObject)tempObjA, secondPath);

        DestroyImmediate(tempObjA);
        DestroyImmediate(tempObjB);

        //Move A To B
        ////Move B To A
        AssetDatabase.MoveAsset(firstPath, tempPath);
        AssetDatabase.MoveAsset(secondPath, firstPath);
        AssetDatabase.MoveAsset(tempPath, secondPath);
        //���ս��:A�����ݱ����B��·��Ҳ��B��ֻ��meta����ԭ���ģ�ͬ��BҲ�ǡ�
        //�����ַ�ʽ����meta
        AssetDatabase.Refresh();

    }

    static void SwapGUIDsForObjects_Asset(Object firstObject, Object secondObject)
    {
        string tempPath = "Assets/TempObj_SwapGuid.asset";
        string firstPath = AssetDatabase.GetAssetPath(firstObject);
        string secondPath = AssetDatabase.GetAssetPath(secondObject);

        #region Swap Serialized Data
        string firstName = firstObject.name;
        string secondName = secondObject.name;
        Object tempObject = Object.Instantiate(firstObject);
        if (tempObject is Mesh tempMesh)//Mesh�Ƚ����⣬Copy֮ǰҪ��Clear��(��Ȼ,����λ�ò������)
            tempMesh.Clear();

        EditorUtility.CopySerialized(firstObject, tempObject);

        if (firstObject is Mesh firstMesh)
            firstMesh.Clear();

        EditorUtility.CopySerialized(secondObject, firstObject);

        if (secondObject is Mesh secondMesh)
            secondMesh.Clear();

        EditorUtility.CopySerialized(tempObject, secondObject);

        DestroyImmediate(tempObject, true);

        //�ָ�����
        firstObject.name = firstName;
        secondObject.name = secondName;
        #endregion

        AssetDatabase.MoveAsset(firstPath, tempPath);
        AssetDatabase.MoveAsset(secondPath, firstPath);
        AssetDatabase.MoveAsset(tempPath, secondPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}