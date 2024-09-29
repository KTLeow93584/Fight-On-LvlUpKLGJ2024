using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

 public abstract class Stat_Component : MonoBehaviour
{
    // -------------------------------------
    [SerializeField]
    private float maxValue;

    [SerializeField]
    private int stageCount = 1;

    [SerializeField]
    private float smoothSpeed = 10.0f;

    [SerializeField]
    private float smoothDelay = 1.5f;
    private float smoothTimer = 0.0f;

    [SerializeField]
    private float regenSpeed = 0.0f;

    [SerializeField]
    private bool regenTilThreshold = false;

    [SerializeField]
    private bool startMax = true;
    // -------------------------------------
    public bool isRegenDisabled = false;

    public TextMeshProUGUI indicatorText = null;
    public Slider smoothTransitionUIIndicator = null;
    public Slider noTransitionUIIndicator = null;
    // -------------------------------------
    [SerializeField]
    private float currentValue;

    private float newThresholdValue;

    private float minThreshold = 0.0f;
    // -------------------------------------
    // Start is called before the first frame update
    protected virtual void Start()
    {
        smoothTimer = 0.0f;
        ResetToMax();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateRegen();

        if (currentValue != newThresholdValue)
            smoothTimer += Time.deltaTime;
        
        if (smoothTimer >= smoothDelay)
        {
            float rateOfChange = (smoothSpeed * Time.deltaTime) * (Mathf.Clamp(newThresholdValue - currentValue, -1.0f, 1.0f));

            if (Mathf.Abs(newThresholdValue - currentValue) < 0.01f)
            {
                smoothTimer = 0.0f;
                currentValue = newThresholdValue;
            }
            else
                currentValue += rateOfChange;

            if (indicatorText)
            {
                // Float Precision Error To Prevent.
                float stageAt = (currentValue / maxValue) * stageCount;
                indicatorText.text = (float)stageCount - stageAt < 0.01 ? stageCount.ToString() : Mathf.Floor(stageAt).ToString();
            }
        }

        if (smoothTransitionUIIndicator)
            smoothTransitionUIIndicator.value = currentValue / maxValue;

        if (noTransitionUIIndicator)
            noTransitionUIIndicator.value = newThresholdValue / maxValue;
    }
    // -------------------------------------
    private void UpdateRegen()
    {
        if (!isRegenDisabled)
            return;

        float regenAmount = regenSpeed * Time.deltaTime;

        newThresholdValue += regenAmount;

        if (regenTilThreshold)
        {
            if (newThresholdValue >= currentValue)
                newThresholdValue = currentValue;
        }
        CheckThresholdCap();
    }

    private void CheckThresholdCap()
    {
        if (newThresholdValue < minThreshold)
            newThresholdValue = minThreshold;
        else if (newThresholdValue > maxValue)
            newThresholdValue = maxValue;
    }
    // -------------------------------------
    public virtual void AdjustToValue(float value)
    {
        newThresholdValue = value;
        CheckThresholdCap();
    }

    public virtual void AddValue(float additive)
    {
        newThresholdValue += additive;
        CheckThresholdCap();
    }

    protected virtual void ResetToMax()
    {
        currentValue = startMax ? maxValue : 0;
        newThresholdValue = currentValue;
    }
    // -------------------------------------
    public void SetUIComponent(Transform smoothTarget = null, Transform noSmoothTarget = null)
    {
        if (smoothTarget)
            smoothTransitionUIIndicator = smoothTarget.GetComponent<Slider>();

        if (noSmoothTarget)
            noTransitionUIIndicator = noSmoothTarget.GetComponent<Slider>();
    }

    public void SetIndicatorText(Transform target)
    {
        indicatorText = target.GetComponent<TextMeshProUGUI>();
    }
    // -------------------------------------
    public float GetCurrentValue()
    {
        return newThresholdValue;
    }
    // -------------------------------------
}
