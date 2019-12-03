using EngineViewer.Actions._3D.Animations;
using EngineViewer.Controls;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.RbfxUtility
{
	public class Engn_Camera
	{
		public Camera camera;


		public Engn_Camera(Scene scene, string name = "Camera")
		{
			CameraNode = scene.CreateChild(name);

			camera = CameraNode.CreateComponent<Camera>();
			CameraNode.Position = new Vector3(0, 5, 0);
		}


		protected const float TouchSensitivity = 2;


		public float Yaw { get; set; }
		public float Pitch { get; set; }
		protected bool TouchEnabled { get; set; }

		internal Ray GetRay()
		{
			return camera.GetScreenRay((float)camera.Input.MousePosition.X / camera.Graphics.Width, (float)camera.Input.MousePosition.Y / camera.Graphics.Height);
		}

		public Node CameraNode { get; set; }

		public void FirstPersonCamera(Application app, float timeStep, float moveSpeed = 10.0f, Node Selected = null)
		{
			/// <summary>
			/// Move camera for 3D samples
			/// </summary>

			const float mouseSensitivity = .1f;

			if (app.UI.FocusElement != null)
				return;
			var mouseMove = app.Input.MouseMove;

			if (app.Input.GetMouseButtonDown(MouseButton.MousebRight))
			{
				Yaw = CameraNode.Rotation.YawAngle;
				Pitch = CameraNode.Rotation.PitchAngle;

				Yaw += mouseSensitivity * mouseMove.X;
				Pitch += mouseSensitivity * mouseMove.Y;
				Pitch = Urho3DNet.MathDefs.Clamp(Pitch, -90, 90);
				cleanComponents();
				CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
			}
			if (app.Input.GetMouseButtonDown(MouseButton.MousebMiddle))
			{
				cleanComponents();
				if (camera.Input.GetKeyDown(Key.KeyShift) && Selected != null)
				{
					//todo: not working correctly

					CameraNode.RotateAround(Selected.Position, new Quaternion(0, mouseMove.X, 0), TransformSpace.TsWorld);
					Pitch = Urho3DNet.MathDefs.Clamp(CameraNode.Rotation.PitchAngle, -90, 90);

					CameraNode.Rotation = new Quaternion(Pitch, CameraNode.Rotation.YawAngle, 0);

					CameraNode.SetDirection(Selected.Position - CameraNode.Position);
				}
				else
				{
					CameraNode.Translate(new Vector3(-mouseMove.X, mouseMove.Y, 0) * moveSpeed * timeStep);
				}
			}

			if (app.Input.MouseMoveWheel != 0)
			{
				cleanComponents();
				CameraNode.Translate(new Vector3(0, 0, 1) * app.Input.MouseMoveWheel * moveSpeed);
			}

			int scale = 1;
			if (app.Input.GetKeyDown(Key.KeyCtrl))
			{
				scale = 3;
			}
			else scale = 1;

			if (app.Input.GetKeyDown(Key.KeyW))
			{
				cleanComponents();
				CameraNode.Translate(new Vector3(0, 0, 1) * moveSpeed * timeStep * scale);
			}
			if (app.Input.GetKeyDown(Key.KeyS))
			{
				cleanComponents();
				CameraNode.Translate(-new Vector3(0, 0, 1) * moveSpeed * timeStep * scale);
			}
			if (app.Input.GetKeyDown(Key.KeyA))
			{
				cleanComponents();
				CameraNode.Translate(-new Vector3(1, 0, 0) * moveSpeed * timeStep * scale);
			}
			if (app.Input.GetKeyDown(Key.KeyD))
			{
				cleanComponents();
				CameraNode.Translate(new Vector3(1, 0, 0) * moveSpeed * timeStep * scale);
			}
		}

		void cleanComponents()
		{
			CameraNode.RemoveComponent(nameof(MoveObject));
			CameraNode.RemoveComponent(nameof(LookAtObject));
		}
		async internal Task MoveCamera(Vector3 vector3, int duration = 2)
		{
			//CameraNode.MoveTo(vector3, Easeing.In, duration, duration / 4);
			CameraNode.RemoveComponent(nameof(MoveObject));
			var move = CameraNode.CreateComponent<MoveObject>();
			if (move == null)
			{
				move = new MoveObject(camera.Context);
			}
			move.PostUpdate = () => { CameraNode.RemoveComponent(move); };
			move.TargetPos = vector3;
			move.Duration = duration;
		}

		internal void LookAt(Vector3 position, int duration = 2)
		{
			CameraNode.RemoveComponent(nameof(LookAtObject));
			var look = CameraNode.CreateComponent<LookAtObject>();
			if (look == null)
			{
				look = new LookAtObject(camera.Context);
			}


			look.PostUpdate = () =>
			{
				CameraNode.RemoveComponent(look);
			};

			look.TargetPos = position;
			look.Duration = duration;
			look.Start();
		}

		async public Task MoveToSelected(Vector3 MoveTo, Vector3 lookat, float offset)
		{
			var direction = CameraNode.Position - MoveTo;
			direction.Normalize();
			var d = CameraNode.Position.DistanceToPoint(MoveTo);

			MoveCamera(CameraNode.Position - (d - offset) * direction, 4);
			Task.Run(() => LookAt(lookat, 1));
		}
	}



}

