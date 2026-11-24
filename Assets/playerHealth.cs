using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetHealth(float current, float max)
    {
        if (slider == null)
        {
            Debug.LogWarning("playerHealth에 Slider가 연결되지 않았습니다.");
            return;
        }

        if (max <= 0f) return;

        slider.value = Mathf.Clamp01(current / max);
    }
}