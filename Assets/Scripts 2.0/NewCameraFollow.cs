using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraFollow : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField] GameObject objectToFollow;

    #endregion

    void FixedUpdate()
    {
		//Follow the object Along the X axis
        this.transform.position = new Vector3(objectToFollow.transform.position.x,
                                                        this.transform.position.y,
                                                        this.transform.position.z);
    }
}
