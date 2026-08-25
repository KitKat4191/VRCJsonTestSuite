
//#define KAT_DO_LOGGING

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using KatSoftware.VRCJsonTestSuite.Runtime;

namespace KatSoftware.VRCJsonTestSuite.Tests
{
    public enum TestResult
    {
        ExpectedResult,
        ShouldSucceedButFailed,
        ShouldFailButSucceeded,
        UndefinedSucceeded,
        UndefinedFailed,
        ParserCrashed
    }
    
    public static class TestCaseProvider
    {
        private const string _PARSING_TESTS_PATH = "JSONTestSuite/test_parsing";
        
        public static object[] GetTestCases(string path = _PARSING_TESTS_PATH)
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>(path);
            
#if KAT_DO_LOGGING
            Debug.Log($"{assets.Length} JSON Tests Loaded!");
#endif

            // ReSharper disable once CoVariantArrayConversion
            return assets.Select(x => new object[] { x.name, x.text }).ToArray();
        }
        
        public static (string, string)[] GetTestCasesTuple(string path = _PARSING_TESTS_PATH)
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>(path);
            
#if KAT_DO_LOGGING
            Debug.Log($"{assets.Length} JSON Tests Loaded!");
#endif

            // ReSharper disable once CoVariantArrayConversion
            return assets.Select(x => (x.name, x.text)).ToArray();
        }
    }

    public abstract class JsonTester
    {
        // This is required or else TestCaseSource won't find it.
        public static object[] GetTestCases() => TestCaseProvider.GetTestCases();
        
        public static TestResult RunTest(JsonValidator validator, string testName, string testJson, out string info)
        {
            info = "";
            string successType = testName.Split("_", StringSplitOptions.RemoveEmptyEntries)[0];

            bool success;
            
            try
            {
                success = validator.TryParse(testJson, out info);
            }
            catch (Exception e)
            {
                info = e.Message;
#if KAT_DO_LOGGING
                Debug.LogException(e);
#endif
                return TestResult.ParserCrashed;
            }

            return successType switch
            {
                "y" => success ? TestResult.ExpectedResult : TestResult.ShouldSucceedButFailed,
                "n" => success ? TestResult.ShouldFailButSucceeded : TestResult.ExpectedResult,
                "i" => success ? TestResult.UndefinedSucceeded : TestResult.UndefinedFailed,
                _ => throw new InvalidDataException("Undefined success type: " + successType)
            };
        }

        protected static void HandleAssertions(TestResult result, string testName, string info)
        {
            switch (result)
            {
                case TestResult.ExpectedResult:
                    Assert.Pass(info);
                    break;
                case TestResult.ShouldSucceedButFailed:
                case TestResult.ShouldFailButSucceeded:
                    Assert.Fail(info);
                    break;
                case TestResult.UndefinedSucceeded:
                case TestResult.UndefinedFailed:
                    Assert.Ignore($"Parser {(result == TestResult.UndefinedSucceeded ? "accepted" : "rejected")} undefined test '{testName}'\n" + info);
                    break;
                case TestResult.ParserCrashed:
                    Assert.Fail(info);
                    break;
                default:
                    Assert.Ignore("Undefined TestResult: " + result);
                    break;
            }
        }
    }
    
    public class VRCJsonTests : JsonTester
    {
        [TestCaseSource(nameof(GetTestCases))]
        public void Parsing(string testName, string testJson)
        {
            LogAssert.ignoreFailingMessages = true;

            TestResult result = RunTest(new VRCJsonValidator(), testName, testJson, out string info);
            HandleAssertions(result, testName, info);
        }
    }
    
    public class NewtonsoftJsonTests : JsonTester
    {
        [TestCaseSource(nameof(GetTestCases))]
        public void Parsing(string testName, string testJson)
        {
            LogAssert.ignoreFailingMessages = true;

            TestResult result = RunTest(new NewtonsoftJsonValidator(), testName, testJson, out string info);
            HandleAssertions(result, testName, info);
        }
    }
}
