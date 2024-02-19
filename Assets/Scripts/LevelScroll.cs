using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScroll : MonoBehaviour
{
    [SerializeField] private float xVelocity = 0f;
    [SerializeField] private float yVelocity = 0f;

    private static Vector3 START_POS = new Vector3(0f, 0f, -10f); //would label it as a constant, but I don't know how to do that

    private void FixedUpdate() {
        transform.position = new Vector3(transform.position.x+xVelocity, transform.position.y+yVelocity, transform.position.z);
    }
    public void ResetPos() {
        transform.position = START_POS;
    }
}
