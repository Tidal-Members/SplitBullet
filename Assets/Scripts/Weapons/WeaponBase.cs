using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
public class WeaponBase
{
    public enum HitType
    {
        HitScan,
        Projectile
    }
    public string localizationKey = "#weapon.base";
    public HitType type = HitType.HitScan;
    public SharedEnums.HoldType holdType = SharedEnums.HoldType.OneHanded;
    public int damage = 10;
    public float hitScanRange = 50f;
    public float hitForce = 100f;
    public float weaponCooldown = 0.5f;
    public Sprite bucketIcon = Resources.Load<Sprite>("Sprites/BucketIcons/pistol");
    public Sprite selectedIcon = Resources.Load<Sprite>("Sprites/SelectedIcons/pistol");
    public Sprite handSprite = Resources.Load<Sprite>("Sprites/Weapons/glock");
    public Color lightColor = Color.white;
    private Player player;
    public WeaponBase(Player ply)
    {
        player = ply;
    }
    public void BaseFire()
    {
        if(player.cooldown)
            return;
        switch(type)
        {
            case HitType.HitScan:
                Vector3 rayOrigin = player.lookScript.weaponLight.transform.position;
                RaycastHit hit;
                // Check if our raycast has hit anything
                if (Physics.Raycast (rayOrigin, player.lookScript.weaponLight.transform.forward, out hit, hitScanRange))
                {
                    // Get a reference to a health script attached to the collider we hit
                    Entity health = hit.collider.GetComponent<Entity>();

                    // If there was a health script attached
                    if (health != null)
                    {
                        // Call the damage function of that script, passing in our gunDamage variable
                        health.Damage (damage);
                    }

                    // Check if the object we hit has a rigidbody attached
                    if (hit.rigidbody != null)
                    {
                        // Add force to the rigidbody we hit, in the direction from which it was hit
                        hit.rigidbody.AddForce (-hit.normal * hitForce);
                    }
                }
                break;
            default:
                Debug.Log("Unimplemented HitType fired!");
                break;
        }
    }
    public virtual void Fire()
    {
        BaseFire();
    }
    public void BaseEquip()
    {
        player.holdState = holdType;
    }
    public virtual void Equip()
    {
        BaseEquip();
    }
    public virtual void Holster()
    {

    }
}
