﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    [SerializeField] private Sprite normalNoteSprite;
    [SerializeField] private Sprite optionalNoteSprite;
    
    private new SpriteRenderer renderer;
    private bool visible;
    private bool touchOptional = false;

    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            var color = renderer.color;
            color.a = visible ? 1 : 0;
            renderer.color = color;
        }
    }
    public bool TouchOptional
    {
        get => touchOptional;
        set
        {
            touchOptional = value;
            renderer.sprite = touchOptional ? optionalNoteSprite : normalNoteSprite;
        }
    }
    public bool Played { get; set; }
    public int MidiKey { get; set; } = 72;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameController.Instance.GameStarted.Value && !GameController.Instance.GameOver.Value)
        {
            transform.Translate(Vector3.down * (GameController.Instance.noteSpeed * Time.deltaTime));
        }
    }

    private void Play()
    {
        if (Played) return;
        Played = true;

        GameController.Instance.Score.Value++;
        var color = renderer.color;
        color.a = 0.5f;
        renderer.color = color;

        // TODO: Use Note Model for multiple note sounds
        GameController.Instance.Pitcher.PlayNote(MidiKey);
    }

    private void Miss()
    {
        StartCoroutine(GameController.Instance.EndGame());
        GameController.Instance.Pitcher.PlayNote(60);
        renderer.color = Color.red;
    }

    public void PlayTouch(bool isHold)
    {
        if (!GameController.Instance.GameStarted.Value || GameController.Instance.GameOver.Value) return;
        if (TouchOptional)
        {
            if (Visible) Play();
        }
        else if (!isHold)
        {
            if (Visible) Play();
            else Miss();
        }
    }

    public void OutOfScreen()
    {
        if (Visible && !Played && !TouchOptional) Miss();
    }
}