using UnityEngine;
using System.Collections;

public class ScriptOnCollider : MonoBehaviour
{
    private ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];

    void Start()
    {
    }
	
    void Update()
    {
	
    }

    // OnParticleCollision在Collider所属游戏对象脚本上执行，other为ParticleSystem所属游戏对象
    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps = other.GetComponent<ParticleSystem>();
        int safeLength = ps.GetSafeCollisionEventSize();
        print("safe length = " + safeLength.ToString());
        if(collisionEvents.Length < safeLength)
            collisionEvents = new ParticleCollisionEvent[safeLength];
        int num = ps.GetCollisionEvents(gameObject, collisionEvents);
        print("received collision event number = " + num.ToString());
        for(int i = 0; i < num; ++i) {
            ParticleCollisionEvent ev = collisionEvents [i];
            Vector3 pos = ev.intersection;
            if(ev.colliderComponent.tag == "Cube")
                print("hit cube at position : " + pos.Str());
            else if(ev.colliderComponent.tag == "Capsule")
                print("hit capsule at position : " + pos.Str());
            else if(ev.colliderComponent.tag == "Cylinder")
                print("hit cylinder at position : " + pos.Str());
        }
    }
}

public static class Vector3Ext
{
    public static string Str(this Vector3 v)
    {
        return string.Format("({0},{1},{2})", v.x, v.y, v.z);
    }
}
