﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WallMesh : MonoBehaviour
{
	//______________________________________________________________________________________

	//______________________________________________________________________________________
	[Range(0, 100)]
	public int 						NumCols = 4;
	[Range(0, 1000)]
	public float 					NumRows = 1000;
	[Range(0, 1000)]
	public float 					VertRadius = 25.0f;
	public float 					ButtonPaddingFrac = 5.0f;
	[Range(0, 1)]
	public float 					ButtonWidthFrac = 0.5f;
	public GameObject 				ButtonPrefab;

	protected Mesh 					m_mesh;
	protected Vector3[] 			m_vertices;
	protected Vector2[] 			m_uvs;
	protected Color[] 				m_colours;

	public bool		 				m_NeedMeshUpdate {get; set;}

	const int vertsPerCol = 4;
	const int trisPerCol = 2;
	const int indicesPerTri = 3;

	//______________________________________________________________________________________
	void Start()
	{
		m_NeedMeshUpdate = true;
	}

	//______________________________________________________________________________________
	void OnValidate()
	{
		m_NeedMeshUpdate = true;
	}

	//______________________________________________________________________________________
	public void Create()
	{
		//Setup array sizes
		{
			int size = vertsPerCol*NumCols;
			m_vertices = new Vector3[size];
			m_uvs = new Vector2[size];
			m_colours = new Color[size];
		}

		//Create & assign new mesh
		m_mesh = GetComponent<MeshFilter>().sharedMesh = CreateWallMesh();
		DestroyButtons();
		CreateButtons();

	}

	//______________________________________________________________________________________
	private Mesh CreateWallMesh()
	{
		Mesh mesh = new Mesh ();

		int[] triangles = new int[indicesPerTri*trisPerCol*NumCols];
		float colAngle = (2*Mathf.PI)/(float)NumCols;
		float buttonWidth = GetButtonWidth();
		float buttonPadding = ButtonPaddingFrac * buttonWidth;
		float height = buttonPadding + NumRows*(buttonWidth + buttonPadding);
		int v = 0;
		int t = 0;
		for (int iCol = 0; iCol < NumCols; iCol++) 
		{
			float x0 = Mathf.Sin(iCol * colAngle) * VertRadius;
			float z0 = Mathf.Cos(iCol * colAngle) * VertRadius;
			float x1 = Mathf.Sin((iCol + 1) * colAngle) * VertRadius;
			float z1 = Mathf.Cos((iCol + 1) * colAngle) * VertRadius;

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

	private void DestroyButtons()
	{
		for (int i = transform.childCount - 1; i > 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	//______________________________________________________________________________________
	private void CreateButtons()
	{
		float colAngle = (2*Mathf.PI)/(float)NumCols;
		float buttonWidth = GetButtonWidth();
		float buttonPadding = ButtonPaddingFrac * buttonWidth;

		for (int iCol = 0; iCol < NumCols; iCol++) 
		{
			float x0 = Mathf.Sin(iCol * colAngle) * VertRadius;
			float z0 = Mathf.Cos(iCol * colAngle) * VertRadius;
			float x1 = Mathf.Sin((iCol + 1) * colAngle) * VertRadius;
			float z1 = Mathf.Cos((iCol + 1) * colAngle) * VertRadius;
			float x = (x0 + x1) * 0.5f;
			float z = (z0 + z1) * 0.5f;

			for (int iRow = 0; iRow < NumRows; iRow++) 
			{
				float y = iRow * (buttonPadding + buttonWidth) + buttonWidth * 0.5f + buttonPadding;
				var pos = new Vector3(x, y, z);
				var posRot = new Vector3(x, 0, z);
				var inst = GameObject.Instantiate(ButtonPrefab, pos, Quaternion.LookRotation(-posRot), transform); 
				inst.transform.localScale = new Vector3(buttonWidth, buttonWidth, buttonWidth);
			}
		}
	}

	float GetButtonWidth()
	{
		float colAngle = (2*Mathf.PI)/(float)NumCols;
		return VertRadius * Mathf.Sin(colAngle) / (Mathf.Sin((Mathf.PI - colAngle)*0.5f)) * ButtonWidthFrac;	
	}

	//______________________________________________________________________________________
	void Update()
	{
		if (m_NeedMeshUpdate)
		{
			Create();
			m_mesh.vertices = m_vertices;
			m_mesh.colors =  m_colours;
			m_mesh.uv = m_uvs;
			m_mesh.RecalculateNormals();
			m_mesh.RecalculateBounds();
		}

		m_NeedMeshUpdate = false;
	}
}
