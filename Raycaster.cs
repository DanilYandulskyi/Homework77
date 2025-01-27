using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public Vector3 Cast(Vector3 origin)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(origin);

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
