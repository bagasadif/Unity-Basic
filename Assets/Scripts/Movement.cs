using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] private int speed = 10;
    [SerializeField] private float jumpForce = 0.1f;
    [SerializeField] private bool usePhysics = true;
	[SerializeField] private bool isRunning = false;
	[SerializeField] private bool isJumping = false;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] float stepHeight = 0.1f;
    [SerializeField] float stepSmooth = 0.1f;

    private Camera _mainCamera;
    private Rigidbody _rb;
    private Controls _controls;
    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
	private static readonly int IsRunning = Animator.StringToHash("isRunning");
	private static readonly int IsJumping = Animator.StringToHash("isJumping");
    
    public CapsuleCollider col;
    public float distToGround;
	public GameObject toggler;

    private void Awake()
    {
        _controls = new Controls();
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        _controls.Disable();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rb = gameObject.GetComponent<Rigidbody>();
        col = gameObject.GetComponent<CapsuleCollider>();
        _animator = gameObject.GetComponentInChildren<Animator>();
        distToGround = col.bounds.extents.y;
    }

    private void Update()
    {
        if (usePhysics)
        {
            return;
        }

		if(_controls.Player.Run.IsPressed() || toggler.GetComponent<Toggle>().isOn){
			isRunning = true;
        } else {
			isRunning = false;
		}

		if (isRunning){
			speed = 30;
			ShakingCamera.Instance.ShakeCamera(3f);
			_animator.SetBool(IsRunning, true);
		} else {
			speed = 10;
			ShakingCamera.Instance.ShakeCamera(0f);
			_animator.SetBool(IsRunning, false);
		}

		if (isJumping){
			_animator.SetBool(IsJumping, true);
			Debug.Log("Jump2");
		} else {
			_animator.SetBool(IsJumping, false);
			Debug.Log("No2");
		}

        if (_controls.Player.Move.IsPressed())
        {
            if (IsGrounded())
            {
                _animator.SetBool(IsWalking, true);
            }

            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            Move(target);
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }

		if (_controls.Player.Jump.IsPressed() && IsGrounded())
        {
            Jump();
        }

		stepClimb();
    }

    private void FixedUpdate()
    {
        if (!usePhysics)
        {
            return;
        }

		if(_controls.Player.Run.IsPressed() || toggler.GetComponent<Toggle>().isOn){
			isRunning = true;
        } else {
			isRunning = false;
		}

		if (isRunning){
			speed = 30;
			ShakingCamera.Instance.ShakeCamera(3f);
			_animator.SetBool(IsRunning, true);	
		} else {
			speed = 10;
			ShakingCamera.Instance.ShakeCamera(0f);
			_animator.SetBool(IsRunning, false);
		}

		if (isJumping){
			_animator.SetBool(IsJumping, true);
		} else {
			_animator.SetBool(IsJumping, false);
		}

		if(IsGrounded() && isJumping == true){
			isJumping = false;
		}

        if (_controls.Player.Move.IsPressed())
        {
            if (IsGrounded())
            {
                _animator.SetBool(IsWalking, true);
            }
            Vector2 input = _controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            MovePhysics(target);
        }
        else
        {
            _animator.SetBool(IsWalking, false);
        }

        if (_controls.Player.Jump.IsPressed() && IsGrounded())
        {
            Jump();
        }
        
        stepClimb();
    }

    private Vector3 HandleInput(Vector2 input)
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        
        return transform.position + direction * speed * Time.deltaTime;
    }

    private void Move(Vector3 target)
    {
        transform.position = target;
    }

    private void MovePhysics(Vector3 target)
    {
        _rb.MovePosition(target); 
    }

	public void setRun(){
		if(isRunning){
			isRunning = false;
		} else {
			isRunning = true;
		}
	}

    public void Jump()
    {
		if(IsGrounded()){
			isJumping = true;
			if(_animator.GetBool(IsJumping)){
				_rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
				_animator.SetBool(IsRunning, false);			
        		_animator.SetBool(IsWalking, false);
			}
		} 
    }

    public void JumpTouch()
    {
		if(IsGrounded()){
			_rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			_animator.SetBool(IsRunning, false);			
        	_animator.SetBool(IsWalking, false);
		} 
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    
    private void stepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f))
        {

            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f,0,1), out hitUpper45, 0.2f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }

        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f,0,1), out hitLowerMinus45, 0.1f))
        {

            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f,0,1), out hitUpperMinus45, 0.2f))
            {
                _rb.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
            }
        }
    }
}
