using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorAnimacionBotonHover : MonoBehaviour
{
    Vector3 escalaInicial;
    private void Start()
    {
        escalaInicial = this.transform.localScale;

        float shakeAmt = 0.25f; // the degrees to shake the camera
        float shakePeriodTime = 1.5f; // The period of each shake
        LTDescr shakeTween = LeanTween.rotateAroundLocal(gameObject, new Vector3(0,1,1), shakeAmt, shakePeriodTime)
        .setEase(LeanTweenType.easeShake) // this is a special ease that is good for shaking
        .setLoopClamp()
        .setRepeat(-1).setIgnoreTimeScale(true); 
    }
    public void AnimacionEscalar()
    {
        LeanTween.scale(this.gameObject, escalaInicial * 1.4f, 0.2f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true); 
    }
    public void AnimacionEscalarInversa()
    {
        LeanTween.scale(this.gameObject, escalaInicial, 0.2f).setEase(LeanTweenType.linear).setIgnoreTimeScale(true); 
    }
}
