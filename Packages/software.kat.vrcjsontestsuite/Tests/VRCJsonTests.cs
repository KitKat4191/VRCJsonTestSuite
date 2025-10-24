
using System;
using System.Linq;
using NUnit.Framework;

using UnityEngine;
using KatSoftware.VRCJsonTestSuite.Runtime;
using UnityEngine.TestTools;

namespace KatSoftware.VRCJsonTestSuite.Tests
{
    public class VRCJsonTests
    {
        [TestCaseSource(nameof(GetTestCases))]
        public void Parsing(string name, string content)
        {
            LogAssert.ignoreFailingMessages = true;
            
            string successType = name.Split("_", StringSplitOptions.RemoveEmptyEntries)[0];

            switch (successType)
            {
                case "y":
                    Assert.AreEqual(ParseResult.Accepted, VRCJsonValidator.Validate(content));
                    break;
                case "n":
                    Assert.AreEqual(ParseResult.Rejected, VRCJsonValidator.Validate(content));
                    break;
                case "i":
                    bool accepted = VRCJsonValidator.Validate(content) == ParseResult.Accepted;
                    Debug.Log($"Parser {(accepted ? "accepted" : "rejected")} undefined case '{name}'");
                    Assert.True(accepted);
                    break;
                default:
                    Assert.True(false);
                    break;
            }
        }

        public static object[] GetTestCases()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("JSONTestSuite/test_parsing");
            
            Debug.Log("Text assets loaded: " + assets.Length);

            return assets.Select(x => new object[] { x.name, x.text }).ToArray();
        }
    }
    
    public class NewtonsoftJsonTests
    {
        [TestCaseSource(nameof(GetTestCases))]
        public void Parsing(string name, string content)
        {
            LogAssert.ignoreFailingMessages = true;
            
            string successType = name.Split("_", StringSplitOptions.RemoveEmptyEntries)[0];

            switch (successType)
            {
                case "y":
                    Assert.AreEqual(ParseResult.Accepted, NewtonsoftJsonValidator.Validate(content));
                    break;
                case "n":
                    Assert.AreNotEqual(ParseResult.Accepted, NewtonsoftJsonValidator.Validate(content));
                    break;
                case "i":
                    bool accepted = NewtonsoftJsonValidator.Validate(content) == ParseResult.Accepted;
                    Debug.Log($"Parser {(accepted ? "accepted" : "rejected")} undefined case '{name}'");
                    Assert.True(accepted);
                    break;
                default:
                    Assert.True(false);
                    break;
            }
        }

        public static object[] GetTestCases()
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>("JSONTestSuite/test_parsing");
            
            Debug.Log("Text assets loaded: " + assets.Length);

            return assets.Select(x => new object[] { x.name, x.text }).ToArray();
        }
    }
}
