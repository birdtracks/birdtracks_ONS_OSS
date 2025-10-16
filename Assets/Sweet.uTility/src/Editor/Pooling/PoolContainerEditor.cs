using System.Linq;
using System.Reflection;
using SweetEngine.Pooling;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace SweetEditor.Pooling
{
	[CustomEditor(typeof (PoolContainer))]
	public class PoolManagerEditor : Editor
	{
		private SerializedProperty _poolInfos;
		private ReorderableList _reorderableList;
		private bool _needsRebuild;




		private void OnEnable()
		{
			_poolInfos = serializedObject.FindProperty("m_PoolInfos");

			_reorderableList = new ReorderableList(serializedObject, _poolInfos, true, true, true, true);
			_reorderableList.elementHeight = 20f;
			_reorderableList.onAddCallback += OnAddElementHandler;
			_reorderableList.onRemoveCallback += OnRemoveElementHandler;
			_reorderableList.drawHeaderCallback += DrawHeaderHandler;
			_reorderableList.drawElementCallback += DrawElementHandler;
		}


		private void OnRemoveElementHandler(ReorderableList list)
		{
			_poolInfos.DeleteArrayElementAtIndex(list.index);
		}


		private void DrawHeaderHandler(Rect rect)
		{

			switch (Event.current.type)
			{
				case EventType.DragUpdated:
					if (rect.Contains(Event.current.mousePosition))
					{
						if (DragAndDrop.objectReferences.All(o => o is GameObject))
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Link;
						}
						else
						{
							DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
						}
					}
					break;
				case EventType.DragPerform:

					if (rect.Contains(Event.current.mousePosition) &&
						DragAndDrop.objectReferences.All(o => o is GameObject))
					{
						for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
						{
							var objectReference = DragAndDrop.objectReferences[i];
							int index = _poolInfos.arraySize;
							_poolInfos.InsertArrayElementAtIndex(index);
							SerializedProperty poolInfo = _poolInfos.GetArrayElementAtIndex(index);
							poolInfo.FindPropertyRelative("Prefab").objectReferenceValue = objectReference;
							poolInfo.FindPropertyRelative("PrePoolCount").intValue = 1;
							_needsRebuild = true;
						}
					}
					break;
			}

			EditorGUI.LabelField(rect, "Pre Pool Items");
		}


		private void DrawElementHandler(Rect rect, int index, bool isactive, bool isfocused)
		{
			SerializedProperty p = _poolInfos.GetArrayElementAtIndex(index);

			rect.yMax -= 2f;
			rect.yMin += 2f;

			Rect countRect = rect;
			countRect.xMin = countRect.xMax - 70f;
			Rect objectRect = rect;
			objectRect.xMax = countRect.xMin - 8;

			EditorGUIUtility.labelWidth = 43f;
			EditorGUI.PropertyField(objectRect, p.FindPropertyRelative("Prefab"), new GUIContent("Prefab"));
			EditorGUI.PropertyField(countRect, p.FindPropertyRelative("PrePoolCount"), new GUIContent("Count"));
		}


		private void OnAddElementHandler(ReorderableList list)
		{
			int index = _poolInfos.arraySize;
			_poolInfos.InsertArrayElementAtIndex(index);
			SerializedProperty poolInfo = _poolInfos.GetArrayElementAtIndex(index);
			poolInfo.FindPropertyRelative("Prefab").objectReferenceValue = null;
			poolInfo.FindPropertyRelative("PrePoolCount").intValue = 1;
			_needsRebuild = true;
		}


		public override void OnInspectorGUI()
		{
			EditorGUILayout.Space();

			_needsRebuild = false;
			_reorderableList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
			{
				_needsRebuild = true;
			}

			if (_needsRebuild)
			{
				target.GetType().GetMethod("RebuildPool", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
			}
		}
	}
}
