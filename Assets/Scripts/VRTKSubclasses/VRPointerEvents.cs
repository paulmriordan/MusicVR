using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_BasePointerRenderer))]
public class VRPointerEvents : MonoBehaviour
{
    public VRTK_CustomRaycast customRaycast;
    public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;
    public float maximumLength = 1000.0f;

    VRTK_InnerCylinderPointerRenderer m_pointerRenderer = null;
    Collider m_lastHit = null;

    bool m_pointerSelectDown = false;

    private void Start()
    {
        m_pointerRenderer = GetComponent<VRTK_InnerCylinderPointerRenderer>();
    }

    public void ActivationButtonPressed(object o, ControllerInteractionEventArgs e)
    {
        //OnActivationButtonPressed.Invoke(o, e);
    }

    public void ActivationButtonReleased(object o, ControllerInteractionEventArgs e)
    {
        //OnActivationButtonReleased.Invoke(o, e);
    }

    public void SelectionButtonPressed(object o, ControllerInteractionEventArgs e)
    {
        TryIssuePointerSelectedDownEvent("OnPointerDown");
        m_pointerSelectDown = true;
        m_lastHit = null;
    }

    public void SelectionButtonReleased(object o, ControllerInteractionEventArgs e)
    {
        TryIssuePointerSelectedDownEvent("OnPointerUp");
        m_pointerSelectDown = false;
        m_lastHit = null;
    }

    public void PointerStateValid(object o, DestinationMarkerEventArgs e)
    {
        //OnPointerStateValid.Invoke(o, e);
    }

    public void PointerStateInvalid(object o, DestinationMarkerEventArgs e)
    {
        //OnPointerStateInvalid.Invoke(o, e);
    }

    private void Update()
    {
        if (m_pointerSelectDown)
        {
            UpdateActivePointer();
        }
    }
    
    void UpdateActivePointer()
    {
        RaycastHit hit;
        GetCurrentColliderHit(out hit);
        if (hit.collider == m_lastHit)
        {
            if (hit.collider)
                hit.collider.SendMessage("OnPointerStay", hit.point, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            if (hit.collider)
                hit.collider.SendMessage("OnPointerEnter", hit.point, SendMessageOptions.DontRequireReceiver);
            if (m_lastHit)
                m_lastHit.SendMessage("OnPointerExit", hit.point, SendMessageOptions.DontRequireReceiver);
        }
        m_lastHit = hit.collider;
    }

    void TryIssuePointerSelectedDownEvent(string eventName)
    {
        RaycastHit hit;
        GetCurrentColliderHit(out hit);
        if (hit.collider)
        {
            hit.collider.SendMessage(eventName, hit.point, SendMessageOptions.DontRequireReceiver);
        }
    }

    bool GetCurrentColliderHit(out RaycastHit out_hit)
    {
        var ray = m_pointerRenderer.GetCurrentRay();
        return VRTK_CustomRaycast.Raycast(customRaycast, ray, out out_hit, layersToIgnore, maximumLength);
    }
}
