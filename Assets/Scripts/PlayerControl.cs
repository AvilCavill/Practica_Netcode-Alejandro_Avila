using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PlayerControl : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        ReverseWalk
    }

    [SerializeField] 
    private float walkSpeed = 3.5f;
    
    [SerializeField]
    private float rotationSpeed = 1.5f;
    
    [SerializeField]
    private Vector2 defaultInitialPlanePosition = new Vector2(-4, 4);
    
    [SerializeField]
    private NetworkVariable<Vector3> networkPositionDirection  = new NetworkVariable<Vector3>();
    
    [SerializeField]
    private NetworkVariable<Vector3> networkRotationDirection  = new NetworkVariable<Vector3>();
   
    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
    
    private CharacterController characterController;
    
    private Animator animator;
    
    // client caching
    private Vector3 oldInputPosition;
    private Vector3 oldInputRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defaultInitialPlanePosition.x, defaultInitialPlanePosition.y),
                transform.position.y, 0);
            Random.Range(defaultInitialPlanePosition.x, defaultInitialPlanePosition.y);
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
            
        }
        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientInput()
    {
        // player position and rotation input
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);
        
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        if (oldInputPosition != inputPosition || oldInputRotation != inputRotation)
        {
            oldInputPosition = inputPosition;
            UpdateClientPositionAndRotationServerRpc(inputPosition * walkSpeed, inputRotation * rotationSpeed);
        }
        
        // player state changes based in input
        
        if (forwardInput > 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        }
        else if (forwardInput < 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);
        }
        else
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
    }
    
    private void ClientMoveAndRotate()
    {
        if (networkPositionDirection.Value != Vector3.zero)
        {
            characterController.SimpleMove(networkPositionDirection.Value);
        }

        if (networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRotationDirection.Value);
            
        }
    }

    private void ClientVisuals()
    {
        if(networkPlayerState.Value == PlayerState.Walk)
        {
            
            animator.SetFloat("Walk", 1);
        }
        else if(networkPlayerState.Value == PlayerState.ReverseWalk)
        {
            animator.SetFloat("Walk", -1);
        }
        else
        {
            animator.SetFloat("Walk", 0);
        }
    }
    
    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPositionDirection, Vector3 newRotationDirection)
    {
        networkPositionDirection.Value = newPositionDirection;
        networkRotationDirection.Value = newRotationDirection;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState newState)
    {
        
        networkPlayerState.Value = newState;
    }
}
