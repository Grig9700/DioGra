using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class GetListeners
{
    public static int GetNonPersistentListenerNumber(this UnityEventBase unityEvent)
    {
        var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        var invokeCallList = field.GetValue(unityEvent);
        var property = invokeCallList.GetType().GetProperty("Count");
        return (int)property.GetValue(invokeCallList);
    }
    
    public static int GetPersistentListenerNumber(this UnityEventBase unityEvent)
    {
        return (int) unityEvent.GetPersistentEventCount();
    }
    
    public static int GetListenerNumber(this UnityEventBase unityEvent)
    {
        return (int) GetNonPersistentListenerNumber(unityEvent) + GetPersistentListenerNumber(unityEvent);
    }

    public static void TestVariableObjectConnections()
    {
        VariableObject[] variableObjects = Resources.LoadAll<VariableObject>("Variables");

        foreach (var obj in variableObjects)
        {
            var references = FindReferencesTo(obj);

            if (!references.Any()) 
            {
                Debug.Log($"{obj.name} is not modified anywhere", obj);
            }
            
            // foreach (var reference in references)
            // {
            //     Debug.Log($"referenced by {reference.name}, {reference.GetType()}", reference);
            // }
        }
    }
    
    private static List<Object> FindReferencesTo(Object to)
    {
        var referencedBy = new List<Object>();
        var allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (var go in allObjects)
        {
            if (PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Regular)
            {
                if (PrefabUtility.GetCorrespondingObjectFromSource(go) == to)
                {
                    //Debug.Log($"referenced by {go.name}, {go.GetType()}", go);
                    referencedBy.Add(go);
                }
            }
            
            var components = go.GetComponents<Component>();
            foreach (var c in components)
            {
                if (!c) continue;
 
                var so = new SerializedObject(c);
                var sp = so.GetIterator();

                while (sp.NextVisible(true))
                {
                    if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
                    
                    if (sp.objectReferenceValue != to) continue;
                    
                    //Debug.Log($"referenced by {c.name}, {c.GetType()}", c);
                    referencedBy.Add(c.gameObject);
                }
            }
        }
 
        return referencedBy;
    }
}
