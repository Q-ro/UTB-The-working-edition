using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RoomPositionManager))]
public class ClickableObject : MonoBehaviour
{

    public typeOfObject typeOfObject; //The type of object this object is
    //public Transform[] wayPoint; //Used to place the object in the room
    public AudioSource mouseClick;
    private NewGameManager _newGameManager = null;// Store a reference to the instance of the game manager
    private SpriteRenderer _spriteRenderer; //Stores a reference to the srpite renderer

    // [SerializeField]
    // private int _currentRoom = 4;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start()
    {
        _newGameManager = NewGameManager.Instance;
        //_spriteRenderer.sprite = NewGameManager.Instance.GetObjectSprite(this.typeOfObject);

    }

    void OnMouseDown()
    {
        if (!_newGameManager.PhoneOut)
        {
            if (Input.GetMouseButtonDown(0) && _newGameManager.CanInteractWithMouse)
            {

                if (this.gameObject.tag == "HasEvent")
                {
                    mouseClick.Play();
                    _newGameManager.CallEvent(typeOfObject);
                }
                else if (this.gameObject.tag == "Phone")
                {
                    mouseClick.Play();
                    NewGameManager.Instance.ShowPhone();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_spriteRenderer.sprite == null)
        {
            _spriteRenderer.sprite = _newGameManager.GetObjectSprite(this.typeOfObject);
        }
    }

    private void OnMouseOver()
    {
        if (_newGameManager.CanInteractWithMouse && !_newGameManager.PhoneOut)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
