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

    private AudioSource _audioSource;

    private MeshRenderer _meshRenderer; 

    private List<CodeBlockContainer> _blocksInsideTrashcan = new List<CodeBlockContainer>();

    private CodeBlockManager _codeBlockManager;

    private List<CodeBlock> _blocksToScaleDown = new List<CodeBlock>(); 

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
        if (this._blocksInsideTrashcan.Count > 0)
        {
            this._meshRenderer.material = this._hoverMaterial;
        }
        else
        {
            this._meshRenderer.material = this._normalMaterial;
        }

        foreach (var block in this._blocksToScaleDown)
        {
            if (block == null) continue;
            if (block.transform.localScale.magnitude > 0.01f)
                block.transform.localScale -= Vector3.one * Time.deltaTime * 0.2f;
        }
    }

    private void OnDropBlock(SelectExitEventArgs args)
    {
        var blockContainerDropped = args.interactableObject.transform.gameObject.GetComponent<CodeBlockContainer>();
        
        if (blockContainerDropped == null) return;
        if (!this._blocksInsideTrashcan.Contains(blockContainerDropped)) return;

        var childrenToRemove = new List<CodeBlock>(blockContainerDropped.Children);
        this._blocksToScaleDown.AddRange(childrenToRemove);
        StartCoroutine(RemoveBlocksDelayed(childrenToRemove));
        
        this._blocksInsideTrashcan.Remove(blockContainerDropped);
        blockContainerDropped.DeleteContainer();
    }

    private IEnumerator RemoveBlocksDelayed(List<CodeBlock> blocksToRemove)
    {
        yield return new WaitForSeconds(0.25f);

        foreach (var codeBlock in blocksToRemove)
        {
            this._codeBlockManager.DeleteBlock(codeBlock);
        }

        this._audioSource.Play();
        this._blocksToScaleDown.Clear();
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter!");
        var codeBlock = other.GetComponent<CodeBlockContainer>();
        if (codeBlock == null) return;

        this._blocksInsideTrashcan.Add(codeBlock);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        var codeBlock = other.GetComponent<CodeBlockContainer>();
        if (codeBlock == null) return;

        this._blocksInsideTrashcan.Remove(codeBlock);
    }
}
