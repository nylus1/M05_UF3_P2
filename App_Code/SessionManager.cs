
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
public static class SessionManager
{ 
    public static void Save(ISession session, string key, object value)
    {
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
                session.SetInt32(key, (int)value);
                break;
            case TypeCode.String:
                session.SetString(key, value.ToString());
                break;
            default:
                session.Set(key, ObjectToByteArray(value));
                break;
        }
    }
    public static T Load<T>(ISession session, string key)
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
                int? tempVal = session.GetInt32(key);
                return (T)Convert.ChangeType(tempVal == null ? tempVal.Value : default(int), typeof(T));
            case TypeCode.String:
                return (T)Convert.ChangeType(session.GetString(key), typeof(T));
            default:
                return (T)Convert.ChangeType(ByteArrayToObject(session.Get(key)), typeof(T));
        }
    }
    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(object obj)
    {
        if (obj == null)
            return null;

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);

        return ms.ToArray();
    }

    // Convert a byte array to an Object
    public static object ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        object obj = (object)binForm.Deserialize(memStream);

        return obj;
    }
}
