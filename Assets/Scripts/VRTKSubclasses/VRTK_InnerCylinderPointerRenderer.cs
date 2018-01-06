using UnityEngine;
using System.Collections;
using VRTK;

public class VRTK_InnerCylinderPointerRenderer : VRTK_StraightPointerRenderer
{
    public Transform Cylinder; //@todo better way to reference this
    public float CylinderRadius = 0.5f; //@todo need to sync with properties

    public Ray GetCurrentRay()
    {
        var origin = GetOrigin();
        return new Ray(origin.transform.position, origin.forward);
    }

    /// <summary>
    /// Override the beam length so it hits the radius of the cylinder
    /// </summary>
    /// <param name="currentLength"></param>
    /// <returns></returns>
    protected override float OverrideBeamLength(float currentLength)
    {
        var circlePos2d = new Vector2(Cylinder.transform.position.x, Cylinder.transform.position.z);

        Transform origin = GetOrigin();
        var rayPos2d = new Vector2(origin.position.x, origin.position.z);
        var rayFwd2d = new Vector2(origin.forward.x, origin.forward.z);
        rayFwd2d.Normalize();
        float hit = 0;

        if (MathHelper.rayCircleNearestPositveIntersection(
            rayPos2d,
            rayFwd2d,
            circlePos2d,
            CylinderRadius,
            ref hit))
        {

            Vector3 ny0 = new Vector3(rayFwd2d.x, 0, rayFwd2d.y);
            Vector3 hitY0 = origin.position +  ny0 * hit;
            Vector3 hitY0_origin_vec = hitY0 - origin.position;

            float uTop = Vector3.Dot(hitY0, ny0) + Vector3.Dot(hitY0, origin.position);
            float uDiv = Vector3.Dot(ny0, origin.forward);
            if (uDiv != 0) // @todo handle ray directly up (ie, uDiv = 0)
            {
                return hit / uDiv;
            }
        }

        if (controllerGrabScript == null || !controllerGrabScript.GetGrabbedObject())
        {
            savedBeamLength = 0f;
        }

        if (fixBeamLengthOnTipGrabInteraction && controllingPointer != null && controllingPointer.interactWithObjects && controllingPointer.grabToPointerTip && attachedToInteractorAttachPoint && controllerGrabScript != null && controllerGrabScript.GetGrabbedObject())
        {
            savedBeamLength = (savedBeamLength == 0f ? currentLength : savedBeamLength);
            return savedBeamLength;
        }
        return currentLength;
    }
}
