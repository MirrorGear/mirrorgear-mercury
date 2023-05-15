using UnityEditor;
using UnityEditor.Compilation;

namespace MirrorGear.Mercury
{
	public class MercuryEditor : EditorWindow
	{
		[InitializeOnEnterPlayMode]
		public static void InitializeOnLoadMethod()
		{
			EditorApplication.delayCall += OnDelayCall;
		}
		private static void OnDelayCall()
		{
			CompilationPipeline.assemblyCompilationStarted -= OnCompileStarted;
			CompilationPipeline.assemblyCompilationStarted += OnCompileStarted;

			CompilationPipeline.assemblyCompilationFinished -= CompilationPipelineOnassemblyCompilationFinished; ;
			CompilationPipeline.assemblyCompilationFinished += CompilationPipelineOnassemblyCompilationFinished;
		}
		private static void CompilationPipelineOnassemblyCompilationFinished(string arg1, CompilerMessage[] arg2)
		{
			//EditorApplication.EnterPlaymode();
		}

		private static void OnCompileStarted(string obj)
		{
			EditorApplication.ExitPlaymode();
		}
	}
}


