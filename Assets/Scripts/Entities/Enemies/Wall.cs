using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get wall out of ground
        transform.position += new Vector3(0f, 5f, 0f);
        SetDimensions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetDimensions()
    {
        var forward = FireRay(transform.position, Vector3.forward);
        var backward = FireRay(transform.position, Vector3.back);
        var left = FireRay(transform.position, Vector3.left);
        var right = FireRay(transform.position, Vector3.right);

        var forwardBackwardsDistance = Vector3.Distance(forward, backward);
        var leftRightDistance = Vector3.Distance(left, right);

        if(forwardBackwardsDistance < leftRightDistance)
        {
            // Set position to mid point
            transform.position = (forward + backward) / 2.0f;

            transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            Debug.Log("New Scale: " + Vector3.Distance(transform.position, forward));

            transform.localScale = new Vector3(Vector3.Distance(transform.position, forward) * 2.0f, transform.localScale.y, transform.localScale.z);
        }

        else
        {
            // Set position to mid point
            transform.position = (left + right) / 2.0f;

            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            Debug.Log("New Scale: " + Vector3.Distance(transform.position, left));

            transform.localScale = new Vector3(Vector3.Distance(transform.position, left) * 2.0f, transform.localScale.y, transform.localScale.z);
        }
    }

    private Vector3 FireRay(Vector3 start, Vector3 direction)
    {
        RaycastHit[] hits = Physics.RaycastAll(start + new Vector3(0f, -4f, 0f), direction);
        
        foreach(RaycastHit hit in hits)
        {
            if(hit.transform.tag == "Wall")
            {
                Debug.Log(hit.transform.position);
                return hit.transform.position;
            }
        }

        return Vector3.positiveInfinity;
    }
}
