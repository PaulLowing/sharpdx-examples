﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using GraphicsClass = Tutorial20.Graphics.Graphics;
using InputClass = Tutorial20.Inputs.Input;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Tutorial20.System;
using Tutorial20.Graphics.Cameras;
using Tutorial20.Graphics.Models;
using Tutorial20.Graphics.Shaders;
using Tutorial20.Graphics.Data;
using System.Threading;
using Tutorial20.Inputs;

namespace Tutorial20.Graphics
{
	public class Graphics : ICloneable
	{
		#region Constructors
		public Graphics()
		{
		}
		#endregion

		#region Variables / Properties
		private DX11 D3D { get; set; }
		private Camera Camera { get; set; }
		private Model Model { get; set; }
		private BumpMapModel BumpMapModel { get; set; }
		private LightShader LightShader { get; set; }
		private Light Light { get; set; }

		private MultiTextureLightShader MultiTextureLightShader { get; set; }
		private MultiTextureShader MultiTextureShader { get; set; }
		private BumpMapShader BumpMapShader { get; set; }
		private LightMapShader LightMapShader { get; set; }
		private AlphaMapShader AlphaMapShader { get; set; }
		private TextureShader TextureShader { get; set; }
		private Texture Texture { get; set; }
		private Bitmap Bitmap { get; set; }
		private Text Text { get; set; }
		private ModelList ModelList { get; set; }
		private Frustum Frustum { get; set; }
		#endregion

		#region Static Variables
		static float Rotation { get; set; }
		#endregion

		#region Methods
		public bool Initialize(SystemConfiguration configuration, IntPtr windowHandle)
		{
			try
			{
				// Create the Direct3D object.
				D3D = new DX11();
				// Initialize the Direct3D object.
				if (!D3D.Initialize(configuration, windowHandle))
				{
					MessageBox.Show("Could not initialize Direct3D", "Error", MessageBoxButtons.OK);
					return false;
				}

				// Create the camera object
				Camera = new Camera();

				// Initialize a base view matrix the camera for 2D user interface rendering.
				Camera.SetPosition(0, 0, -10);
				Camera.Render();
				var baseViewMatrix = Camera.ViewMatrix;

				// Create the model class.
				BumpMapModel = new BumpMapModel();

				// Initialize the model object.
				if (!BumpMapModel.Initialize(D3D.Device, "sphere.txt", new[] { "stone01.dds", "bump01.dds" }))
				{
					MessageBox.Show("Could not initialize the model object", "Error", MessageBoxButtons.OK);
					return false;
				}

				// Create the bump map shader object.
				BumpMapShader = new BumpMapShader();

				// Initialize the bump map shader object.
				if (!BumpMapShader.Initialize(D3D.Device, windowHandle))
				{
					MessageBox.Show("Could not initialize the light shader", "Error", MessageBoxButtons.OK);
					return false;
				}

				// Create the light object.
				Light = new Light();

				// Initialize the light object.
				Light.SetAmbientColor(0.15f, 0.15f, 0.15f, 1.0f);
				Light.SetDiffuseColor(1, 1, 1, 1f);
				Light.SetDirection(0, 0, 1);
				Light.SetSpecularColor(0, 1, 1, 1);
				Light.SetSpecularPower(32);

				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Could not initialize Direct3D\nError is '" + ex.Message + "'");
				return false;
			}
		}

		public void Shutdown()
		{
			// Release the light object.
			Light = null;

			// Release the light shader object.
			if (MultiTextureShader != null)
			{
				MultiTextureShader.Shuddown();
				MultiTextureShader = null;
			}

			// Release the model object.
			if (Model != null)
			{
				Model.Shutdown();
				Model = null;
			}

			// Release the light shader object.
			if (BumpMapShader != null)
			{
				BumpMapShader.Shuddown();
				BumpMapShader = null;
			}

			// Release the model object.
			if (BumpMapModel != null)
			{
				BumpMapModel.Shutdown();
				BumpMapModel = null;
			}

			// Release the text object.
			if (Text != null)
			{
				Text.Shutdown();
				Text = null;
			}

			// Release the camera object.
			if (Camera != null)
			{
				Camera = null;
			}

			// Release the Direct3D object.
			if (D3D != null)
			{
				D3D.Shutdown();
				D3D = null;
			}
		}

		public bool Render()
		{
			// Clear the buffer to begin the scene.
			D3D.BeginScene(0f, 0f, 0f, 1f);

			// Generate the view matrix based on the camera position.
			Camera.Render();

			// Get the world, view, and projection matrices from camera and d3d objects.
			var viewMatrix = Camera.ViewMatrix;
			var worldMatrix = D3D.WorldMatrix;
			var projectionMatrix = D3D.ProjectionMatrix;

			// Rotate the world matrix by the rotation value so that the triangle will spin.
			Rotate();

			// Rotate the world matrix by the rotation value so that the triangle will spin.
			Matrix.RotationY(Rotation, out worldMatrix);

			//// Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
			//Model.Render(D3D.DeviceContext);

			//// Render the model using the color shader.
			//if (!MultiTextureShader.Render(D3D.DeviceContext, Model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, Model.TextureCollection.Select(item => item.TextureResource).ToArray()))
			//    return false;

			// Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
			BumpMapModel.Render(D3D.DeviceContext);

			// Render the model using the color shader.
			if (!BumpMapShader.Render(D3D.DeviceContext, BumpMapModel.IndexCount, worldMatrix, viewMatrix, projectionMatrix, BumpMapModel.TextureCollection.Select(item => item.TextureResource).ToArray(), Light.Direction, Light.DiffuseColor))
				return false;

			// Present the rendered scene to the screen.
			D3D.EndScene();

			return true;
		}
		#endregion

		#region Static Methods
		public static void Rotate()
		{
			Rotation += (float)Math.PI * 0.005f;
			if (Rotation > 360)
				Rotation -= 360;
		}
		#endregion

		#region Override Methods
		public object Clone()
		{
			return MemberwiseClone();
		}
		#endregion
	}
}
