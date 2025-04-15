using Newtonsoft.Json;

namespace TransportNetworking.transport.network.rpcprotocol;

[Serializable]
public class Response
{
    public Response() { }
    
    [JsonProperty("type")]
    public ResponseType Type { get; set; }
    
    
    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    public object Data { get; set; }

    public override string ToString()
    {
        return $"Response{{type='{Type}', data='{Data}'}}";
    }

    public class Builder
    {
        private Response response = new Response();

        public Builder Type(ResponseType type)
        {
            response.Type = type;
            return this;
        }

        public Builder Data(object data)
        {
            response.Data = data;
            return this;
        }

        public Response Build()
        {
            return response;
        }
    }
}