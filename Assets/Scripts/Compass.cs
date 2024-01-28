using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform playerLittleGuy;
    public Transform arrow;
    private Vector3 target;

    public void UpdateTarget(Vector3 target)
    {
        this.target = target;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 BtoA = target - playerLittleGuy.position;
        Quaternion spinBtoA = Quaternion.LookRotation(BtoA);
        Quaternion spinCCW90 = Quaternion.Euler(0, -90, 0);
        float rot = (spinCCW90 * spinBtoA).eulerAngles.y;
        arrow.transform.rotation = Quaternion.Euler(0, 0, -rot);
    }
}
