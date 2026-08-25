
using System;
using System.IO;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using KatSoftware.VRCJsonTestSuite.Tests;
using KatSoftware.VRCJsonTestSuite.Runtime;

namespace KatSoftware.VRCJsonTestSuite.Editor.Windows
{
    public class VrcJsonTestWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset mVisualTreeAsset;
        [SerializeField] private VisualTreeAsset mListItem;

        [MenuItem("KatSoftware/VRCJson Test/VRCJson Test Window")]
        public static void OpenWindow()
        {
            var window = GetWindow<VrcJsonTestWindow>();
            window.titleContent = new GUIContent("VRCJson Test Window");
            window.maxSize = new Vector2(1024, 700);
            window.minSize = window.maxSize;
        }
        
        private Button _runTestsButton;
        private ListView _listView;
        private VisualElement _listElement;
        private readonly List<(string, TestResult, TestResult, string)> _items = new ();

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            
            VisualElement tree = mVisualTreeAsset.Instantiate();
            root.Add(tree);
            
            _runTestsButton = root.Q<Button>("run-tests-button");
            _runTestsButton.clicked += RunTests;
            
            _listView = root.Q<ListView>();
            _listView.makeItem = MakeItem;
            _listView.bindItem = BindItem;
            _listView.itemsSource = _items;
            _listView.selectionType = SelectionType.None;
            _listView.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;
        }

        private void RunTests()
        {
            (string, string)[] cases = TestCaseProvider.GetTestCasesTuple();
            
            _items.Clear();

            foreach ((string , string) testParams in cases)
            {
                string testName = testParams.Item1;
                string testJson = testParams.Item2;
                
                if (testJson.Length > 36)
                    testJson = testJson[..36] + "(...)";
                
                TestResult newtonsoft = JsonTester.RunTest(new NewtonsoftJsonValidator(), testName, testJson);
                TestResult vrcjson = JsonTester.RunTest(new VRCJsonValidator(), testName, testJson);

                _items.Add((testName, newtonsoft, vrcjson, testJson));
            }

            _listView?.RefreshItems();
        }
        
        private VisualElement MakeItem() => mListItem.Instantiate();
        
        private void BindItem(VisualElement element, int index)
        {
            var testNameText = element.Q<Label>("test-name-text");
            var testValueText = element.Q<Label>("test-value-text");
            
            var testResult1 = element.Q<VisualElement>("result-1");
            var testResult2 = element.Q<VisualElement>("result-2");
            
            testNameText.text = _items[index].Item1;
            testResult1.style.backgroundColor = GetColor(_items[index].Item2);
            testResult2.style.backgroundColor = GetColor(_items[index].Item3);
            testValueText.text = _items[index].Item4;
        }

        private Color GetColor(TestResult testResult)
        {
            return testResult switch
            {
                TestResult.ExpectedResult => _green,
                TestResult.ShouldSucceedButFailed => _orange,
                TestResult.ShouldFailButSucceeded => _yellow,
                TestResult.UndefinedSucceeded => _lightBlue,
                TestResult.UndefinedFailed => _darkBlue,
                TestResult.ParserCrashed => _red,
                _ => throw new ArgumentOutOfRangeException(nameof(testResult), testResult, null)
            };
        }

        private Color _green = HexToColor("#CCFFCC");
        private Color _orange = HexToColor("#CC6600");
        private Color _yellow = HexToColor("#FFCC33");
        private Color _lightBlue = HexToColor("#64C8FA");
        private Color _darkBlue = HexToColor("#0066FF");
        private Color _red = HexToColor("#FF3333");
        private Color _gray = HexToColor("#666666");
        private static Color HexToColor(string hex)
        {
            if (!ColorUtility.TryParseHtmlString(hex, out Color myColor))
                throw new InvalidDataException(hex);
            return myColor;
        }
    }
}
