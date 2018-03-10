using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Stores a reference to the sprite renderer
    private SpriteRenderer _spriteRenderer;
    //Stores a reference to the animator
    private Animator _animator;

    //Stores a reference to the game manager
    // private NewGameManager _gameManager;

    //Player propierties
    public float speed;
    private bool _canMove = false;

    private int _breathAnxietyLevel = 2; //Keeps track of the current anxiety level of the player
    private int _heartAnxietyLevel = 2; //Keeps track of the current anxiety level of the player

    // private int _anxietyLevel = 100; //Keeps track of the current anxiety level of the player
    // public int anxietyLevel
    // {
    //     get { return this._anxietyLevel; }
    //     set { this._anxietyLevel = value; }
    // }

    // private int _numberOfFails = 0; //Keeps track of the number of fails the player has made
    // public int numberOfFails
    // {
    //     get { return this._numberOfFails; }
    //     set { this._numberOfFails = value; }
    // }

    // private int _numberOfPills = 0; //Keeps track of the number of fails the player has made
    // public int numberOfPills
    // {
    //     get { return this._numberOfPills; }
    //     set { this._numberOfPills = value; }
    // }

    //Audio sources and clips
    public AudioSource moveSound;
    public AudioSource heartbeat;
    public AudioSource breathing;
    public AudioClip[] heartSounds = new AudioClip[3];
    public AudioClip[] breathingSounds = new AudioClip[3];
    public float maxVolume; //Max volume for sounds

    // Use this for initialization
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        heartbeat.clip = heartSounds[_heartAnxietyLevel];
        heartbeat.Play();

        breathing.clip = breathingSounds[_breathAnxietyLevel];
        breathing.Play();

        // breathing.volume = maxVolume;
        // heartbeat.volume = maxVolume;

    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if (this._canMove && NewGameManager.Instance.CurrentRoom != 0)
        {
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

            transform.position += move * speed * Time.deltaTime;


            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                _animator.SetBool("Walking", true);
                if (!moveSound.isPlaying)
                {
                    moveSound.volume = 1f;
                    moveSound.Play();
                }

                if (Input.GetAxisRaw("Horizontal") < 0)
                    _spriteRenderer.flipX = true;
                if (Input.GetAxisRaw("Horizontal") > 0)
                    _spriteRenderer.flipX = false;
            }
            else
            {
                _animator.SetBool("Walking", false);
                if (moveSound.isPlaying)
                {
                    moveSound.volume -= 0.1f;
                    moveSound.Stop();
                }
            }

        }
        else
        {
            _animator.SetBool("Walking", false);
            if (moveSound.isPlaying)
            {
                moveSound.volume -= 0.1f;
                moveSound.Stop();
            }
        }
    }

    public void AnxietyDown()
    {
        heartbeat.Stop();
        breathing.Stop();

        if (_heartAnxietyLevel - 1 < 0 || _breathAnxietyLevel - 1 < 0)
        {
            return;
        }

        _heartAnxietyLevel = Mathf.Clamp(_heartAnxietyLevel - 1, 0, 2);
        _breathAnxietyLevel = Mathf.Clamp(_breathAnxietyLevel - 1, 0, 2);

        heartbeat.clip = heartSounds[_heartAnxietyLevel];
        breathing.clip = breathingSounds[_breathAnxietyLevel];

        heartbeat.Play();
        breathing.Play();
    }

    public void AnxietyUp()
    {
        heartbeat.Stop();
        breathing.Stop();

        _heartAnxietyLevel = Mathf.Clamp(_heartAnxietyLevel + 1, 0, 2);
        _breathAnxietyLevel = Mathf.Clamp(_breathAnxietyLevel + 1, 0, 2);

        heartbeat.clip = heartSounds[_heartAnxietyLevel];
        breathing.clip = breathingSounds[_breathAnxietyLevel];

        heartbeat.Play();
        breathing.Play();
    }

    public void PlayerEndAnimation()
    {
        _animator.SetTrigger("EndAnimation");
    }


    public void LoadCredits()
    {
        NewGameManager.Instance.LoadCredits();
    }


    public void FreezePlayer()
    {
        this._canMove = false;
    }

    public void UnfreezePlayer()
    {
        this._canMove = true;
    }

}
