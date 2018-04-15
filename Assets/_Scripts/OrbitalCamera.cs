using System;
using System.Collections;
using System.Collections.Generic;
using Smooth;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitalCamera : MonoBehaviour
{
	[SerializeField] private float _rotYSpeed = 2.5f;
	[SerializeField] private float _rotXSpeed = 2.5f;
	[SerializeField] private float _zoomSpeed = 25;
	[SerializeField] private float _minZoomDist = 10f;
	[SerializeField] private Transform _target;

	private static OrbitalCamera _instance;
	public static OrbitalCamera Instance { get { return _instance; } }
	public static bool Refocus = false;

	private Plane _zoomPlane = new Plane();
	private bool _zooming = false;

	private void Start()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this)
		{
			Destroy(this.gameObject);
		}

		this.transform.LookAt(_target);
	}

	private void Update()
	{
		bool didRotate = false;
		if (!_zooming)
		{
			if (Input.GetMouseButton(0) && Math.Abs(Input.GetAxis("Mouse X")) > 0.01f)
			{
				float sign = (Vector3.Dot(_target.up, this.transform.up) >= 0) ? 1 : -1;
				this.transform.RotateAround(_target.position, Vector3.up, sign * Input.GetAxis("Mouse X") * _rotYSpeed);
				didRotate = true;
			}

			if (Input.GetMouseButton(0) && Math.Abs(Input.GetAxis("Mouse Y")) > 0.01f)
			{
				this.transform.RotateAround(_target.position, this.transform.right, -Input.GetAxis("Mouse Y") * _rotXSpeed);
				didRotate = true;
			}
		}

		if (!didRotate && Input.mouseScrollDelta.y != 0)
		{
			Zoom(Input.mouseScrollDelta.y);
		}
	}

	private void LateUpdate()
	{
		// force the camera to lookAt the target
		if (Refocus)
		{
			if (Vector3.Dot(this.transform.up, _target.up) < 0)
			{
				this.transform.LookAt(_target, -Vector3.up);
			}
			else
			{
				this.transform.LookAt(_target, Vector3.up);
			}
		}
	}

	private void Zoom(float value)
	{
		Vector3 targetPos = _target.position;
		Vector3 currentPos = this.transform.position;

		_zoomPlane.SetNormalAndPosition(targetPos - currentPos, targetPos);

		float moveValue = value * _zoomSpeed;
		Vector3 nextPos = currentPos + (targetPos - currentPos).normalized * moveValue;

		// If next position is lesser than _minZoom or the other side of the super volume, then we place nextPos at minzoom
		if (Vector3.Magnitude(targetPos - nextPos) < _minZoomDist || _zoomPlane.GetSide(nextPos))
		{
			nextPos = targetPos - (targetPos - currentPos).normalized * _minZoomDist;
		}
		StartCoroutine(SmoothZoom(currentPos, nextPos));
	}

	IEnumerator SmoothZoom(Vector3 form, Vector3 to)
	{
		_zooming = true;
		float smoothTime = 0.1f;
		Vector3 interPos = new Vector3();
		for (float t = 0; t <= smoothTime; t += Time.deltaTime)
		{
			float t1 = t / smoothTime;
			interPos.x = MathfMore.SmootherStep(form.x, to.x, t1);
			interPos.y = MathfMore.SmootherStep(form.y, to.y, t1);
			interPos.z = MathfMore.SmootherStep(form.z, to.z, t1);
			this.transform.position = interPos;
			yield return new WaitForEndOfFrame();
		}
		_zooming = false;
	}
}
