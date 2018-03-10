/*
* Author: Andres Mrad (Q-ro).
* Previous File by: Pedro Almeida.
* Description : An attempt to clean up, DRY and improve previous code from the GameJam.
*/

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
//using System.Linq;

//An enum to keep track of the type of dialog that should be displayed
public enum typeOfDialog
{
    Intro,
    Room,
    Failure,
    Spamming,
    GoBack,
    Test
};

/// <summary>
/// An Static gameobject tasked with managing and displaying the text for the different scenes of the game
/// </summary>
public class NewTextManager : MonoBehaviour
{
    // A reference to the textmanager object
    private static NewTextManager _instance = null;

    //public FileStyleUriParser file;

    public GameObject canvasBackgroundImage;
    public string pathToDialog = "/Scripts/XML/NewDialogFinal.xml";
    public Text textComp;
    public GameObject textBox;
    public Vector2 textboxPosition;
    [Tooltip("How many rooms are in the xml file")]
    public int numberOfRooms = 5;   //Number of rooms in the game
                                    //private Animator _anim;
    private NewGameManager _gameManager;

    [HideInInspector]
    public int currentlyDisplayingText = 0;
    public bool fullScreenBackgorund;
    bool displayingText = false;
    // bool canClick = true;
    bool isTextboxActive = false;

    //Stores a dialog to be displayed next
    string[] goatText;

    List<List<string[]>> levelDialogs = new List<List<string[]>>();
    //All the Level Dialogs
    List<string[]> spamDialogs = new List<string[]>();
    //All the Dialogs to be displayed when the player spams the click button
    List<string[]> introDialogs = new List<string[]>();
    //All the intro Dialogs
    List<string[]> goBackDialogs = new List<string[]>();
    //All the dialogs to be displayed when the player is sent back
    List<List<string[]>> failureDialogs = new List<List<string[]>>();
    //All the dialogs to be displayed when the player fails the sequence

    XmlDocument xmlDoc;


    public static NewTextManager Instance
    {
        get
        {
            return _instance;
        }
    }



