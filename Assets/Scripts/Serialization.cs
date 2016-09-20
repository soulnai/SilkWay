using UnityEngine;
using System.Collections;
using FullSerializer;
using System;

public class Serialization {

    public static string Serialize(Type type, object value)
    {
        // serialize the data 
        fsData data;
        fsSerializer _serializer = new fsSerializer();
        _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON 
        return fsJsonPrinter.CompressedJson(data);
    }

    public static object Deserialize(Type type, string serializedState)
    {
        // step 1: parse the JSON data 
        fsData data = fsJsonParser.Parse(serializedState);
        fsSerializer _serializer = new fsSerializer();
        // step 2: deserialize the data 
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }
}
