using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screeshake : MonoBehaviour {

    private float ShakeX;
    private float ShakeXSpeed = 0.8f;


    public void setShake(float someY)
    {
        ShakeX = someY;
    }

    void Update()
    {        
        Vector2 _newPosition = new Vector2(ShakeX, 0);
        if (ShakeX < 0)
        {
            ShakeX *= ShakeXSpeed;
        }
        ShakeX = -ShakeX;
        transform.Translate(_newPosition, Space.Self);
    }
}
