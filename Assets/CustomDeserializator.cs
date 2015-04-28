using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using System.Reflection;
using ObjDict = System.Collections.Generic.Dictionary<string, object>;
using TypeDict = System.Collections.Generic.Dictionary<System.Type, object>;



public static class CustomDeserializator
{
   private static TypeDict _jsonObjTypeDict = new TypeDict();


    public static T Deserialize <T>(string json) where T: new ()
    {
        ObjDict jsonDict = JsonReader.Deserialize (json) as ObjDict;

        object valueObj;

        ComposeTypeDict (jsonDict);

        T newObj = new T();

        FieldInfo[] fieldsInfo = newObj.GetType().GetFields
            (
            BindingFlags.NonPublic |
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public
            );


        foreach (var field in fieldsInfo)
        {
            if (_jsonObjTypeDict.TryGetValue (field.FieldType, out valueObj))
            {

                field.SetValue(newObj, valueObj);
            }
        }
        return newObj;
    }



    private static void ComposeTypeDict (ObjDict dict)
    {       
        foreach (var value in dict.Values)
        {
            ObjDict objDict = value as ObjDict;

            if (objDict != null)
            {
                ComposeTypeDict (objDict);
            }
            else
            {
                if (!_jsonObjTypeDict.ContainsKey(value.GetType()))
                _jsonObjTypeDict.Add (value.GetType(), value);  
            }
        }
    }
}