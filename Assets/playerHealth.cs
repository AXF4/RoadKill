using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;

    // 팀원이 이 함수만 호출하면 됨
    public void SetHealth(int current, int max)
    {
        if (max <= 0) return;
        slider.value = (float)current / max;
    }
}