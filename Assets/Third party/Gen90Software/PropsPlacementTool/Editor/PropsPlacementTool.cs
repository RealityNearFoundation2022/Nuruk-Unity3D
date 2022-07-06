using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Gen90Software.Tools
{
	public class PropsPlacementTool : EditorWindow
	{
		public enum PatternType
		{
			Line,
			Circle,
			Curve,
			Grid,
			Point
		}

		public enum CountmentType
		{
			Count,
			Delta
		}

		public enum DrawType
		{
			Raycast,
			Position,
			Rotation
		}

		private enum DrawStatus
		{
			None,
			DrawFirst,
			DrawSecond,
			DrawThird,
			DrawForth,
			Ready
		}

		private struct PropsData
		{
			public Vector3 pos;
			public Vector3 fwd;
			public Vector3 uwd;
			public Vector3 scl;
			public bool active;
			public int propsIndex;
			public GameObject go;

			public void ApplyOffset (Vector3 oP, Vector3 oR, Vector3 oS)
			{
				Vector3 lwd = Vector3.Cross(fwd, uwd);

				pos +=
					lwd * oP.x +
					uwd * oP.y +
					fwd * oP.z;

				Quaternion rotator = Quaternion.AngleAxis(oR.x, lwd) * Quaternion.AngleAxis(oR.y, uwd) * Quaternion.AngleAxis(oR.z, fwd);
				fwd = rotator * fwd;
				uwd = rotator * uwd;

				scl += oS;
			}
		}

		private const float gizmoThickness = 2.0f;
		private const float gizmoScaleMin = 0.01f;
		private const float gizmoScaleMax = 0.05f;
		private const int normalizeResolution = 20;
		private const string version = "1.1.0 beta";


		private PatternType pattern = PatternType.Line;
		private DrawType drawType = DrawType.Raycast;
		private Transform propsParent;
		private int propsCollectionSize = 1;
		private GameObject[] propsCollection;
		private CountmentType countment = CountmentType.Count;
		private int countX = 5;
		private int countY = 5;
		private float deltaX = 10.0f;
		private float deltaY = 10.0f;
		private bool normalizeCurvePosition = true;
		private bool pointingCurveRotation = false;
		private bool useFirst = true;
		private bool useMiddle = true;
		private bool useLast = true;
		private float fillRate = 1.0f;
		private int seed = 123;
		private bool useSurface = false;
		private LayerMask surfaceMask = 0;
		private float surfaceDistance = 10.0f;
		private bool surfaceOverridePos = true;
		private bool surfaceOverrideRot = true;
		private LayerMask surfaceOverrideActivity = 0;
		private Vector3 offsetPos = Vector3.zero;
		private Vector3 offsetRot = Vector3.zero;
		private Vector3 offsetScl = Vector3.zero;
		private Vector3 jitterPos = Vector3.zero;
		private Vector3 jitterRot = Vector3.zero;
		private Vector3 jitterScl = Vector3.zero;
		private bool uniformOffsetScl = true;
		private bool uniformJitterScl = true;
		private LayerMask drawMask = 0;
		private float drawDistance = 1000.0f;
		private float gizmoScale = 0.01f;
		private bool usePreview;


		[SerializeField]
		private Vector3 point1;
		[SerializeField]
		private Vector3 point2;
		[SerializeField]
		private Vector3 point3;
		[SerializeField]
		private Vector3 point4;
		[SerializeField]
		private Vector3 normal1;
		[SerializeField]
		private Vector3 normal2;
		[SerializeField]
		private Vector3 normal3;
		[SerializeField]
		private Vector3 normal4;


		private Vector3 pointX;
		private Vector3 normalX;
		private float[] controlRatesX;
		private float[] controlRatesY;
		private PropsData[] controlProps;
		private float[] normalizeLUT;


		private DrawStatus status;
		private bool initControl;
		private bool reposition1;
		private bool reposition2;
		private bool reposition3;
		private bool reposition4;
		private Quaternion handleRotation1;
		private Quaternion handleRotation2;
		private Quaternion handleRotation3;
		private Quaternion handleRotation4;


		private Vector2 viewScroll;
		private bool foldoutSurface;
		private bool foldoutOffset;
		private bool foldoutJitter;
		private bool foldoutSettings;


		#region INITIALIZATION
		[MenuItem("Tools/Gen90Software/Props Placement Tool")]
		public static void InitPropsPlacementToolWindow ()
		{
			PropsPlacementTool ppt = GetWindow<PropsPlacementTool>();
			ppt.titleContent.text = "Props Placement Tool";
			ppt.Show();

			ppt.viewScroll = Vector2.zero;
			ppt.foldoutOffset = false;
			ppt.foldoutJitter = false;
			ppt.foldoutSettings = false;

			ppt.surfaceMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(~Physics.IgnoreRaycastLayer);
			ppt.drawMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(~Physics.IgnoreRaycastLayer);
		}

		public void OnEnable ()
		{
			SceneView.duringSceneGui += OnSceneGUI;
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		public void OnDisable ()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			Undo.undoRedoPerformed -= OnUndoRedo;

			DeletePreviewPropsObjects();
		}

		public void OnUndoRedo ()
		{
			CalculateControlRates();
			CalculateControlProps();
			Repaint();
			SceneView.RepaintAll();
		}
		#endregion

		#region MAIN
		public void OnGUI ()
		{
			//DRAW AREA
			EditorGUILayout.Space(10);
			if (pattern != PatternType.Point)
			{
				switch (status)
				{
					case DrawStatus.None:
						if (GUILayout.Button("Draw " + pattern.ToString(), new GUIStyle("Button") { font = EditorStyles.boldFont, fontSize = 14 }, GUILayout.MinHeight(30)))
						{
							status = DrawStatus.DrawFirst;
							drawType = DrawType.Raycast;
						}
						break;

					case DrawStatus.DrawFirst:
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.HelpBox("Select the first point!", MessageType.None);
						if (GUILayout.Button("Cancel", GUILayout.Width(60)))
						{
							status = initControl ? DrawStatus.Ready : DrawStatus.None;
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space(10);
						break;

					case DrawStatus.DrawSecond:
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.HelpBox("Select the second point!", MessageType.None);
						if (GUILayout.Button("Cancel", GUILayout.Width(60)))
						{
							status = initControl ? DrawStatus.Ready : DrawStatus.None;
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space(10);
						break;

					case DrawStatus.DrawThird:
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.HelpBox("Select the third point!", MessageType.None);
						if (GUILayout.Button("Cancel", GUILayout.Width(60)))
						{
							status = initControl ? DrawStatus.Ready : DrawStatus.None;
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space(10);
						break;

					case DrawStatus.DrawForth:
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.HelpBox("Select the forth point!", MessageType.None);
						if (GUILayout.Button("Cancel", GUILayout.Width(60)))
						{
							status = initControl ? DrawStatus.Ready : DrawStatus.None;
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space(10);
						break;

					case DrawStatus.Ready:
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Redraw " + pattern.ToString(), new GUIStyle("Button") { font = EditorStyles.boldFont, fontSize = 14 }, GUILayout.MinHeight(30)))
						{
							status = DrawStatus.DrawFirst;
							drawType = DrawType.Raycast;
						}
						if (GUILayout.Button("Clear " + pattern.ToString(), new GUIStyle("Button") { font = EditorStyles.boldFont, fontSize = 14 }, GUILayout.MinHeight(30), GUILayout.MaxWidth(120)))
						{
							status = DrawStatus.None;
							ClearControlPoints();
						}
						EditorGUILayout.EndHorizontal();
						break;
				}
			}
			else
			{
				if (!initControl)
				{
					if (status != DrawStatus.None)
					{
						status = DrawStatus.None;
					}
				}
				EditorGUILayout.HelpBox("Place objects with cursor!", MessageType.None);
				EditorGUILayout.Space(10);
			}

			//SCROLL VIEW START
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			viewScroll = EditorGUILayout.BeginScrollView(viewScroll);
			EditorGUI.BeginChangeCheck();

			//PATTERN AREA
			GUILayout.Label(new GUIContent("Pattern",
				"Set the pattern of placement.\n\n" +
				"Line:\tPlace objects along a line.\n" +
				"Circle:\tPlace objects along a circle line.\n" +
				"Curve:\tPlace objects along a curved line.\n" +
				"Grid:\tPlace objects along a grid.\n" +
				"Point:\tPlace objects individually."
				), EditorStyles.boldLabel);
			EditorGUI.BeginChangeCheck();
			pattern = (PatternType) GUILayout.Toolbar((int) pattern, System.Enum.GetNames(typeof(PatternType)));
			if (EditorGUI.EndChangeCheck())
			{
				DeletePreviewPropsObjects();
			}

			if (pattern != PatternType.Point)
			{
				//DRAW AREA
				EditorGUILayout.Space(10);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.Label(new GUIContent("Draw",
					"Set the type of control point placement.\n\n" +
					"Raycast:\tPlace or modify control points with raycast.\n" +
					"Position:\tModify position with a transform handle.\n" +
					"Rotation:\tModify rotation with a transform handle."
					), EditorStyles.boldLabel);
				EditorGUI.BeginChangeCheck();
				drawType = (DrawType) GUILayout.Toolbar((int) drawType, System.Enum.GetNames(typeof(DrawType)));
				if (EditorGUI.EndChangeCheck())
				{
					handleRotation1 = Quaternion.LookRotation(normal1);
					handleRotation2 = Quaternion.LookRotation(normal2);
					handleRotation3 = Quaternion.LookRotation(normal3);
					handleRotation4 = Quaternion.LookRotation(normal4);
				}
			}

			//OBJECTS AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.LabelField("Objects", EditorStyles.boldLabel);
			propsParent = EditorGUILayout.ObjectField("Props Parent", propsParent, typeof(Transform), true) as Transform;
			EditorGUI.BeginChangeCheck();
			propsCollectionSize = EditorGUILayout.IntField("Props Collection", propsCollectionSize);
			if (propsCollection == null)
			{
				propsCollection = new GameObject[1];
			}
			if (propsCollection != null && propsCollectionSize != propsCollection.Length && propsCollectionSize > 0)
			{
				System.Array.Resize(ref propsCollection, propsCollectionSize);
			}
			for (int i = 0; i < propsCollectionSize; i++)
			{
				propsCollection[i] = EditorGUILayout.ObjectField("        Object " + (i + 1).ToString("#0"), propsCollection[i], typeof(GameObject), false) as GameObject;
			}
			if (EditorGUI.EndChangeCheck())
			{
				DeletePreviewPropsObjects();
			}

			if (pattern != PatternType.Point)
			{
				//COUNTMENT AREA
				EditorGUILayout.Space(10);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.Label(new GUIContent("Countment",
					"Set the objects counting logic.\n\n" +
					"Count:\tPlace objects by count.\n" +
					"Delta:\tPlace objects by distance."
					), EditorStyles.boldLabel);
				countment = (CountmentType) GUILayout.Toolbar((int) countment, System.Enum.GetNames(typeof(CountmentType)));
				if (pattern == PatternType.Grid)
				{
					if (countment == CountmentType.Count)
					{
						countX = EditorGUILayout.IntField(new GUIContent("Count X", "Set the count of placement along first axis."), countX);
						countY = EditorGUILayout.IntField(new GUIContent("Count Y", "Set the count of placement along second axis."), countY);
					}
					else
					{
						deltaX = EditorGUILayout.FloatField(new GUIContent("Distance X", "Set the distance of placement along first axis."), deltaX);
						deltaY = EditorGUILayout.FloatField(new GUIContent("Distance Y", "Set the distance of placement along second axis."), deltaY);
					}
				}
				else
				{
					if (countment == CountmentType.Count)
					{
						countX = EditorGUILayout.IntField(new GUIContent("Count", "Set the count of placement."), countX);
						if (pattern == PatternType.Curve)
						{
							normalizeCurvePosition = EditorGUILayout.Toggle(new GUIContent("Normalize Positions", "Normalize the distance of curved placement."), normalizeCurvePosition);
						}
					}
					else
					{
						deltaX = EditorGUILayout.FloatField(new GUIContent("Distance", "Set the distance of placement."), deltaX);
					}

					if (pattern == PatternType.Circle || pattern == PatternType.Curve)
					{
						pointingCurveRotation = EditorGUILayout.Toggle(new GUIContent("Pointing Rotations", "Point the object forward toward the next object."), pointingCurveRotation);
					}
				}

				//FILLMENT AREA
				EditorGUILayout.Space(10);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.Label(new GUIContent("Fillment",
					"Set the fill of placement.\n\n" +
					"First:\tPlace object on the first position.\n" +
					"Middle:\tPlace object on middle positions.\n" +
					"Last:\tPlace object on the last position."
					), EditorStyles.boldLabel);
				EditorGUILayout.BeginHorizontal();
				useFirst = GUILayout.Toggle(useFirst, "First", new GUIStyle("Button"));
				useMiddle = GUILayout.Toggle(useMiddle, "Middle", new GUIStyle("Button"));
				useLast = GUILayout.Toggle(useLast, "Last", new GUIStyle("Button"));
				EditorGUILayout.EndHorizontal();
				fillRate = EditorGUILayout.Slider(new GUIContent("Fill Rate", "Set the rate of placement."), fillRate, 0.0f, 1.0f);
				EditorGUI.BeginChangeCheck();
				seed = EditorGUILayout.IntSlider(new GUIContent("Seed", "Set the seed of randomizations."), seed, 0, 999);
				if (EditorGUI.EndChangeCheck())
				{
					DeletePreviewPropsObjects();
				}

				//SURFACE AREA
				EditorGUILayout.Space(10);
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				foldoutSurface = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutSurface, "Surface");
				if (foldoutSurface)
				{
					useSurface = EditorGUILayout.Toggle(new GUIContent("        Place On Surface", "Raycast to surface and adjust the objects transform."), useSurface);
					surfaceMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUILayout.MaskField(new GUIContent("        Surface Mask", "Masking the surface adjust."), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(surfaceMask), InternalEditorUtility.layers));
					surfaceDistance = EditorGUILayout.FloatField(new GUIContent("        Surface Distance", "Limit the distance of surface adjust."), surfaceDistance);
					surfaceOverridePos = EditorGUILayout.Toggle(new GUIContent("        Adjust Position", "Adjust objects position to surface."), surfaceOverridePos);
					surfaceOverrideRot = EditorGUILayout.Toggle(new GUIContent("        Adjust Rotation", "Adjust objects rotation to surface."), surfaceOverrideRot);
					surfaceOverrideActivity = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUILayout.MaskField(new GUIContent("        Remove On Layer", "Remove objects on this layer."), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(surfaceOverrideActivity), InternalEditorUtility.layers));
				}
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			//OFFSET AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			foldoutOffset = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutOffset, "Offset");
			if (foldoutOffset)
			{
				offsetPos = EditorGUILayout.Vector3Field("        Position", offsetPos);
				offsetRot = EditorGUILayout.Vector3Field("        Rotation", offsetRot);
				if (uniformOffsetScl)
				{
					EditorGUILayout.LabelField("        Scale");
					offsetScl = new Vector3(EditorGUILayout.FloatField("        X | Y | Z", offsetScl.x), offsetScl.y, offsetScl.z);
				}
				else
				{
					offsetScl = EditorGUILayout.Vector3Field("        Scale", offsetScl);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if (GUILayout.Button("Reset", GUILayout.MaxWidth(200)))
				{
					offsetPos = Vector3.zero;
					offsetRot = Vector3.zero;
					offsetScl = Vector3.zero;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				uniformOffsetScl = GUILayout.Toggle(uniformOffsetScl, "Uniform Scale: " + (uniformOffsetScl ? "On" : "Off"), new GUIStyle("Button"), GUILayout.MaxWidth(200));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			//JITTER AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			foldoutJitter = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutJitter, "Jitter");
			if (foldoutJitter)
			{
				jitterPos = EditorGUILayout.Vector3Field("        Position", jitterPos);
				jitterRot = EditorGUILayout.Vector3Field("        Rotation", jitterRot);
				if (uniformJitterScl)
				{
					EditorGUILayout.LabelField("        Scale");
					jitterScl = new Vector3(EditorGUILayout.FloatField("        X | Y | Z", jitterScl.x), jitterScl.y, jitterScl.z);
				}
				else
				{
					jitterScl = EditorGUILayout.Vector3Field("        Scale", jitterScl);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if (GUILayout.Button("Reset", GUILayout.MaxWidth(200)))
				{
					jitterPos = Vector3.zero;
					jitterRot = Vector3.zero;
					jitterScl = Vector3.zero;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				uniformJitterScl = GUILayout.Toggle(uniformJitterScl, "Uniform Scale: " + (uniformJitterScl ? "On" : "Off"), new GUIStyle("Button"), GUILayout.MaxWidth(200));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			//SETTINGS AREA
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			foldoutSettings = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutSettings, "Settings");
			if (foldoutSettings)
			{
				drawMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(EditorGUILayout.MaskField(new GUIContent("        Draw Mask", "Masking the control point selection."), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(drawMask), InternalEditorUtility.layers));
				drawDistance = EditorGUILayout.FloatField(new GUIContent("        Draw Distance", "Limit the distance of control point selection."), drawDistance);
				gizmoScale = EditorGUILayout.Slider(new GUIContent("        Gizmo Scale", "Set scale of gizmos."), gizmoScale, gizmoScaleMin, gizmoScaleMax);
				EditorGUILayout.Space(20);
				EditorGUILayout.LabelField("        Version: " + version);
			}
			EditorGUILayout.EndFoldoutHeaderGroup();

			//END CHECK
			EditorGUILayout.Space(20);
			if (EditorGUI.EndChangeCheck())
			{
				propsCollectionSize = Mathf.Max(propsCollectionSize, 1);
				countX = Mathf.Max(countX, 2);
				countY = Mathf.Max(countY, 2);
				deltaX = Mathf.Max(deltaX, 0.1f);
				deltaY = Mathf.Max(deltaY, 0.1f);

				surfaceMask &= ~(1 << 2);
				surfaceOverrideActivity &= ~(1 << 2);

				drawMask &= ~(1 << 2);
				drawDistance = Mathf.Max(drawDistance, 1.0f);

				CalculateControlRates();
				CalculateControlProps();
				SceneView.RepaintAll();
			}
			EditorGUILayout.EndScrollView();

			//PLACE AREA
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUI.BeginChangeCheck();
			usePreview = GUILayout.Toggle(usePreview, "Preview: " + (usePreview ? "On" : "Off"), new GUIStyle("Button"));
			if (EditorGUI.EndChangeCheck())
			{
				PreviewProps();
			}
			EditorGUILayout.Space(5);
			if (pattern != PatternType.Point)
			{
				if (GUILayout.Button("Place", new GUIStyle("Button") { font = EditorStyles.boldFont, fontSize = 14 }, GUILayout.MinHeight(30)))
				{
					PlaceProps();
				}
			}
			else
			{
				EditorGUILayout.HelpBox("Place objects with cursor!", MessageType.None);
				EditorGUILayout.Space(12);
			}
			EditorGUILayout.Space(10);
		}

		public void OnSceneGUI (SceneView sceneView)
		{
			DrawGizmos(Camera.current.transform.position);

			if (status == DrawStatus.None && pattern != PatternType.Point)
				return;

			if (!initControl && status == DrawStatus.Ready)
			{
				if (normal3 == Vector3.zero)
				{
					point3 = point2 + Vector3.Cross(point2 - point1, Vector3.up);
					normal3 = normal2;
				}

				if (normal4 == Vector3.zero)
				{
					point4 = point1 + (point3 - point2);
					normal4 = normal1;
				}

				initControl = true;
			}

			if (Event.current.modifiers != EventModifiers.None)
				return;

			if (pattern != PatternType.Point)
			{
				if (drawType == DrawType.Position)
				{
					PositionWithHandle();
					return;
				}

				if (drawType == DrawType.Rotation)
				{
					RotationWithHandle();
					return;
				}
			}

			Vector3 screenPosition = Event.current.mousePosition;
			screenPosition.y = Camera.current.pixelHeight - screenPosition.y;

			if (!new Rect(0, 0, Camera.current.pixelWidth, Camera.current.pixelHeight).Contains(screenPosition))
				return;

			if (!Physics.Raycast(Camera.current.ScreenPointToRay(screenPosition), out RaycastHit hit, drawDistance, drawMask))
				return;

			if (pattern == PatternType.Point)
			{
				PlaceByCursor(ref hit);
				return;
			}

			if (status == DrawStatus.Ready)
			{
				RepositionControlPoints(ref hit);
				return;
			}

			DrawControlPoints(ref hit);
		}
		#endregion

		#region CONTROL_POINTS
		private void DrawControlPoints (ref RaycastHit hit)
		{
			Handles.color = new Color(1.0f, 0.75f, 0.0f, 0.5f);
			Handles.DrawSolidDisc(hit.point, hit.normal, hit.distance * gizmoScale);

			if (Event.current.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
			}

			if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
			{
				Undo.RecordObject(this, "Draw control point");
				switch (status)
				{
					case DrawStatus.DrawFirst:
						point1 = hit.point;
						normal1 = hit.normal;
						status = DrawStatus.DrawSecond;
						Repaint();
						break;

					case DrawStatus.DrawSecond:
						point2 = hit.point;
						normal2 = hit.normal;
						status = (int) pattern < 2 ? DrawStatus.Ready : DrawStatus.DrawThird;
						Repaint();
						break;

					case DrawStatus.DrawThird:
						point3 = hit.point;
						normal3 = hit.normal;
						status = (int) pattern < 3 || (pattern == PatternType.Grid && countment == CountmentType.Delta) ? DrawStatus.Ready : DrawStatus.DrawForth;
						Repaint();
						break;

					case DrawStatus.DrawForth:
						point4 = hit.point;
						normal4 = hit.normal;
						status = DrawStatus.Ready;
						Repaint();
						break;
				}
				Event.current.Use();

				CalculateControlRates();
				CalculateControlProps();
			}
		}

		private void RepositionControlPoints (ref RaycastHit hit)
		{
			float discRadius = hit.distance * gizmoScale;
			float discRadiusDouble = discRadius * 2.0f;

			if (reposition1 || Vector3.Distance(hit.point, point1) < discRadiusDouble)
			{
				RepositionControlPoint(ref hit, ref point1, ref normal1, ref discRadius, ref reposition1);
				return;
			}

			if (reposition2 || Vector3.Distance(hit.point, point2) < discRadiusDouble)
			{
				RepositionControlPoint(ref hit, ref point2, ref normal2, ref discRadius, ref reposition2);
				return;
			}

			if ((int) pattern < 2)
				return;

			if (reposition3 || Vector3.Distance(hit.point, point3) < discRadiusDouble)
			{
				RepositionControlPoint(ref hit, ref point3, ref normal3, ref discRadius, ref reposition3);
				return;
			}

			if ((int) pattern < 3 || (pattern == PatternType.Grid && countment == CountmentType.Delta))
				return;

			if (reposition4 || Vector3.Distance(hit.point, point4) < discRadiusDouble)
			{
				RepositionControlPoint(ref hit, ref point4, ref normal4, ref discRadius, ref reposition4);
				return;
			}
		}

		private void RepositionControlPoint (ref RaycastHit hit, ref Vector3 point, ref Vector3 normal, ref float radius, ref bool resposition)
		{
			Handles.color = new Color(1.0f, 0.75f, 0.0f, 0.5f);
			Handles.DrawSolidDisc(point, normal, radius);

			if (Event.current.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
			}

			if (Event.current.button == 0 && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
			{
				Undo.RecordObject(this, "Reposition control point");
				point = hit.point;
				normal = hit.normal;
				resposition = true;
				Event.current.Use();

				Repaint();
			}

			if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
			{
				resposition = false;
				Event.current.Use();

				CalculateControlRates();
				CalculateControlProps();
			}
		}

		private void PositionWithHandle ()
		{
			EditorGUI.BeginChangeCheck();

			Vector3 newPoint1 = point1;
			Vector3 newPoint2 = point2;
			Vector3 newPoint3 = point3;
			Vector3 newPoint4 = point4;

			newPoint1 = Handles.PositionHandle(newPoint1, Quaternion.identity);
			newPoint2 = Handles.PositionHandle(newPoint2, Quaternion.identity);
			if ((int) pattern > 1)
			{
				newPoint3 = Handles.PositionHandle(newPoint3, Quaternion.identity);
				if ((int) pattern > 2 && countment == CountmentType.Count)
				{
					newPoint4 = Handles.PositionHandle(newPoint4, Quaternion.identity);
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, "Reposition control point");
				point1 = newPoint1;
				point2 = newPoint2;
				point3 = newPoint3;
				point4 = newPoint4;

				CalculateControlRates();
				CalculateControlProps();
			}
		}

		private void RotationWithHandle ()
		{
			EditorGUI.BeginChangeCheck();

			handleRotation1 = Handles.RotationHandle(handleRotation1, point1);
			handleRotation2 = Handles.RotationHandle(handleRotation2, point2);
			if ((int) pattern > 1)
			{
				handleRotation3 = Handles.RotationHandle(handleRotation3, point3);
				if ((int) pattern > 2 && countment == CountmentType.Count)
				{
					handleRotation4 = Handles.RotationHandle(handleRotation4, point4);
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(this, "Reposition control point normal");
				normal1 = handleRotation1 * Vector3.forward;
				normal2 = handleRotation2 * Vector3.forward;
				normal3 = handleRotation3 * Vector3.forward;
				normal4 = handleRotation4 * Vector3.forward;

				CalculateControlRates();
				CalculateControlProps();
			}
		}

		private void ClearControlPoints ()
		{
			point1 = Vector3.zero;
			point2 = Vector3.zero;
			point3 = Vector3.zero;
			point4 = Vector3.zero;
			pointX = Vector3.zero;
			normal1 = Vector3.zero;
			normal2 = Vector3.zero;
			normal3 = Vector3.zero;
			normal4 = Vector3.zero;
			normalX = Vector3.zero;

			DeletePreviewPropsObjects();
			controlProps = null;

			initControl = false;
		}
		#endregion

		#region CREATE_DATA
		private void CalculateControlRates ()
		{
			if (pattern == PatternType.Point)
			{
				controlRatesX = new float[1];
				controlRatesY = new float[0];
				controlRatesX[0] = 0.0f;
				return;
			}

			CalculateControlRatesX();

			if (pattern != PatternType.Grid)
				return;

			CalculateControlRatesY();
		}

		private void CalculateControlRatesX ()
		{
			if (pattern == PatternType.Curve && (normalizeCurvePosition || countment == CountmentType.Delta))
			{
				CalculateNormalizeLUT();
			}

			if (countment == CountmentType.Delta)
			{
				countX = Mathf.FloorToInt(LengthOfAxisX() / deltaX) + 1;
			}

			int finalCount = (useFirst ? 1 : 0) + (useMiddle || pattern == PatternType.Grid ? countX - 2 : 0) + (useLast ? 1 : 0);
			int range = countX - 1;
			float distance = LengthOfAxisX();

			controlRatesX = new float[finalCount];
			for (int i = 0, x = 0; i <= range; i++)
			{
				if (!useFirst && i == 0)
					continue;

				if (!useMiddle && i > 0 && i < range && pattern != PatternType.Grid)
					continue;

				if (!useLast && i == range)
					continue;

				if (countment == CountmentType.Count)
				{
					controlRatesX[x] = i / (float) (pattern == PatternType.Circle ? range + 1 : range);
				}
				else
				{
					controlRatesX[x] = i * deltaX / distance;
				}

				if (pattern == PatternType.Curve && (normalizeCurvePosition || countment == CountmentType.Delta))
				{
					controlRatesX[x] = GetRateOnNormalizeLUT(controlRatesX[x] * distance);
				}

				x++;
			}
		}

		private void CalculateControlRatesY ()
		{
			if (countment == CountmentType.Delta)
			{
				countY = Mathf.FloorToInt(LengthOfAxisY() / deltaY) + 1;
			}

			int finalCount = (useFirst ? 1 : 0) + (useMiddle || pattern == PatternType.Grid ? countY - 2 : 0) + (useLast ? 1 : 0);
			int range = countY - 1;
			float distance = LengthOfAxisY();

			controlRatesY = new float[finalCount];
			for (int i = 0, y = 0; i <= range; i++)
			{
				if (!useFirst && i == 0)
					continue;

				if (!useMiddle && i > 0 && i < range && pattern != PatternType.Grid)
					continue;

				if (!useLast && i == range)
					continue;

				if (countment == CountmentType.Count)
				{
					controlRatesY[y] = i / (float) range;
				}
				else
				{
					controlRatesY[y] = i * deltaY / distance;
				}
				y++;
			}
		}

		private void CalculateControlProps ()
		{
			int allDataCount = controlRatesX.Length;
			if (pattern == PatternType.Grid)
			{
				allDataCount *= controlRatesY.Length;
			}

			if (controlProps == null)
			{
				controlProps = new PropsData[allDataCount];
			}

			if (controlProps.Length < allDataCount)
			{
				System.Array.Resize(ref controlProps, allDataCount);
			}

			if (controlProps.Length > allDataCount)
			{
				for (int i = controlProps.Length - 1; i >= allDataCount; i--)
				{
					if (controlProps[i].go == null)
						continue;

					DestroyImmediate(controlProps[i].go);
				}
				System.Array.Resize(ref controlProps, allDataCount);
			}

			switch (pattern)
			{
				case PatternType.Line:
					CalculateControlPropsLine();
					break;

				case PatternType.Circle:
					CalculateControlPropsCircle();
					break;

				case PatternType.Curve:
					CalculateControlPropsCurve();
					break;

				case PatternType.Grid:
					CalculateControlPropsGrid();
					break;

				case PatternType.Point:
					CalculateControlPropsPoint();
					break;
			}
			ApplySurfaceAdjust();
			ApplyOffset();
			ApplyRandoms();

			PreviewProps();
		}

		private void CalculateControlPropsLine ()
		{
			Vector3 dir = (point2-point1).normalized;

			for (int x = 0; x < controlRatesX.Length; x++)
			{
				controlProps[x].pos = Vector3.Lerp(point1, point2, controlRatesX[x]);
				controlProps[x].fwd = dir;
				controlProps[x].uwd = Vector3.Lerp(normal1, normal2, controlRatesX[x]);
				controlProps[x].scl = Vector3.one;
				controlProps[x].active = true;
			}
		}

		private void CalculateControlPropsCircle ()
		{
			Vector3 line12 = (point2-point1).normalized;
			Vector3 line12X = Vector3.Cross(normal1, line12);
			Vector3 circleNormal = Vector3.Cross(line12, line12X);
			float circleRadius = (point2 - point1).magnitude;

			for (int x = 0; x < controlRatesX.Length; x++)
			{
				float rad = controlRatesX[x] * 2.0f * Mathf.PI;
				float onCircleX = Mathf.Cos(rad) * circleRadius;
				float onCircleY = Mathf.Sin(rad) * circleRadius;
				Vector3 onCircleXY = line12 * onCircleX + line12X * onCircleY;

				controlProps[x].pos = point1 + onCircleXY;
				controlProps[x].fwd = Vector3.Cross(circleNormal, onCircleXY).normalized;
				controlProps[x].uwd = circleNormal;
				controlProps[x].scl = Vector3.one;
				controlProps[x].active = true;
			}

			if (!pointingCurveRotation)
				return;

			for (int x = 0; x < controlRatesX.Length; x++)
			{
				if (x == controlRatesX.Length - 1)
				{
					controlProps[x].fwd = (controlProps[0].pos - controlProps[x].pos).normalized;
					continue;
				}

				controlProps[x].fwd = (controlProps[x + 1].pos - controlProps[x].pos).normalized;
			}
		}

		private void CalculateControlPropsCurve ()
		{
			for (int x = 0; x < controlRatesX.Length; x++)
			{
				Vector3 pA = Vector3.Lerp(point1, point2, controlRatesX[x]);
				Vector3 pB = Vector3.Lerp(point2, point3, controlRatesX[x]);
				Vector3 nA = Vector3.Lerp(normal1, normal2, controlRatesX[x]);
				Vector3 nB = Vector3.Lerp(normal2, normal3, controlRatesX[x]);

				controlProps[x].pos = Vector3.Lerp(pA, pB, controlRatesX[x]);
				controlProps[x].fwd = (pB - pA).normalized;
				controlProps[x].uwd = Vector3.Lerp(nA, nB, controlRatesX[x]);
				controlProps[x].scl = Vector3.one;
				controlProps[x].active = true;
			}

			if (!pointingCurveRotation)
				return;

			for (int x = 0; x < controlRatesX.Length; x++)
			{
				if (x == controlRatesX.Length - 1)
					continue;

				controlProps[x].fwd = (controlProps[x + 1].pos - controlProps[x].pos).normalized;
			}
		}

		private void CalculateControlPropsGrid ()
		{
			Vector3 dir = (point2-point1).normalized;
			Vector3 v12 = point2-point1;
			Vector3 v23 = point3-point2;

			for (int x = 0; x < controlRatesX.Length; x++)
			{
				Vector3 pA = Vector3.Lerp(point1, point2, controlRatesX[x]);
				Vector3 pB = Vector3.Lerp(point4, point3, controlRatesX[x]);
				Vector3 nA = Vector3.Lerp(normal1, normal2, controlRatesX[x]);
				Vector3 nB = Vector3.Lerp(normal4, normal3, controlRatesX[x]);
				for (int y = 0; y < controlRatesY.Length; y++)
				{
					int xy = x * controlRatesY.Length + y;
					if (countment == CountmentType.Count)
					{
						controlProps[xy].pos = Vector3.Lerp(pA, pB, controlRatesY[y]);
					}
					else
					{
						controlProps[xy].pos = point1 + (controlRatesX[x] * v12.magnitude * v12.normalized) + (controlRatesY[y] * v23.magnitude * v23.normalized);
					}
					controlProps[xy].fwd = dir;
					controlProps[xy].uwd = Vector3.Lerp(nA, nB, controlRatesY[y]);
					controlProps[xy].scl = Vector3.one;
					controlProps[xy].active = useMiddle || (x == 0 || x == controlRatesX.Length - 1 || y == 0 || y == controlRatesY.Length - 1);
				}
			}
		}

		private void CalculateControlPropsPoint ()
		{
			Quaternion rotator = Quaternion.FromToRotation(Vector3.up, normalX);

			controlProps[0].pos = pointX;
			controlProps[0].fwd = rotator * Vector3.forward;
			controlProps[0].uwd = normalX;
			controlProps[0].scl = Vector3.one;
			controlProps[0].active = true;
		}
		#endregion

		#region MODIFY_DATA
		private void ApplySurfaceAdjust ()
		{
			if (!useSurface)
				return;

			for (int i = 0; i < controlProps.Length; i++)
			{
				if (!Physics.Raycast(controlProps[i].pos + controlProps[i].uwd * (surfaceDistance * 0.5f), -controlProps[i].uwd, out RaycastHit hit, surfaceDistance, surfaceMask))
					continue;

				if (surfaceOverridePos)
				{
					controlProps[i].pos = hit.point;
				}

				if (surfaceOverrideRot)
				{
					Quaternion rotator = Quaternion.FromToRotation(controlProps[i].uwd, hit.normal);
					controlProps[i].fwd = rotator * controlProps[i].fwd;
					controlProps[i].uwd = hit.normal;
				}

				if (surfaceOverrideActivity != 0)
				{
					controlProps[i].active &= !IsContainsLayer(surfaceOverrideActivity, hit.collider.gameObject.layer);
				}
			}
		}

		private void ApplyOffset ()
		{
			for (int i = 0; i < controlProps.Length; i++)
			{
				controlProps[i].ApplyOffset(
					offsetPos,
					offsetRot,
					uniformOffsetScl ?
						offsetScl.x * Vector3.one :
						offsetScl
				);
			}
		}

		private void ApplyRandoms ()
		{
			Random.State lastState = Random.state;
			Random.InitState(GetRandomSeed());
			for (int i = 0; i < controlProps.Length; i++)
			{
				controlProps[i].ApplyOffset(
					new Vector3(Random.Range(-jitterPos.x, jitterPos.x), Random.Range(-jitterPos.y, jitterPos.y), Random.Range(-jitterPos.z, jitterPos.z)),
					new Vector3(Random.Range(-jitterRot.x, jitterRot.x), Random.Range(-jitterRot.y, jitterRot.y), Random.Range(-jitterRot.z, jitterRot.z)),
					uniformJitterScl ?
						Random.Range(-jitterScl.x, jitterScl.x) * Vector3.one :
						new Vector3(Random.Range(-jitterScl.x, jitterScl.x), Random.Range(-jitterScl.y, jitterScl.y), Random.Range(-jitterScl.z, jitterScl.z))
				);

				controlProps[i].active &= Random.value < GetFillRate();
				controlProps[i].propsIndex = Random.Range(0, propsCollectionSize);
			}
			Random.state = lastState;
		}
		#endregion

		#region PREVIEW
		private void PreviewProps ()
		{
			for (int i = 0; i < controlProps.Length; i++)
			{
				if (!usePreview || status != DrawStatus.Ready)
				{
					if (controlProps[i].go)
					{
						controlProps[i].go.SetActive(false);
					}
					continue;
				}

				if (propsCollection[controlProps[i].propsIndex] == null)
					continue;

				GameObject newObject;

				if (controlProps[i].go == null)
				{
					newObject = Instantiate(propsCollection[controlProps[i].propsIndex]);
					newObject.hideFlags = HideFlags.HideAndDontSave;
				}
				else
				{
					newObject = controlProps[i].go;
				}

				if (newObject == null)
					continue;

				newObject.transform.localPosition = controlProps[i].pos;
				newObject.transform.localRotation = Quaternion.LookRotation(controlProps[i].fwd, controlProps[i].uwd);
				newObject.transform.localScale = controlProps[i].scl;
				foreach (Transform t in newObject.GetComponentsInChildren<Transform>(false))
				{
					t.gameObject.layer = 2;//Ignore raycast layer
				}
				newObject.SetActive(controlProps[i].active);

				controlProps[i].go = newObject;
			}
		}

		private void DeletePreviewPropsObjects ()
		{
			if (controlProps == null)
				return;

			for (int i = 0; i < controlProps.Length; i++)
			{
				if (controlProps[i].go == null)
					continue;

				DestroyImmediate(controlProps[i].go);
			}
		}
		#endregion

		#region PLACEMENT
		private void PlaceProps ()
		{
			for (int i = 0; i < controlProps.Length; i++)
			{
				if (!controlProps[i].active || propsCollection[controlProps[i].propsIndex] == null)
					continue;

				GameObject newObject;
				PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType (propsCollection[controlProps[i].propsIndex]);

				if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
				{
					newObject = (GameObject) PrefabUtility.InstantiatePrefab(propsCollection[controlProps[i].propsIndex]);
				}
				else
				{
					newObject = Instantiate(propsCollection[controlProps[i].propsIndex]);
				}

				if (newObject == null)
				{
					Debug.LogError("Error instantiating prefab");
					return;
				}

				Undo.RegisterCreatedObjectUndo(newObject, "Place Props");
				newObject.transform.localPosition = controlProps[i].pos;
				newObject.transform.localRotation = Quaternion.LookRotation(controlProps[i].fwd, controlProps[i].uwd);
				newObject.transform.localScale = controlProps[i].scl;
				newObject.transform.parent = propsParent;
			}
		}

		private void PlaceByCursor (ref RaycastHit hit)
		{
			pointX = hit.point;
			normalX = hit.normal;

			DeletePreviewPropsObjects();
			CalculateControlRates();
			CalculateControlProps();

			if (Event.current.type == EventType.Layout)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
			}

			if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
			{
				Event.current.Use();
				PlaceProps();
			}
		}
		#endregion

		#region GIZMOS
		private void DrawGizmos (Vector3 viewPos)
		{
			if (status == DrawStatus.None && pattern != PatternType.Point)
				return;

			switch (pattern)
			{
				case PatternType.Line:
					DrawGizmosLine(Mathf.Min(Vector3.Distance(viewPos, point1), Vector3.Distance(viewPos, point2)) * gizmoScale);
					break;

				case PatternType.Circle:
					DrawGizmosCircle(Mathf.Min(Vector3.Distance(viewPos, point1), Vector3.Distance(viewPos, point2)) * gizmoScale);
					break;

				case PatternType.Curve:
					DrawGizmosCurve(Mathf.Min(Vector3.Distance(viewPos, point1), Vector3.Distance(viewPos, point2), Vector3.Distance(viewPos, point3)) * gizmoScale);
					break;

				case PatternType.Grid:
					DrawGizmosGrid(Mathf.Min(Vector3.Distance(viewPos, point1), Vector3.Distance(viewPos, point2), Vector3.Distance(viewPos, point3), Vector3.Distance(viewPos, point4)) * gizmoScale);
					break;

				case PatternType.Point:
					DrawGizmosPropsPreview(Vector3.Distance(viewPos, pointX) * gizmoScale);
					break;
			}
		}

		private void DrawGizmosLine (float gizmoRadius)
		{
			Vector3 line12 = (point2-point1).normalized;

			Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
			Handles.DrawWireDisc(point1, normal1, gizmoRadius, gizmoThickness);
			if (normal2 == Vector3.zero)
				return;
			Handles.DrawWireDisc(point2, normal2, gizmoRadius, gizmoThickness);

			Handles.DrawLine(point1 + line12 * gizmoRadius, point2 - line12 * gizmoRadius, gizmoThickness);

			DrawGizmosPropsPreview(gizmoRadius);
		}

		private void DrawGizmosCircle (float gizmoRadius)
		{
			Vector3 line12 = (point2-point1).normalized;

			Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
			Handles.DrawWireDisc(point1, normal1, gizmoRadius, gizmoThickness);
			if (normal2 == Vector3.zero)
				return;
			Handles.DrawWireDisc(point2, normal2, gizmoRadius, gizmoThickness);

			Handles.DrawWireDisc(point1, Vector3.Cross(line12, Vector3.Cross(line12, normal1)), (point2 - point1).magnitude, gizmoThickness);
			Handles.DrawLine(point1 + line12 * gizmoRadius, point2 - line12 * gizmoRadius, gizmoThickness);

			DrawGizmosPropsPreview(gizmoRadius);
		}

		private void DrawGizmosCurve (float gizmoRadius)
		{
			Vector3 line12 = (point2-point1).normalized;
			Vector3 line23 = (point3-point2).normalized;

			Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
			Handles.DrawWireDisc(point1, normal1, gizmoRadius, gizmoThickness);
			if (normal2 == Vector3.zero)
				return;
			Handles.DrawWireDisc(point2, normal2, gizmoRadius, gizmoThickness);
			if (normal3 == Vector3.zero)
				return;
			Handles.DrawWireDisc(point3, normal3, gizmoRadius, gizmoThickness);

			Handles.DrawLine(point1 + line12 * gizmoRadius, point2 - line12 * gizmoRadius, gizmoThickness);
			Handles.DrawLine(point2 + line23 * gizmoRadius, point3 - line23 * gizmoRadius, gizmoThickness);

			DrawGizmosPropsPreview(gizmoRadius);
		}

		private void DrawGizmosGrid (float gizmoRadius)
		{
			Vector3 line12 = (point2-point1).normalized;
			Vector3 line23 = (point3-point2).normalized;
			Vector3 line34 = (point4-point3).normalized;
			Vector3 line41 = (point1-point4).normalized;

			Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.5f);
			Handles.DrawWireDisc(point1, normal1, gizmoRadius, gizmoThickness);
			if (normal2 == Vector3.zero)
				return;
			Handles.DrawWireDisc(point2, normal2, gizmoRadius, gizmoThickness);
			if (normal3 == Vector3.zero)
				return;
			Handles.DrawWireDisc(point3, normal3, gizmoRadius, gizmoThickness);
			if (normal4 == Vector3.zero)
				return;
			if (countment == CountmentType.Count)
			{
				Handles.DrawWireDisc(point4, normal4, gizmoRadius, gizmoThickness);
			}

			Handles.DrawLine(point1 + line12 * gizmoRadius, point2 - line12 * gizmoRadius, gizmoThickness);
			Handles.DrawLine(point2 + line23 * gizmoRadius, point3 - line23 * gizmoRadius, gizmoThickness);
			if (countment == CountmentType.Count)
			{
				Handles.DrawLine(point3 + line34 * gizmoRadius, point4 - line34 * gizmoRadius, gizmoThickness);
				Handles.DrawLine(point4 + line41 * gizmoRadius, point1 - line41 * gizmoRadius, gizmoThickness);
			}

			DrawGizmosPropsPreview(gizmoRadius);
		}

		private void DrawGizmosPropsPreview (float gizmoRadius)
		{
			if (controlProps == null || usePreview)
				return;

			Handles.color = new Color(0.5f, 0.25f, 0.0f, 0.5f);
			for (int i = 0; i < controlProps.Length; i++)
			{
				if (!controlProps[i].active)
					continue;

				Handles.DrawSolidDisc(controlProps[i].pos, controlProps[i].uwd, gizmoRadius * controlProps[i].scl.x);
			}
		}
		#endregion

		#region UTILITIES
		private int GetRandomSeed ()
		{
			if (pattern == PatternType.Point)
				return seed + Mathf.RoundToInt(pointX.sqrMagnitude);

			return seed;
		}

		private float GetFillRate ()
		{
			if (pattern == PatternType.Point)
				return 1.0f;

			return fillRate;
		}

		private float LengthOfAxisX ()
		{
			switch (pattern)
			{
				case PatternType.Line:
					return Vector3.Distance(point1, point2);

				case PatternType.Circle:
					return 2.0f * Vector3.Distance(point1, point2) * Mathf.PI;

				case PatternType.Curve:
					if (normalizeCurvePosition || countment == CountmentType.Delta)
					{
						return normalizeLUT[normalizeResolution - 1];
					}
					return Vector3.Distance(point1, point2) + Vector3.Distance(point2, point3);

				case PatternType.Grid:
					return Vector3.Distance(point1, point2);
			}

			return 0.0f;
		}

		private float LengthOfAxisY ()
		{
			if (pattern == PatternType.Grid)
				return Vector3.Distance(point2, point3);

			return 0.0f;
		}

		private Vector3 GetPositionOnCurve (float rate)
		{
			Vector3 pA = Vector3.Lerp(point1, point2, rate);
			Vector3 pB = Vector3.Lerp(point2, point3, rate);
			return Vector3.Lerp(pA, pB, rate);
		}

		private void CalculateNormalizeLUT ()
		{
			normalizeLUT = new float[normalizeResolution];
			for (int i = 0; i < normalizeLUT.Length; i++)
			{
				if (i == 0)
				{
					normalizeLUT[i] = 0.0f;
					continue;
				}

				Vector3 pLast = GetPositionOnCurve((i - 1) / (normalizeResolution - 1.0f));
				Vector3 pCurrent = GetPositionOnCurve(i / (normalizeResolution - 1.0f));

				normalizeLUT[i] = Vector3.Distance(pLast, pCurrent) + normalizeLUT[i - 1];
			}
		}

		private float GetRateOnNormalizeLUT (float distance)
		{
			float arcLength = normalizeLUT[normalizeResolution - 1];
			if (distance >= 0.0f && distance <= arcLength)
			{
				for (int i = 0; i < normalizeLUT.Length - 1; i++)
				{
					if (distance > normalizeLUT[i] && distance < normalizeLUT[i + 1])
					{
						return Mathf.Lerp(i / (normalizeLUT.Length - 1.0f), (i + 1) / (normalizeLUT.Length - 1.0f), Mathf.InverseLerp(normalizeLUT[i], normalizeLUT[i + 1], distance));
					}
				}
			}

			return distance / arcLength;
		}

		public bool IsContainsLayer (LayerMask mask, int layer)
		{
			return (mask & (1 << layer)) != 0;
		}
		#endregion
	}
}