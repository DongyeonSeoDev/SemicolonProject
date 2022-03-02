using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SlimeSkin : MonoBehaviour
{
    private PlayerInput playerInput = null;
    private SpriteShapeRenderer renderer = null;

    private Material[] materials;

    void Start()
    {
        renderer = GetComponent<SpriteShapeRenderer>();
        playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();

        materials = renderer.materials;

    }

    void Update()
    {
        float waveZ = 0f;

        for (int i = 0; i < 2; i++)
        {
            if (playerInput.MoveVector.x == 0f)
            {
                waveZ = materials[i].GetVector("_WaveVisuals").y;
            }

            materials[i].SetFloat("_WaveZ", waveZ);
        }
    }
}
