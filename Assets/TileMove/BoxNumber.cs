// BoxNumber.cs
using TMPro;
using UnityEngine;

public class BoxNumber : MonoBehaviour
{
    [Range(0,9)] [SerializeField] int number = 0;
    [SerializeField] TextMeshPro label;

    public int Value => number;

    void Awake()
    {
        if (!label) label = GetComponentInChildren<TextMeshPro>();
        Refresh();
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!label) label = GetComponentInChildren<TextMeshPro>();
        Refresh();
    }
#endif
    public void SetValue(int v) { number = Mathf.Clamp(v,0,9); Refresh(); }

    void Refresh()
    {
        if (label) label.text = number.ToString();
    }
}
