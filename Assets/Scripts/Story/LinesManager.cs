using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class LinesManager : MonoBehaviour
{
    private static LinesManager instance;
    public static LinesManager Instance { get { return instance; } private set { } }

    public GameObject lineUI;

    public Color LerpColorA;
    public Color LerpColorB;

    [SerializeField] public static bool isPlayingLines;

    protected TextMeshProUGUI textMeshPro;
    protected Animator textAnimator;
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
        isPlayingLines = false;

        audio = GetComponent<AudioSource>();
        textMeshPro= lineUI.GetComponentInChildren<TextMeshProUGUI>();
        textAnimator = lineUI.GetComponent<Animator>();

        plotManager = Resources.Load<PlotManager>("DataAssets/Lines");
        allLines = plotManager.lines;
    }
    
    public void DisplayLine(int plotID,int index)
    {
        if (isPlayingLines && index ==0) return;

        if(index == 0)
        {
            StopAllCoroutines();
            StartCoroutine(SetLineUI(true, 0f));
            isPlayingLines = true;

            //textAnimator.SetBool("FadeIn", true);
            //textAnimator.SetBool("FadeOut", false);
        }

        for(int i = 0; i < allLines.Length; i++)
        {
            if(allLines[i].plotID==plotID && allLines[i].index == index)
            {
                currentLine = allLines[i];
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
            if (textAnimator != null)
            {
                textAnimator.SetBool("FadeIn", false);
                textAnimator.SetBool("FadeOut", true);
            }


            StartCoroutine(SetLineUI(false,1f));
        }
    }

    IEnumerator SetLineUI(bool active, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(lineUI!=null)
        lineUI.SetActive(active);

        if (!active)
        {
            isPlayingLines = false;
        }
    }
}
