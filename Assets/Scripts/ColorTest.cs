using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour
{
    [SerializeField]
    private List<SpriteRenderer> spriteRenderers;

    public float baseRed;
    public float baseGreen;
    public float baseBlue;
    public float stepRed;
    public float stepGreen;
    public float stepBlue;
    public int baseIter;

    // Update is called once per frame
    void Update()
    {
        var i = baseIter;
        foreach (var spriterender in spriteRenderers)
        {
            spriterender.color = calculateColor(i);
            i++;
        }
    }

    private Color calculateColor(int i)
    {
        return new Color(
            (((float)i * stepRed) + baseRed) % 1f,
            (((float)i * stepGreen) + baseGreen) % 1f,
            (((float)i * stepBlue) + baseBlue) % 1f);
    }
}
