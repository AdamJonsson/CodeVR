using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlockInteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask _codeBlockContainerInteractionMask;
    [SerializeField] private LayerMask _codeBlockInteractionMask;

    [SerializeField] private LayerMask _uiInteractionMask;

    [SerializeField] private XRRayInteractor _leftController;
    [SerializeField] private XRRayInteractor _rightController;
    [SerializeField] private XRInteractionManager _xrInteractionManager;

    [SerializeField] private InputAction _startSelectingRightController;
    [SerializeField] private InputAction _startSelectingLeftController;
    [SerializeField] private InputAction _duplicateButton;

    // Start is called before the first frame update
    void Start()
    {
        this._leftController.raycastMask = _codeBlockInteractionMask;
        this._rightController.raycastMask = _codeBlockInteractionMask;
        this._startSelectingRightController.performed += (context) => this.OnGripChange(context, this._rightController);
        this._startSelectingLeftController.performed += (context) => this.OnGripChange(context, this._leftController);
        this._duplicateButton.performed += (context) => this.StartDuplicationOfBlock();

        this._startSelectingRightController.Enable();
        this._startSelectingLeftController.Enable();
        this._duplicateButton.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGripChange(InputAction.CallbackContext context, XRRayInteractor controller)
    {
        bool isPressingDown = context.ReadValue<float>() > 0.1f;
        this.ToggleControllerUIInteraction(!isPressingDown, controller);
    }

    private void ToggleControllerUIInteraction(bool enable, XRRayInteractor controller)
    {
        if (enable)
        {
            controller.raycastMask = controller.raycastMask | (_uiInteractionMask.value);
        }
        else
        {
            controller.raycastMask = controller.raycastMask & ~(_uiInteractionMask.value);
        }
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
            var containerTransform = container.Interactable.GetAttachTransform(interactor);
            containerTransform.SetPositionAndRotation(
                attachmentTransform.position, 
                container.transform.rotation
            );
        }
        this._xrInteractionManager.SelectExit(interactor, interactor.firstInteractableSelected);
        this._xrInteractionManager.SelectEnter(interactor, container.Interactable);
    }

    private CodeBlock GetBlockHeldByRightHand()
    {
        if (this._rightController.firstInteractableSelected == null) return null;
        var codeBlockContainer = this._rightController.firstInteractableSelected.transform.gameObject.GetComponent<CodeBlockContainer>();
        if (codeBlockContainer == null) return null;
        return codeBlockContainer.CodeBlockOrigin;
    }

    private void StartDuplicationOfBlock()
    {
        var codeBlockToDuplicate = this.GetBlockHeldByRightHand();
        if (codeBlockToDuplicate == null) return;

        var streachBlock = Instantiate(codeBlockToDuplicate, codeBlockToDuplicate.transform.position, codeBlockToDuplicate.transform.rotation);
        var streachBlockComponent = streachBlock.GetComponent<CodeBlockDuplicateStretcher>();
        streachBlockComponent.enabled = true;

        streachBlockComponent.StartStretchMode(this._rightController, codeBlockToDuplicate.transform, codeBlockToDuplicate);
        this._xrInteractionManager.SelectExit(this._rightController, codeBlockToDuplicate.Container.Interactable);
        // this._rightController.enabled = false;
    }
}
