using Newtonsoft.Json;

namespace TransportNetworking.transport.network.rpcprotocol;

[Serializable]
public class Request
{
    private Request()
    {
    }

    [JsonProperty("type")]
    public RequestType Type { get; set; }

    [JsonProperty(TypeNameHandling = TypeNameHandling.Auto)]
    public object Data { get; set; }

    public override string ToString()
    {
        return $"Request{{type='{Type}', data='{Data}'}}";
    }

    public class Builder
    {
        private Request request = new Request();

        public Builder Type(RequestType type)
        {
            request.Type = type;
            return this;
        }

        public Builder Data(object data)
        {
            request.Data = data;
            return this;
        }

        public Request Build()
        {
            return request;
        }
    }
}