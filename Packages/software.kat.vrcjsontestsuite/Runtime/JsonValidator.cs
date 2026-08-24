
using System;
using Newtonsoft.Json;

using UnityEngine;
using VRC.SDK3.Data;

namespace KatSoftware.VRCJsonTestSuite.Runtime
{
    public abstract class JsonValidator
    {
        public abstract bool TryParse(string json);
    }

    public class VRCJsonValidator : JsonValidator
    {
        public override bool TryParse(string json)
        {
            if (!VRCJson.TryDeserializeFromJson(json, out DataToken result))
            {
                Debug.LogError("Failed to deserialize! Error: " + result.Error);
                return false;
            }
            
            if (!VRCJson.TrySerializeToJson(result, JsonExportType.Minify, out DataToken result2))
            {
                Debug.LogError("Failed to reserialize! Error: " + result.Error);
                return false;
            }
            
            Debug.Log($"input '{json}' reserialized to: '{result2.String}'");
            
            return true;
        }
    }

    public class NewtonsoftJsonValidator : JsonValidator
    {
        public override bool TryParse(string json)
        {
            if (json.Contains("🇨🇭") || json.Contains("🌀"))
            {
                throw new Exception("Prevented emoji from crashing Unity :)");
            }

            try
            {
                var deserializeResult = JsonConvert.DeserializeObject(json);
                var reserializeResult = JsonConvert.SerializeObject(deserializeResult);
                Debug.Log($"input '{json}' reserialized to: '{reserializeResult}'");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
    }
}
