using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    private Transform pipeHeadTransform;
    private Transform pipeBodyTransform;
    private bool isBottom;

    public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool isBottom)
    {
        this.pipeHeadTransform = pipeHeadTransform;
        this.pipeBodyTransform = pipeBodyTransform;
        this.isBottom = isBottom;
    }

    public void Move(float moveSpeed)
    {
        pipeHeadTransform.position += new Vector3(-1, 0, 0) * moveSpeed * Time.deltaTime;
        pipeBodyTransform.position += new Vector3(-1, 0, 0) * moveSpeed * Time.deltaTime;
    }
    
    public float GetXPos()
    {
        return pipeHeadTransform.position.x;
    }

    public bool IsBottom()
    {
        return isBottom;
    }

    public void DestroySelf()
    {
        Destroy(pipeHeadTransform.gameObject);
        Destroy(pipeBodyTransform.gameObject);
    }
}
