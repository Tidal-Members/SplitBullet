using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
public class WeaponBase : MonoBehaviour
{
    public enum HitType
    {
        HitScan,
        Projectile
    }
    public string localizationKey = "#weapon.base";
    public int health = 100;
    public HitType type = HitType.HitScan;
    public SharedEnums.HoldType holdType = SharedEnums.HoldType.OneHanded;
    public int damage = 10;
    public int hitScanRange = -1;
    public Sprite bucketIcon;
    public Sprite selectedIcon;
    public Sprite handSprite;
    public Sprite[] fireAnim;
    public void BaseFire()
    {
        switch(type)
        {
            case HitType.HitScan:
                break;
            default:
                Debug.Log("Unimplemented HitType fired!");
                break;
        }
    }
    public virtual void Fire()
    {

    }
    public void BaseEquip()
    {
        GetComponentInParent<Player>().holdState = holdType;
    }
    public virtual void Equip()
    {

    }
    public virtual void Holster()
    {

    }
}
