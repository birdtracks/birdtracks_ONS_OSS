using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpriteSheetRenderer : MonoBehaviour, INotificationReceiver
{
    public Sprite[] spriteFrames;
    public float framesPerSecond = 10f;

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    private bool _isActive = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the GameObject.");
        }
    }

    void Update()
    {
        if (!_isActive || spriteRenderer == null || spriteFrames == null || spriteFrames.Length == 0) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / framesPerSecond)
        {
            currentFrame = (currentFrame + 1) % spriteFrames.Length;
            spriteRenderer.sprite = spriteFrames[currentFrame];
            frameTimer = 0f;
        }
    }

    public void Play()
    {
        _isActive = true;
    }

    public void Stop()
    {
        _isActive = false;
    }

    public void SetFrames(Sprite[] sprites)
    {
        _isActive = false;

        spriteFrames = sprites;
        currentFrame = 0;
        frameTimer = 0f;

        if (spriteRenderer != null && spriteFrames != null && spriteFrames.Length > 0)
        {
            spriteRenderer.sprite = spriteFrames[0];
        }
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if(!_isActive)
            Play();
        else
        {
            Stop();
        }
    }
}