using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class XMLSaveAndLoad<T>
{
    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static T Load(string path)
    {
        var serializer = new XmlSerializer(typeof(T));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return (T)serializer.Deserialize(stream);
        }
    }
}
