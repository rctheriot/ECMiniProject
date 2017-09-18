using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Evolution : MonoBehaviour
{

    public GameObject SpherePrefab;

    public int populationInit;

    public List<Individual> listOfPopulation = new List<Individual>();
    public Color populationColor;
    public Color deathColor;

    public int offspringCount;
    public Text offspringText;

    public Landscape landscape;

    public float avgFitness;

    public Mutation selectedMutation;

    public enum Mutation
    {
        mutation1,
        mutation2,
    }


    public class Individual
    {
        public float fitness;
        public float p1;
        public float p2;

        public GameObject gameObject;

        public void DestroyIndividual(Color deathColor)
        {
            Renderer rend = gameObject.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Standard");
            rend.material.SetColor("_Color", deathColor);
            Destroy(gameObject, 2.0f);
        }

    }

    // Use this for initialization
    void Start()
    {
        landscape = GameObject.Find("Landscape").GetComponent<Landscape>();

        for (int i = 0; i < populationInit; i++)
        {
            Individual ind = new Individual();

            ind.p1 = Random.Range(landscape.xBound.x, landscape.xBound.y);
            ind.p2 = Random.Range(landscape.zBound.x, landscape.zBound.y);
            ind.fitness = Fitness(ind.p1, ind.p2);
            ind.gameObject = Instantiate(SpherePrefab, new Vector3(ind.p1, ind.fitness, ind.p2), transform.rotation, transform);
            ind.gameObject.transform.parent = transform;
            Renderer rend = ind.gameObject.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Standard");
            rend.material.SetColor("_Color", populationColor);
            listOfPopulation.Add(ind);
            offspringCount++;
        }
        offspringText.text = "Births: " + offspringCount + "  Generation: " + (int)(offspringCount / 10);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Evolve(ref listOfPopulation);
        }
    }

    float Fitness(float xPos, float zPos)
    {
        if (landscape.selectedFormula == Landscape.Formulas.formula1)
        {
            //Formula 1
            float fitness = (xPos * xPos) + (zPos * zPos);
            return fitness;
        }
        else if (landscape.selectedFormula == Landscape.Formulas.formula2)
        {
            //Formula 2
            float fitness = 10 * Mathf.Cos(3.0f * Mathf.Pow(Mathf.Pow(xPos, 2.0f) + Mathf.Pow(zPos, 2.0f), 0.5f) + 1f * 0.5f);
            return fitness;
        }
        return 0;
    }

    public void Evolve(ref List<Individual> currentPopulation)
    {
        offspringCount++;
        offspringText.text = "Briths: " + offspringCount + "  Generation: " + (int)(offspringCount / populationInit);

        //Randomly select parent to produce offsrping
        Individual selectedParent = currentPopulation[Random.Range(0, currentPopulation.Count)];

        //New Offspring
        Individual ind = new Individual();

        //Mutate offspring
        if (selectedMutation == Mutation.mutation1)
            Mutation1(selectedParent, ind);
        if (selectedMutation == Mutation.mutation2)
            Mutation2(selectedParent, ind);

        //Limit to landscape
        ind.p1 = Mathf.Max(landscape.xBound.x, Mathf.Min(ind.p1, landscape.xBound.y));
        ind.p2 = Mathf.Max(landscape.zBound.x, Mathf.Min(ind.p1, landscape.xBound.y));

        //Calculate new fitness score
        ind.fitness = Fitness(ind.p1, ind.p2);

        //Select indivdual from population to compare child to
        Individual compIndividual = currentPopulation[Random.Range(0, currentPopulation.Count)];

        //Child is not fit to live
        if (ind.fitness < compIndividual.fitness) return;

        //Remove the compared individual
        currentPopulation.Remove(compIndividual);
        compIndividual.DestroyIndividual(deathColor);

        //Create new Child
        Vector3 indPos = new Vector3(ind.p1, ind.fitness, ind.p2);
        indPos = Vector3.Scale(indPos, transform.parent.localScale);
        indPos = RotatePointAroundPivot(indPos, Vector3.zero, transform.parent.rotation.eulerAngles);
        ind.gameObject = Instantiate(SpherePrefab, indPos, Quaternion.identity, transform);
        Renderer rend = ind.gameObject.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        rend.material.SetColor("_Color", populationColor);
        currentPopulation.Add(ind);

    }

    //Used to rotate new children to match landscape grid
    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    void Mutation1(Individual selParent, Individual offspring)
    {
        offspring.p1 = selParent.p1 + Random.Range(-0.1f, 0.1f);
        offspring.p2 = selParent.p2 + Random.Range(-0.1f, 0.1f);
    }

    void Mutation2(Individual selParent, Individual offspring)
    {
        offspring.p1 = selParent.p1 + Random.Range(-0.5f, 0.5f);
        offspring.p2 = selParent.p2 + Random.Range(-0.5f, 0.5f);
    }

    public float GetAverageFitness()
    {
        float avgFit = 0;
        for (int i = 0; i < listOfPopulation.Count; i++)
        {
            avgFit += listOfPopulation[i].fitness;
        }
        avgFit /= listOfPopulation.Count;
        return avgFit;

    }


}
