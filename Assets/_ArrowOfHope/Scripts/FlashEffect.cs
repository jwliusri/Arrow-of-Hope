using System.Collections;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
    }

    public void Trigger(float time = 0.25f)
    {
        Trigger(Color.white, time);
    }

    public void Trigger(Color color, float time = 0.25f)
    {
        StartCoroutine(Flash(time, color));
    }

    IEnumerator Flash(float time,  Color color)
    {
        _material.SetColor("_FlashColor", color);
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / time));
            _material.SetFloat("_FlashAmount", currentFlashAmount);
            yield return null;
        }
    }

}
