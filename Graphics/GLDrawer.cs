using UnityEngine;

namespace Common.Graphics
{
    public enum GLDrawType
	{
		LINE,
		SOLID
	}

    public static class GLDrawer
	{
		private static int CircleDivisions = 30;
		private static int SphereDivisions = 12;

		private static readonly Vector3[] circlewVertices;
		private static readonly Vector3[] circleVertices;
		private static readonly Vector3[] cubeVerteces;
		private static readonly Vector3[] coneVertices;
		private static readonly Vector3[] planeVertices;
		private static readonly Vector3[] cylinderVertices;
		private static readonly Vector3[] sphereVertices;
		private static readonly Vector3[] torusVertices;

		private static Material lineMaterial;
		private static Material gridMaterial;
		private static void CreateLineMaterial()
		{
			if (!lineMaterial)
			{
				// Unity has a built-in shader that is useful for drawing
				// simple colored things.
				Shader shader = Shader.Find("Hidden/Internal-Colored");
				lineMaterial = new Material(shader);
				lineMaterial.hideFlags = HideFlags.HideAndDontSave;
				// Turn on alpha blending
				lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				// Turn backface culling off
				lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				// Turn off depth writes
				lineMaterial.SetInt("_ZWrite", 1);
				lineMaterial.SetInt("_ZTest", 0);
			}
		}

		private static void CreateGridMaterial()
		{
			Shader shader = Shader.Find("Cloudberrie/Grid");
			gridMaterial = new Material(shader);
		}

		static GLDrawer()
		{
			//Create circle vertices
			circlewVertices = new Vector3[(CircleDivisions + 1)];
			int deg = 360 / CircleDivisions;
			for (int i = 0; i < 360; i += 2 * deg)
			{
				float x = Mathf.Sin(Mathf.Deg2Rad * i);
				float y = Mathf.Cos(Mathf.Deg2Rad * i);

				float x2 = Mathf.Sin(Mathf.Deg2Rad * (i + deg));
				float y2 = Mathf.Cos(Mathf.Deg2Rad * (i + deg));

				circlewVertices[i / deg] = new Vector3(x, y);
				circlewVertices[i / deg + 1] = new Vector3(x2, y2);
			}

			circlewVertices[circlewVertices.Length- 1] = circlewVertices[0];

			cubeVerteces = new Vector3[24]; //4 vertexes per face

			//X aligned face
			cubeVerteces[0] = new Vector3(-0.5f,-0.5f,-0.5f);
			cubeVerteces[1] = new Vector3(-0.5f,-0.5f,0.5f);
			cubeVerteces[2] = new Vector3(-0.5f,0.5f,0.5f);
			cubeVerteces[3] = new Vector3(-0.5f,0.5f,-0.5f);

			//Y aligned face
			cubeVerteces[4] = new Vector3(-0.5f,-0.5f,-0.5f);
			cubeVerteces[5] = new Vector3(-0.5f,-0.5f,0.5f);
			cubeVerteces[6] = new Vector3(0.5f,-0.5f,0.5f);
			cubeVerteces[7] = new Vector3(0.5f,-0.5f,-0.5f);

			//Z aligned face
			cubeVerteces[8] = new Vector3(-0.5f,-0.5f,-0.5f);
			cubeVerteces[9] = new Vector3(-0.5f,0.5f,-0.5f);
			cubeVerteces[10] = new Vector3(0.5f,0.5f,-0.5f);
			cubeVerteces[11] = new Vector3(0.5f,-0.5f,-0.5f);

			//Duplicate faces
			Vector3[] transformationVectors = new[] {Vector3.right, Vector3.up, Vector3.forward};
			for (int i = 12; i < 24; i++)
			{
				int mirrorIndex = i - 12;
				cubeVerteces[i] = cubeVerteces[mirrorIndex] + transformationVectors[mirrorIndex/4];
			}

			//Create circle vertices
			circleVertices = new Vector3[CircleDivisions + 1];

			for (int i = 0; i < 360; i += 2 * deg)
			{
				float x = Mathf.Sin(Mathf.Deg2Rad * i);
				float y = Mathf.Cos(Mathf.Deg2Rad * i);

				circleVertices[i / deg] = new Vector3(x, y);
			}

			circleVertices[circlewVertices.Length- 11] = circleVertices[0];

			//Create plane verts
			planeVertices = new Vector3[4];

			planeVertices[0] = new Vector3(-0.5f,-0.5f);
			planeVertices[1] = new Vector3(-0.5f,0.5f);
			planeVertices[2] = new Vector3(0.5f,0.5f);
			planeVertices[3] = new Vector3(0.5f,-0.5f);

			//Create cylinder verts
			cylinderVertices = new Vector3[CircleDivisions * 2 + 2];

			for (int i = 0; i < 360; i += 2 * deg)
			{
				float x = Mathf.Sin(Mathf.Deg2Rad * i);
				float y = Mathf.Cos(Mathf.Deg2Rad * i);

				cylinderVertices[i / deg] = new Vector3(x, 1, y);
				cylinderVertices[i / deg + 1] = new Vector3(x, -1, y);
			}

			cylinderVertices[circlewVertices.Length - 1] = cylinderVertices[1];
			cylinderVertices[circlewVertices.Length - 2] = cylinderVertices[0];

			CreateGridMaterial();
			CreateLineMaterial();
		}

