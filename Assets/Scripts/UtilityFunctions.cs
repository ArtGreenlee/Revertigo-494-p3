using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunctions : MonoBehaviour
{

    public static List<Vector3> sideVectors = new List<Vector3>
    {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
        Vector3.forward,
        Vector3.back
    };

    public static Quaternion getRotationawayFromSide(Vector3 vecIn)
    {
        Vector3 side = getClosestSide(vecIn);
        return Quaternion.LookRotation(side);
        /*if (side == Vector3.forward)
        {
            return Quaternion.LookRotation(Vector3.back);
        }
        else if (side == Vector3.back)
        {
            return Quaternion.LookRotation(Vector3.forward);
        }
        else if (side == Vector3.left)
        {
            return Quaternion.LookRotation(Vector3.right);
        }
        else if (side == Vector3.right)
        {
            return Quaternion.LookRotation(Vector3.left);
        }
        else if (side == Vector3.up)
        {
            return Quaternion.LookRotation(Vector3.down);
        }
        else if (side == Vector3.down)
        {
            return Quaternion.LookRotation(Vector3.up);
        }
        else {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return new Quaternion();
        }*/
    }

    public static Quaternion getRotationawayFromSide(Vector3 vecIn, Vector3 upDirection)
    {
        Vector3 side = getClosestSide(vecIn);
        return Quaternion.LookRotation(side, upDirection);
        /*if (side == Vector3.forward)
        {
            return Quaternion.LookRotation(Vector3.back, upDirection);
        }
        else if (side == Vector3.back)
        {
            return Quaternion.LookRotation(Vector3.forward, upDirection);
        }
        else if (side == Vector3.left)
        {
            return Quaternion.LookRotation(Vector3.right, upDirection);
        }
        else if (side == Vector3.right)
        {
            return Quaternion.LookRotation(Vector3.left, upDirection);
        }
        else if (side == Vector3.up)
        {
            return Quaternion.LookRotation(Vector3.down, upDirection);
        }
        else if (side == Vector3.down)
        {
            return Quaternion.LookRotation(Vector3.up, upDirection);
        }
        else
        {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return new Quaternion();
        }*/
    }

    public static Vector3 getClosestSide(Vector3 checkVec)
    {
        checkVec = checkVec.normalized;
        float minDistance = 35;
        Vector3 returnSide = Vector3.zero;
        foreach (Vector3 sideVec in sideVectors)
        {
            float curDistance = Vector3.Distance(checkVec, sideVec);
            if (curDistance < minDistance)
            {
                minDistance = curDistance;
                returnSide = sideVec;
            }
        }
        return returnSide;
    }
    public static Vector3 snapVector(Vector3 vecInTemp)
    {
        Vector3 vecIn = vecInTemp;
        if (vecIn.z <= -16)
        {
            vecIn.z = -16;
        }
        else if (vecIn.z >= 16)
        {
            vecIn.z = 16;
        }
        else
        {
            vecIn.z = Mathf.Round(vecIn.z);
        }

        if (vecIn.y <= -16)
        {
            vecIn.y = -16;
        }
        else if (vecIn.y >= 16)
        {
            vecIn.y = 16;
        }
        else
        {
            vecIn.y = Mathf.Round(vecIn.y);
        }

        if (vecIn.x <= -16)
        {
            vecIn.x = -16;
        }
        else if (vecIn.x >= 16)
        {
            vecIn.x = 16;
        }
        else
        {
            vecIn.x = Mathf.Round(vecIn.x);
        }

        return vecIn;
    }

    public static bool validEnemyVector(Vector3 checkVec)
    {
        Vector3 side = getClosestSide(checkVec);
        if (side == Vector3.up || side == Vector3.down)
        {
            if (Mathf.Abs(checkVec.x) > 16 || Mathf.Abs(checkVec.z) > 16)
            {
                return false;
            }
            if (Mathf.Abs(checkVec.y) != 16)
            {
                return false;
            }
        }
        else if (side == Vector3.forward || side == Vector3.back)
        {
            if (Mathf.Abs(checkVec.x) > 16 || Mathf.Abs(checkVec.y) > 16)
            {
                return false;
            }
            if (Mathf.Abs(checkVec.z) != 16)
            {
                return false;
            }
        }
        else if (side == Vector3.left || side == Vector3.right)
        {
            if (Mathf.Abs(checkVec.z) > 16 || Mathf.Abs(checkVec.y) > 16)
            {
                return false;
            }
            if (Mathf.Abs(checkVec.x) != 16)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        return true;
    }

    public IEnumerator shrinkAndDisappear()
    {
        yield return new WaitForEndOfFrame();
    }

}
