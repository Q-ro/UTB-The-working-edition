using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A Enum to keep the type of object in the room we are referring to more readable
public enum typeOfObject { Pills, Bed, PhoneCall, Breathing, Computer, Calendar, Diary, Book, CorkBoard };

[Serializable]
public struct SpriteForObject
{
    public typeOfObject typeOfObject;
    public Sprite sprite;
}

//Static game object that manages the states of the game
[RequireComponent(typeof(NewPhone))]
public class NewGameManager : MonoBehaviour
{
    private static NewGameManager _instance = null; //Static reference to the game manager game object
    public static NewGameManager Instance
    {
        get
        {
            return _instance;
        }
    }


    #region Editor Variables

    [SerializeField]
    private GameObject player; //Stores a reference to the player object

    [SerializeField]
    private GameObject phone; //Stores a reference to the phone object

    // [SerializeField]
    // private GameObject canvasBackgroundImage;

    [SerializeField]
    private GameObject DeadGradient;

    [SerializeField]
    private Camera mainCamera; //A reference to the main camera

    //A hack to store references to the clickable obejects sprites, used to allow for 1 prefab to controll all the clickable objects interations
    [SerializeField]
    private SpriteForObject[] objectSprites;

    //The sequence to be performed in order to bead the game
    [SerializeField]
    private typeOfObject[] room2Sequence;
    [SerializeField]
    private typeOfObject[] room3Sequence;
    [SerializeField]
    private typeOfObject[] room4Sequence;
    [SerializeField]
    private typeOfObject[] room5Sequence;

    [SerializeField]
    private int numberOfPlayableRooms = 4;// how many playable rooms are there
    [SerializeField]
    private int maxPNumberOfPlayerFailures = 5; //The maximum number of failures a player can commit per room

    [SerializeField]
    private int currentRoom = 4; //keeps track of the current room

    #endregion

    #region Private Variables

    private int _currentSequenceIndex = 0; //Keeps track of the current object to be clicked in the sequence
    private bool _canMove = true;
    private bool _canInteractWithMouse = true;
    private bool _playerIsSpamming = false;
    private bool _playerLastActionFailed = false;
    private bool _roomTransition = false;
    private int _numberOfPlayerFailures = 0; //Keeps track of the amount of times the player has failed
    private typeOfObject _lastClickedObject; //Keeps track of the last clicked object
    private Dictionary<typeOfObject, Sprite> _objectSprite = new Dictionary<typeOfObject, Sprite>();
    //public Sprite[] objectSprites;
    private RoomPositionManager[] _roomObjects;
    private bool _phoneOut;

    private int _numberOfPills = 0; //Keeps track of the number of fails the player has made

    #endregion

    #region Property Accesors

    public bool PhoneOut
    {
        get { return _phoneOut; }
        // set { this._phoneOut = value; }
    }
    public bool CanInteractWithMouse
    {
        get
        {
            return _canInteractWithMouse;
        }
    }
    public int CurrentRoom
    {
        get
        {
            return currentRoom;
        }
    }

    public typeOfObject LastClickedObject
    {
        get
        {
            return _lastClickedObject;
        }
    }

    #endregion



