using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Shapes
{
	public static Mesh Circle(float radius,int lonRes, int latRes)
	{
		Vector3[] vertices = new Vector3[lonRes * latRes];
		Color[] colors = new Color[lonRes * latRes];
		int[] indices = new int[6 * (lonRes - 1) * (latRes - 1)];
		int indicesIndex = 0;
		for (int j = 0; j < lonRes; j++)
		{
			float teta = -Mathf.PI + (float) j / (float)(lonRes - 1) * 2 * Mathf.PI;
			Color color = Color.HSVToRGB((float) j / ((float) lonRes - 1), 1, 1);
			for (int i = 0; i < latRes; i++)
			{
				float sigma = -Mathf.PI / 2 + (float) i / (float) (latRes - 1) * Mathf.PI;
				float x = radius * Mathf.Cos(teta) * Mathf.Cos(sigma);
				float z = radius * Mathf.Sin(teta) * Mathf.Cos(sigma);
				float y = radius * Mathf.Sin(sigma);
				vertices[GetIndex(j, i, lonRes)] = new Vector3(x, y, z);
				colors[GetIndex(j, i, lonRes)] = color;
				if (i != latRes - 1)
				{
					if (j != lonRes - 1)
					{
						indices[indicesIndex] = GetIndex(j, i, lonRes);
						indices[indicesIndex + 1] = GetIndex(j, i + 1, lonRes);
						indices[indicesIndex + 2] = GetIndex(j + 1, i, lonRes);
						indicesIndex += 3;
					}
					if (j != 0)
					{
						indices[indicesIndex] = GetIndex(j, i, lonRes);
						indices[indicesIndex + 1] = GetIndex(j - 1, i + 1, lonRes);
						indices[indicesIndex + 2] = GetIndex(j, i + 1, lonRes);
						indicesIndex += 3;
					}
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
		mesh.colors = colors;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		return mesh;
	}

	public static Mesh SuperVolume(float radius, SuperShapeParams shape1, SuperShapeParams shape2, int lonRes, int latRes)
	{
		Vector3[] vertices = new Vector3[lonRes * latRes];
		Color[] colors = new Color[lonRes * latRes];
		int[] indices = new int[6 * (lonRes - 1) * (latRes - 1)];
		int indicesIndex = 0;
		for (int j = 0; j < lonRes; j++)
		{
			float teta = -Mathf.PI + (float)j / (float)(lonRes - 1) * 2 * Mathf.PI;
			Color color = Color.HSVToRGB((float)j / ((float)lonRes - 1), 1, 1);
			float r1 = SuperShape(teta, 1, 1, shape1.n1, shape1.n2, shape1.n3, shape1.m1, shape1.m2);
			for (int i = 0; i < latRes; i++)
			{
				float sigma = -Mathf.PI / 2 + (float)i / (float)(latRes - 1) * Mathf.PI;
				float r2 = SuperShape(sigma, 1, 1, shape2.n1, shape2.n2, shape2.n3, shape2.m1, shape2.m2);
				float x = radius * r1 * Mathf.Cos(teta) * r2 * Mathf.Cos(sigma);
				float z = radius * r1 * Mathf.Sin(teta) * r2 * Mathf.Cos(sigma);
				float y = radius * r2 * Mathf.Sin(sigma);
				vertices[GetIndex(j, i, lonRes)] = new Vector3(x, y, z);
				colors[GetIndex(j, i, lonRes)] = color;
				if (i != latRes - 1)
				{
					if (j != lonRes - 1)
					{
						indices[indicesIndex] = GetIndex(j, i, lonRes);
						indices[indicesIndex + 1] = GetIndex(j, i + 1, lonRes);
						indices[indicesIndex + 2] = GetIndex(j + 1, i, lonRes);
						indicesIndex += 3;
					}
					if (j != 0)
					{
						indices[indicesIndex] = GetIndex(j, i, lonRes);
						indices[indicesIndex + 1] = GetIndex(j - 1, i + 1, lonRes);
						indices[indicesIndex + 2] = GetIndex(j, i + 1, lonRes);
						indicesIndex += 3;
					}
				}
			}
		}

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
		mesh.colors = colors;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		return mesh;
	}

	/// <summary>
	/// Update a mesh created with the SuperVolume methode
	/// lonRes and latRes must be the sames than during the SuperVolume creation
	/// </summary>
	public static void UpdateSuperVolume(ref Mesh svMesh, float radius, SuperShapeParams shape1, SuperShapeParams shape2, int lonRes, int latRes)
	{
		Vector3[] vertices = svMesh.vertices;
		for (int j = 0; j < lonRes; j++)
		{
			float teta = -Mathf.PI + (float)j / (float)(lonRes - 1) * 2 * Mathf.PI;
			Color color = Color.HSVToRGB((float)j / ((float)lonRes - 1), 1, 1);
			float r1 = SuperShape(teta, 1, 1, shape1.n1, shape1.n2, shape1.n3, shape1.m1, shape1.m2);
			for (int i = 0; i < latRes; i++)
			{
				float sigma = -Mathf.PI / 2 + (float)i / (float)(latRes - 1) * Mathf.PI;
				float r2 = SuperShape(sigma, 1, 1, shape2.n1, shape2.n2, shape2.n3, shape2.m1, shape2.m2);
				float x = radius * r1 * Mathf.Cos(teta) * r2 * Mathf.Cos(sigma);
				float z = radius * r1 * Mathf.Sin(teta) * r2 * Mathf.Cos(sigma);
				float y = radius * r2 * Mathf.Sin(sigma);
				vertices[GetIndex(j, i, lonRes)] = new Vector3(x, y, z);
			}
		}

		svMesh.vertices = vertices;
		svMesh.RecalculateBounds();
		svMesh.RecalculateNormals();
		svMesh.RecalculateTangents();
	}

	private static float SuperShape(float angle, float a, float b, float n1, float n2, float n3, float m1, float m2)
	{
		float h1 = Mathf.Abs(Mathf.Cos(m1 * angle / 4) / a);
		h1 = Mathf.Pow(h1, n2);
		float h2 = Mathf.Abs(Mathf.Sin(m2 * angle / 4) / b);
		h2 = Mathf.Pow(h2, n3);
		return Mathf.Pow(h1 + h2, -1 / n1);
	}

	private static int GetIndex(int x, int y, int xMax)
	{
		return x + y * xMax;
	}
}

[Serializable]
public class SuperShapeParams
{
	public float n1;
	public float n2;
	public float n3;
	public float m1;
	public float m2;
}
