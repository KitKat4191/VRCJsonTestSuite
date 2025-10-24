
using System;
using Newtonsoft.Json;
using UnityEngine;
using VRC.SDK3.Data;

namespace KatSoftware.VRCJsonTestSuite.Runtime
{
    public enum ParseResult
    {
        Accepted,
        Rejected,
        Exception,
        Timeout
    }

    public class VRCJsonValidator
    {
        public static ParseResult Validate(string json)
        {
            try
            {
                Debug.Log("Validating: " + json);

                if (!VRCJson.TryDeserializeFromJson(json, out DataToken result))
                {
                    Debug.LogError("Failed to deserialize! Error: " + result.Error);
                    return ParseResult.Rejected;
                }

                if (!VRCJson.TrySerializeToJson(result, JsonExportType.Minify, out DataToken result2))
                {
                    Debug.LogError("Failed to reserialize! Error: " + result.Error);
                    return ParseResult.Rejected;
                }

                Debug.Log("Reserialized to: " + result2.String);

                return ParseResult.Accepted;
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return ParseResult.Exception;
            }
        }
    }
    
    public class NewtonsoftJsonValidator
    {
        public static ParseResult Validate(string json)
        {
            try
            {
                if (json.Contains("🇨🇭") || json.Contains("🌀"))
                {
                    throw new Exception("Prevented emoji from crashing Unity :)");
                }
                
                Debug.Log("Validating: " + json);
                
                var deserializeResult = JsonConvert.DeserializeObject(json);
                var reserializeResult = JsonConvert.SerializeObject(deserializeResult);

                Debug.Log("Reserialized to: " + reserializeResult);

                return ParseResult.Accepted;
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                return ParseResult.Exception;
            }
        }
    }
}
