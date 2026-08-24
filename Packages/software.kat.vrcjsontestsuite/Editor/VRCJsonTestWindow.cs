
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace KatSoftware.VRCJsonTestSuite.Editor
{
    public class VRCJsonTestWindow : EditorWindow
    {
        [MenuItem("KatSoftware/VRCJson Test/VRCJson Test Window")]
        public static void OpenWindow()
        {
            var window = GetWindow<VRCJsonTestWindow>();
            window.titleContent = new GUIContent("VRCJson Test Window");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy
            var label = new Label("Hello World!");
            root.Add(label);

            // Create button
            var button = new Button
            {
                name = "button",
                text = "Button"
            };
            root.Add(button);

            // Create toggle
            var toggle = new Toggle
            {
                name = "toggle",
                label = "Toggle"
            };
            root.Add(toggle);
        }
    }
}
