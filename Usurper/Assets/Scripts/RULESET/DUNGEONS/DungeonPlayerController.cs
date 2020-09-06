using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DungeonPlayerController : MonoBehaviour
{
    public enum PlayerFacing
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }


    private const float angleDeviation = 1f;

    public float turnSpeed = 0.1f;
    public float movementSpeed = 0.7f;
    public bool rotating;
    public bool moving;

    private PlayerFacing facing = PlayerFacing.NORTH;

    // -45 to 45 (NORTH), 45 to 135 (EAST), 135 to 180 and -135 to -180 (SOUTH), -45 to -135 (WEST) 


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !rotating && !moving)
        {
            StartCoroutine(MovePlayerForward());
        }
        if (Input.GetKeyDown(KeyCode.E) && !rotating && !moving)
        {
            StartCoroutine(MovePlayerStrafe(1));
        }
        if (Input.GetKeyDown(KeyCode.Q) && !rotating && !moving)
        {
            StartCoroutine(MovePlayerStrafe(-1));
        }

        if (Input.GetKeyDown(KeyCode.D) && !rotating && !moving)
        {
            StartCoroutine(RotatePlayerY( 90));
        }
        if (Input.GetKeyDown(KeyCode.A) && !rotating && !moving)
        {
            StartCoroutine(RotatePlayerY(-90));
        }
        if (Input.GetKeyDown(KeyCode.S) && !rotating && !moving)
        {
            StartCoroutine(RotatePlayerY(180));
        }
    }


    public IEnumerator RotatePlayerY(float deltaAngle)
    {
        rotating = true;
        float currentAngle = transform.rotation.eulerAngles.y;
        float finalAngle = currentAngle + deltaAngle;

        while (Mathf.Abs(currentAngle - finalAngle) > angleDeviation)
        {
            currentAngle = Mathf.Lerp(currentAngle, finalAngle, turnSpeed);

            transform.rotation = Quaternion.Euler(0, currentAngle, 0);

            yield return new WaitForFixedUpdate();
        }

        transform.rotation = Quaternion.Euler(0, finalAngle, 0); // To snap it to avoid drift.
        SetCurrentFacing(transform.rotation.eulerAngles.y);

        rotating = false;
        yield break;
    }

    public IEnumerator MovePlayerForward()
    {
        //We need to move the player a step forward based on it's facing!
        moving = true;
        Vector3 currentPos = transform.position;
        Vector3 finalPos = currentPos;

        if (facing == PlayerFacing.NORTH || facing == PlayerFacing.SOUTH)   // World Z-axis, North = 1, South = -1
        {
            finalPos.z += (facing == PlayerFacing.NORTH) ? 1 : -1;
        }
        if (facing == PlayerFacing.WEST || facing == PlayerFacing.EAST)     // World X-axis, East = 1, South = -1
        {
            finalPos.x += (facing == PlayerFacing.EAST) ? 1 : -1;
        }


        while (Vector3.Distance(currentPos, finalPos) > 0.1f)
        {
            currentPos = Vector3.Lerp(currentPos, finalPos, movementSpeed);

            transform.position = currentPos;

            yield return new WaitForFixedUpdate();
        }

        transform.position = finalPos;
        moving = false;

        yield break;
    }

    public IEnumerator MovePlayerStrafe(int strafeDir)
    {
        moving = true;
        Vector3 currentPos = transform.position;
        Vector3 finalPos = currentPos;

        switch (facing)
        {
            case PlayerFacing.NORTH:
                finalPos.x += (strafeDir == 1) ? 1 : -1;
                break;

            case PlayerFacing.EAST:
                finalPos.z += (strafeDir == 1) ? -1 : 1;
                break;

            case PlayerFacing.SOUTH:
                finalPos.x += (strafeDir == 1) ? -1 : 1;
                break;

            case PlayerFacing.WEST:
                finalPos.z += (strafeDir == 1) ? 1 : -1;
                break;
        }

        while (Vector3.Distance(currentPos, finalPos) > 0.1f)
        {
            currentPos = Vector3.Lerp(currentPos, finalPos, movementSpeed);

            transform.position = currentPos;

            yield return new WaitForFixedUpdate();
        }

        transform.position = finalPos;
        moving = false;

        yield break;
    }

    public void SetCurrentFacing(float yRot)
    {
        facing = PlayerFacing.NORTH;
        if (yRot > 315 && yRot <=  45) facing = PlayerFacing.NORTH;
        if (yRot > 45 && yRot <= 135) facing = PlayerFacing.EAST;
        if (yRot > 135 && yRot <= 225) facing = PlayerFacing.SOUTH;
        if (yRot > 225 && yRot <= 315) facing = PlayerFacing.WEST;

        //Debug.Log($"facing : {facing}, yRot : {yRot}");
    }

}
