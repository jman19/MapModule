using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.UI;

public class DataPlotter : MonoBehaviour
{
    public List<float> Xpoints=new List<float>();
    public List<float> Ypoints = new List<float>();
    public List<float> Zpoints = new List<float>();

    // Full column names
    public String xName;
    public String yName;
    public String zName;

    //Title Text
    public String titleName;

    public float plotScale = 10;

    // The prefab for the data points that will be instantiated
    public GameObject PointPrefab;

    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    // Object which will contain text in hiearchy
    public GameObject Title;

    // Use this for initialization
    void Start()
    {

        // Get maxes of each axis
        float xMax = FindMaxValue(Xpoints);
        float yMax = FindMaxValue(Ypoints);
        float zMax = FindMaxValue(Zpoints);

        // Get minimums of each axis
        float xMin = FindMinValue(Xpoints);
        float yMin = FindMinValue(Ypoints);
        float zMin = FindMinValue(Zpoints);

        //find mean values so we can postion text at center points of generated plot
        float xMid = FindMiddle(xMax, xMin);
        float zMid = FindMiddle(zMax, zMin);
        float yMid = FindMiddle(yMax, yMin);

        //center the pivot of object to middle of plot 
        PointHolder.transform.position = new Vector3(normalize(xMid,xMax,xMin),normalize(yMid,yMax,yMin),normalize(zMid,zMax,zMin))*plotScale;

        //Loop through Pointlist
        for (var i = 0; i < Xpoints.Count; i++)
        {
            // Get value in poinList at ith "row", in "column" Name, normalize
            float x = normalize(Xpoints[i], xMax, xMin);

            float y = normalize(Ypoints[i], yMax, yMin);

            float z = normalize(Zpoints[i], zMax, zMin);


            // Instantiate as gameobject variable so that it can be manipulated within loop
            GameObject dataPoint = Instantiate(
                    PointPrefab,
                    new Vector3(x, y, z) * plotScale,
                    Quaternion.identity);

            // Make child of PointHolder object, to keep points within container in hiearchy
            dataPoint.transform.parent = PointHolder.transform;

            // Assigns original values to dataPointName
            string dataPointName =
                "("+"x:"+
                Xpoints[i]+ ","
                + "y:"+Ypoints[i] + ","
                + "z:"+Zpoints[i]+")";

            // Assigns name to the prefab
            dataPoint.transform.name = dataPointName;

            // Scale dataPoint depending on Plot scale
            dataPoint.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f) * plotScale;
            // Gets material color and sets it to a new RGB color we define
            dataPoint.GetComponent<Renderer>().material.color =
                new Color(x, y, z, 1.0f);
        }

        GameObject title = Instantiate(Title, new Vector3( normalize(xMid,xMax,xMin), normalize((yMax+(float)0.5),yMax,yMin), normalize(zMid,zMax,zMin)) * plotScale, Quaternion.identity);
        //add title 
        title.transform.parent = PointHolder.transform;
        title.GetComponent<TextMesh>().text = titleName;
        title.transform.name = "title";
        //scale the size of text depending on PlotScale
        title.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f)*plotScale;
        //add x label
        title = Instantiate(Title, new Vector3(normalize(xMid, xMax, xMin), normalize(yMin, yMax, yMin), normalize(zMin, zMax, zMin)) * plotScale, Quaternion.Euler(90,0,0));
        title.transform.parent = PointHolder.transform;
        title.GetComponent<TextMesh>().text = xName;
        title.transform.name = "x-label";
        //scale the size of text depending on PlotScale
        title.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f)*plotScale;
        //add z label
        title = Instantiate(Title, new Vector3(normalize(xMin, xMax, xMin), normalize(yMin, yMax, yMin), normalize(zMid, zMax, zMin)) * plotScale, Quaternion.Euler(90, 90, 0));
        title.transform.parent = PointHolder.transform;
        title.GetComponent<TextMesh>().text = zName;
        title.transform.name = "z-label";
        title.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f)*plotScale;
        //add y label
        title = Instantiate(Title, new Vector3(normalize(xMin, xMax, xMin), normalize(yMid, yMax, yMin), normalize(zMax, zMax, zMin)) * plotScale, Quaternion.Euler(0, 0, 90));
        title.transform.parent = PointHolder.transform;
        title.GetComponent<TextMesh>().text = yName;
        title.transform.name = "y-label";
        //scale the size of text depending on PlotScale
        title.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f)*plotScale;

        InitalizeInteraction(xMax,yMax,zMax,xMin,yMin,zMin);

        //center the plot in middle of the screen
        PointHolder.transform.position = new Vector3(0, 0, 0);
    }

    private float FindMaxValue(List<float> values)
    {
        //set initial value to first value
        float maxValue = values[0];

        //Loop through, overwrite existing maxValue if new value is larger
        for (var i = 0; i < values.Count; i++)
        {
            if (maxValue < values[i])
                maxValue = values[i];
        }

        //Spit out the max value
        return maxValue;
    }

    private float FindMinValue(List<float> values)
    {

        float minValue = values[0];

        //Loop through, overwrite existing minValue if new value is smaller
        for (var i = 0; i < values.Count; i++)
        {
            if (values[i] < minValue)
                minValue = values[i];
        }

        return minValue;
    }

    private float FindMeanValue(List<float> values)
    {
        float total = 0;
        //Loop through, overwrite existing minValue if new value is smaller
        for (var i = 0; i < values.Count; i++)
        {
            total = total + values[i];
        }
        return total / values.Count;
    }

    private float FindMiddle(float max, float min)
    {
        return (max + min) / 2;
    }

    private float normalize(float value,float max, float min)
    {
        return (value - min) / (max - min);
    }

    private void InitalizeInteraction(float xMax,float yMax,float zMax,float xMin,float yMin, float zMin)
    {
        float xMid = FindMiddle(xMax, xMin);
        float zMid = FindMiddle(zMax, zMin);
        float yMid = FindMiddle(yMax, yMin);

        BoxCollider boxCollider = PointHolder.AddComponent<BoxCollider>();
        PointHolder.transform.gameObject.GetComponent<BoxCollider>().size = new Vector3(normalize(xMax,xMax,xMin), normalize(yMax, yMax, yMin), normalize(zMax, zMax, zMin));

        PointHolder.AddComponent<BoundingBox>();
        PointHolder.AddComponent<ManipulationHandler>();
    }
}
