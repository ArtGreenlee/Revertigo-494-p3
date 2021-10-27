using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityFunctions : MonoBehaviour
{
    public enum Side { top, bottom, left, right, front, back, error }

    public static Quaternion getRotationawayFromSide(Side sideIn)
    {
        if (sideIn == Side.front)
        {
            return Quaternion.LookRotation(Vector3.back);
        }
        else if (sideIn == Side.back)
        {
            return Quaternion.LookRotation(Vector3.forward);
        }
        else if (sideIn == Side.left)
        {
            return Quaternion.LookRotation(Vector3.right);
        }
        else if (sideIn == Side.right)
        {
            return Quaternion.LookRotation(Vector3.left);
        }
        else if (sideIn == Side.top)
        {
            return Quaternion.LookRotation(Vector3.down);
        }
        else if (sideIn == Side.bottom)
        {
            return Quaternion.LookRotation(Vector3.up);
        }
        else {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return new Quaternion();
        }
    }

    public static Side getSide(Vector3 pos)
    {
        if (pos.x == 10)
        {
            return Side.right;
        }
        if (pos.x == -10)
        {
            return Side.left;
        }
        if (pos.y == 10)
        {
            return Side.top;
        }
        if (pos.y == -10)
        {
            return Side.bottom;
        }
        if (pos.z == -10)
        {
            return Side.back;
        }
        if (pos.z == 10)
        {
            return Side.front;
        }
        return Side.error;
    }
    public static Vector3 snapVector(Vector3 vecInTemp)
    {
        Vector3 vecIn = vecInTemp;
        if (vecIn.z <= -10)
        {
            vecIn.z = -10;
        }
        else if (vecIn.z >= 10)
        {
            vecIn.z = 10;
        }
        else
        {
            vecIn.z = Mathf.Round(vecIn.z);
        }

        if (vecIn.y <= -10)
        {
            vecIn.y = -10;
        }
        else if (vecIn.y >= 10)
        {
            vecIn.y = 10;
        }
        else
        {
            vecIn.y = Mathf.Round(vecIn.y);
        }

        if (vecIn.x <= -10)
        {
            vecIn.x = -10;
        }
        else if (vecIn.x >= 10)
        {
            vecIn.x = 10;
        }
        else
        {
            vecIn.x = Mathf.Round(vecIn.x);
        }

        return vecIn;
    }

    public static bool validEnemyVector(Vector3 checkVec)
    {
        Side side = getSide(checkVec);
        if (side == Side.top || side == Side.bottom)
        {
            if (Mathf.Abs(checkVec.x) > 10 || Mathf.Abs(checkVec.z) > 10)
            {
                return false;
            }
            if (Mathf.Abs(checkVec.y) != 10)
            {
                return false;
            }
        }
        else if (side == Side.front || side == Side.back)
        {
            if (Mathf.Abs(checkVec.x) > 10 || Mathf.Abs(checkVec.y) > 10)
            {
                return false;
            }
            if (Mathf.Abs(checkVec.z) != 10)
            {
                return false;
            }
        }
        else if (side == Side.left || side == Side.right)
        {
            if (Mathf.Abs(checkVec.z) > 10 || Mathf.Abs(checkVec.y) > 10)
            {
                return false;
            }
            if (Mathf.Abs(checkVec.x) != 10)
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

}
