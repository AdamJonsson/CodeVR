using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlockContainer : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable _interactable;
    private List<CodeBlock> _children = new List<CodeBlock>();
    private CodeBlockInteractionManager _codeBlockInteractionManager;
    private CodeBlock _codeBlockOrigin;

    public XRGrabInteractable Interactable { get { return this._interactable; }}

    private bool _hasDeleteFlag = false;
    public bool HasDeleteFlag { get => this._hasDeleteFlag; }

    public CodeBlock CodeBlockOrigin { get => this._codeBlockOrigin; }

    // Start is called before the first frame update
    void Start()
    {
        this._codeBlockInteractionManager = FindObjectOfType<CodeBlockInteractionManager>();
        this._interactable.selectExited.AddListener(OnDeselect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDeselect(SelectExitEventArgs args)
    {
        this._codeBlockInteractionManager.MakeInteractorCodeBlockInteractable(args.interactorObject);
        this._hasDeleteFlag = true;
        StartCoroutine(this.DeleteContainerDelayed(1.0f));
    }

    private void DeleteContainer(bool deleteChildren = false)
    {
        if (!deleteChildren)
        {
            foreach (var child in this._children)
            {
                child.MoveOutFromContainer(this);
            }
        }

        Destroy(this.gameObject);
    }

    private IEnumerator DeleteContainerDelayed(float delaySeconds, bool deleteChildren = false)
    {
        yield return new WaitForSeconds(delaySeconds);
        DeleteContainer(deleteChildren);
    }

    public void AddCodeBlock(CodeBlock block)
    {
        this._children.Add(block);
    }

    public void SetCodeBlockOrigin(CodeBlock block)
    {
        this._codeBlockOrigin = block;
    }

}
