using System.Linq;
using UnityEditor;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

namespace UniSplineForge.Scripts
{
	[CustomEditor(typeof(SplineWall))]
	public class SplineWallEditor : Editor
	{
		void OnEnable()
		{
            Spline.Changed += OnSplineChanged;
            EditorSplineUtility.AfterSplineWasModified += OnSplineModified;
            SplineContainer.SplineAdded += OnContainerSplineSetModified;
            SplineContainer.SplineRemoved += OnContainerSplineSetModified;
		}

		void OnDisable()
		{
            Spline.Changed -= OnSplineChanged;
            EditorSplineUtility.AfterSplineWasModified -= OnSplineModified;
            SplineContainer.SplineAdded -= OnContainerSplineSetModified;
            SplineContainer.SplineRemoved -= OnContainerSplineSetModified;
		}
		
		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			if (EditorGUI.EndChangeCheck())
			{
				Debug.Log($"changed!");
				if (target is SplineWall)
				{
					(target as SplineWall)?.Rebuild();
				}
			}
		}

		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modificationType)
		{
			OnSplineModified();
		}
		
		private void OnSplineModified(Spline spline)
		{
			OnSplineModified();
		}
		
		private void OnContainerSplineSetModified(SplineContainer container, int spline)
		{
			OnSplineModified();
		}

		// ReSharper disable Unity.PerformanceAnalysis
		private void OnSplineModified()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// プレイモード中なら何もしない
				return;
			}
			
			// 本来は対象のSplineが編集されたときだけメッシュを再計算する方がいい
			if (target is SplineWall)
			{
				(target as SplineWall)?.Rebuild();
			}
		}
	}
}