		public static void WirePlane(Vector3 center, Vector3 size, Color color)
		{

		}

		public static void Plane(Vector3 center, Vector3 pivot, Vector3 rotation, Vector3 size, Color color)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 m =Matrix4x4.identity;
			m.SetTRS(center,Quaternion.Euler(rotation),size);

			GL.MultMatrix(m);
			GL.Begin(GL.QUADS);
			GL.Color(color);
			for(int i = 0; i < planeVertices.Length; i++)
				GL.Vertex(planeVertices[i]);
			GL.End();
			GL.PopMatrix();
		}

		public static void Cone(Vector3 center, Vector3 pivot, Vector3 rotation, float radius, float height, Color color)
		{
			Circle(center, pivot, rotation, radius, color, GLDrawType.SOLID);

			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 m = Matrix4x4.identity;
			m.SetTRS(center,Quaternion.Euler(rotation),Vector3.one*radius);

			GL.MultMatrix(m);
			GL.Begin(GL.TRIANGLE_STRIP);
			GL.Color(color);


			for (int i = 0; i < circleVertices.Length; i++)
			{
				GL.Vertex(circleVertices[i] + pivot);
				GL.Vertex( pivot + Vector3.back * height);
			}

			GL.End();
			GL.PopMatrix();
		}

		//Note: there are some misaligned vertices here. Only good for now because it's not going to be seen :p
		public static void Cylinder(Vector3 center, Vector3 pivot, Vector3 rotation, Vector3 size, float radius, Color color)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 m = Matrix4x4.identity;
			m.SetTRS(center,Quaternion.Euler(rotation),size*radius);

			GL.MultMatrix(m);
			GL.Begin(GL.TRIANGLE_STRIP);
			GL.Color(color);

			for(int i = 0; i < cylinderVertices.Length; i++)
				GL.Vertex(cylinderVertices[i] + pivot);

