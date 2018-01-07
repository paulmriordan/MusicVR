using UnityEngine;
using System.Collections;
using VRTK;

public interface ICylinder
{
    float GetCylinderRadius();
    Vector3 GetCylinderOrigin();
}

public class VRTK_InnerCylinderPointerRenderer : VRTK_StraightPointerRenderer
{
    public Transform CylinderRef;

    private ICylinder m_cylinder;

    private void Awake()
    {
        m_cylinder = CylinderRef.GetComponent<ICylinder>();
    }

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
        float lengthToCylinderWall = 0;
        if (GetBeamLengthToCylinderWall(ref lengthToCylinderWall))
        {
            return lengthToCylinderWall;
        }
        return base.OverrideBeamLength(currentLength);
    }

    private bool GetBeamLengthToCylinderWall(ref float beamLength)
    {
        if (GetBeamLengthToCylinderWall2D(ref beamLength))
        {
            return Project2DDistOnto3DNormal(ref beamLength);
        }
        return false;
    }

    private bool GetBeamLengthToCylinderWall2D(ref float beamLength)
    {
        var ray2d = GetRay2D();
        return MathHelper.rayCircleNearestPositveIntersection(
            ray2d,
            m_cylinder.GetCylinderOrigin().ToVector2XZ(),
            m_cylinder.GetCylinderRadius(),
            ref beamLength);
    }

    /// <summary>
    /// Gets the current pointer ray in the 2d x-z plane
    /// </summary>
    private Ray2D GetRay2D()
    {
        Transform origin = GetOrigin();
        var origin2d = origin.position.ToVector2XZ();
        var forward2d = origin.forward.ToVector2XZ();
        forward2d.Normalize();
        return new Ray2D(origin2d, forward2d);
    }

    private bool Project2DDistOnto3DNormal(ref float beamLength)
    {
        float xzPlaneProjFac = PointerNormalProjXZFac();
        if (xzPlaneProjFac != 0) // @pr todo: handle ray directly up (ie, dotNormals = 0)
        {
            beamLength = beamLength / xzPlaneProjFac;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Projection factor for projecting vector onto 2D x-z plane
    /// </summary
    private float PointerNormalProjXZFac()
    {
        Transform origin = GetOrigin();
        Vector3 forwardY0 = origin.forward;
        forwardY0.y = 0;
        forwardY0.Normalize();
        return Vector3.Dot(forwardY0, origin.forward);
    }
}
