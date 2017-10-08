using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Count
{
    public int like;
    public int just;
    public int really;
    public int right;
    public int speed;
    public float volume;
}


public class gameManager : MonoBehaviour {
    float elapsed = 0f;
    public Image slider;
    public double gesturesTotal=0;
    public Text txt;
    public Text Gesturetxt;
    public Image volumeImage;
    Vector3 initLocation;
    public void GetData()
    {
        Debug.Log("Requesting...");
        StartCoroutine(GetText());
    }

    void Start()
    {
        //txt = gameObject.GetComponent<Text>();
        InvokeRepeating("GetData", 1f, 1f);
        initLocation = slider.rectTransform.localPosition;
        Debug.Log(initLocation);
    }
    
    void Update()
    {
        //elapsed += Time.deltaTime;
        //if (elapsed >= 1f)
        //{
        //    elapsed = elapsed % 1f;
        //    OutputTime();
        //}
        if (gesturesTotal<=10)
        {
            Gesturetxt.text = "Do more gestures!";
        } else
        {
            Gesturetxt.text = "";
        }
    }


    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/count.json");
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            Count c = JsonUtility.FromJson<Count>(www.downloadHandler.text);
            txt.text = "";
            if (c.just>0)
            {
                txt.text += "just: " + c.just + "\n";
            }
            if (c.like > 0)
            {
                txt.text += "like: " + c.like + "\n";
            }
            if (c.really > 0)
            {
                txt.text += "really: " + c.really + "\n";
            }
            if (c.right > 0)
            {
                txt.text += "right: " + c.right + "\n";
            }
            //c.speed
            float newX = initLocation.x + c.speed * 10;
            if (newX > 200) newX = 200;
            Vector3 newLocation = new Vector3(newX, initLocation.y, initLocation.z);
            slider.rectTransform.localPosition = newLocation;
            volumeImage.rectTransform.localScale = new Vector3( 1,c.volume*100f,1);
            //txt.text = www.downloadHandler.text;

        }
    }
}
