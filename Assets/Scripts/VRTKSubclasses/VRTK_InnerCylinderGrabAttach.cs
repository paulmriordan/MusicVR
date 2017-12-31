// Rotator Track Grab Attach|GrabAttachMechanics|50090
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Inner Cylinder Grab Attach Track Grab Attach script is used to track the object. 
    /// The cylinder can be dragged along its y-axis, or rotated around its y-axis
    /// 
    /// Note: movement is applied directly to the rigidbody while dragging, rather than applying force. 
    /// 
    /// </summary>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_InnerCylinderGrabAttach")]
    public class VRTK_InnerCylinderGrabAttach : VRTK_TrackObjectGrabAttach
    {
        public float LinearSmoothTime = 5.0f;
        public float AngularSmoothTime = 5.0f;

        private Vector3 m_linearVel = Vector3.zero;
        private float m_angularVel = 0;

        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            var returnVal = base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
            CacheVelocitiesFromRigidbody();
            return returnVal;
        }

        /// <summary>
        /// The StopGrab method ends the grab of the current object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If true will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            isReleasable = false;
            if (applyGrabbingObjectVelocity)
                ApplyVeloctiesToRigidbody();
            base.StopGrab(applyGrabbingObjectVelocity);
        }

        public override void ProcessFixedUpdate()
        {
            ProcessFixedUpdateLinearMovement();
            ProcessFixedUpdatedAngularMovement();
        }

        protected override void SetTrackPointOrientation(ref Transform trackPoint, Transform currentGrabbedObject, Transform controllerPoint)
        {
            trackPoint.position = controllerPoint.position;
            trackPoint.rotation = controllerPoint.rotation;
        }

        private void ApplyVeloctiesToRigidbody()
        {
            grabbedObjectRigidBody.velocity = m_linearVel;
            grabbedObjectRigidBody.angularVelocity = new Vector3(0, m_angularVel * Mathf.Deg2Rad, 0);
        }

        private void CacheVelocitiesFromRigidbody()
        {
            m_linearVel = grabbedObjectRigidBody.velocity;
            m_angularVel = grabbedObjectRigidBody.angularVelocity.y * Mathf.Rad2Deg;
        }

        private void ProcessFixedUpdateLinearMovement()
        {
            var dist = trackPoint.position - initialAttachPoint.position;
            var targetThisUpdate = Vector3.SmoothDamp(transform.position, transform.position + dist, ref m_linearVel, LinearSmoothTime * Time.fixedDeltaTime);
            grabbedObjectRigidBody.MovePosition(targetThisUpdate);
        }

        private void ProcessFixedUpdatedAngularMovement()
        {
            // get new y rotation for this update
            float newYRotation = 0;
            {
                // get diff between initial attach point and track point
                float angleDiffInitialAndTrack = 0;
                {
                    var centreToInitalAttachPoint = initialAttachPoint.position - transform.position;
                    var centreToTrackPoint = trackPoint.position - transform.position;
                    angleDiffInitialAndTrack = MathHelper.AngleBetweenVectors(
                        new Vector2(centreToInitalAttachPoint.x, centreToInitalAttachPoint.z),
                        new Vector2(centreToTrackPoint.x, centreToTrackPoint.z));
                }

                var currentYRotation = grabbedObjectRigidBody.rotation.eulerAngles.y;
                var targetYRotation = currentYRotation + angleDiffInitialAndTrack;
                newYRotation = MovementHelpers.SmoothDampAngle(currentYRotation, targetYRotation, ref m_angularVel, AngularSmoothTime * Time.fixedDeltaTime);
            }

            grabbedObjectRigidBody.MoveRotation(Quaternion.Euler(0, newYRotation, 0));
        }
    }
}