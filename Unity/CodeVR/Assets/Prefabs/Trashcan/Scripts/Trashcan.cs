using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Trashcan : MonoBehaviour
{
    [SerializeField] private XRRayInteractor _leftController;

    [SerializeField] private XRRayInteractor _rightController;

    [SerializeField] private Material _normalMaterial;
    [SerializeField] private Material _hoverMaterial;

    [SerializeField] private AnimationCurve _hoverExpandAnimation = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

    private AudioSource _audioSource;

    private MeshRenderer _meshRenderer; 

    private List<CodeBlockContainer> _blocksInsideTrashcan = new List<CodeBlockContainer>();

    private CodeBlockManager _codeBlockManager;

    private CodeBlockContainer _containerToRemove; 

    // Start is called before the first frame update
    void Start()
    {
        this._audioSource = this.GetComponent<AudioSource>();
        this._meshRenderer = this.GetComponent<MeshRenderer>();
        this._leftController.selectExited.AddListener(OnDropBlock);
        this._rightController.selectExited.AddListener(OnDropBlock);
        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();
    }

    // Update is called once per frame
    void Update()
    {
        this.ApplyHoverColor();
        this.AnimateRemoval();
        this.AnimateHover();
    }

    private void ApplyHoverColor()
    {
        if (this._blocksInsideTrashcan.Count > 0)
        {
            this._meshRenderer.material = this._hoverMaterial;
        }
        else
        {
            this._meshRenderer.material = this._normalMaterial;
        }
    }

    private void AnimateRemoval()
    {
        if (this._containerToRemove == null) return;
        if (this._containerToRemove.transform.localScale.magnitude > 0.01f)
            this._containerToRemove.transform.localScale -= Vector3.one * Time.deltaTime * 3.0f;
    }

    private void AnimateHover()
    {
        if (this._containerToRemove != null) return;
        foreach (var container in this._blocksInsideTrashcan)
        {
            container.transform.localScale = Vector3.one * this._hoverExpandAnimation.Evaluate(Time.timeSinceLevelLoad);
        }
    }

    private void OnDropBlock(SelectExitEventArgs args)
    {
        var blockContainerDropped = args.interactableObject.transform.gameObject.GetComponent<CodeBlockContainer>();
        
        if (blockContainerDropped == null) return;
        if (!this._blocksInsideTrashcan.Contains(blockContainerDropped)) return;

        var childrenToRemove = new List<CodeBlock>(blockContainerDropped.Children);
        this._containerToRemove = blockContainerDropped;
        StartCoroutine(RemoveBlocksDelayed(childrenToRemove));
        
    }

    private IEnumerator RemoveBlocksDelayed(List<CodeBlock> blocksToRemove)
    {
        yield return new WaitForSeconds(0.25f);

        foreach (var codeBlock in blocksToRemove)
        {
            this._codeBlockManager.DeleteBlock(codeBlock);
        }

        this._audioSource.Play();
        this._containerToRemove.DeleteContainerKeepChildren();
        this._blocksInsideTrashcan.Clear();
        this._containerToRemove = null;
    }


    private void OnTriggerEnter(Collider other)
    {
        var codeBlock = other.GetComponent<CodeBlockContainer>();
        if (codeBlock == null) return;

        this._blocksInsideTrashcan.Add(codeBlock);
    }

    private void OnTriggerExit(Collider other)
    {
        var codeBlock = other.GetComponent<CodeBlockContainer>();
        if (codeBlock == null) return;

        this.RestoreOriginalScaleForContainers(this._blocksInsideTrashcan);
        this._blocksInsideTrashcan.Remove(codeBlock);
    }

    private void RestoreOriginalScaleForContainers(List<CodeBlockContainer> containers)
    {
        foreach (var container in containers)
        {
            container.transform.localScale = Vector3.one;
        }
    }
}
