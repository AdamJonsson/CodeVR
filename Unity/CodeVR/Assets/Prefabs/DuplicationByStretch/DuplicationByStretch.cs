using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DuplicationByStretch : MonoBehaviour
{
    private CodeBlockManager _codeBlockManager;

    [SerializeField] private float _distanceToDuplicate = 1.0f;
    [SerializeField] private AudioClip _duplicateSound;
    [SerializeField] private AudioSource _stretchAudioSource;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AnimationCurve _cancelAnimation = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private float _cancelAnimationDuration = 0.5f;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private ParticleSystem _particles;

    private CodeBlock _newBlock;
    private CodeBlock _referenceToBlockBeingDuplicated;

    private bool _cancelAnimationHasStarted = false;

    private DateTime _timeCancelAnimationStarted;

    private Vector3 _positionWhenCancelAnimationStarted;

    private CodeBlock _cancelAnimationBlock;
    private CodeBlock _cancelAnimationTargetBlock;

    private float CurrentDistance => Vector3.Distance(this._referenceToBlockBeingDuplicated.transform.position, this._newBlock.transform.position);

    private float DistanceLeft => Mathf.Max(this._distanceToDuplicate - CurrentDistance, 0.0f);

    private float DistanceLeftInterpolation => Mathf.Clamp(this.CurrentDistance / this._distanceToDuplicate, 0.0f, 1.0f);

    private float _lineBaseSize = 0.070f;
    private float _lineMinSize = 0.005f;
    private float LineWidth => (1 - DistanceLeftInterpolation) * _lineBaseSize + _lineMinSize;

    private bool _snapSequenceStarted = false;
    
    private float _lastDistance = 0.0f;

    private CodeBlockInteractionManager _interactionManager;

    // Start is called before the first frame update
    void Start()
    {
        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();
    }

    // Update is called once per frame
    void Update()
    {
        this._line.SetPositions(new Vector3[] {
            this._referenceToBlockBeingDuplicated.transform.position,
            this._newBlock.transform.position
        });

        this._line.startWidth = LineWidth;

        if ((this.DistanceLeft <= 0 || !this._newBlock.IsSolo) && !this._snapSequenceStarted)
            StartCoroutine(StartSnapSequence());

        if (!this._newBlock.IsCurrentlyBeingMoved && !this._cancelAnimationHasStarted && !this._snapSequenceStarted )
            this.StartCancelAnimation(this._newBlock, this._referenceToBlockBeingDuplicated);

        if (this._referenceToBlockBeingDuplicated.IsSolo && !this._referenceToBlockBeingDuplicated.IsCurrentlyBeingMoved && !this._cancelAnimationHasStarted && !this._snapSequenceStarted )
            this.StartCancelAnimation(this._referenceToBlockBeingDuplicated, this._newBlock);

        if (this._cancelAnimationHasStarted && !this._snapSequenceStarted)
            this.RunCancelAnimation();
        

        var speed = Mathf.Abs(this.CurrentDistance - this._lastDistance) / Time.deltaTime;
        this._stretchAudioSource.volume = Mathf.Clamp(speed * 0.5f, 0.0f, 0.5f);
        this._stretchAudioSource.pitch = 1 + DistanceLeftInterpolation;
        this._lastDistance = this.CurrentDistance;

        if (!this._snapSequenceStarted && !this._cancelAnimationHasStarted)
            this._interactionManager.VibrateHandsIfSomethingIsBeingHeld(DistanceLeftInterpolation * speed * 0.5f);
        
    }

    private void RunCancelAnimation()
    {
        var durationOfCancelAnimation = (float)(DateTime.Now - this._timeCancelAnimationStarted).TotalSeconds;
        var curveValue = this._cancelAnimation.Evaluate(durationOfCancelAnimation / _cancelAnimationDuration);
        this._cancelAnimationBlock.transform.position = this._cancelAnimationTargetBlock.transform.position * curveValue + this._positionWhenCancelAnimationStarted * (1 - curveValue);
        this._cancelAnimationBlock.transform.rotation = this._cancelAnimationTargetBlock.transform.rotation; 

        if (durationOfCancelAnimation > this._cancelAnimationDuration)
        {
            this._codeBlockManager.DeleteBlock(this._cancelAnimationBlock);
            Destroy(this.gameObject);
        }
    }

    private void StartCancelAnimation(CodeBlock blockToAnimate, CodeBlock targetBlock)
    {
        this._cancelAnimationHasStarted = true;
        this._timeCancelAnimationStarted = DateTime.Now;
        this._positionWhenCancelAnimationStarted = blockToAnimate.transform.position;
        this._cancelAnimationBlock = blockToAnimate;
        this._cancelAnimationTargetBlock = targetBlock;
    }

    public void SetBlockToFollow(CodeBlock newBlock, CodeBlock blockBeingDuplicated, CodeBlockInteractionManager interactionManager)
    {
        this._newBlock = newBlock;
        this._interactionManager = interactionManager;
        this._referenceToBlockBeingDuplicated = blockBeingDuplicated;
        this.transform.position = newBlock.transform.position;
    }

    private IEnumerator StartSnapSequence()
    {
        this._snapSequenceStarted = true;
        this._particles.transform.position = this._referenceToBlockBeingDuplicated.transform.position + (this._newBlock.transform.position - this._referenceToBlockBeingDuplicated.transform.position) / 2;
        this._particles.transform.LookAt(this._referenceToBlockBeingDuplicated.transform, Vector3.up);
        this._particles.Play();
        this._audioSource.PlayOneShot(this._duplicateSound);
        this._interactionManager.VibrateHandsIfSomethingIsBeingHeld(1.0f);
        this._line.gameObject.SetActive(false);
        this._stretchAudioSource.Stop();
        yield return new WaitForSeconds(1.0f);
        Destroy(this.gameObject);
    }
}
