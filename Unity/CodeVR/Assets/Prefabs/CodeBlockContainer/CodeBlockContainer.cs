using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlockContainer : MonoBehaviour
{
    [SerializeField] private XRGrabInteractable _interactable;
    private List<CodeBlock> _children = new List<CodeBlock>();
    public List<CodeBlock> Children { get => this._children; }
    private CodeBlockInteractionManager _codeBlockInteractionManager;
    private CodeBlock _codeBlockOrigin;

    public IXRSelectInteractable Interactable { get { return this._interactable; }}

    private bool _hasDeleteFlag = false;
    public bool HasDeleteFlag { get => this._hasDeleteFlag; }

    public CodeBlock CodeBlockOrigin { get => this._codeBlockOrigin; }

    public bool CanBeDeletedUsingTrashcan 
    {
        get
        {
            foreach (var child in this.Children)
            {
                if (!child.IsDeleteableUsingTrashcan) return false;
            }
            return true;
        }
    }

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
        StartCoroutine(this.DeleteContainerKeepChildrenDelayed(1.0f));
    }

    public void DeleteContainerKeepChildren()
    {
        foreach (var child in this._children)
        {
            if (child == null) continue;
            child.MoveOutFromContainer(this);
        }

        Destroy(this.gameObject);
    }

    public void RemoveChild(CodeBlock child)
    {
        this._children.Remove(child);
    }

    private IEnumerator DeleteContainerKeepChildrenDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        DeleteContainerKeepChildren();
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
