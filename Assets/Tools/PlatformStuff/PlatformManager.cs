using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("Mobile Stuff")]
    [SerializeField] private GameObject mobilePanel;
    [SerializeField] private bool runMobileInEditor;

    private bool isMobile;

    // Start is called before the first frame update
    void Start()
    {
        isMobile = (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer
           || (Application.platform == RuntimePlatform.WindowsEditor && runMobileInEditor));

        //if it should be using mobile controls
        if (isMobile)
        {
            //set the controls to active
            if(mobilePanel != null) mobilePanel.SetActive(true);
        }
        //otherwise 
        else
        {
            //disable them
            if (mobilePanel != null) mobilePanel.SetActive(false);
        }
    }

    public bool GetIsMobile()
    {
        return isMobile;
    }

    static public bool GetIsMobileStatic()
    {
        return GameObject.Find("PlatformManager").GetComponent<PlatformManager>().GetIsMobile();
    }
}
