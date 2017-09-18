using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Landscape : MonoBehaviour {

    public Formulas selectedFormula;
    public Vector2 xBound;
    public Vector2 zBound;
    public float yMin = float.MaxValue;
    public float yMax = float.MinValue;

    private bool pause;
    public Text pauseText;

    [Space(10)]
    public Color landscapeColor;
    public float lineWidth;

    public int numLines;

    public Evolution[] evoList;
    public LineChart chart;

    public enum Formulas
    {
        formula1,
        formula2,
    }

	// Use this for initialization
	void Awake () {
        CreateLandscape();
        CreateBoundingBox();
    }

    void Start()
    {
        transform.localScale = new Vector3(.6f, .05f, .6f);
        transform.rotation = Quaternion.Euler(-20, 40, -10);
        pause = true;
        InvokeRepeating("Evolve", 1.0f, 0.5f);
    }

    public void pauseStart()
    {
        pause = !pause;
        if (pause)
        {
            pauseText.text = "Play";
            pauseText.transform.parent.GetComponent<Image>().color = Color.green;
        } else
        {
            pauseText.text = "Pause";
            pauseText.transform.parent.GetComponent<Image>().color = Color.red;
        }
    }

    void Evolve()
    {
        if (pause) return;
        if (evoList[0].offspringCount > 300) return;

        for (int i =0; i < evoList.Length; i++)
        {
            evoList[i].Evolve();
        }

        chart.BirthUpdate();
    }

    void CreateLandscape()
    {
        GameObject grid = new GameObject();
        grid.transform.parent = transform;
        grid.name = "Grid";

        //Create Grid
        for (int i = 0; i < numLines; i++)
        {
            float xPos = ((float)i / (numLines - 1)) * (xBound.x - xBound.y) + xBound.y;
            float zPos = ((float)i / (numLines - 1)) * (zBound.x - zBound.y) + zBound.y;

            CreateLandScapeLine(new Vector3(xBound.x, 0, zPos), new Vector3(xBound.y, 0, zPos), grid.transform);
            CreateLandScapeLine(new Vector3(xPos, 0, zBound.y), new Vector3(xPos, 0, zBound.x), grid.transform);
        }

    }

    void CreateLandScapeLine(Vector3 StartPoint, Vector3 EndPoint, Transform GridParent)
    {
        GameObject newLine = new GameObject();
        newLine.name = StartPoint.ToString() + ", " + EndPoint.ToString();
        newLine.transform.parent = GridParent;
        LineRenderer lr = newLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended"));
        lr.material.SetColor("_TintColor", landscapeColor);
        lr.useWorldSpace = false;
        lr.positionCount = 20;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        for (int i = 0; i < lr.positionCount; i++)
        {
            if (selectedFormula == Formulas.formula1)
            {
                float xPos = (StartPoint.x - EndPoint.x) * ((float)i / (lr.positionCount - 1)) + EndPoint.x;
                float zPos = (StartPoint.z - EndPoint.z) * ((float)i / (lr.positionCount - 1)) + EndPoint.z;

                //Formula 1
                float yPos = (xPos * xPos) + (zPos * zPos);

                if (yPos < yMin) yMin = yPos;
                if (yPos > yMax) yMax = yPos;

                Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
                lr.SetPosition(i, pointPosition);
            }
            else if (selectedFormula == Formulas.formula2)
            {
                float xPos = (StartPoint.x - EndPoint.x) * ((float)i / (lr.positionCount - 1)) + EndPoint.x;
                float zPos = (StartPoint.z - EndPoint.z) * ((float)i / (lr.positionCount - 1)) + EndPoint.z;

                //Formula 2
                float yPos = 10 * Mathf.Cos(3.0f * Mathf.Pow(Mathf.Pow(xPos, 2.0f) + Mathf.Pow(zPos, 2.0f), 0.5f) + 1f * 0.5f);

                if (yPos < yMin) yMin = yPos;
                if (yPos > yMax) yMax = yPos;

                Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
                lr.SetPosition(i, pointPosition);
            }

        }

    }

    void CreateBoundingBox()
    {
        GameObject boundingBox = new GameObject();
        boundingBox.transform.parent = transform;
        boundingBox.name = "BoundingBox";

        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.x), new Vector3(xBound.x, yMin, zBound.y), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.x), new Vector3(xBound.y, yMin, zBound.x), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.y), new Vector3(xBound.x, yMin, zBound.y), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.y), new Vector3(xBound.y, yMin, zBound.x), boundingBox.transform);

        CreateBoundingLine(new Vector3(xBound.x, yMax, zBound.x), new Vector3(xBound.x, yMax, zBound.y), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.x, yMax, zBound.x), new Vector3(xBound.y, yMax, zBound.x), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.y, yMax, zBound.y), new Vector3(xBound.x, yMax, zBound.y), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.y, yMax, zBound.y), new Vector3(xBound.y, yMax, zBound.x), boundingBox.transform);

        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.x), new Vector3(xBound.x, yMax, zBound.x), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.y), new Vector3(xBound.x, yMax, zBound.y), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.x), new Vector3(xBound.y, yMax, zBound.x), boundingBox.transform);
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.y), new Vector3(xBound.y, yMax, zBound.y), boundingBox.transform);
    }

    void CreateBoundingLine(Vector3 StartPoint, Vector3 EndPoint, Transform bbParent)
    {
        GameObject newLine = new GameObject();
        newLine.transform.parent = bbParent;
        LineRenderer lr = newLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Standard"));
        lr.material.SetColor("_Color", Color.white);
        lr.useWorldSpace = false;
        lr.positionCount = 2;
        lr.startWidth = 0.075f;
        lr.endWidth = 0.075f;
        lr.SetPosition(0, StartPoint);
        lr.SetPosition(1, EndPoint);
    }

    public void ScaleX(float value)
    {
        transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);
    }
    public void ScaleY(float value)
    {
        transform.localScale = new Vector3(transform.localScale.x, value, transform.localScale.z);
    }
    public void ScaleZ(float value)
    {
        transform.localScale = new Vector3( transform.localScale.x, transform.localScale.y, value);
    }

    public void RotateX(float value)
    {
        float degrees = Mathf.Rad2Deg * value;
        if (degrees > 180) degrees -= 360;
        transform.rotation = Quaternion.Euler(degrees, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
    public void RotateY(float value)
    {
        float degrees = Mathf.Rad2Deg * value;
        if (degrees > 180) degrees -= 360;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, degrees, transform.rotation.eulerAngles.z);
    }
    public void RotateZ(float value)
    {
        float degrees = Mathf.Rad2Deg * value;
        if (degrees > 180) degrees -= 360;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, degrees);
    }

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SetSeed", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}