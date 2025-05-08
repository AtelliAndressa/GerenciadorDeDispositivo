using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Realms;

namespace DeviceManager.API.Models
{
    public class Dispositivo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Descricao { get; set; }

        [BsonElement("CodigoReferencia")]
        public string CodigoReferencia { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataAtualizacao { get; set; }
    }

}
