using UnityEngine;

public abstract class BaseLauncher : MonoBehaviour
{
    [SerializeField] protected float maxDeflectionAngle = 15f;

    protected GameObject target;

    protected Quaternion CalculateDeflection()
    {
        float deflectionHor = Random.Range(-maxDeflectionAngle, maxDeflectionAngle);
        float deflectionVert = Random.Range(-maxDeflectionAngle, maxDeflectionAngle);

        Vector3 euler = CalculateRotToTarget().eulerAngles;
        euler = new Vector3(euler.x + deflectionHor, euler.y + deflectionVert, euler.z);
        return Quaternion.Euler(euler);
    }

    private Quaternion CalculateRotToTarget()
    {
        if (target)
            return Quaternion.LookRotation(target.transform.position - transform.position);
        else
            return transform.rotation;
    }
}