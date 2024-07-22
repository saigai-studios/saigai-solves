using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnimWithButton : MonoBehaviour
{
    public Animator animator;
    public GameObject play, reverse;
    public bool slideOut, slideGone;

    public void PlayAnimation()
    {
        if (slideOut == false && slideGone == false)
        {
            animator.SetTrigger("TheTrigger");
            StartCoroutine(FirstButton());
            slideOut = true;
        }
        else if (slideOut == true)
        {
            animator.SetTrigger("Reverse");
            StartCoroutine(ReverseButton());
            animator.SetTrigger("Default");
            slideOut = false;
            slideGone = true;
        }
        else if (slideGone == true)
        {
            animator.ResetTrigger("Default");
            animator.SetTrigger("TheTrigger");
            StartCoroutine(FirstButton());
            slideOut = true;
            slideGone = false;
        }


        IEnumerator FirstButton()
        {
            yield return new WaitForSeconds(0.75f);

            play.SetActive(false);
            reverse.SetActive(true);
        }
        IEnumerator ReverseButton()
        {
            yield return new WaitForSeconds(0.75f);

            play.SetActive(true);
            reverse.SetActive(false);
        }
    }

}
