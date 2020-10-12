using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    private Player player;
    public WeaponBase activeWeapon;
    public Light weaponLight;
    public SpriteRenderer weaponSprite;
    // Start is called before the first frame update
    void Awake()
    {
        player = GetComponentInParent<Player>();
        activeWeapon = new WeaponBase(player);
        weaponLight = transform.Find("Light").GetComponent<Light>();
        weaponSprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        weaponLight.color = activeWeapon.lightColor;
        weaponSprite.sprite = activeWeapon.handSprite;
    }
}
