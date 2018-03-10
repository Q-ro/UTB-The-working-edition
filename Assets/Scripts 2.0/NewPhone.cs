using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class NewPhone : MonoBehaviour
{


    [SerializeField]
    private GameObject callButton; // the button that triggers the call a friend action
    [SerializeField]
    private GameObject breathButton; // the button that triggers the perform breath exercise action
    public GameObject arrow;
    public Text clock;


    private Animator _animator;

    bool _isShown;
    bool _appclicked = false;

    // Use this for initialization
    void Start()
    {
        _isShown = false;
        _animator = this.GetComponent<Animator>();
        arrow.GetComponent<Button>().onClick.AddListener(ArrowClicked);
        callButton.GetComponent<Button>().onClick.AddListener(CallApp);
        breathButton.GetComponent<Button>().onClick.AddListener(BreathApp);
    }

    // Update is called once per frame
    void Update()
    {
        //string time[] = System.DateTime.Now.ToString().Split(' ', '\t');
        //string date = time.Split(" ");
        clock.text = System.DateTime.Now.ToString();
    }

    public void ShowPhone()
    {
        if (!_isShown)
        {
            //gameObject.SetActive(true);
            _animator.SetTrigger("PopUp");
            arrow.GetComponent<Button>().interactable = true;
            callButton.GetComponent<Button>().interactable = true;
            breathButton.GetComponent<Button>().interactable = true;
            // NewGameManager.Instance.PhoneOut = true;
        }
        else
        {
            RemovePhone();
            _appclicked = false;
            // _animator.SetTrigger("PopDown");
            // StartCoroutine(PhoneDown());
        }
    }

    public void BreathApp()
    {
        NewGameManager.Instance.CallEvent(typeOfObject.Breathing);
        RemovePhone();
        _appclicked = true;
    }
    public void CallApp()
    {
        NewGameManager.Instance.CallEvent(typeOfObject.PhoneCall);
        RemovePhone();
        _appclicked = true;
    }

    private void PhoneIsDown()
    {
        NewGameManager.Instance.HidePhone(_appclicked);
        _appclicked = false;
        //gameObject.SetActive(false);
    }
    // IEnumerator PhoneDown()
    // {
    //     yield return new WaitForSeconds(1.5f);
    //     //phone.SetActive(false);
    // }

    public void RemovePhone()
    {
        _animator.SetTrigger("PopDown");

        arrow.GetComponent<Button>().interactable = false;
        callButton.GetComponent<Button>().interactable = false;
        breathButton.GetComponent<Button>().interactable = false;

        //NewGameManager.Instance.PhoneOut = false;
    }

    public void ArrowClicked()
    {
        _appclicked = false;
        RemovePhone();
    }
}
