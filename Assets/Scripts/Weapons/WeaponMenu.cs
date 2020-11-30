﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WeaponMenu : MonoBehaviour
{
    public Transform menu;
    private bool menuActive = false;
    private float fixedDeltaTime;
    public Image img;
    void Awake()
    {
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }
    // Start is called before the first frame update
    void Start()
    {
        img.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire2")){
            Time.timeScale = 0.25f;
            FadeImage(false);
            menuActive = true;
        } else if(Input.GetButtonUp("Fire2")){
            Time.timeScale = 1.0f;
            FadeImage(true);
            menuActive = false;
        }
    }
    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}