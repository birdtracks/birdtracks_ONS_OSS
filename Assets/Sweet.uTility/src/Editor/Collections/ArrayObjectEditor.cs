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
	[CustomEditor(typeof(ArrayObject), true)]
	public class ArrayObjectEditor : Editor
	{
		private SerializedProperty _array;
		private Type _arrayElementType;
		private ReorderableList _reorderableList;
		private List<Object> _dragScratch;




		private void OnEnable()
		{
			_array = serializedObject.FindProperty("m_Array");
			_arrayElementType = Type.GetType(_array.type);
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

			_dragScratch.Clear();
			switch (Event.current.type)
			{
				case EventType.DragUpdated:
					if (rect.Contains(Event.current.mousePosition))
					{
						DragAndDrop.visualMode = ResolveDragReferences(_dragScratch, _arrayElementType)
							? DragAndDropVisualMode.Move
							: DragAndDropVisualMode.Rejected;
					}
					break;
				case EventType.DragPerform:

					if (rect.Contains(Event.current.mousePosition))
					{
						if (ResolveDragReferences(_dragScratch, _arrayElementType))
						{
							for (int i = 0; i < _dragScratch.Count; i++)
							{
								var objectReference = _dragScratch[i];
								int index = _array.arraySize;
								_array.InsertArrayElementAtIndex(index);
								_array.GetArrayElementAtIndex(index).objectReferenceValue = objectReference;
							}
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

			EditorGUIUtility.labelWidth = 75f;
			EditorGUI.PropertyField(rect, e, new GUIContent(string.Format("Element {0}", index)));
		}


		private void OnAddElementHandler(ReorderableList list)
		{
			int index = _array.arraySize;
			_array.InsertArrayElementAtIndex(index);

			SerializedProperty e = _array.GetArrayElementAtIndex(index);
			SerializedPropertyUtility.ClearSerializedPropertyValue(e);
		}


		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();
			_reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}


		private static bool ResolveDragReferences(List<Object> outList, Type validType)
		{
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
