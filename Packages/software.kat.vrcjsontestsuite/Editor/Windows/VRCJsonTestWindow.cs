
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
        
        private bool _hideExpected;
        private bool _hideUndefined;
        private bool _diff;
        
        private ListView _listView;
        private Label _elementCount;
        
        private readonly List<(string, TestResult, string, TestResult, string, string)> _items = new ();
        private List<(string, TestResult, string, TestResult, string, string)> _filteredItems = new ();
        
        private void UpdateFilteredItems()
        {
            _filteredItems.Clear();
            _filteredItems.AddRange(_items);
            if (_hideExpected)
                _filteredItems = _filteredItems.Where(x => x.Item4 != TestResult.CorrectFail && x.Item4 != TestResult.CorrectSuccess).ToList();

            if (_hideUndefined)
                _filteredItems = _filteredItems.Where(x =>
                    x.Item4 is not (TestResult.UndefinedFailed or TestResult.UndefinedSucceeded)).ToList();
            
            if (_diff)
                _filteredItems = _filteredItems.Where(x => x.Item2 != x.Item4).ToList();
        }

        private void HandleUpdatedGUI()
        {
            UpdateFilteredItems();
                
            if (_listView != null) _listView.itemsSource = _filteredItems;
            _listView?.RefreshItems();
            
            _elementCount.text = $"Showing {_filteredItems.Count}/{_items.Count}";
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            
            VisualElement tree = mVisualTreeAsset.Instantiate();
            root.Add(tree);
            
            var toggleExpected = root.Q<Toggle>("hide-expected-toggle");
            toggleExpected.RegisterValueChangedCallback(evt =>
            {
                _hideExpected = evt.newValue;
                HandleUpdatedGUI();
            });
            toggleExpected.value = false;
            
            var toggleUndefined = root.Q<Toggle>("hide-undefined-toggle");
            toggleUndefined.RegisterValueChangedCallback(evt =>
            {
                _hideUndefined = evt.newValue;
                HandleUpdatedGUI();
            });
            toggleUndefined.value = false;

            var diffToggle = root.Q<Toggle>("diff-toggle");
            diffToggle.RegisterValueChangedCallback(evt =>
            {
                _diff = evt.newValue;
                HandleUpdatedGUI();
            });
            diffToggle.value = false;
            
            var runTestsButton = root.Q<Button>("run-tests-button");
            runTestsButton.clicked += RunTests;
            
            _elementCount = root.Q<Label>("element-count-text");
            
            _listView = root.Q<ListView>();
            _listView.makeItem = MakeItem;
            _listView.bindItem = BindItem;
            _listView.selectionType = SelectionType.None;
            var scrollView = _listView.Q<ScrollView>();
            scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            scrollView.mouseWheelScrollSize = 50;

            RunTests();
        }

        private void RunTests()
        {
            (string, string)[] cases = TestCaseProvider.GetTestCasesTuple();
            
            _items.Clear();

            foreach ((string, string) testParams in cases)
            {
                string testName = testParams.Item1;
                string testJson = testParams.Item2;
                
                if (testJson.Length > 36)
                    testJson = testJson[..36] + "(...)";
                
                TestResult newtonsoftResult = JsonTester.RunTest(new NewtonsoftJsonValidator(), testName, testJson, out string newtonsoftInfo);
                newtonsoftInfo = $"{GetDebugText(newtonsoftResult)}\n{newtonsoftInfo}";
                TestResult vrcjsonResult = JsonTester.RunTest(new VRCJsonValidator(), testName, testJson, out string vrcJsonInfo);
                vrcJsonInfo = $"{GetDebugText(vrcjsonResult)}\n{vrcJsonInfo}";

                _items.Add((testName, newtonsoftResult, newtonsoftInfo, vrcjsonResult, vrcJsonInfo, testJson));
            }

            HandleUpdatedGUI();
        }
        
        private VisualElement MakeItem() => mListItem.Instantiate();
        
        private void BindItem(VisualElement element, int index)
        {
            var testNameText = element.Q<Label>("test-name-text");
            var testValueText = element.Q<Label>("test-value-text");
            
            var testResult1 = element.Q<VisualElement>("result-1");
            var testResult2 = element.Q<VisualElement>("result-2");
            
            testNameText.text = _filteredItems[index].Item1;
            testResult1.style.backgroundColor = GetColor(_filteredItems[index].Item2);
            testResult1.tooltip = _filteredItems[index].Item3;
            testResult2.style.backgroundColor = GetColor(_filteredItems[index].Item4);
            testResult2.tooltip = _filteredItems[index].Item5;
            testValueText.text = _filteredItems[index].Item6;
        }

        private string GetDebugText(TestResult testResult)
        {
            return testResult switch
            {
                TestResult.CorrectFail => "Expected rejected and it was rejected.",
                TestResult.CorrectSuccess => "Expected accepted and it was accepted.",
                TestResult.ShouldSucceedButFailed => "Expected accepted but got rejected!",
                TestResult.ShouldFailButSucceeded => "Expected rejected but got accepted!",
                TestResult.UndefinedSucceeded => "Undefined case was accepted by the parser.",
                TestResult.UndefinedFailed => "Undefined case was rejected by the parser.",
                TestResult.ParserCrashed => "The parser crashed!",
                _ => throw new ArgumentOutOfRangeException(nameof(testResult), testResult, null)
            };
        }

        private Color GetColor(TestResult testResult)
        {
            return testResult switch
            {
                TestResult.CorrectFail => _green,
                TestResult.CorrectSuccess => _green,
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
