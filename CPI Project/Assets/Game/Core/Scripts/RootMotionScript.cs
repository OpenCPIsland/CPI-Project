using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]   
public class RootMotionScript : MonoBehaviour {
            
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        Vector3 newPosition = transform.position;
        newPosition.z += animator.GetFloat("Runspeed") * Time.unscaledDeltaTime;
        transform.position = newPosition;
    }

}
