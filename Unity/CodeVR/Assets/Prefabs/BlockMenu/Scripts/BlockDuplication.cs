using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CodeBlock))]
[RequireComponent(typeof(CodeBlockSize))]
[RequireComponent(typeof(XRSimpleInteractable))]
[ExecuteInEditMode]
public class BlockDuplication : MonoBehaviour
{
    [SerializeField] private CodeBlock _prefab;
    
    private XRSimpleInteractable _interactable;

    private CodeBlockManager _codeBlockManager;

    public Action<CodeBlock> OnClone;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<CodeBlock>().enabled = false;        
        this.GetComponent<CodeBlockSize>().enabled = false;
        this._interactable = this.GetComponent<XRSimpleInteractable>();

        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();

        foreach (var connector in this.GetComponentsInChildren<CodeBlockConnector>())
        {
            connector.enabled = false;
            connector.GetComponent<BoxCollider>().enabled = false;
        }

        if (Application.isPlaying)
            this._interactable.selectEntered.AddListener(OnUserSelected);
    }

    void OnUserSelected(SelectEnterEventArgs args)
    {
        var spawnPosition = this.transform.position;
        var spawnedBlock = this._codeBlockManager.CreateNewBlock(this._prefab, spawnPosition, this.transform.rotation);
        var interactor = args.interactorObject as XRRayInteractor;
        spawnedBlock.MakeUserGrabSelfAndConnectedBlocks(interactor, playGrabSound: true);
        this.OnClone.Invoke(spawnedBlock);
    }


}
