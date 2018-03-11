using UnityEngine;
using System.Collections;

public class ScriptOnParticle : MonoBehaviour
{
    ParticleCollisionEvent[] pces = new ParticleCollisionEvent[16];

    void Start()
    {
    }
	
    void Update()
    {
    }

    void OnParticleCollision(GameObject other)
    {
        print("OnParticleCollision execute on ParticleSystem");
        print("Collider tag is " + other.tag);
        ParticleSystem ps = GetComponent<ParticleSystem>();
        int safeLen = ps.GetSafeCollisionEventSize();
        if(pces.Length < safeLen)
            pces = new ParticleCollisionEvent[safeLen];
        int num = ps.GetCollisionEvents(other, pces);
        print("Collision event num = " + num.ToString());
    }
}
