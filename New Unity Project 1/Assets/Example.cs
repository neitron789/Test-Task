using System;
using JsonFx.Json;
using JsonFx.Json.Resolvers;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;
using UnityEngine;
using Debug = UnityEngine.Debug;



public class Example:MonoBehaviour
{
    private void Start()
    {
        var resolver = new CombinedResolverStrategy
            (
                new JsonResolverStrategy()
            ); 

        JsonInitClass _instance = new JsonInitClass();

        Debug.Log (_instance.EquipedWeapon);
        Debug.Log (_instance.FNum);
        Debug.Log (_instance.BValue);

        DataWriterSettings dataWriterSettings = new DataWriterSettings(resolver);

        JsonReader reader = new JsonReader(new DataReaderSettings(resolver));

        JsonWriter writer = new JsonWriter(dataWriterSettings);

        string json = writer.Write(new Ololo());

        _instance = reader.Read<JsonInitClass>(json);

        Debug.Log (_instance.EquipedWeapon);
        Debug.Log (_instance.FNum);
        Debug.Log (_instance.BValue);
    }
}


public class JsonInitClass
{
    [JsonName("Weapon")]
    public string EquipedWeapon = "knife";

    [JsonName("Instance")]
    public A _aInstance = new A();

    public double FNum = 1f;

    public bool BValue = true;

    public string StrValue = "";

    public string Str = "";



}

public class Ololo
{
    [JsonName("Weapon")]
    public string Weapon = "rifle";

    [JsonName("Instance")]
    public A A = new A();

    public double FNum = 3.2f;

    [JsonIgnore]
    public bool BValue = false;

    public string StrValue = "";

    public string Str = "";
}

public class A
{
    public int k = 2123123;
    public Boolean q = true;
    [JsonIgnore]
    public ABC abcStruct;
}

public struct ABC
{
    public float floatNum;

    public ABC(float f)
    {
        floatNum = f;
    }
}



[System.AttributeUsage(System.AttributeTargets.Class)]
public class DataContractAttribute : System.Attribute
{
    private string _name;

    public DataContractAttribute(string name, string namespaceStr)
    {
        Name = name;

        _name = Name;

        Namespace = namespaceStr;
    }

    public string Namespace
    {
        get;
        set;
    }


    public string Name
    {
        get;
        set;
    }
}

[System.AttributeUsage(System.AttributeTargets.Field)]
public class DataMemberAttribute : System.Attribute
{
     private string _name;

    public DataMemberAttribute(string name)
    {
        Name = name;

        _name = Name;
    }

    public string Name
    {
        get;
        set;
    }
}


