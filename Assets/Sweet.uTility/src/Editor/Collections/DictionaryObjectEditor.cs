using System;
using System.Collections.Generic;
using SweetEditor.Utility;
using SweetEngine.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SweetEditor.Collections
{
	[CustomEditor(typeof(DictionaryObject), true)]
	public class DictionaryObjectEditor : Editor
	{
		private SerializedProperty _array;
		private Type _keyType;
		private Type _valueType;
		private ReorderableList _reorderableList;
		private List<Object> _dragScratch;




		private void OnEnable()
		{
			_array = serializedObject.FindProperty("m_Elements");

			Type pairType = target.GetType().BaseType.GetGenericArguments()[2];
			_keyType = pairType.BaseType.GetGenericArguments()[0];
			_valueType = pairType.BaseType.GetGenericArguments()[1];

			_dragScratch = new List<Object>();

			_reorderableList = new ReorderableList(serializedObject, _array, true, true, true, true);
			_reorderableList.elementHeight = 20f;
			_reorderableList.onAddCallback += OnAddElementHandler;
			_reorderableList.onRemoveCallback += OnRemoveElementHandler;
			_reorderableList.drawHeaderCallback += DrawHeaderHandler;
			_reorderableList.drawElementCallback += DrawElementHandler;
		}


		private void OnRemoveElementHandler(ReorderableList list)
		{
			_array.DeleteArrayElementAtIndex(list.index);
		}


		private void DrawHeaderHandler(Rect rect)
		{
			EditorGUI.LabelField(rect, "Elements");
			
			switch (Event.current.type)
			{
				case EventType.DragUpdated:
					if (rect.Contains(Event.current.mousePosition))
					{
						if (ResolveDragReferences(_dragScratch, _keyType) ||
						    ResolveDragReferences(_dragScratch, _valueType))
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Move;
						}
						else
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						}
					}
					break;
				case EventType.DragPerform:
					if (rect.Contains(Event.current.mousePosition))
					{
						string property;

						if (ResolveDragReferences(_dragScratch, _keyType))
						{
							property = "Key";
						}
						else if (ResolveDragReferences(_dragScratch, _valueType))
						{
							property = "Value";
						}
						else
						{
							break;
						}

						for (int i = 0; i < _dragScratch.Count; i++)
						{
							var objectReference = _dragScratch[i];
							int index = _array.arraySize;
							_array.InsertArrayElementAtIndex(index);
							_array.GetArrayElementAtIndex(index).FindPropertyRelative(property).objectReferenceValue = objectReference;
						}
					}
					break;
			}
		}


		private void DrawElementHandler(Rect rect, int index, bool isactive, bool isfocused)
		{
			SerializedProperty e = _array.GetArrayElementAtIndex(index);

			rect.yMax -= 2f;
			rect.yMin += 2f;

			Rect keyRect = rect;
			keyRect.xMax = keyRect.xMin + keyRect.width*0.5f - 3f;
			Rect valueRect = rect;
			valueRect.xMin = valueRect.xMax - valueRect.width * 0.5f + 3f;

			EditorGUIUtility.labelWidth = 27f;
			EditorGUI.PropertyField(keyRect, e.FindPropertyRelative("Key"), new GUIContent("Key"));
			EditorGUIUtility.labelWidth = 38f;
			EditorGUI.PropertyField(valueRect, e.FindPropertyRelative("Value"), new GUIContent("Value"));
		}


		private void OnAddElementHandler(ReorderableList list)
		{
			int index = _array.arraySize;
			_array.InsertArrayElementAtIndex(index);

			SerializedProperty e = _array.GetArrayElementAtIndex(index);
			SerializedPropertyUtility.ClearSerializedPropertyValue(e.FindPropertyRelative("Key"));
			SerializedPropertyUtility.ClearSerializedPropertyValue(e.FindPropertyRelative("Value"));
		}


		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();
			_reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}


		private static bool ResolveDragReferences(List<Object> outList, Type validType)
		{
			outList.Clear();
			bool allDragsValid = true;

			if (!typeof(Object).IsAssignableFrom(validType))
			{
				allDragsValid = false;
			}
			else
			{
				for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
				{
					Object objectReference = DragAndDrop.objectReferences[i];

					if (!validType.IsInstanceOfType(objectReference) &&
						(objectReference is GameObject &&
							(!typeof(Component).IsAssignableFrom(validType) ||
							 ((GameObject)objectReference).GetComponent(validType) == null)))
					{
						allDragsValid = false;
						continue;
					}

					outList.Add(objectReference);
				}
			}

			return allDragsValid;
		}
	}
}
