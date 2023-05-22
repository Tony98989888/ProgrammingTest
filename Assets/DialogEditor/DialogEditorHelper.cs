using System;
using UnityEngine;
using DialogEditor;

namespace DialogEditor
{
    public enum DialogType
    {
        Single,
        Multiple
    }
    
    public static class DialogEditorHelper
    {
        public static Rect ToRect(this Vector2 pos)
        {
            return new Rect(pos, Vector2.zero);
        }
        
    }
}