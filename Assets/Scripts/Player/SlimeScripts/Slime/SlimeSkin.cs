using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SlimeSkin : MonoBehaviour
{
    private PlayerInput playerInput = null;
    private SpriteShapeRenderer shapeRenderer = null;

    private Material[] materials;

    [SerializeField]
    private Vector3 waveVisuals = Vector3.zero;
    [SerializeField]
    private Vector3 waveVisualsWhenIdle = Vector3.zero;

    void Start()
    {
        shapeRenderer = GetComponent<SpriteShapeRenderer>();
        playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();

        materials = shapeRenderer.materials;
    }

    void Update()
    {
        Vector3 newWave = Vector3.zero;

        for (int i = 0; i < 2; i++)
        {
            if (playerInput.MoveVector != Vector2.zero)
            {
                newWave.x = waveVisuals.x;
                newWave.y = waveVisuals.y;

                if (playerInput.MoveVector.x == 0f)
                {
                    newWave.z = waveVisuals.z;
                }
            }
            else
            {
                newWave.x = waveVisualsWhenIdle.x;
                newWave.y = waveVisualsWhenIdle.y;
                newWave.z = waveVisualsWhenIdle.z;
            }

            materials[i].SetVector("_WaveVisuals", newWave);
            materials[i].SetVector("_WaveDirections", SlimeGameManager.Instance.Player.GetComponent<PlayerInput>().MoveVector);
        }
    }
}
