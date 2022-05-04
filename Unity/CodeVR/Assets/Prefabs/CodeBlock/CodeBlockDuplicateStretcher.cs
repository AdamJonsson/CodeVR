using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlockDuplicateStretcher : MonoBehaviour
{
    private XRRayInteractor _hand;
    private XRSimpleInteractable _interactable;
    private Vector3 _originalPosition;
    private float _startDistanceFromHand;

    private CodeBlockManager _codeBlockManager;

    [SerializeField] private AudioClip _duplicateSound;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private float _duplicationSequenceDuration = 0.25f;

    [SerializeField] private AnimationCurve _duplicationAnimationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    private CodeBlock _blockToDuplicate;

    private bool _duplicationSequenceHasStarted = false;

    private DateTime _timeDuplicationSequenceStarted;

    private float _scaleAtStartOfDuplicationSequence = 1.0f;
    private Vector3 _positionAtStartOfDuplicationSequence;

    // Start is called before the first frame update
    void Start()
    {
        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();
        this._interactable = GetComponent<XRSimpleInteractable>();
        this._interactable.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_duplicationSequenceHasStarted)
            this.RunStreachSequence();
        else
            this.RunDuplicationSequence();
    }

    private void RunStreachSequence()
    {
        var distanceVector = this._originalPosition - this._hand.transform.position;
        var projectedDistanceOnForward = Vector3.Project(distanceVector, this.transform.forward);
        var startStreachAmount = 0.06f;
        var distance = projectedDistanceOnForward.magnitude - this._startDistanceFromHand + startStreachAmount;
        this.transform.localScale = new Vector3(0.08f, 0.08f, distance);
        this.transform.position = this._originalPosition - this.transform.forward * distance / 2.0f;

        const float meterToDuplicate = 1.0f;
        if (distance > meterToDuplicate)
            StartCoroutine(StartDuplicationSequence());
    }

    private void RunDuplicationSequence()
    {
        var secondsIntoSequence = ((float)(DateTime.Now - this._timeDuplicationSequenceStarted).TotalSeconds);
        var animationFactor = this._duplicationAnimationCurve.Evaluate(secondsIntoSequence / this._duplicationSequenceDuration - 0.05f);
        this.transform.localScale = new Vector3(
            0.08f * (1 - animationFactor) + 0.075f * animationFactor, 
            0.08f * (1 - animationFactor) + 0.075f * animationFactor, 
            this._scaleAtStartOfDuplicationSequence * (1 - animationFactor) + 0.075f * animationFactor
        );
        this.transform.position = this._positionAtStartOfDuplicationSequence * (1 - animationFactor) + (this._hand.transform.position + this._hand.transform.forward * 0.1f) * (animationFactor);
    }

    public void StartStretchMode(XRRayInteractor interactorHoldingTheBlock, Transform startTransform, CodeBlock blockToDuplicate)
    {
        // interactorHoldingTheBlock.selectExited.AddListener(OnLetGoBeforeDuplication);
        this._hand = interactorHoldingTheBlock;
        this._originalPosition = startTransform.position;
        this._startDistanceFromHand = Vector3.Distance(this._hand.transform.position, this._originalPosition);
        this._blockToDuplicate = blockToDuplicate;
    }

    private void OnLetGoBeforeDuplication(SelectExitEventArgs args)
    {
        this._hand.enabled = true;
        Destroy(this.gameObject);
    }

    private IEnumerator StartDuplicationSequence()
    {
        this._duplicationSequenceHasStarted = true;
        this._scaleAtStartOfDuplicationSequence = this.transform.localScale.z;
        this._positionAtStartOfDuplicationSequence = this.transform.position;

        this._timeDuplicationSequenceStarted = DateTime.Now;
        this._audioSource.PlayOneShot(this._duplicateSound);

        yield return new WaitForSeconds(_duplicationSequenceDuration);
        StartCoroutine(this.EndDuplicationSequence());
    }

    private IEnumerator EndDuplicationSequence()
    {        
        this._hand.enabled = true;
        var newBlock = this._codeBlockManager.CreateNewBlock(this._blockToDuplicate, this.transform.position, this.transform.rotation);
        yield return new WaitForSeconds(0.1f);
        newBlock.MakeUserGrabSelfAndConnectedBlocks(this._hand, true);
        Destroy(this.gameObject);
    }
}
