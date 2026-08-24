
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KatSoftware.VRCJsonTestSuite.Editor.Windows
{
    public class VRCJsonTestWindow : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset;

        [MenuItem("KatSoftware/VRCJson Test/VRCJson Test Window")]
        public static void OpenWindow()
        {
            var window = GetWindow<VRCJsonTestWindow>();
            window.titleContent = new GUIContent("VRCJson Test Window");
            window.maxSize = new Vector2(550, 550);
            window.minSize = window.maxSize;
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);

            // Instantiate UXML
            VisualElement tree = m_VisualTreeAsset.Instantiate();
            root.Add(tree);
        }
    }
}
