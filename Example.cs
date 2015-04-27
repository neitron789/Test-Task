using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;
using Debug = UnityEngine.Debug;


public class Example:MonoBehaviour
{
   

    private void Start()
    {
        string json = JsonWriter.Serialize (new Ololo());



        JsonInitClass _jsonInitInst = CustomDeserializator.Deserialize<JsonInitClass>(json);

        Debug.Log(_jsonInitInst.BValue);
        Debug.Log(_jsonInitInst.FNum);
        Debug.Log(_jsonInitInst.INum);
        Debug.Log(_jsonInitInst.StrValue);
    }
}


public class JsonInitClass
{
    public int INum = 3;
    public double FNum = 3.2f;
    public bool BValue = false;
    public string StrValue = "";
}


public class Ololo
{
    [JsonMember]
    public int k = 2123123;
    [JsonMember]
    public int q = 2123123;
    [JsonMember]
    public string d = "casdcjksdmkl";
    [JsonMember]
    public A aInstance = new A();
}

public class A
{
    [JsonMember]
    public Boolean q = true;

    [JsonMember]
    public ABC abcStruct = new ABC (123.45f);
}

public struct ABC
{
    [JsonMember]
    public float floatNum;

    public ABC(float f)
    {
        floatNum = f;
    }
}







