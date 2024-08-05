using UnityEngine;

public class NpcTakeoff : MonoBehaviour
{
    private float targetVerticalSpeed, currVerticalSpeed;
    private NpcAir NpcAir;

    private float VerticalSpeed => NpcAir.VerticalSpeed;
    private float Acceleration => NpcAir.Acceleration;
    private float MinHeight => NpcAir.MinHeight;
    private Translation translation => NpcAir.Translation;

    public bool EndOfTakeoff => transform.position.y > MinHeight && currVerticalSpeed == 0f;

    private void Start()
    {
        NpcAir = GetComponent<NpcAir>();
    }

    public void Move() => VerticalTranslate();

    private void VerticalTranslate()
    {
        targetVerticalSpeed = transform.position.y <= MinHeight ? VerticalSpeed : 0f;
        currVerticalSpeed = (Mathf.Abs(currVerticalSpeed - targetVerticalSpeed) < Acceleration && targetVerticalSpeed == 0f) ? 
            0f : Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        translation.SetVerticalTranslation(currVerticalSpeed);
    }
}
