using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class NotificationSystem : MonoBehaviour
{
    //singleton
    public static NotificationSystem instance { get; private set; }

    [SerializeField] private int maxMessages = 5;
    [SerializeField] private float tweenTime = 2f;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 offsetPerNotif;
    [SerializeField] Transform parent;

    //struct with the data for a notification
    private struct Notifcation
    {
        public int taskId;
        public int servity; //0 white message, 1 yellow message, 2 red message
        public string message; //the acutal notification text that appears on the HUD

        public Notifcation(int id, int sev, string text)
        {
            taskId = id;
            servity = sev;
            message = text;
        }
    }

    //struct that will hold all of the notifcations that have been created until they are truly destory (and not just removed from the list)
    private class NotifReferenceHolder
    {
        public Notifcation notifObj;
        public GameObject gameObj;
        public Color col;
        public bool handled;

        public NotifReferenceHolder(Notifcation n, GameObject o)
        {
            notifObj = n;
            gameObj = o;
            col = gameObj.GetComponent<TMP_Text>().color;
            handled = false;
        }
    }
    private List<NotifReferenceHolder> AllExistingNotifs;

    //queue of the notifcations that need to go up
    private Queue<Notifcation> notifcationsInWait;

    //list of all the active notifcations
    private List<NotifReferenceHolder> activeNotifications;

    //the assets for each message
    [SerializeField] private GameObject messagePrefab;

    //when this script is first awoken
    private void Awake()
    {
        //if an instance of the class already exists just delete this new one and exit the function
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        //if the function hasn't exited then this must be the only instance so set it to the instance
        instance = this;

        //make sure the attached persists on load
        DontDestroyOnLoad(gameObject);

        notifcationsInWait = new Queue<Notifcation>();
        activeNotifications = new List<NotifReferenceHolder>();
        AllExistingNotifs = new List<NotifReferenceHolder>();
    }

    private void Update()
    {
        //iterate over all of the notifcations that exist 
        foreach(var notifRef in AllExistingNotifs)
        {
            //if it has not already been handled and it does not exist in the active notifcations list then begin the process of deleting it
            if (!notifRef.handled && !activeNotifications.Contains(notifRef))
            {
                BeginDeleteNotif(notifRef);
            }
        }
    }

    public void AddMessage(int task, int severity, string message)
    {
        //remove all existing messages with the new messages id
        RemoveMessagesWithId(task);

        //make a new notfication
        Notifcation newNotif = new Notifcation(task, severity, message);
        //and add it to the queue
        notifcationsInWait.Enqueue(newNotif);

        //check if something can be added to the queue
        CheckQueue();
    }

    public void RemoveMessagesWithId(int id)
    {
        //remove from the active list
        activeNotifications.RemoveAll(notif => notif.notifObj.taskId == id);

        //and from the in wait queue
        notifcationsInWait = new Queue<Notifcation>(notifcationsInWait.Where(notif => notif.taskId != id));

        //check if something can be added to the queue
        CheckQueue();
    }

    private void CheckQueue()
    {
        //if there is space for a new message and there are messages to add, put a new message up
        if(activeNotifications.Count < maxMessages && notifcationsInWait.Count > 0)
        {
            DisplayNewNotif(notifcationsInWait.Dequeue());
        }
    }

    private void DisplayNewNotif(Notifcation newNotif)
    {
        //instiate a new notifcation
        GameObject newObject = Instantiate(messagePrefab, Vector3.zero, 
            Quaternion.identity, parent);

        //newObject.transform.SetParent(parent);
        RectTransform newTrans = newObject.GetComponent<RectTransform>();
        newTrans.anchoredPosition3D = targetPosition + ((activeNotifications.Count + 1) * offsetPerNotif);

        //set the message and color
        TMP_Text newText = newObject.GetComponent<TMP_Text>();
        if (newNotif.servity == 0) newText.color = new Color(0f, 1f, 0f, 0f);
        else if (newNotif.servity == 1) newText.color = new Color(1f, 1f, 0f, 0f);
        else if (newNotif.servity == 2) newText.color = new Color(1f, 0f, 0f, 0f);
        newText.text = newNotif.message;

        //begin tweening
        LeanTween.value(newObject, a => newText.color = a, newText.color,  
            new Color(newText.color.r, newText.color.g, newText.color.b, 1.0f), tweenTime);

        LeanTween.move(newObject.GetComponent<RectTransform>(), (targetPosition + ((activeNotifications.Count) * offsetPerNotif)), tweenTime);

        //make a reference for it and add it to the lists
        NotifReferenceHolder notifRef = new NotifReferenceHolder(newNotif, newObject);

        activeNotifications.Add(notifRef);
        AllExistingNotifs.Add(notifRef);
    }

    private void BeginDeleteNotif(NotifReferenceHolder notifRef)
    {
        //begin tweening all of the other notifcations
        for(int i = 0; i < activeNotifications.Count; i++)
        {
            //cancel any ongoing tweens
            LeanTween.cancel(activeNotifications[i].gameObj);

            //move into the correction position
            LeanTween.move(activeNotifications[i].gameObj.GetComponent<RectTransform>(), (targetPosition + ((i) * offsetPerNotif)), tweenTime);

            //finish any transparency tweening that might not be done
            TMP_Text objText = activeNotifications[i].gameObj.GetComponent<TMP_Text>();
            if (objText.color.a < 1f) LeanTween.value(activeNotifications[i].gameObj, a => objText.color = a, objText.color,
                new Color(objText.color.r, objText.color.g, objText.color.b, 1.0f), (1f - objText.color.a) * tweenTime);
        }

        //set it to be handled - this doesn't work I don't know what to do about it
        notifRef.handled = true;

        //get a refernece to the reference
        TMP_Text message = notifRef.gameObj.GetComponent<TMP_Text>();

        //tween the message's alpha from 1 to 0, when done destroy the gameobject and remove it from the list of all notifcations
        LeanTween.cancel(notifRef.gameObj);

        LeanTween.value(message.gameObject, a => message.color = a, message.color,
            new Color(message.color.r, message.color.g, message.color.b, 0.0f), 0.5f * tweenTime).setOnComplete(delegate () {
                AllExistingNotifs.Remove(notifRef);
                Destroy(notifRef.gameObj);
            });
    }
}