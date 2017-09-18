using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChart : MonoBehaviour {

    public Evolution[] evoList;
    private List<LineRenderer> lineList;

    private GameObject background;

    public Text FitMax, FitMin;

    public Landscape landscape;


    // Use this for initialization
    void Start () {

        lineList = new List<LineRenderer>();
        background = transform.Find("Quad").gameObject;

        FitMax.text = Mathf.Round(landscape.yMax).ToString();
        FitMin.text = Mathf.Round(landscape.yMin).ToString();
        //Vector3[] vertices = background.GetComponent<MeshFilter>().mesh.vertices;
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    Vector3 vert = transform.worldToLocalMatrix.MultiplyPoint3x4(vertices[i]);
        //    Debug.Log(i + ": " + vert);
        //}
        
        for (int i = 0; i < evoList.Length; i++)
        {
            GameObject newObject = new GameObject();
            newObject.transform.parent = transform.Find("Quad");
            newObject.transform.localPosition = new Vector3(-.5f, -.5f, 0);
            newObject.transform.localScale = Vector3.one;
            LineRenderer lr = newObject.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended"));
            lr.material.SetColor("_TintColor", evoList[i].populationColor);
            lr.useWorldSpace = false;
            lr.positionCount = evoList[i].populationInit;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;

            lineList.Add(lr);


            for (int j = 0; j < lr.positionCount; j++)
            {
                float fitness = evoList[i].GetAverageFitness();
                float xPos = ((float)j / 300.0f);
                float yPos = (fitness- landscape.yMin) / (landscape.yMax - landscape.yMin);
                float zPos = -1f;
                Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
                lr.SetPosition(j, pointPosition);
            }

        }


    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            BirthUpdate();
        }
    }

    void BirthUpdate()
    {
   
        for (int i = 0; i < evoList.Length; i++)
        {
            lineList[i].positionCount++;
            float fitness = evoList[i].GetAverageFitness();
            float xPos = ((float)lineList[i].positionCount / 300.0f);
            float yPos = (fitness - landscape.yMin) / (landscape.yMax - landscape.yMin);
            float zPos = -1f;
            Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
            lineList[i].SetPosition(lineList[i].positionCount - 1, pointPosition);
        }

    }
}
