using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(VRTK_BasePointerRenderer))]
public class VRPointerEvents : MonoBehaviour
{
    VRTK_InnerCylinderPointerRenderer m_pointerRenderer = null;

    bool m_selectPressed = false;

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

    public VRTK_CustomRaycast customRaycast;
    public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;
    public float maximumLength = 1000.0f;

    public void SelectionButtonPressed(object o, ControllerInteractionEventArgs e)
    {
        TryIssuePointerSelectedDownEvent();
        m_selectPressed = true;
    }

    public void SelectionButtonReleased(object o, ControllerInteractionEventArgs e)
    {
        var ray = m_pointerRenderer.GetCurrentRay();
        RaycastHit pointerCollidedWith;
        bool success = VRTK_CustomRaycast.Raycast(customRaycast, ray, out pointerCollidedWith, layersToIgnore, maximumLength);

        //var hit = m_pointerRenderer.GetDestinationHit();
        //var objInt = m_pointerRenderer.GetObjectInteractor();
        if (pointerCollidedWith.collider)
        {
            Debug.Log("released object " + pointerCollidedWith.collider + " e " + e);
            pointerCollidedWith.collider.SendMessage("OnPointerSelectedButtonReleased", pointerCollidedWith.point, SendMessageOptions.DontRequireReceiver);
        }
        m_selectPressed = false;
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
        if (m_selectPressed)
        {
            TryIssuePointerSelectedDownEvent();
        }
    }

    void TryIssuePointerSelectedDownEvent()
    {
        var ray = m_pointerRenderer.GetCurrentRay();
        RaycastHit pointerCollidedWith;
        bool success = VRTK_CustomRaycast.Raycast(customRaycast, ray, out pointerCollidedWith, layersToIgnore, maximumLength);

        //var hit = m_pointerRenderer.GetDestinationHit();
        //var objInt = m_pointerRenderer.GetObjectInteractor();
        if (pointerCollidedWith.collider)
        {
            Debug.Log("pressed object " + pointerCollidedWith.collider);
            pointerCollidedWith.collider.SendMessage("OnPointerSelectedButtonPressed", pointerCollidedWith.point, SendMessageOptions.DontRequireReceiver);
        }
    }
}
