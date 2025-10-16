using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SpriteSheetAnimator : MonoBehaviour, INotificationReceiver
{
    public Sprite[] spriteFrames;
    public float framesPerSecond = 10f;

    private Image spriteImage;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    private bool _isActive = false;

    void Start()
    {
        spriteImage = GetComponent<Image>();
    }

    void Update()
    {
        if(!_isActive) return;
        
        frameTimer += Time.deltaTime;

        if (frameTimer >= 1f / framesPerSecond)
        {
            currentFrame = (currentFrame + 1) % spriteFrames.Length;
            spriteImage.sprite = spriteFrames[currentFrame];
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
        
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        Play();
    }
}