    #region Unity functions

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(_instance);
        }
        _instance = this;
        // DontDestroyOnLoad(gameObject);

        if (xmlDoc == null)
        {
            //Load Xml document
            loadXMLFile();
        }

        _gameManager = NewGameManager.Instance;

        LoadByNameDoc("Intro", introDialogs); //Load the intro dialogs for when the player is jsut randomly clicking on things
        LoadRoomDialogs(); //Load all the dialogs for each room
        LoadFailureDialogs(); //Loads the failure dialogs for to be shown if the player fails the sequence
        LoadByNameDoc("Spamming", spamDialogs); //Load the spam dialogs for when the player is jsut randomly clicking on things
        LoadByNameDoc("GoBack", goBackDialogs); //Load the dialogs to be displayed when the player is sent back to a previous room due numerous fails
    }

    void Start()
    {

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (textBox.activeSelf)
            {
                SkipToNextText();
            }
        }
    }



    #endregion

    #region Dialog Management

    void SkipToNextText()
    {
        if (currentlyDisplayingText != goatText.Length && isTextboxActive)
        {
            StopAllCoroutines();
            // StopCoroutine(AnimateText());

            if (currentlyDisplayingText > goatText.Length)
            {

            }
            else if (displayingText == true)
            {
                displayingText = false;
                textComp.text = goatText[currentlyDisplayingText];
                return;
            }
            else if (displayingText == false)
            {
                currentlyDisplayingText++;
                StartCoroutine(AnimateText());
                return;
            }
        }
    }

    IEnumerator AnimateText()
    {


        if (currentlyDisplayingText == 0)
        {
            textBox.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }

        isTextboxActive = true;
        textComp.text = "";
        for (int j = currentlyDisplayingText; j < goatText.Length; j++)
        {
            int textLength = goatText[j].Length;

            currentlyDisplayingText = j;
            displayingText = true;
            for (int i = 0; i < textLength; i++)
            {
                textComp.text += goatText[j][i];
                yield return new WaitForSeconds(.03f);
            }
            displayingText = false;
            yield return new WaitForSeconds(2f);
            if (j != goatText.Length - 1)
            {
                textComp.text = "";
            }
        }
        textComp.text = "";

        yield return new WaitForSeconds(.25f);
        _gameManager.UnfreezePlayerActions();


        textBox.SetActive(false);
        isTextboxActive = false;
        currentlyDisplayingText = 0;

        if (fullScreenBackgorund)
        {
            StartCoroutine(FadeBackgroundOut());
        }
        else
        {
            NewGameManager.Instance.RoomTransitionEnded();
        }
    }

    IEnumerator FadeBackgroundIn()
    {
        //FreezePlayerActions();
        // canvasBackgroundImage.SetActive(true);
        // Color color = canvasBackgroundImage.GetComponent<Image>().color;
        // color.a = 0;
        // canvasBackgroundImage.GetComponent<Image>().color = color;

        canvasBackgroundImage.SetActive(true);
        Color color = canvasBackgroundImage.GetComponent<Image>().color;
        color.a = 0;
        canvasBackgroundImage.GetComponent<Image>().color = color;

        while (color.a < 1)
        {
            color.a = color.a + 0.5f;
            canvasBackgroundImage.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 1;
        canvasBackgroundImage.GetComponent<Image>().color = color;


    }

    IEnumerator FadeBackgroundOut()
    {
        Color color = canvasBackgroundImage.GetComponent<Image>().color;
        color.a = 1;
        canvasBackgroundImage.GetComponent<Image>().color = color;

        // MoveRoomElements();
        yield return new WaitForSeconds(.2f);

        while (color.a > Mathf.Epsilon)
        {
            color.a = color.a - 0.05f;
            canvasBackgroundImage.GetComponent<Image>().color = color;
            yield return null;
        }

        color.a = 0;
        canvasBackgroundImage.GetComponent<Image>().color = color;

        isTextboxActive = false;
        canvasBackgroundImage.SetActive(false);

        NewGameManager.Instance.RoomTransitionEnded();
    }


    public void CallTextBox(typeOfDialog typeOfDialogToDisplay, int textId, bool withBackground = false, Color? color = null)
    {
        // _gameManager.FreezePlayerActions();
        if (textBox.activeSelf)
        {
            StopAllCoroutines();
        }
        //canClick = true;

        fullScreenBackgorund = withBackground;

        if (withBackground && color != null)
            canvasBackgroundImage.GetComponent<Image>().color = (Color)color;

        if (fullScreenBackgorund)
        {
            canvasBackgroundImage.SetActive(true);
            Color tempColor = canvasBackgroundImage.GetComponent<Image>().color;
            tempColor.a = 1;
            canvasBackgroundImage.GetComponent<Image>().color = tempColor;
            // StartCoroutine(FadeBackgroundIn());
        }

        switch (typeOfDialogToDisplay)
        {
            case typeOfDialog.Intro:
                //Debug.Log ();
                goatText = introDialogs[textId];
                // fullScreenBackgorund = true;
                break;
            case typeOfDialog.Room:
                goatText = levelDialogs[_gameManager.CurrentRoom][textId];
                //canvasBackgroundImage.GetComponent<Image>().color = Color.white;
                // fullScreenBackgorund = false;
                break;
            case typeOfDialog.Failure:
                goatText = failureDialogs[(int)_gameManager.LastClickedObject][Random.Range(0, 2)];
                // fullScreenBackgorund = false;
                break;
            case typeOfDialog.GoBack:
                goatText = goBackDialogs[_gameManager.CurrentRoom];
                break;
            case typeOfDialog.Spamming:
                goatText = spamDialogs[textId];
                // fullScreenBackgorund = false;
                break;
            default:
                goatText = new string[] { "You thought I was gonna display the appropiate text, but it was I, DIO !" };
                break;
        }


        textBox.SetActive(true);
        //_anim.SetTrigger ("FadeIn");
        //_anim.ResetTrigger ("FadeIn");
        StartCoroutine(AnimateText());

    }

    /*
	 		if (_gameManager.playerIsSpamming) {
			SetCurrentSpamDialog ();
		} else if (_gameManager.playerLastActionFailed) {
			SetCurrentFailDialog (_gameManager.lastClickedObject, Random.Range (0, 2));
		} else {
	 	void SetCurrentSpamDialog ()
	{       
		currentDialog = Random.Range (0, spamDialogs.Count - 1);
		goatText = spamDialogs [currentDialog];
	}

	void SetCurrentFailDialog (typeOfObject id, int failRate)
	{
		currentDialog = failRate;
		goatText = failureDialogs [id] [currentDialog];
	}
	 */

    public bool IsTextboxActive()
    {
        return isTextboxActive;
    }

    #endregion

    #region XMLOperations

    private void loadXMLFile()
    {
        xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
        xmlDoc.Load(Application.dataPath + pathToDialog);
    }

    void LoadRoomDialogs()
    {
        for (int i = 1; i <= numberOfRooms; i++)
        {
            //Debug.Log("loaded room :" + i);
            List<string[]> dialogs = new List<string[]>(); //Temporary storage for the loaded dialogs
            LoadByNameDoc("room_" + i.ToString(), dialogs); //Get the dialogs
            levelDialogs.Add(dialogs); //Add them to the list
        }
    }

    void LoadFailureDialogs()
    {
        //Gets the Object dialogs using the defined Enum, names in the xml file must mach this data type anmes
        foreach (string objectName in Enum.GetNames(typeof(typeOfObject)))
        {
            List<string[]> dialogs = new List<string[]>(); //Temporary storage for the loaded dialogs
            LoadByNameDoc(objectName, dialogs);
            failureDialogs.Add(dialogs);
        }
    }

    //Load strings from XML doc specific to the level
    void LoadByNameDoc(string name, List<string[]> list)
    {
        //Select level
        XmlNodeList levelsList = xmlDoc.GetElementsByTagName(name);
        XmlNodeList level = levelsList[0].ChildNodes;

        //Debug.Log("trying to load " + name);

        //For each string in each dialog save it in the level dialogs list
        foreach (XmlNode dialog in level)
        {
            XmlNodeList dialogList = dialog.ChildNodes;
            string[] stringArray = new string[dialogList.Count];
            for (int i = 0; i < stringArray.Length; i++)
            {
                stringArray[i] = dialogList[i].InnerText;
            }
            list.Add(stringArray);
        }
    }

    #endregion
}
