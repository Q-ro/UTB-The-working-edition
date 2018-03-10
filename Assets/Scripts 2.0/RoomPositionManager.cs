using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomPositionManager : MonoBehaviour
{

    #region Editor Variables

    [SerializeField] private RoomPosition[] roomPosition;

    #endregion

    #region Private Variables
    [Serializable]
    public struct RoomPosition
    {
        public int roomNumber;
        public Transform positionInRoom;
    }
    // [SerializeField] private Dictionary<int, Transform> _roomPositions = new Dictionary<int, Transform>();
    [SerializeField] private Dictionary<int, float> _roomPositions = new Dictionary<int, float>();
    #endregion


    void Awake()
    {

        //A small hack to connect the variables from the editor with the internal diccionary to make indexing and sorting more easier
        foreach (var item in roomPosition)
        {
            if (item.positionInRoom != null)
            {
                // Transform position = item.positionInRoom;
                // _roomPositions.Add(item.roomNumber, position);
                _roomPositions.Add(item.roomNumber, item.positionInRoom.position.x);

            }
        }
    }

    /// <summary>
    /// Sets the position of the object for the given room number
    /// </summary>
    /// <param name="roomNumber"></param>
    public void UpdateRoomPosition(int roomNumber)
    {
        if (_roomPositions.ContainsKey(roomNumber))
        {
            this.gameObject.SetActive(true);

            //Move the object to the given Position in the X axis
            this.transform.position = new Vector3(_roomPositions[roomNumber],
                                                    this.transform.position.y,
                                                    this.transform.position.z);
            // this.transform.position = _roomPositions[roomNumber].position;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

}
