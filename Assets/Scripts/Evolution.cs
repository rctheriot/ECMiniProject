using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Evolution : MonoBehaviour
{

    public GameObject SpherePrefab;

    public string name;

    public int populationInit;

    public static int seed;

    public List<Individual> listOfPopulation = new List<Individual>();
    public Color populationColor;
    public Color deathColor;

    public int offspringCount;
    public Text offspringText;

    private Landscape landscape;

    public float avgFitness;

    public Mutation selectedMutation;

    public enum Mutation
    {
        mutation1,
        mutation2,
        mutation3
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
            Destroy(gameObject, 1.0f);
        }

    }

    // Use this for initialization
    void Awake()
    {
        landscape = GameObject.Find("Landscape").GetComponent<Landscape>();
        Random.InitState(seed);
        Random initRandom = new Random();
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
        offspringText.text = "[Births: " + offspringCount +
                     "] [Generation: " + (int)(offspringCount / populationInit) +
                     "] [Mutation: " + name +
                     "] [AvgDistance: " + AvgDistance() + "]";
        offspringText.color = populationColor;

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

    public void Evolve()
    {
        offspringCount++;
        offspringText.text = "[Births: " + offspringCount +
                     "] [Generation: " + (int)(offspringCount / populationInit) +
                     "] [Mutation: " + name +
                     "] [AvgDistance: " + AvgDistance() + "]";

        //Randomly select parent to produce offsrping
        Individual selectedParent = listOfPopulation[Random.Range(0, listOfPopulation.Count)];

        //New Offspring
        Individual ind = new Individual();

        //Mutate offspring
        if (selectedMutation == Mutation.mutation1)
            Mutation1(selectedParent, ind);
        if (selectedMutation == Mutation.mutation2)
            Mutation2(selectedParent, ind);
        if (selectedMutation == Mutation.mutation3)
            Mutation2(selectedParent, ind);

        //Limit to landscape
        ind.p1 = Mathf.Max(landscape.xBound.x, Mathf.Min(ind.p1, landscape.xBound.y));
        ind.p2 = Mathf.Max(landscape.zBound.x, Mathf.Min(ind.p1, landscape.xBound.y));

        //Calculate new fitness score
        ind.fitness = Fitness(ind.p1, ind.p2);

        //Select indivdual from population to compare child to
        Individual compIndividual = listOfPopulation[Random.Range(0, listOfPopulation.Count)];

        //Child is not fit to live
        if (ind.fitness < compIndividual.fitness) return;

        //Remove the compared individual
        listOfPopulation.Remove(compIndividual);
        compIndividual.DestroyIndividual(deathColor);

        //Create new Child
        Vector3 indPos = new Vector3(ind.p1, ind.fitness, ind.p2);
        indPos = Vector3.Scale(indPos, transform.parent.localScale);
        indPos = RotatePointAroundPivot(indPos, Vector3.zero, transform.parent.rotation.eulerAngles);
        ind.gameObject = Instantiate(SpherePrefab, indPos, Quaternion.identity, transform);
        Renderer rend = ind.gameObject.GetComponent<Renderer>();
        rend.material.shader = Shader.Find("Standard");
        rend.material.SetColor("_Color", populationColor);
        listOfPopulation.Add(ind);

        offspringText.text = "[Births: " + offspringCount +
                     "] [Generation: " + (int)(offspringCount / populationInit) +
                     "] [Mutation: " + name +
                     "] [AvgDistance: " + AvgDistance() + "]";

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

    void Mutation3(Individual selParent, Individual offspring)
    {
        float highestFit = float.MinValue;
        for (int i = 0; i < listOfPopulation.Count; i++)
        {
            if (highestFit < listOfPopulation[i].fitness) highestFit = listOfPopulation[i].fitness;
        }

        highestFit = Mathf.Max(-2.0f, Mathf.Min(2.0f, highestFit));
        offspring.p1 = selParent.p1 + Random.Range(-highestFit, highestFit);
        offspring.p2 = selParent.p2 + Random.Range(-highestFit, highestFit);
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

    public float AvgDistance()
    {
        List<float> distanceList = new List<float>();

        for (int i = 0; i < listOfPopulation.Count; i++)
        {
            for (int j = 0; j < listOfPopulation.Count; j++)
            {
                if (i == j) continue;
                float dist = Vector2.Distance(new Vector2(listOfPopulation[i].p1, listOfPopulation[i].p2), new Vector2(listOfPopulation[j].p1, listOfPopulation[j].p2));
                distanceList.Add(dist);
            }
        }

        float avgDist = 0;

        for (int i = 0; i < distanceList.Count; i++)
        {
            avgDist += distanceList[i];
        }

        avgDist /= distanceList.Count;

        return avgDist;
    }


}
