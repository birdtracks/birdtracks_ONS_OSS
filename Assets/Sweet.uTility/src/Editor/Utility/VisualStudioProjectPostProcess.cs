using UnityEditor;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace SweetEditor.Utility
{
	public class VisualStudioProjectPostProcess : AssetPostprocessor
	{
		private const string _EDITOR_PREFS_KEY = "remap_solution_project_hints";
		private const string _REMAP_SOLUTION_PROJECT_HINTS = "Framework/Remap Solution Project Hints";




		public static bool RemapSolutionProjectHints
		{
			get { return EditorPrefs.GetBool(_EDITOR_PREFS_KEY, false); }
			set { EditorPrefs.SetBool(_EDITOR_PREFS_KEY, value);}
		}




		[MenuItem(_REMAP_SOLUTION_PROJECT_HINTS)]
		public static void ToggleSimulationMode()
		{
			RemapSolutionProjectHints = !RemapSolutionProjectHints;
		}

		[MenuItem(_REMAP_SOLUTION_PROJECT_HINTS, true)]
		public static bool ToggleSimulationModeValidate()
		{
			Menu.SetChecked(_REMAP_SOLUTION_PROJECT_HINTS, RemapSolutionProjectHints);
			AssetDatabase.Refresh();
			return true;
		}




		public static void OnGeneratedCSProjectFiles()
		{
			if (!RemapSolutionProjectHints)
			{
				return;
			}

			GenerateSoultion();
		}

		
		private static void GenerateSoultion()
		{
			string currentDir = Directory.GetCurrentDirectory();
			string[] projects =
				Directory.GetFiles(currentDir, "*.csproj").ToArray();

			foreach (var file in projects)
			{
				ProcessProject(file);
			}
		}


		private static void ProcessProject(string file)
		{
			XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
			XDocument csproj = XDocument.Load(file);
			var hints = csproj.Descendants(ns + "HintPath");

			foreach (var hint in hints)
			{
				string newHint = hint.Value.Insert(0, "Z:");
				newHint = newHint.Replace('/', '\\');
				hint.Value = newHint;
			}

			csproj.Save(file);
		}
	}
}
