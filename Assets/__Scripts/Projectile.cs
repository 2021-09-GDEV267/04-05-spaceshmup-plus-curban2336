using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;
    private string _direction;

    // This public property masks the field _type and takes action when it is set
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

    public string direction
    {
        get
        {
            return (_direction);
        }
        set
        {
            SetDirection(value);
        }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }
        else if (bndCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(string newDirection)
    {
        _direction = newDirection;
    }

    ///<summary>
    /// Sets the _type private field and colors this projectile to match the
    /// WeaponDefinition.
    /// </summary>
    /// <param name="eType">The WeaponType to use.</param>
    public void SetType(WeaponType eType)
    {
        // Set the _type
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }
}
