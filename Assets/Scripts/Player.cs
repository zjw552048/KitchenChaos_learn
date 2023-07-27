using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    private Transform playTransform;

    private bool isWalking;

    private void Awake() {
        playTransform = transform;
    }

    private void Update() {
        var inputVector = new Vector2();

        if (Input.GetKey(KeyCode.W)) {
            inputVector.y = 1;
        }

        if (Input.GetKey(KeyCode.S)) {
            inputVector.y = -1;
        }

        if (Input.GetKey(KeyCode.A)) {
            inputVector.x = -1;
        }

        if (Input.GetKey(KeyCode.D)) {
            inputVector.x = 1;
        }

        inputVector.Normalize();

        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        playTransform.position += moveDir * moveSpeed * Time.deltaTime;
        playTransform.forward = Vector3.Slerp(playTransform.forward, moveDir, rotateSpeed * Time.deltaTime);

        isWalking = moveDir != Vector3.zero;
        Debug.Log("isWalking: " + isWalking);
    }

    public bool IsWalking() {
        return isWalking;
    }
}