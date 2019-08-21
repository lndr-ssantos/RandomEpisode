using System;
using System.Text;

namespace RandomEpisodeAPI.Infra.Gateways
{
    public class Query
    {
        public Query(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException($"{nameof(apiKey)} can not be Null nor Empty");
            }
            _uriBuilder = new StringBuilder($"?api_key={apiKey}");
        }


        private StringBuilder _uriBuilder;

        public void Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"{key} can not be Null nor Empty");
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"{value} can not be Null nor Empty");
            }
            _uriBuilder.Append($"&{key}={value}");
        }

        public override string ToString()
        {
            return _uriBuilder.ToString();
        }
    }
}
