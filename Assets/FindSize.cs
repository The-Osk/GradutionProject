using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSize : MonoBehaviour
{
    Collider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = gameObject.GetComponent<Collider>();
        Debug.Log(collider.bounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
