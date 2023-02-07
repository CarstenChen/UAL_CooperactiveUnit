using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class LinesManager : MonoBehaviour
{
    private static LinesManager instance;
    public static LinesManager Instance { get { return instance; } private set { } }

    public GameObject lineUI;
    protected TextMeshProUGUI textMeshPro;
    public Color LerpColorA;
    public Color LerpColorB;

    protected AudioSource audio;
    protected PlotManager plotManager;
    protected Lines[] allLines;
    protected Lines currentLine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        plotManager = Resources.Load<PlotManager>("DataAssets/Lines");
        textMeshPro= lineUI.GetComponentInChildren<TextMeshProUGUI>();
        allLines = plotManager.lines;
    }
    
    public void DisplayLine(int plotID,int index)
    {
        for(int i = 0; i < allLines.Length; i++)
        {
            if(allLines[i].plotID==plotID && allLines[i].index == index)
            {
                currentLine = allLines[i];
                StartCoroutine(LineFadeIn());
                textMeshPro.text = currentLine.text;
                StartCoroutine(WaitSoundEndToNextLine(currentLine));
            }
        }
    }
    IEnumerator WaitSoundEndToNextLine(Lines line)
    {

        audio.clip = line.audio;
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        
        if(line.nextIndex != -1)
        {
            DisplayLine(line.plotID, line.nextIndex);
        }
        else
        {
            StartCoroutine(LineFadeOut());
        }
    }

    IEnumerator LineFadeIn()
    {
        lineUI.SetActive(true);
        lineUI.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, 0.04f);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator LineFadeOut()
    {
        lineUI.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, 0.04f);
        yield return new WaitForSeconds(0.5f);
        lineUI.SetActive(false);
    }
}
