using UnityEngine;

public class Flag : MonoBehaviour, IUnitTarget
{
    public Transform Transform => transform;

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
