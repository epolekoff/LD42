using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItem : MonoBehaviour {

    protected const float MinScale = 1f;
    protected const float MaxScale = 100f;
    public float Scale { get { return transform.localScale.x; } }
    public float WorldScale { get { return transform.lossyScale.x; } }
    public float ScaleRatio { get { return (Scale - MinScale) / (MaxScale - MinScale); } }

    private const float SelectionOutlineSize = 0.1f;
    private Color DeselectedColor = new Color(1, 1, 1, 1);
    protected bool m_isSelected;

    /// <summary>
    /// update
    /// </summary>
    void Update()
    {
        if (!m_isSelected)
        {
            Deselect();
        }
        else
        {
            m_isSelected = false;
        }
    }

    /// <summary>
    /// Get the largest size of the bounding box.
    /// </summary>
    /// <returns></returns>
    public float GetMaxBounds()
    {
        Vector3 bounds = GetComponent<Renderer>().bounds.size * WorldScale;
        return Mathf.Max(Mathf.Max(bounds.x, bounds.y), bounds.z);
    }

    /// <summary>
    /// Show outline in color.
    /// </summary>
    /// <param name="canSelect"></param>
	public void SetSelectionVisual(bool canSelect)
    {
        m_isSelected = true;
        GetComponent<Renderer>().material.SetFloat("_OutlineWidth", SelectionOutlineSize);
        GetComponent<Renderer>().material.SetColor("_OutlineColor",
            canSelect ?
            GameManager.Instance.SelectionColorPositive :
            GameManager.Instance.SelectionColorNegative);
    }

    /// <summary>
    /// Hide outline
    /// </summary>
    private void Deselect()
    {
        GetComponent<Renderer>().material.SetFloat("_OutlineWidth", 0);
        GetComponent<Renderer>().material.SetColor("_OutlineColor", DeselectedColor);
    }
}
