// Rotator Track Grab Attach|GrabAttachMechanics|50090
namespace VRTK.GrabAttachMechanics
{
    using UnityEngine;

    /// <summary>
    /// The Rotator Track Grab Attach script is used to track the object but instead of the object tracking the direction of the controller, a force is applied to the object to cause it to rotate.
    /// </summary>
    /// <remarks>
    /// This is ideal for hinged joints on items such as wheels or doors.
    /// </remarks>
    /// <example>
    /// `VRTK/Examples/021_Controller_GrabbingObjectsWithJoints` demonstrates this grab attach mechanic on the Wheel and Door objects in the scene.
    /// </example>
    [AddComponentMenu("VRTK/Scripts/Interactions/Grab Attach Mechanics/VRTK_RotatorTrackGrabAttach")]
    public class VRTK_RotatorTrackGrabAttach : VRTK_TrackObjectGrabAttach
    {
        public float Speed = 5.0f;
        public float RotateSpeed = 5.0f;

        private Vector3 m_linearVel = Vector3.zero;
        private float m_angularVel = 0;

        /// <summary>
        /// The StopGrab method ends the grab of the current object and cleans up the state.
        /// </summary>
        /// <param name="applyGrabbingObjectVelocity">If true will apply the current velocity of the grabbing object to the grabbed object on release.</param>
        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            isReleasable = false;
            grabbedObjectRigidBody.velocity = m_linearVel;
            grabbedObjectRigidBody.angularVelocity = new Vector3(0, m_angularVel * Mathf.Deg2Rad, 0);
            base.StopGrab(applyGrabbingObjectVelocity);
        }

        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            var returnVal = base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
            m_linearVel = grabbedObjectRigidBody.velocity;
            m_angularVel = grabbedObjectRigidBody.angularVelocity.y * Mathf.Rad2Deg;
            return returnVal;
        }

        /// <summary>
        /// The ProcessFixedUpdate method is run in every FixedUpdate method on the interactable object. It applies a force to the grabbed object to move it in the direction of the grabbing object.
        /// </summary>
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

        private void ProcessFixedUpdateLinearMovement()
        {
            var dist = trackPoint.position - initialAttachPoint.position;
            var targetThisUpdate = Vector3.SmoothDamp(transform.position, transform.position + dist, ref m_linearVel, Speed * Time.fixedDeltaTime);
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
                newYRotation = MovementHelpers.SmoothDampAngle(currentYRotation, targetYRotation, ref m_angularVel, RotateSpeed * Time.fixedDeltaTime);
            }

            grabbedObjectRigidBody.MoveRotation(Quaternion.Euler(0, newYRotation, 0));

        }
    }
}