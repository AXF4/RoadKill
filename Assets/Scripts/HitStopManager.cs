using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    [Header("UI 오버레이")]
    [SerializeField] private Canvas hitStopCanvas;
    [SerializeField] private Image grayscaleOverlay; // 화면 전체를 덮는 밝은 회색 반투명 패널

    [Header("색상 설정")]
    [SerializeField] private Color flashColor = new Color(0.85f, 0.85f, 0.85f, 0.75f); // 밝기를 높인 회색조

    private Coroutine hitStopRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 캔버스 및 오버레이 초기화
        if (grayscaleOverlay != null)
        {
            grayscaleOverlay.color = Color.clear;
            grayscaleOverlay.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 히트스탑 및 강조 연출 트리거
    /// </summary>
    public void TriggerHitStop(GameObject attacker, GameObject victim, float duration = 0.08f)
    {
        if (hitStopRoutine != null)
        {
            StopCoroutine(hitStopRoutine);
        }

        hitStopRoutine = StartCoroutine(HitStopProcess(attacker, victim, duration));
    }

    private IEnumerator HitStopProcess(GameObject attacker, GameObject victim, float duration)
    {
        // 1. 플레이어와 피격 적의 스프라이트를 최상위 레이어로 임시 승격
        var raisedRenderers = new List<(SpriteRenderer sr, string origLayer, int origOrder)>();
        RaiseLayer(attacker, raisedRenderers);
        RaiseLayer(victim, raisedRenderers);

        // 2. 화면 전체 밝은 회색조 오버레이 활성화
        if (grayscaleOverlay != null)
        {
            grayscaleOverlay.gameObject.SetActive(true);
            grayscaleOverlay.color = flashColor;
        }

        // 3. 시간 일시 정지 (히트 스탑)
        Time.timeScale = 0.0f;

        // 실제 시간(Realtime) 기준으로 대기
        yield return new WaitForSecondsRealtime(duration);

        // 4. 복구
        Time.timeScale = 1.0f;

        if (grayscaleOverlay != null)
        {
            grayscaleOverlay.color = Color.clear;
            grayscaleOverlay.gameObject.SetActive(false);
        }

        // 스프라이트 렌더러 레이어 원복
        foreach (var item in raisedRenderers)
        {
            if (item.sr != null)
            {
                item.sr.sortingLayerName = item.origLayer;
                item.sr.sortingOrder = item.origOrder;
            }
        }

        hitStopRoutine = null;
    }

    private void RaiseLayer(GameObject target, List<(SpriteRenderer, string, int)> list)
    {
        if (target == null) return;
        var renderers = target.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
        {
            list.Add((sr, sr.sortingLayerName, sr.sortingOrder));
            sr.sortingLayerName = "Foreground"; // 또는 UI 바로 아래 레이어
            sr.sortingOrder += 100;
        }
    }
}