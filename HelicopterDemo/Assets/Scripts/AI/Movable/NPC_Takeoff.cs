using UnityEngine;

public class NPC_Takeoff : MonoBehaviour
{
    private float targetVerticalSpeed, currVerticalSpeed;
    private NPC_Mover NPC_Mover;

    private float VerticalSpeed => NPC_Mover.VerticalSpeed;
    private float Acceleration => NPC_Mover.Acceleration;
    private float MinHeight => NPC_Mover.MinHeight;
    private Translation Translation => NPC_Mover.Translation;

    private void Start()
    {
        NPC_Mover = GetComponent<NPC_Mover>();
    }

    public void Move() => VerticalTranslate();

    public bool Check_ToPatrolling() => transform.position.y > MinHeight && currVerticalSpeed == 0f;

    private void VerticalTranslate()
    {
        targetVerticalSpeed = transform.position.y <= MinHeight ? VerticalSpeed : 0f;
        currVerticalSpeed = (Mathf.Abs(currVerticalSpeed - targetVerticalSpeed) < Acceleration && targetVerticalSpeed == 0f) ? 
            0f : Mathf.Lerp(currVerticalSpeed, targetVerticalSpeed, Acceleration * Time.deltaTime);
        Translation.SetVerticalTranslation(currVerticalSpeed);
    }
}
