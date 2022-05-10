using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    public string soundId = "UIClickSFX1";

    private void Start()
    {
        if (!string.IsNullOrEmpty(soundId))
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySoundBox(soundId);
            });
        }    
    }
}
