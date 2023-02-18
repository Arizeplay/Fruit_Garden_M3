using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    public AppMetrica app;
    public string name_param;
    public List<string> name_vals;
    public List<string> val;
    // Start is called before the first frame update
 
    public void TestSob()
    {
        string json = "{";
        int i = 0;
        foreach (string s in name_vals)
        {
            json += $"\"{s}\":\"{val[i]}\",";
            i++;
        }
        json.Remove(json.Length - 1);
        json += "}";
        //Debug.Log(json);
        app.SendEvent(name_param, json);

    }

    public void Show_id_AppMet()
    {
        if (num_cl > 16)
        {
            gameObject.GetComponent<Text>().color = new Color(50, 50, 50, 255);
            AppMetrica app = GameObject.Find("AppMetrica").GetComponent<AppMetrica>();
            app.GetUsrId();
            gameObject.GetComponent<Text>().text = app.ids;
        }
       
        num_cl++;
        Debug.Log(num_cl);
    }

    public void Open_privacy_policy()
    {
        Application.OpenURL(Project.main.privacy);
    }
    private int num_cl = 0;
}
