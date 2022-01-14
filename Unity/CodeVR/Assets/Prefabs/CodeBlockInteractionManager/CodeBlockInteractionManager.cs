using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlockInteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask _codeBlockContainerInteractionMask;
    [SerializeField] private LayerMask _codeBlockInteractionMask;

    [SerializeField] private XRRayInteractor _leftController;
    [SerializeField] private XRRayInteractor _rightController;
    [SerializeField] private XRInteractionManager _xrInteractionManager;

    // Start is called before the first frame update
    void Start()
    {
        this._leftController.raycastMask = _codeBlockInteractionMask;
        this._rightController.raycastMask = _codeBlockInteractionMask;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeInteractorCodeBlockContainerInteractable(IXRSelectInteractor interactor)
    {
        XRRayInteractor castedInteractor = interactor as XRRayInteractor;
        castedInteractor.raycastMask = _codeBlockContainerInteractionMask;
        castedInteractor.playHapticsOnHoverEntered = false;
    }

    public void MakeInteractorCodeBlockInteractable(IXRSelectInteractor interactor)
    {
        XRRayInteractor castedInteractor = interactor as XRRayInteractor;
        castedInteractor.raycastMask = _codeBlockInteractionMask;
        castedInteractor.playHapticsOnHoverEntered = true;
    }

    public void MakeInteractorGrabContainer(CodeBlockContainer container, IXRSelectInteractor interactor, bool offsetGrab)
    {
        this.MakeInteractorCodeBlockContainerInteractable(interactor);
        if (offsetGrab)
        {
            var attachmentTransform = interactor.GetAttachTransform(container.Interactable);
            var attachmentGameObject = new GameObject();
            attachmentGameObject.transform.parent = container.transform;
            attachmentGameObject.transform.SetPositionAndRotation(
                attachmentTransform.position, 
                container.transform.rotation
            );
            container.Interactable.attachTransform = attachmentGameObject.transform;
        }
        this._xrInteractionManager.SelectEnter(interactor, container.Interactable);
    }
}
