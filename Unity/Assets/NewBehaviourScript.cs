using UnityEngine;
using System.Collections;

public class VideoPlayback : MonoBehaviour
{

    public GameObject Sphere;

    // Use this for initialization
    void Start()
    {

        MovieTexture video360 = (MovieTexture)Sphere.GetComponent<Renderer>().material.mainTexture;
        video360.loop = true;
        video360.Play();
    }

}