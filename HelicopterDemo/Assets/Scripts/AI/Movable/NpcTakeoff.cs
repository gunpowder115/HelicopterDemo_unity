using UnityEngine;

public class NpcTakeoff : MonoBehaviour
{
    private float targetVerticalSpeed, currVerticalSpeed;
    private Npc npc;
    private NpcAir npcAir;

    private float VerticalSpeed => npcAir.VerticalSpeed;
    private float Acceleration => npc.Acceleration;
    private float MinHeight => npcAir.MinHeight;
    private Translation translation => npc.Translation;

    public bool EndOfTakeoff => transform.position.y > MinHeight && currVerticalSpeed == 0f;

    private void Start()
    {
        npc = GetComponent<Npc>();
        npcAir = GetComponent<NpcAir>();
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