    #region Unity Methods

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(_instance);
        }
        _instance = this;
        // DontDestroyOnLoad(gameObject);


    }

    // Use this for initialization
    void Start()
    {
        // NewTextManager.Instance = NewTextManager.Instance;

        _roomObjects = FindObjectsOfType(type: typeof(RoomPositionManager)) as RoomPositionManager[];

        //Populate the real dictionary that matches typeOfObject to Sprite resources
        foreach (var row in objectSprites)
        {
            _objectSprite.Add(row.typeOfObject, row.sprite);
        }

        if (NewTextManager.Instance != null)
        {
            if (currentRoom == 4)
                NewTextManager.Instance.CallTextBox(typeOfDialog.Intro, 0, true, Color.black);
        }

        MoveRoomElements();

    }

    // Update is called once per frame
    void Update()
    {
        if (!this._canMove)
        {
            player.GetComponent<PlayerController>().FreezePlayer();
        }
        else
        {
            player.GetComponent<PlayerController>().UnfreezePlayer();
        }

        if (currentRoom == 0 && !NewTextManager.Instance.IsTextboxActive() && !_roomTransition)
        {
            player.GetComponent<PlayerController>().PlayerEndAnimation();
        }
    }


    #endregion

    #region Clickable Objects

    //The game is over when the player "overdoses"
    void GameOver()
    {
        //Should call the game over scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
    }

    //Returns the right Sprite to render by the clickable object by typeOfObject
    public Sprite GetObjectSprite(typeOfObject objectType)
    {
        return _objectSprite[objectType];
    }

    //Display the phone canvas on screen
    public void ShowPhone()
    {
        //Keeps track of wheter the phone is out or not
        if (!_phoneOut)
        {
            _phoneOut = true;
            _canMove = false;
            phone.GetComponent<NewPhone>().ShowPhone();
        }
    }

    public void HidePhone(bool isApp)
    {
        //Keeps track of wheter the phone is out or not
        if (_phoneOut)
        {
            _phoneOut = false;
            if (!isApp)
                _canMove = true;
        }
    }

    public void CallEvent(typeOfObject objectType)
    {

        typeOfObject[] currentRoomSequence = GetCurrentRoomSequence();
        _lastClickedObject = objectType;


        // Debug.Log("Object : " + _lastClickedObject + " Room : " + currentRoom + " Sequence Index :" + _currentSequenceIndex);

        if (_lastClickedObject == currentRoomSequence[_currentSequenceIndex])
        {


            //We made it to the end of the sequence, so move to the next room
            if (_currentSequenceIndex + 1 >= currentRoomSequence.Length)
            {
                //We are moving the player toward room 1 A.K.A. freedom
                FreezePlayerActions();
                NewTextManager.Instance.CallTextBox(typeOfDialog.Room, _currentSequenceIndex, true, Color.white);
                RoomChange(true);
            }
            else
            {
                //Debug.Log("Success" + " " + currentSequenceIndex);
                FreezePlayerActions();
                NewTextManager.Instance.CallTextBox(typeOfDialog.Room, _currentSequenceIndex);
                _currentSequenceIndex++;
            }
        }
        else
        {
            _numberOfPlayerFailures++;

            if (_lastClickedObject == typeOfObject.Pills)
            {
                _numberOfPills++;
            }

            if (_numberOfPills > 3)
            {
                GameOver();
            }

            //Fail the player
            //Screen shake
            mainCamera.GetComponent<NewScreenShake>().SetShake();
            FreezePlayerActions();
            NewTextManager.Instance.CallTextBox(typeOfDialog.Failure, _currentSequenceIndex);

            if (_numberOfPlayerFailures > maxPNumberOfPlayerFailures)
            {
                //TODO: Add the screen transitions
                FreezePlayerActions();
                NewTextManager.Instance.CallTextBox(typeOfDialog.GoBack, _currentSequenceIndex, true, Color.black);
                RoomChange(false);
            }
        }
    }

    private typeOfObject[] GetCurrentRoomSequence()
    {
        switch (CurrentRoom)
        {
            case (4):
                return room5Sequence;
            case (3):
                return room4Sequence;
            case (2):
                return room3Sequence;
            case (1):
                return room2Sequence;
            default:
                return room5Sequence;
        }

        //return room5Sequence;
    }
    private void RoomChange(bool isUp)
    {
        _roomTransition = true;
        currentRoom = isUp ? Mathf.Clamp(currentRoom - 1, 0, numberOfPlayableRooms) : Mathf.Clamp(currentRoom + 1, 0, numberOfPlayableRooms);
        _currentSequenceIndex = 0;
        _numberOfPlayerFailures = 0;

        if (isUp)
        {
            player.GetComponent<PlayerController>().AnxietyDown();
        }
        else
        {
            player.GetComponent<PlayerController>().AnxietyUp();
        }


        if (currentRoom == 0)
        {
            FreezePlayerActions();
            DeadGradient.SetActive(false);
            // _player.GetComponent<PlayerController>().PlayerEndAnimation();
        }
        MoveRoomElements();
    }


    public void RoomTransitionEnded()
    {
        _roomTransition = false;
    }

    void MoveRoomElements()
    {
        foreach (var item in _roomObjects)
        {
            item.UpdateRoomPosition(currentRoom);
        }
    }

    //Checks if the player is spamming
    private void CheckSpamming()
    {

    }

    public void LoadCredits()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Video");
    }

    #endregion

    #region Player Interactions

    public void FreezePlayerActions()
    {
        _canInteractWithMouse = false;
        _canMove = false;
    }

    public void UnfreezePlayerActions()
    {
        _canInteractWithMouse = true;
        _canMove = true;
    }

    #endregion
}
