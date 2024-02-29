using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider hitbox;
    public int attackPower;
    void Start()
    {
        hitbox = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
