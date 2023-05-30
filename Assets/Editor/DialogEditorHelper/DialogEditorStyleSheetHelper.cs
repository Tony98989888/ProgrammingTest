using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogEditor.Helper 
{
    public static class DialogEditorStyleSheetHelper
    {
        public static Color DefaultNodeBGColor = new Color(26f / 255f, 18f / 255f, 11f / 255f);

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

