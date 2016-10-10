using System;
using UnityEngine;

namespace Extensions
{
	public class Camera2DFollow : MonoBehaviour
	{
		public Transform target;
		public float damping = 1;
		public float lookAheadFactor = 3;
		public float lookAheadReturnSpeed = 0.5f;
		public float lookAheadMoveThreshold = 0.1f;
		public bool moveOnY = true;
		public float yStartPos = 1f;

		private float m_OffsetZ;
		private Vector2 m_LastTargetPosition;
		private Vector2 m_CurrentVelocity;
		private Vector2 m_LookAheadPos;
		[HideInInspector]
		public bool freezeCamera = false;


		// Use this for initialization
		private void Start ()
		{
			m_LastTargetPosition = target.position;
			//m_OffsetZ = (transform.position - target.position).z;
			m_OffsetZ = transform.position.z;

		}


		// Update is called once per frame
		private void Update ()
		{

			if (freezeCamera)
				return;

			// only update lookahead pos if accelerating or changed direction
			float xMoveDelta = target.position.x - m_LastTargetPosition.x;

			bool updateLookAheadTarget = Mathf.Abs (xMoveDelta) > lookAheadMoveThreshold;

			if (updateLookAheadTarget) {
				m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign (xMoveDelta);
			} else {
				m_LookAheadPos = Vector3.MoveTowards (m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
			}

			Vector2 targPos = target.position;
			if (!moveOnY)
				targPos = new Vector3 (target.position.x, yStartPos);


			Vector2 aheadTargetPos = targPos + m_LookAheadPos + Vector2.right;
			Vector2 pos = transform.position;
			//Vector2 newPos = Vector2.SmoothDamp (pos, aheadTargetPos, ref m_CurrentVelocity, damping);
			Vector2 newPos = Vector2.SmoothDamp (pos, aheadTargetPos, ref m_CurrentVelocity, damping, 10f, Time.deltaTime);

			transform.position = new Vector3 (newPos.x, newPos.y, m_OffsetZ);

			m_LastTargetPosition = target.position;
		}
	}
}
