using UnityEditor;
using UnityEngine.UIElements;

namespace DialogEditor.Helper 
{
    public static class DialogEditorStyleSheetHelper
    {
        public static VisualElement ApplyStyleSheet(this VisualElement element, params string[] styleSheetsName) 
        {
            foreach (string styleSheetName in styleSheetsName) 
            {
                StyleSheet styleSheet = EditorGUIUtility.Load(styleSheetName) as StyleSheet;
                element.styleSheets.Add(styleSheet);
            }

            return element;
        }

        public static VisualElement ApplyClasses(this VisualElement element, params string[] className) 
        {
            foreach (var item in className)
            {
                element.AddToClassList(item);
            }

            return element;
        }
    }
}

