using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTest : MonoBehaviour
{
    int randomNum = 3;

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            Debug.Log(Random.Range(randomNum - 3, randomNum + 3)); // 0, 1, 2, 3, 4, 5
        }
    }
}
