using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    string fileName = "";

    public class Plot
    {
        public int plot;
    }

    // Start is called before the first frame update
    void Start()
    {
        fileName = Application.dataPath + "/plot.csv";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WriteCSV()
    {
        
    }
}
