using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererAnimator : MonoBehaviour
{
    private LineRenderer lineRenderer = null;

    [SerializeField]
    private List<Material> materials = new List<Material>();

    [SerializeField]
    private float speed = 1f;

    private int currentsampleNum = 0;
    private int samples = 0;
    private float delayEach = 0f; // Material1���� Materail2�� �̵��ϴµ� �ɸ��� �ð�

    private float timer = 0f;
    private float maxTime = 0f;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    private void OnEnable()
    {
        samples = materials.Count;
        maxTime = samples / speed;
        delayEach = 1 / speed;
    }

    void Update()
    {
        CheckTime();
        SetMaterial();
    }
    private void CheckTime()
    {
        timer += Time.deltaTime * speed;

        if(timer > maxTime)
        {
            timer = 0f;
        }

        currentsampleNum = (int)(timer / delayEach);
    }
    private void SetMaterial()
    {
        lineRenderer.material = materials[currentsampleNum];
    }
}
