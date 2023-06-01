using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogEditor
{
    public class DialogNodeGroup : Group
    {
        Color m_defaultBorderColor;
        float m_defaultBorderWidth;

        public string PreviousTitle;

        public enum GroupStyle
        {
            Normal,
            Error
        }

        GroupStyle m_style = GroupStyle.Normal;

        public DialogNodeGroup(string title, Vector2 position)
        {

            this.title = title;
            PreviousTitle = title;
            SetPosition(position.ToRect()) ;
            m_defaultBorderColor = contentContainer.style.borderBottomColor.value;
            m_defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void UpdateGroupColor(GroupStyle style, Color color)
        {
            m_style = style;
            switch (style)
            {
                case GroupStyle.Normal:
                    contentContainer.style.borderTopColor = m_defaultBorderColor;
                    contentContainer.style.borderBottomWidth = m_defaultBorderWidth;
                    break;
                case GroupStyle.Error:
                    contentContainer.style.borderTopColor = color;
                    contentContainer.style.borderBottomWidth = m_defaultBorderWidth;
                    break;
            }
        }
    }
}

