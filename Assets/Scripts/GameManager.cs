using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<LineController> allLines;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winClip;

    public void CheckAllLines(CircleController cc, CircleController target)
    {
        var linesCC = allLines.Where(x => x.connectedCircles.Contains(cc)).ToList();
        var linesTarget = allLines.Where(x => x.connectedCircles.Contains(target)).ToList();

        foreach (var line in linesCC)
        {
            line.checkTemp = !(line.connectedCircles.Contains(cc) && line.connectedCircles.Contains(target));
        }

        foreach (var line in linesTarget)
        {
            line.checkTemp = !(line.connectedCircles.Contains(cc) && line.connectedCircles.Contains(target));
        }

        foreach (var line in linesCC)
        {
            if (line.checkTemp)
            {
                line.SwapCircle(cc, target);
            }
        }

        foreach (var line in linesTarget)
        {
            if (line.checkTemp)
            {
                if(linesCC.Contains(line) == false)
                {
                    line.SwapCircle(target, cc);
                }
            }
        }

        bool isConnected = true;
        foreach (var line in allLines)
        {
            if (!line.CheckLines())
            {
                isConnected = false;
                break;
            }
        }

        if (isConnected)
        {
            PlaySound(winClip);
            winPanel.SetActive(true);
        }
    }

    public void ChangeScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
