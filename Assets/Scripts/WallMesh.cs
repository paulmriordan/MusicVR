using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WallMesh : MonoBehaviour
{
	protected Mesh 					m_mesh;
	protected Vector3[] 			m_vertices;
	protected Vector2[] 			m_uvs;
    protected Color[]               m_colours;
    private MusicWallData			m_data;

	public bool		 				m_NeedMeshUpdate {get; set;}

	const int vertsPerCol = 4;
	const int trisPerCol = 2;
	const int indicesPerTri = 3;

	//______________________________________________________________________________________
	public void Create(MusicWallData data)
	{
		m_data = data;

		//Setup array sizes
		{
			int size = vertsPerCol*m_data.CompositionData.NumCols;
			m_vertices = new Vector3[size];
			m_uvs = new Vector2[size];
			m_colours = new Color[size];
		}

		//Create & assign new mesh
		m_mesh = GetComponent<MeshFilter>().sharedMesh = CreateWallMesh();

		m_NeedMeshUpdate = true;
	}

	//______________________________________________________________________________________
	private Mesh CreateWallMesh()
	{
		Mesh mesh = new Mesh ();

		int[] triangles = new int[indicesPerTri*trisPerCol*m_data.CompositionData.NumCols];
		float colAngle = (2*Mathf.PI)/(float)m_data.CompositionData.NumCols;
		float height = m_data.GetTotalHeight();
		int v = 0;
		int t = 0;
		for (int iCol = 0; iCol < m_data.CompositionData.NumCols; iCol++) 
		{
			float x0 = Mathf.Sin(iCol * colAngle) * m_data.Radius;
			float z0 = Mathf.Cos(iCol * colAngle) * m_data.Radius;
			float x1 = Mathf.Sin((iCol + 1) * colAngle) * m_data.Radius;
			float z1 = Mathf.Cos((iCol + 1) * colAngle) * m_data.Radius;

			{
				triangles [t++] = v + 0;
				triangles [t++] = v + 1;
				triangles [t++] = v + 2;
				triangles [t++] = v + 2;
				triangles [t++] = v + 1;
				triangles [t++] = v + 3;
			}

			Matrix4x4 transformation = Matrix4x4.identity;
			{
				var clr = Color.black;
				//Top verts, for displaying side images
				m_vertices [v] = new Vector3 (x1, height, z1);
				m_colours[v] = clr;
				m_uvs [v++] = new Vector2 (1.0f, 1.0f);
				m_vertices [v] = new Vector3 (x1, 0, z1);
				m_colours[v] = clr;
				m_uvs [v++] = new Vector2 (1.0f, 0);
				m_vertices [v] = new Vector3 (x0, height, z0);
				m_colours[v] = clr;
				m_uvs [v++] = new Vector2 (0, 1.0f);
				m_vertices [v] = new Vector3 (x0, 0, z0);
				m_colours[v] = clr;
				m_uvs [v++] = new Vector2 (0,0);
			}
		}

		mesh.vertices = m_vertices;
		mesh.uv = m_uvs;
		mesh.colors = m_colours;
		mesh.triangles = triangles;
		return mesh;
	}

	//______________________________________________________________________________________
	void Update()
	{
		if (m_NeedMeshUpdate)
		{
			m_mesh.vertices = m_vertices;
			m_mesh.colors =  m_colours;
			m_mesh.uv = m_uvs;
			m_mesh.RecalculateNormals();
			m_mesh.RecalculateBounds();
			m_NeedMeshUpdate = false;
		}
	}
}
