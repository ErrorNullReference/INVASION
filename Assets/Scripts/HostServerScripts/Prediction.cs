using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prediction
{
	public float ConvergeMultiplier = 0.05f;

	Vector3 predictedPosition, extrapolatedPosition;
	Quaternion predictedRotation, extrapolatedRotation;
	Vector3 predictedSpeed;
	float t, predictedRotationSpeed, latency;

	public Prediction (Vector3 StartPosition, Quaternion StartRotation)
	{
		predictedPosition = StartPosition;
		predictedRotation = StartRotation;
	}

	/// <summary>
	/// Predicts position and rotation based on the given position and rotation and on the time passed form the last call. Returns the time the interpolation should last.
	/// </summary>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	/// <param name="yourPosition">Your position.</param>
	/// <param name="yourRotation">Your rotation.</param>
	public float Predict (Vector3 position, Quaternion rotation, out Vector3 yourPosition, out Quaternion yourRotation, out Vector3 speed)
	{
		//add the 0.1 seconds interval between transform packets to the latency
		latency = Client.Latency / 2f + 0.1f;

		t = Time.deltaTime / (latency * (1 + ConvergeMultiplier));

		predictedSpeed = position - predictedPosition;
		extrapolatedPosition = predictedPosition + predictedSpeed * latency;
		predictedPosition = extrapolatedPosition;
		yourPosition = position + (extrapolatedPosition - position) * t;

		predictedRotationSpeed = Quaternion.Angle (rotation, predictedRotation);
		extrapolatedRotation = predictedRotation * Quaternion.AngleAxis (predictedRotationSpeed * latency, Vector3.up);
		predictedRotation = extrapolatedRotation;
		yourRotation = Quaternion.RotateTowards (rotation, extrapolatedRotation, predictedRotationSpeed * t);

		speed = predictedSpeed;

		return latency;
	}

	public void Reset (Vector3 StartPosition, Quaternion StartRotation)
	{
		predictedPosition = StartPosition;
		predictedRotation = StartRotation;
	}
}
