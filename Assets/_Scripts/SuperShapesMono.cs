using System;
using System.Collections;
using System.Collections.Generic;
using Smooth;
using UnityEngine;
using Random= UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class SuperShapesMono : MonoBehaviour
{
	[SerializeField] private int _radius = 10;
	[SerializeField] private int _lonRes = 50;
	[SerializeField] private int _latRes = 50;
	[SerializeField] private float _smoothTime = 2f;
	[SerializeField] private SuperShapeParams[] _shapes;

	private MeshFilter _meshFilter;
	private Mesh _shapeMesh;

	private SuperShapeParams[] _nextShapes;
	private SuperShapeParams[] _currentShapes;

	void Start ()
	{
		_shapeMesh = Shapes.SuperVolume(_radius, _shapes[0], _shapes[1], _lonRes, _latRes);
		_meshFilter = this.GetComponent<MeshFilter>();
		_meshFilter.mesh = _shapeMesh;
		InitializeShapeArrays();
	}
	
	void Update () {

		if (Input.GetMouseButtonDown(1))
		{
			StopAllCoroutines();
			StartCoroutine(FromToShape(_smoothTime));
		}
	}

	IEnumerator FromToShape(float smoothTime)
	{
		OrbitalCamera.Refocus = true;

		for (int i = 0; i < _shapes.Length; i++)
		{
			PreSmoothSwitch(i);
		}
		for (float t = 0; t <= smoothTime; t += Time.deltaTime)
		{
			float t1 = t / smoothTime;
			for (int i = 0; i < _shapes.Length; i++)
			{
				_currentShapes[i].n1 = MathfMore.SmootherStep(_shapes[i].n1, _nextShapes[i].n1, t1);
				_currentShapes[i].n2 = MathfMore.SmootherStep(_shapes[i].n2, _nextShapes[i].n2, t1);
				_currentShapes[i].n3 = MathfMore.SmootherStep(_shapes[i].n3, _nextShapes[i].n3, t1);
				_currentShapes[i].m1 = MathfMore.SmootherStep(_shapes[i].m1, _nextShapes[i].m1, t1);
				_currentShapes[i].m2 = MathfMore.SmootherStep(_shapes[i].m2, _nextShapes[i].m2, t1);
			}
			_shapeMesh = Shapes.SuperVolume(_radius, _currentShapes[0], _currentShapes[1], _lonRes, _latRes);
			_meshFilter.mesh = _shapeMesh;
			yield return new WaitForEndOfFrame();
		}

		OrbitalCamera.Refocus = false;
	}

	private void PreSmoothSwitch(int i)
	{
		_shapes[i].n1 = _currentShapes[i].n1;
		_shapes[i].n2 = _currentShapes[i].n2;
		_shapes[i].n3 = _currentShapes[i].n3;
		_shapes[i].m1 = _currentShapes[i].m1;
		_shapes[i].m2 = _currentShapes[i].m2;
		_nextShapes[i].n1 = Random.Range(0.1f, 10f);
		_nextShapes[i].n2 = Random.Range(0.1f, 4.5f);
		_nextShapes[i].n3 = Random.Range(0.1f, 10f);
		_nextShapes[i].m1 = Random.Range(2f, 8f);
		_nextShapes[i].m2 = Random.Range(2f, 8f);
	}

	private void InitializeShapeArrays()
	{
		_nextShapes = new SuperShapeParams[_shapes.Length];
		_currentShapes = new SuperShapeParams[_shapes.Length];
		for (int i = 0; i < _shapes.Length; i++)
		{
			_nextShapes[i] = new SuperShapeParams();
			_currentShapes[i] = new SuperShapeParams();
			_currentShapes[i].n1 = _shapes[i].n1;
			_currentShapes[i].n2 = _shapes[i].n2;
			_currentShapes[i].n3 = _shapes[i].n3;
			_currentShapes[i].m1 = _shapes[i].m1;
			_currentShapes[i].m2 = _shapes[i].m2;
		}
	}

}
