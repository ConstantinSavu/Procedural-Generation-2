using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    internal ObjectPool Parent;

    public virtual void OnDisable()
    {
        Parent.ReturnObjectToPool(this);
    }
}
