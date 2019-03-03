using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(30f * Vector3.forward * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        UnitCtrl unit = other.gameObject.GetComponent<UnitCtrl>();
        if (unit != null)
        {
            Gamef.Damage(5, DamageType.unset, null, unit.unitInfo);
        }
        Destroy(gameObject);
    }
}


//public class Test
//{
//    public enum SpellType
//    {

//    };
//    public void Spell(SpellType type, params object[] paramList)
//    {

//    }
//}