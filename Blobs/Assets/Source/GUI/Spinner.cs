using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using Legend;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Spinner : MonoBehaviour
{
    public float Duration = 2.5f;
    public string FormatText;

    Action onComplete;
    long start = 0;
    long target = 1000;
    float startTime;
    Text targetLabel;
    bool isRunning;
    long currentValue;

    public void SpinTo(long targetValue, long startValue = 0, Action onComplete = null)
    {
        if (startValue == -1)
            startValue = currentValue;

        this.start = startValue;
        this.target = targetValue;
        this.onComplete = onComplete;
        //print("SpinTo");

        //var tween = GetComponent<TweenAlpha>();
        //if (tween != null)
        //{
        //    tween.enabled = true;
        //    tween.PlayForward();
        //}

        targetLabel = GetComponent<Text>();
        targetLabel.text = String.Format(FormatText, startValue);
        targetLabel.enabled = true;

        //this.Delay(() =>
        //{
            //print("spin start");

            startTime = Time.unscaledTime;
            isRunning = true;
            if (GetComponent<AudioSource>() != null)
                GetComponent<AudioSource>().Play();
        //}, 0.7f);
    }

    //void Awake()
    //{
    //    //print("Awake");
    //    //var tween = GetComponent<TweenAlpha>();
    //    //if (tween != null)
    //    //    tween.enabled = false;

    //    //startTime = Time.time;
    //    //FormatText = targetLabel.text;
    //    //targetLabel.enabled = false;
    //}

    void Update()
    {
        if (isRunning)
        {
            var elapsedPercent = Math.Min((Time.unscaledTime - startTime) / Duration, 1f);
            currentValue = (long)(elapsedPercent * (target - start)) + start;
            targetLabel.text = String.Format(FormatText, currentValue);

            if (elapsedPercent >= 1)
            {
                isRunning = false;

                if (GetComponent<ParticleSystem>() != null)
                    GetComponent<ParticleSystem>().Play();

                foreach (var particle in GetComponentsInChildren<ParticleSystem>())
                    particle.Play();

                if (GetComponent<AudioSource>() != null)
                    GetComponent<AudioSource>().Stop();

                if (onComplete != null)
                    onComplete();

                //Destroy(this, 2);
            }
        }
    }
}
