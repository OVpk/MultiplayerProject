using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerNetwork : NetworkBehaviour
{
    private readonly NetworkVariable<Vector3> playerPosition = new(
        Vector3.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    [SerializeField] private KeyCode leftKey;
    [SerializeField] private KeyCode rightKey;
    [SerializeField] private KeyCode downKey;
    [SerializeField] private KeyCode upKey;
    [SerializeField] private float moveSpeed;
    
    private Vector3 inputDirection;

    private void Update()
    {
        if (IsOwner && IsClient)
        {
            ReadInput();
            SubmitInputServerRpc(inputDirection);
        }

        if (!IsServer)
            transform.position = playerPosition.Value;
    }

    private void ReadInput()
    {
        inputDirection = Vector3.zero;

        if (Input.GetKey(upKey)) inputDirection.z = 1;
        if (Input.GetKey(downKey)) inputDirection.z = -1;
        if (Input.GetKey(leftKey)) inputDirection.x = -1;
        if (Input.GetKey(rightKey)) inputDirection.x = 1;
    }

    [Rpc(SendTo.Server)]
    void SubmitInputServerRpc(Vector3 direction)
    {
        if (!IsServer) return;

        playerPosition.Value += direction.normalized * (moveSpeed * Time.deltaTime);
        transform.position = playerPosition.Value;
    }
}



public struct PlayerData : INetworkSerializable
{
    public int life;
    public bool isStunt;
    public FixedString128Bytes message;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref life);
        serializer.SerializeValue(ref isStunt);
        serializer.SerializeValue(ref message);
    }
}