using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxInScene : MonoBehaviour
{
    public Box box;
    public Vector3 initBoxSize;
    RaycastHit hit;

    
    public bool isStaticObject()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if(hit.point.y >= -initBoxSize.y / 2)
            {
                box.lbdCorner[1] = (int)hit.point.y;
                transform.position = new Vector3(transform.position.x, hit.point.y + transform.localScale.y / 2, transform.position.z);
                return false;
            }
            else
            {
                if (box.lbdCorner[1] == -initBoxSize.y / 2)
                {
                    return true;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, -initBoxSize.y + transform.localScale.y / 2, transform.position.z);
                    box.lbdCorner[1] = (int) - initBoxSize.y / 2;
                    return false;
                }
                
            }
        }
        transform.position = new Vector3(transform.position.x, -initBoxSize.y + transform.localScale.y / 2, transform.position.z);
        box.lbdCorner[1] = (int)- initBoxSize.y / 2;
        return true;
    }
}
