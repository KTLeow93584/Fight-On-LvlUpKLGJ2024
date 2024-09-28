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
    private float regenSpeed = 0.0f;
    
    [SerializeField]
    private bool startMax = true;
    // -------------------------------------
    public TextMeshProUGUI indicatorText = null;
    public Slider uiComponent = null;
    // -------------------------------------
    [SerializeField]
    private float currentValue;

    private float newThresholdValue;

    private float minThreshold = 0.0f;
    // -------------------------------------
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ResetToMax();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateRegen();
        float rateOfChange = (smoothSpeed * Time.deltaTime) * (Mathf.Clamp(newThresholdValue - currentValue, -1.0f, 1.0f)) ;
        
        if (Mathf.Abs(newThresholdValue - currentValue) < Mathf.Abs(rateOfChange))
            currentValue = newThresholdValue;
        else
            currentValue += rateOfChange;

        if (indicatorText)
        {
            // Float Precision Error To Prevent.
            float stageAt = (currentValue / maxValue) * stageCount;
            indicatorText.text = (float)stageCount - stageAt < 0.01 ? stageCount.ToString() : Mathf.Floor(stageAt).ToString();
        }

        if (uiComponent)
            uiComponent.value = currentValue/maxValue;
    }
    // -------------------------------------
    private void UpdateRegen()
    {
        float regenAmount = regenSpeed * Time.deltaTime;

        newThresholdValue += regenAmount;
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
    public void SetUIComponent(Transform target)
    {
        uiComponent = target.GetComponent<Slider>();
    }

    public void SetIndicatorText(Transform target)
    {
        indicatorText = target.GetComponent<TextMeshProUGUI>();
    }
    // -------------------------------------
}
