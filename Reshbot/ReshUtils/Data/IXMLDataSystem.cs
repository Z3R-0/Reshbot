using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ReshUtils.Data {

    public abstract class IXMLModel {

        DateTime created_at { get; }
        DateTime last_updated { get; set; }
    }

    public abstract class IXMLDataSystem {

        XmlSerializer Serializer { get; }
        IXMLModel DataModel { get; }
        TextWriter TextWriter { get; }
        string DataPath { get; set; }
        Stream ReadStream { get; }
        
        public IXMLDataSystem(Type dataModel) {
            Serializer = new XmlSerializer(dataModel.GetType());
            TextWriter = new StreamWriter($"data/{dataModel}/");
            ReadStream = new FileStream($"data/{dataModel}/", FileMode.Open);
        }

        public IXMLDataSystem(Type dataModel, string path) {
            DataPath = path;
            Serializer = new XmlSerializer(dataModel);
            TextWriter = new StreamWriter(DataPath);
            ReadStream = new FileStream(path, FileMode.Open);
        }

        public void Serialize(IXMLModel objectToSerialize) {
            Serializer.Serialize(TextWriter, objectToSerialize);
        }

        public T Deserialize<T>() {
            return (T) Serializer.Deserialize(ReadStream);
        }
    }
}
