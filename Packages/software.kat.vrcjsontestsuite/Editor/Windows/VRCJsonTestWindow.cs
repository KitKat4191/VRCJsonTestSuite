
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
            VisualElement root = rootVisualElement;
            
            VisualElement tree = m_VisualTreeAsset.Instantiate();
            root.Add(tree);
            
            
        }
    }
}
