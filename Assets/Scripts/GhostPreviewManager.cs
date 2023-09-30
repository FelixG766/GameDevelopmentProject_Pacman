using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPreviewManager : MonoBehaviour
{
    private Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ChangeSequence());
    }

    private IEnumerator ChangeSequence()
    {
        while(true){
            // Change Direction to 1, 2, 3 with a 3-second wait between each change.
            yield return ChangeIsDead(false);
            yield return new WaitForSeconds(3f);
            yield return ChangeDirection(1);
            yield return new WaitForSeconds(3f);
            yield return ChangeDirection(2);
            yield return new WaitForSeconds(3f);
            yield return ChangeDirection(3);
        
            // Change isScared to true.
            yield return new WaitForSeconds(3f);
            yield return ChangeIsScared(true);
        
            // Change Direction to 2, 1, 0 with a 3-second wait between each change.
            yield return new WaitForSeconds(3f);
            yield return ChangeDirection(2);
            yield return new WaitForSeconds(3f);
            yield return ChangeDirection(1);
            yield return new WaitForSeconds(3f);
            yield return ChangeDirection(0);
        
            // Change isScared to false and isRecovering to true.
            yield return new WaitForSeconds(3f);
            yield return ChangeIsScared(false);
            yield return ChangeIsRecovering(true);

            //Change isDead to true
            yield return new WaitForSeconds(3f);
            yield return ChangeIsRecovering(false);
            yield return ChangeIsDead(true);
        }
    }

    private IEnumerator ChangeDirection(int direction)
    {
        animator.SetInteger("Direction", direction);
        yield return null;
    }

    private IEnumerator ChangeIsScared(bool isScared)
    {
        animator.SetBool("isScared", isScared);
        yield return null;
    }

    private IEnumerator ChangeIsRecovering(bool isRecovering)
    {
        animator.SetBool("isRecovering", isRecovering);
        yield return null;
    }

    private IEnumerator ChangeIsDead(bool isDead)
    {
        animator.SetBool("isDead", isDead);
        yield return null;
    }
}

