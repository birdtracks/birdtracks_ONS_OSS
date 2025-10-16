using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;


namespace SweetEditor.Utility
{
	public static class AssetDatabaseUtility
	{
		private const char _FOLDER_SEPERATOR = '/';
		private const char _EXTENSION_SEPERATOR = '.';




		public static string GetAssetFolderPath(Object assetObject)
		{
			Assert.IsNotNull(assetObject);

			return GetAssetFolderPath(assetObject.GetInstanceID());
		}


		public static string GetAssetFolderPath(int instanceId)
		{
			string path = AssetDatabase.GetAssetPath(instanceId);
			int endFolderSeperator = path.LastIndexOf(_FOLDER_SEPERATOR);

			if (endFolderSeperator != -1)
			{
				path = path.Substring(0, endFolderSeperator + 1);
			}

			return path;
		}


		public static string GetAssetName(Object assetObject)
		{
			Assert.IsNotNull(assetObject);

			return GetAssetName(assetObject.GetInstanceID());
		}


		public static string GetAssetName(int instanceId)
		{
			string path = AssetDatabase.GetAssetPath(instanceId);
			int endFolderSeperator = path.LastIndexOf(_FOLDER_SEPERATOR) + 1;
			int endExtension = path.LastIndexOf(_EXTENSION_SEPERATOR);

			if (endFolderSeperator != -1)
			{
				path = path.Substring(endFolderSeperator, endExtension - endFolderSeperator);
			}

			return path;
		}
	}
}
