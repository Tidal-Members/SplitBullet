using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public enum HitType
    {
        HitScan,
        Projectile
    }
    public int health = 100;
}
