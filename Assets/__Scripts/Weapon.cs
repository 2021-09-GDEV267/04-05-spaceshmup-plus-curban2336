using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a shield power-up.
/// Items marked [NI] below are Not Implemented in the IGDPD book.
/// </summary>
public enum WeaponType
{
    none, // The default / no weapons
    blaster, // A simple blaster
    spread, // Two shots simultaneously
    phaser, // [NI] Shots that move in waves
    missile, // [NI] Homing missiles
    laser, // [NI] Damage over time
    shield // Raise shieldLevel
}

/// <summary>
/// The WeaponDefinition class allows you to set the properties
/// of a specific weapon in the Inspector. The Main class has
/// an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // Letter to show on the power-up
    public Color color = Color.white; // Color of Collar & power-up
    public GameObject projectilePrefab; // Prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0; // Amount of damage caused
    public float continuousDamage = 0; // Damage per second (Laser)
    public float delayBetweenShots = 0;
    public float velocity = 20; // Speed of projectiles
}
public class Weapon : MonoBehaviour {
    static public Transform PROJECTILE_ANCHOR;
    private GameObject parent;
    private GameObject[] phaserList;

    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // Time last shot was fired
    private Renderer collarRend;
    private Vector3 velo;
    public float timer;

    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Call SetType() for the default _type of WeaponType.none
        SetType(_type);

        // Dynamically create an anchor for all Projectiles
        if(PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Find the fireDelegate of the root GameObject
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
        if (rootGO.GetComponent<Enemy>() != null)
        {
            rootGO.GetComponent<Enemy>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // You can fire immediately after _type is set.
    }

    public void Fire()
    {
        //Debug.Log("Weapon Fired:" + gameObject.name);
        // If this.gameObject is inactive, return
        if (!gameObject.activeInHierarchy) return;
        // If it hasn't been enough time between shots, return
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                if(p.tag == "ProjectileEnemy")
                {
                    p.rigid.velocity = vel;
                }
                else
                {
                    p.rigid.velocity = vel;
                }
                break;

            case WeaponType.spread:
                p = MakeProjectile(); // Make middle Projectile
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Make right Projectile
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make secondary right Projectile
                p.transform.rotation = Quaternion.AngleAxis(15, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile(); // Make secondary left Projectile
                p.transform.rotation = Quaternion.AngleAxis(-15, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

            case WeaponType.phaser:
                //parent = Instantiate<GameObject>(def.projectilePrefab);
                //parent.tag = "Untagged";
                //Rigidbody rb = parent.GetComponent<Rigidbody>();
                //MeshRenderer mesh = parent.GetComponent<MeshRenderer>();
                //parent.transform.position = collar.transform.position;
                //parent.transform.SetParent(PROJECTILE_ANCHOR, true);
                //rb.velocity = vel;
                //mesh.enabled = false;
                velo = vel;
                p = MakeProjectile(); // Make right Projectile;
                p.direction = "Right";
                //vel.y -= 1;
                p.transform.position = new Vector3(p.transform.position.x+1, p.transform.position.y, p.transform.position.z);
                p.rigid.velocity = vel;
                p = MakeProjectile(); // Make left Projectile
                p.direction = "Left";
                p.transform.position = new Vector3(p.transform.position.x - 1, p.transform.position.y, p.transform.position.z);
                p.rigid.velocity = vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if(transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        if (type == WeaponType.phaser)
        {
            //Rigidbody rb = go.GetComponent<Rigidbody>();
            //rb.constraints = RigidbodyConstraints.FreezePositionX;
            //go.transform.SetParent(parent.transform, true);
            go.transform.SetParent(PROJECTILE_ANCHOR, true);
        }
        else
        {
            go.transform.SetParent(PROJECTILE_ANCHOR, true);
        }
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return p;
    }

    public void Update()
    {
        if (Time.time % 1.5 >= 1)
        {
            timer = 1;
        }
        else if (Time.time % 1.5 < 1 && Time.time % 1.5 >= 0)
        {
            timer = -1;
        }

        if (type == WeaponType.phaser)
        {
            GameObject rootGO = transform.root.gameObject;
            if(rootGO.tag == "Hero")
            {
                phaserList = GameObject.FindGameObjectsWithTag("ProjectileHero");
            }
            else if(rootGO.tag == "Enemy")
            {
                phaserList = GameObject.FindGameObjectsWithTag("ProjectileEnemy");
            }
            if (phaserList != null)
            {
                foreach (GameObject element in phaserList)
                {
                    Vector3 tempVel = element.GetComponent<Rigidbody>().velocity;
                    float theta = Mathf.PI * 2 * (lastShotTime - Time.time) / 2;
                    float sin = Mathf.Sin(theta);
                    if(element.GetComponent<Projectile>().direction == "Left")
                    {
                        tempVel.x = (10 * sin) * timer;
                    }
                    else if (element.GetComponent<Projectile>().direction == "Right")
                    {
                        tempVel.x = -((10 * sin) * timer);
                    }
                    
                    element.GetComponent<Rigidbody>().velocity = tempVel;
                }
            }
        }
    }
}
