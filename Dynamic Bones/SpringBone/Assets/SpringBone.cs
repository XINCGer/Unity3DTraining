using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBone : MonoBehaviour {

    public Vector3 springEnd = Vector3.left;

    private float springLength {
        get {
            return springEnd.magnitude;
        }
    }
        
    public bool useSpecifiedRotation;

    public Vector3 customRotation;

    public float stiffness = 1.0f;

    public float bounciness = 40.0f;

    [Range(0.0f,0.9f)]
    public float dampness = 0.1f;

    // Use this for initialization
    void Start () {
        currentTipPos = transform.TransformPoint(springEnd);
        if (transform.parent != null) {
            parBone = transform.parent.GetComponentInParent<SpringBone>();
        }
    }
	
    private bool updated = false;
	// Update is called once per frame
	void Update () {
        updated = false;	
	}

    private void LateUpdate() {
        UpdateSpring();

        /*
        Debug.DrawLine(currentTipPos - Vector3.right * 1, currentTipPos + Vector3.right * 1);
        Debug.DrawLine(currentTipPos - Vector3.up * 1, currentTipPos + Vector3.up * 1);
        Debug.DrawLine(currentTipPos - Vector3.forward * 1, currentTipPos + Vector3.forward * 1);*/
    }

    Vector3 currentTipPos;
    SpringBone parBone;
    Vector3 velocity;   //current velocity of tip.

    private void UpdateSpring() {
        if (updated)
            return;
        if (parBone != null)
            parBone.UpdateSpring();     //make sure update is from parent to child. ( or things will just mess up  :D)

        updated = true;

        var lastFrameTip = currentTipPos;

        if (useSpecifiedRotation)
            transform.localRotation = Quaternion.Euler(customRotation);

        currentTipPos = transform.TransformPoint(springEnd);

        var force = bounciness * (currentTipPos - lastFrameTip);  //spring force.

        force += stiffness * (currentTipPos - transform.position).normalized;  //stiffness

        force -= dampness * velocity;               //damp force. 

        velocity = velocity + force * Time.deltaTime;       //v = v0 + at. we don't need integration here, you won't notice any "wrong".

        currentTipPos = lastFrameTip + velocity * Time.deltaTime; //s = s0 + vt

        currentTipPos = springLength * (currentTipPos - transform.position).normalized + transform.position;    //clamp length.

        transform.rotation =
            Quaternion.FromToRotation(transform.TransformDirection(springEnd), (currentTipPos - transform.position).normalized)
            * transform.rotation;
    }
}
