using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Smooth
{
	public enum SmoothType
	{
		Lerp,
		SmoothStep,
		SmootherStep,
		SmoothestStep
	}

	public static class MathfMore
	{
		public static float SmootherStep(float pStart, float pEnd, float t)
		{
			return Mathf.Lerp(pStart, pEnd, t * t * t * (t * (t * 6f - 15f) + 10f));
		}

		public static float SmoothestStep(float pStart, float pEnd, float t)
		{
			return Mathf.Lerp(pStart, pEnd, -20f * t * t * t * t * t * t * t + 70f * t * t * t * t * t * t + -84f * t * t * t * t * t + 35f * t * t * t * t);
		}
	}
}

