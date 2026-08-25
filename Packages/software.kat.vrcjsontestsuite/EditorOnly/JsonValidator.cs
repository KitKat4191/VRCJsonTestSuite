
//#define KAT_DO_LOGGING

using System;
using Newtonsoft.Json;

#if KAT_DO_LOGGING
using UnityEngine;
#endif
using VRC.SDK3.Data;

namespace KatSoftware.VRCJsonTestSuite.Runtime
{
    public abstract class JsonValidator
    {
        public abstract bool TryParse(string json, out string info);
    }

    public class VRCJsonValidator : JsonValidator
    {
        public override bool TryParse(string json, out string info)
        {
            if (!VRCJson.TryDeserializeFromJson(json, out DataToken result))
            {
                info = "Failed to deserialize! Error: " + result.Error;
#if KAT_DO_LOGGING
                Debug.LogError(info);
#endif
                return false;
            }
            
            if (!VRCJson.TrySerializeToJson(result, JsonExportType.Minify, out DataToken result2))
            {
                info = "Failed to reserialize! Error: " + result.Error;
#if KAT_DO_LOGGING
                Debug.LogError(info);
#endif
                return false;
            }

            info = $"input '{json}' reserialized to: '{result2.String}'";
#if KAT_DO_LOGGING
            Debug.Log(info);
#endif
            
            return true;
        }
    }

    public class NewtonsoftJsonValidator : JsonValidator
    {
        public override bool TryParse(string json, out string info)
        {
            if (json.Contains("🇨🇭") || json.Contains("🌀"))
            {
                info = "Prevented emoji from crashing Unity :)";
                throw new Exception(info);
            }

            try
            {
                var deserializeResult = JsonConvert.DeserializeObject(json);
                var reserializeResult = JsonConvert.SerializeObject(deserializeResult);
                info = $"input '{json}' reserialized to: '{reserializeResult}'";
#if KAT_DO_LOGGING
                Debug.Log(info);
#endif
                return true;
            }
            catch (Exception e)
            {
                info = e.Message;
#if KAT_DO_LOGGING
                Debug.LogException(e);
#endif
                return false;
            }
        }
    }
}
