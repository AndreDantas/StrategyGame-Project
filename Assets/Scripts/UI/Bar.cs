using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Bar : MonoBehaviour
{

    [SerializeField]
    [Range(0, 1)]
    float fillAmount = 1f;
    [SerializeField]
    Image content;
    public Color barColor = new Color(255, 0, 0);
    public Image GetImage()
    {
        return content;
    }
    public void SetContent(Image img)
    {
        this.content = img;
    }
    public float MaxValue { get; set; }

    public float Value
    {

        set
        {
            fillAmount = Map(value, 0, MaxValue);
        }
    }

    void Start()
    {
        if (content)
            content.color = barColor;
    }

    private void OnValidate()
    {
        UpdateBar();
    }

    public void UpdateBar()
    {
        if (content.fillAmount != this.fillAmount)
            content.fillAmount = this.fillAmount;
    }

    float Map(float value, float inMin, float inMax)
    {


        return Mathf.InverseLerp(inMin, inMax, value);
    }
}
