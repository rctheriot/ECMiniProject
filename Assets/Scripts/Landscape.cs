using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landscape : MonoBehaviour {

    public Formulas selectedFormula;
    public Vector2 xBound;
    public Vector2 zBound;
    private float yMin = float.MaxValue;
    private float yMax = float.MinValue;

    [Space(10)]
    public Color landscapeColor;
    public float lineWidth;

    public int numLines;

    public enum Formulas
    {
        formula1,
        formula2,
        formula3
    }

	// Use this for initialization
	void Start () {
        CreateLandscape();
        CreateBoundingBox();
    }

    void CreateLandscape()
    {
        //Create XAxis
        for (int i = 0; i < numLines; i++)
        {
            float xPos = ((float)i / (numLines - 1)) * (xBound.x - xBound.y) + xBound.y;
            float zPos = ((float)i / (numLines - 1)) * (zBound.x - zBound.y) + zBound.y;

            CreateLandScapeLine(new Vector3(xBound.x, 0, zPos), new Vector3(xBound.y, 0, zPos));
            CreateLandScapeLine(new Vector3(xPos, 0, zBound.y), new Vector3(xPos, 0, zBound.x));
        }

    }

    void CreateLandScapeLine(Vector3 StartPoint, Vector3 EndPoint)
    {
        GameObject newLine = new GameObject();
        newLine.name = StartPoint.ToString() + ", " + EndPoint.ToString();
        newLine.transform.parent = transform;
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
                float yPos = (xPos * xPos) + (zPos * zPos);

                if (yPos < yMin) yMin = yPos;
                if (yPos > yMax) yMax = yPos;

                Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
                lr.SetPosition(i, pointPosition);
            }
            else if (selectedFormula == Formulas.formula2)
            {
                lr.SetPosition(i, new Vector3(0, i, 0));
            }
            else if (selectedFormula == Formulas.formula3)
            {
                lr.SetPosition(i, new Vector3(0, 0, i));
            }

        }

    }

    void CreateBoundingBox()
    {
        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.x), new Vector3(xBound.x, yMin, zBound.y));
        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.x), new Vector3(xBound.y, yMin, zBound.x));
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.y), new Vector3(xBound.x, yMin, zBound.y));
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.y), new Vector3(xBound.y, yMin, zBound.x));

        CreateBoundingLine(new Vector3(xBound.x, yMax, zBound.x), new Vector3(xBound.x, yMax, zBound.y));
        CreateBoundingLine(new Vector3(xBound.x, yMax, zBound.x), new Vector3(xBound.y, yMax, zBound.x));
        CreateBoundingLine(new Vector3(xBound.y, yMax, zBound.y), new Vector3(xBound.x, yMax, zBound.y));
        CreateBoundingLine(new Vector3(xBound.y, yMax, zBound.y), new Vector3(xBound.y, yMax, zBound.x));

        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.x), new Vector3(xBound.x, yMax, zBound.x));
        CreateBoundingLine(new Vector3(xBound.x, yMin, zBound.y), new Vector3(xBound.x, yMax, zBound.y));
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.x), new Vector3(xBound.y, yMax, zBound.x));
        CreateBoundingLine(new Vector3(xBound.y, yMin, zBound.y), new Vector3(xBound.y, yMax, zBound.y));
    }

    void CreateBoundingLine(Vector3 StartPoint, Vector3 EndPoint)
    {
        GameObject newLine = new GameObject();
        newLine.transform.parent = transform;
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
}
