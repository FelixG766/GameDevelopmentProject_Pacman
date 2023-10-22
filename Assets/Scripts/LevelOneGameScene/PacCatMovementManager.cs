using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacCatMovementManager : MonoBehaviour
{
    private float stepDistance = 0.32f;
    private Vector3[] targetPositions;
    private int currentTargetIndex = 1;
    private int direction = 2;
    private Vector3 nextTarget;
    private Animator animator;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        targetPositions = new Vector3[]{
            new Vector3(0.32f,-0.32f,0),
            new Vector3(1.92f,-0.32f,0),
            new Vector3(1.92f,-1.6f,0),
            new Vector3(0.32f,-1.6f,0)
        };

        nextTarget = targetPositions[currentTargetIndex];
        animator = GetComponent<Animator>();
        animator.SetInteger("Direction", direction);
        
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        StartCoroutine(MovePacCat());
        
    }

    private IEnumerator MovePacCat(){
        while(true){
            audioSource.Play();
            float distance = Vector3.Distance(transform.position, nextTarget);
            while(distance > 0){
                transform.position = Vector3.MoveTowards(transform.position, nextTarget,stepDistance * Time.deltaTime * 2);
                distance = Vector3.Distance(transform.position, nextTarget);
                if(distance == 0){
                    currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Length;
                    nextTarget = targetPositions[currentTargetIndex];
                    direction = CalculateDirection(nextTarget);
                    animator.SetInteger("Direction", direction);
                }
                yield return null;
            }
            if (distance != 0){
                yield return new WaitForSeconds(1f);
            }else{
                yield return new WaitForSeconds(0.5f);                
            }


        }
    }

    private int CalculateDirection(Vector3 nextTarget){
        if(Mathf.Approximately(nextTarget.x,transform.position.x)){
            if(nextTarget.y > transform.position.y){
                return 1;
            }else{
                return 3;
            }
        }else{
            if(nextTarget.x > transform.position.x){
                return 2;
            }else{
                return 0;
            }
        }
    }
    
}
