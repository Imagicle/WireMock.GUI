using Newtonsoft.Json;

namespace WireMock.GUI.Utility
{
    internal static class JsonUtilities
    {
        public static string Minify(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            try
            {
                var deserializedJson = JsonConvert.DeserializeObject(json);
                return JsonConvert.SerializeObject(deserializedJson);
            }
            catch
            {
                return json;
            }
        }
    }
}