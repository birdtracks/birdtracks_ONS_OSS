using UnityEditor;


namespace SweetEditor.Utility
{
	public static class SerializedPropertyUtility
	{
		public static void ClearSerializedPropertyValue(SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.ObjectReference:
					property.objectReferenceValue = null;
					break;
				case SerializedPropertyType.String:
					property.stringValue = string.Empty;
					break;
			}
		}
	}
}
