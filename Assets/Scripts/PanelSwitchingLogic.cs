using System.Collections;
using DG;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class PanelSwitchingLogic : MonoBehaviour
{
    public List<GameObject> Panel = new List<GameObject>();
    public List<Vector3> Panel_Pos = new List<Vector3>();
    public int Panel_Num;
    private float _timer = 1;
    private bool _start;
    private bool _isProgressing;

    private void Start()
    {
        if (Panel.Count != 0)
        {
            for (int i = 0; i <= Panel.Count - 1; i++)
            {
                Panel_Pos.Add(Panel[i].transform.position);
            }
        }
        Panel[Panel_Num].transform.position = Vector3.zero;
    }
    private void Update()
    {
        Step1_Start();
        PlayAnimation();
    }

    public void Step1_Start()
    {
        _timer-=Time.deltaTime;
        if (_timer <= 0&& _start==false)
        {
            GoNext();
            _start =true;
        }

    }
    public void GoNext()
    {
        if(Panel_Num==Panel.Count-1) { SceneManager.LoadScene(1); }
        _isProgressing = true;
        Panel_Num++;
        Panel[Panel_Num-1].transform.position = Panel_Pos[Panel_Num-1];
        Panel[Panel_Num].transform.position = Vector3.zero;

    }

    public void PlayAnimation()
    {
        if (_isProgressing)
        {
            Panel[Panel_Num].transform.GetChild(0).GetComponent<Image>().DOFade(1, 0.5f);
            if (Panel[Panel_Num].transform.GetChild(1)!=null) Panel[Panel_Num].transform.GetChild(1).GetComponent<Image>().DOFade(1, 0.5f);

            _isProgressing = false;
        }
    }
}
