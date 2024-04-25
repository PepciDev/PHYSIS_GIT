using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScaler : MonoBehaviour
{
    Vector3 orginalScale;
    public float scaleMultiplier = 1.1f;

    private void Start()
    {
        orginalScale = transform.localScale;
    }

    public void PointerEnter()
    {
        transform.localScale = orginalScale * scaleMultiplier;
    }

    public void PointerExit()
    {
        transform.localScale = orginalScale;
    }
}
