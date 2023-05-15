using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public CharacterController controller;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        var moveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        moveDir.Normalize();
        controller.Move(speed * Time.deltaTime * moveDir);
    }
}
