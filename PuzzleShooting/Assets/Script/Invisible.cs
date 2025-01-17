﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Invisible : MonoBehaviour
{
    public Image image;
    public Image time;
    public Text Explanation;
    public TextMeshProUGUI number;
    Image TimeImage;

    TextMeshProUGUI[] texts;

    GameObject panel;
    PlayerController _playerController;

    PanelController _panelController;

    float startTime;

    [NonSerialized]
    public int size;
    public int Filesize;
    [NonSerialized]
    public int[] Numbers;
    public int[] Answer;
    int EmptyNum;

    public TextMeshProUGUI ClearText;

    public bool isSuccess = false;

    void Start()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        panel = GameObject.Find("Panel");
        _panelController = GameObject.Find("PanelController").GetComponent<PanelController>();
        Create_Image();
        CreateAnswer(size);

        Invoke("Finish" , 40f);
        startTime = Time.time;
    }

    void Update()
    {
        if(isSuccess)
        {
            var obj = Instantiate(ClearText);
            obj.transform.SetParent(panel.transform , false);
            obj.rectTransform.anchoredPosition = new Vector2(0f , -70f);

            Invoke("Succese" , 0.8f);
        }
        {
            float t = -360 * (Time.time - startTime) / 40f;
            TimeImage.rectTransform.rotation = Quaternion.Euler(0 , 0 , t);
        }
        CheckAnswer();
    }
    void Create_Image()
    {
        float prov = (float)Screen.height / 450;
        System.Random r = new System.Random();
        int FileNum = r.Next(1 , Filesize);
        int i = 0;

        var text = Instantiate(Explanation , panel.transform);
        text.rectTransform.sizeDelta = new Vector2(prov , 100f * prov);
        text.rectTransform.anchoredPosition = new Vector2(0f , 170f * prov);
        text.fontSize = (int)(text.fontSize * prov);
        text.text = "";

        TextAsset ex = Resources.Load(@"CSV/Invisible/Explanation") as TextAsset;
        StringReader Ex = new StringReader(ex.text);

        while(Ex.Peek() > -1)
        {
            text.text += Ex.ReadLine();
            text.text += "\n";
        }

        TimeImage = Instantiate(time , panel.transform);
        TimeImage.rectTransform.anchoredPosition = new Vector2(40 * prov , -180 * prov);
        TimeImage.rectTransform.sizeDelta *= prov;

        TextAsset csv = Resources.Load(@"CSV/Invisible/Invisible" + FileNum.ToString()) as TextAsset;
        StringReader st = new StringReader(csv.text);
        string[] info = st.ReadLine().Split(',');

        size = int.Parse(info[1]);
        Numbers = new int[size];
        texts = new TextMeshProUGUI[size];

        while(st.Peek() > -1)
        {
            string[] values = st.ReadLine().Split(',');

            var obj = Instantiate(image , new Vector3(0 , 0 , 0) , Quaternion.identity);
            obj.transform.SetParent(panel.transform , false);
            obj.rectTransform.anchoredPosition = new Vector2(float.Parse(values[1]) * prov , float.Parse(values[2]) * prov);
            obj.rectTransform.sizeDelta *= new Vector2(prov , prov);

            var obj2 = Instantiate(number , new Vector3(0 , 0 , 0) , Quaternion.identity);
            obj2.transform.SetParent(obj.transform , false);
            obj2.GetComponent<InvisibleImage>().index = i;
            obj2.GetComponent<InvisibleImage>().Num = int.Parse(values[4]);
            obj2.rectTransform.sizeDelta *= new Vector2(prov , prov);
            Numbers[i] = int.Parse(values[4]);

            if (values[4] == "0") EmptyNum = i;

            texts[i] = obj2;
            i++;
        }
    }
    void Succese()
    {
        _playerController.Invisible();
        Finish();
    }
    void Finish()
    {
        if(isSuccess) GameObject.Find("GameController").GetComponent<GameController>().IntervalSpawn(3 , _panelController.skillnum , 15f);
        else GameObject.Find("GameController").GetComponent<GameController>().IntervalSpawn(3 , _panelController.skillnum , 25f);

        _panelController.canskill[_panelController.skillnum] = false;
        _panelController.isSkill = true;

        _panelController.skillSpeed = 1;
        //スキル使用時に遅くなった時間を戻す
        Time.timeScale = 1.0f;

        foreach (Transform n in panel.transform)
        {
            Destroy(n.gameObject);
        }
        _panelController.Finish();
        panel.SetActive(false);
        _panelController.FinishTimeSet();
        Destroy(this.gameObject);
    }
    public int ChangeNum(int index,int num)
    {
        int side = (int)Math.Sqrt(size);
        if (EmptyNum == index - 1 || EmptyNum == index + 1 || EmptyNum == index - side || EmptyNum == index + side)
        {
            Numbers[EmptyNum] = Numbers[index];
            Numbers[index] = 0;
            texts[EmptyNum].GetComponent<InvisibleImage>().Num = num;
            EmptyNum = index;
            return 0;
        }
        else return num;
    }
    void CreateAnswer(int length)
    {
        Answer = new int[length];
        for(int i = 0; i< length;i++)
        {
            Answer[i] = i + 1;
            if (i == length - 1) Answer[i] = 0;
        }
    }
    void CheckAnswer()
    {
        int count = 0;
        for(int i = 0;i < size;i++)
        {
            if(Numbers[i] == Answer[i])
            {
                count++;
            }
        }
        if(count == size)
        {
            isSuccess = true;
        }
    }
}
