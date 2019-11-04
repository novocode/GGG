using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
	protected float moveTime = 0.1f;
	protected LayerMask blockingLayer; //This will block everything from passing through
	
	private BoxCollider2D boxCollider;
	private Rigidbody2D rB2D;
	private float inverseMoveTime;
	
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
		rB2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f/moveTime;
    }
}
