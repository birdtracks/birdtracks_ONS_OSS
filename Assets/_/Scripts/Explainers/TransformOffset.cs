using UnityEngine;

public sealed class TransformOffset : MonoBehaviour
{
    [SerializeField] private Transform m_Overrider;
    [SerializeField] private Transform m_Transform;


    public void ApplyTransformOffset()
    {
        m_Overrider.position = m_Transform.position;
        m_Overrider.rotation = m_Transform.rotation;
        m_Overrider.localScale = m_Overrider.localScale*m_Transform.localScale.x;
    }
}