			GL.End();
			GL.PopMatrix();
		}

		public static void Circle(Vector3 center, Vector3 pivot, Vector3 rotation, float radius, Color color, GLDrawType drawType)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 m = Matrix4x4.identity;
			m.SetTRS(center,Quaternion.Euler(rotation),Vector3.one*radius);

			GL.MultMatrix(m);
			GL.Begin(drawType == GLDrawType.LINE ? GL.LINE_STRIP : GL.TRIANGLE_STRIP);
			GL.Color(color);

			Vector3[] elements = drawType == GLDrawType.LINE ? circlewVertices : circleVertices;

			foreach (var t in elements)
				GL.Vertex(t + pivot);

			GL.End();
			GL.PopMatrix();
		}

		public static void QuarterCircle(Vector3 center, Vector3 pivot, Vector3 rotation, float radius, Color color, GLDrawType drawType)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 m = Matrix4x4.identity;
			m.SetTRS(center,Quaternion.Euler(rotation),Vector3.one*radius);

			GL.MultMatrix(m);
			GL.Begin(drawType == GLDrawType.LINE ? GL.LINE_STRIP : GL.TRIANGLE_STRIP);
			GL.Color(color);

			Vector3[] elements = drawType == GLDrawType.LINE ? circlewVertices : circleVertices;

			for(int i = 0; i < elements.Length / 4 + 1; i++)
				GL.Vertex(elements[i] + pivot);

			GL.End();
			GL.PopMatrix();
		}

		public static void WireSphere(Vector3 center, Vector3 pivot, float radius, Color color)
		{
			Circle(center, pivot, Vector3.zero, radius, color, GLDrawType.LINE);
			Circle(center, pivot, Vector3.right * 90, radius, color, GLDrawType.LINE);
			Circle(center, pivot, Vector3.up * 90, radius, color, GLDrawType.LINE);
		}

		public static void WireSphere(Vector3 center, Vector3 pivot, float radius, Color color1, Color color2, Color color3)
		{
			Circle(center, pivot, Vector3.zero, radius, color1, GLDrawType.LINE);
			Circle(center, pivot, Vector3.right * 90, radius, color2, GLDrawType.LINE);
			Circle(center, pivot, Vector3.up * 90, radius, color3, GLDrawType.LINE);
		}

		public static void Cube(Vector3 center, Vector3 size, Vector3 rotation, Color color)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 m =Matrix4x4.identity;
			m.SetTRS(center,Quaternion.Euler(rotation),size);

			GL.MultMatrix(m);
			GL.Begin(GL.QUADS);
			GL.Color(color);
			for(int i = 0; i<cubeVerteces.Length; i++)
				GL.Vertex(cubeVerteces[i]);
			GL.End();
			GL.PopMatrix();
		}

		public static void Line(Vector3 start, Vector3 end, Color color, bool setPass = true)
		{
			if (setPass)
			{
				lineMaterial.SetPass(0);
			}

			GL.PushMatrix();
			GL.Begin(GL.LINES);
			GL.Color(color);
			GL.Vertex(start);
			GL.Vertex(end);
			GL.End();
			GL.PopMatrix();
		}

		public static void Sphere(Vector3 center, float radius, int segments, Color color)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 mat = Matrix4x4.identity;
			mat.SetTRS(center, Quaternion.identity, Vector3.one * radius);
			GL.MultMatrix(mat);

			for(int i = 0; i <= segments; i++) {
				float lat0 = Mathf.PI * (-0.5f + (float) (i - 1) / segments);
				float z0  = Mathf.Sin(lat0);
				float zr0 =  Mathf.Cos(lat0);

				float lat1 = Mathf.PI * (-0.5f + (float) i / segments);
				float z1 = Mathf.Sin(lat1);
				float zr1 = Mathf.Cos(lat1);

				GL.Begin(GL.TRIANGLE_STRIP);
				GL.Color(color);

				for(int j = 0; j <= segments; j++) {
					float lng = 2 * Mathf.PI * (j - 1) / segments;
					float x = Mathf.Cos(lng);
					float y = Mathf.Sin(lng);

					GL.Vertex3(x * zr0, y * zr0, z0);
					GL.Vertex3(x * zr1, y * zr1, z1);
				}
				GL.End();
			}

			GL.PopMatrix();
		}

		public static void Torus(Vector3 center, Vector3 rotation, float thickness, float radius, int segments, int columnSegment, Color color)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 mat = Matrix4x4.identity;
			mat.SetTRS(center, Quaternion.Euler(rotation), Vector3.one);
			GL.MultMatrix(mat);

			float TWOPI = 2 * (Mathf.PI);

			for (int i = 0; i < segments; i++) {
				GL.Begin(GL.TRIANGLE_STRIP);
				GL.Color(color);
				for (int j = 0; j <= columnSegment; j++) {
					for (int k = 1; k >= 0; k--) {

						float s = (i + k) % segments + 0.5f;
						float t = j % columnSegment;

						float x = (radius + thickness * Mathf.Cos(s * TWOPI / segments)) * Mathf.Cos(t * TWOPI / columnSegment);
						float y = (radius + thickness * Mathf.Cos(s * TWOPI / segments)) * Mathf.Sin(t * TWOPI / columnSegment);
						float z = thickness * Mathf.Sin(s * TWOPI / segments);

						GL.Vertex3(x, y, z);
					}
				}
				GL.End();
			}

			GL.PopMatrix();
		}

		public static void CurvedCylinder(Vector3 center, Vector3 rotation, float size, float radius, float bendRadius, int segments,  Color color)
		{
			lineMaterial.SetPass(0);

			GL.PushMatrix();
			Matrix4x4 mat = Matrix4x4.identity;
			mat.SetTRS(center, Quaternion.Euler(rotation), Vector3.one * size);
			GL.MultMatrix(mat);

			for (int i = 0; i < segments; i++ )
			{
				float w0 =  i / (float)segments;
				float w1 = (i+1) / (float)segments;

				GL.Begin(GL.TRIANGLE_STRIP);
				GL.Color(color);

				for (int j = 0; j <= 360; ++ j )
				{
					float angle = j * (Mathf.PI * 2) / 180.0f;
					float x = radius * Mathf.Cos(angle) + 1;
					float y = radius * Mathf.Sin(angle);

					float xb = Mathf.Sin( w0 * bendRadius) * x;
					float yb = y;
					float zb = Mathf.Cos( w0 * bendRadius) * x;
					GL.Vertex3( xb, yb, zb );

					xb = Mathf.Sin( w1 * bendRadius) * x;
					yb = y;
					zb = Mathf.Cos( w1 * bendRadius) * x;
					GL.Vertex3(xb, yb, zb );
				}
				GL.End();
			}


			GL.PopMatrix();
		}

		public static void AA3DGrid(Vector3 pivot, Vector3Int dimensions, Vector3 cellExtents, Color color)
		{
			int xDim = dimensions.x + 1;
			int yDim = dimensions.y + 1;
			int zDim = dimensions.z + 1;

			gridMaterial.SetPass(0);

			GL.PushMatrix();
			GL.Begin(GL.LINES);
			GL.Color(color);

			float hLen = dimensions.x * cellExtents.x;
			for (int y = 0; y < yDim; y++)
			{
				for (int z = 0; z < zDim; z++)
				{
					Vector3 start = pivot + new Vector3(0, y * cellExtents.y, z * cellExtents.z);
					Vector3 end = start + new Vector3(hLen, 0, 0);
					GL.Vertex(start);
					GL.Vertex(end);
				}
			}

			float tLen = dimensions.z * cellExtents.z;
			for (int x = 0; x < xDim; x++)
			{
				for (int y = 0; y < yDim; y++)
				{
					Vector3 start = pivot + new Vector3(x * cellExtents.x, y * cellExtents.y, 0);
					Vector3 end = start + new Vector3(0, 0, tLen);
					GL.Vertex(start);
					GL.Vertex(end);
				}
			}

			GL.End();
			GL.PopMatrix();
		}
	}
}