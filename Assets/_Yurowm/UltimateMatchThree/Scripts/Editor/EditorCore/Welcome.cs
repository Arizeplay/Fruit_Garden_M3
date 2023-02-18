// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Welcome
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Yurowm.Contact;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [InitializeOnLoad]
  internal class Welcome : Banner
  {
    private static string deviceID;
    private static string bundle;
    private static string accessKey = (string) null;
    private static bool eula;
    private static UnityEngine.Color redColor;
    private static string invoice = "";
    private static string name = "";
    private static string email = "";
    private static string company = "";
    private static int teamSize = 0;
    private InvoiceVerify request;
    private InvoiceVerify.Result requestResult;
    private UnityEngine.Color errorColor = new UnityEngine.Color(1f, 0.4f, 0.4f, 1f);

    static Welcome()
    {
      Welcome.redColor = UnityEngine.Color.Lerp(UnityEngine.Color.white, UnityEngine.Color.red, 0.6f);
      EditorApplication.update += new EditorApplication.CallbackFunction(Welcome.OnLoad);
    }

    private static void OnLoad()
    {
      EditorApplication.update -= new EditorApplication.CallbackFunction(Welcome.OnLoad);
      Welcome.deviceID = SystemInfo.deviceUniqueIdentifier;
      Welcome.invoice = ProjectInfo.accessKey;
      Welcome.email = ProjectInfo.email;
      Welcome.name = ProjectInfo.buyer;
      Welcome.company = ProjectInfo.company;
      Welcome.bundle = Application.identifier;
      DateTime lastCheck = ProjectInfo.last_check;
      Welcome.eula = !new PrefVariable(EULA.agreed_key).Bool;
      if (Welcome.invoice.IsNullOrEmpty())
      {
        if (!(lastCheck + new TimeSpan(24, 0, 0) < DateTime.Now))
          return;
        Banner.NewWindowOnLoad(typeof (Welcome));
      }
      else if (lastCheck + new TimeSpan(0, 10, 0) < DateTime.Now || Math.Abs((DateTime.Now - lastCheck).TotalMinutes) > 10.0)
      {
        AsyncExecutor asyncExecutor = new AsyncExecutor(new Action(Welcome.SRequest), delay: 30f);
      }
      else
      {
        if (!Welcome.eula)
          return;
        EditorApplication.update += (EditorApplication.CallbackFunction) (() => Banner.NewWindowOnLoad(typeof (EULA)));
      }
    }

    private static void SRequest()
    {
      try
      {
        InvoiceVerify invoiceVerify = new InvoiceVerify();
        invoiceVerify.Initialize(Welcome.invoice, 27990, Welcome.deviceID, Welcome.bundle);
        invoiceVerify.Execute();
        InvoiceVerify.Result result = (InvoiceVerify.Result) null;
        if (invoiceVerify != null)
          result = (InvoiceVerify.Result) invoiceVerify.GetResult();
        if (result == null)
          return;
        if (result.success)
        {
          if (!string.IsNullOrEmpty(result.accessKey))
            Welcome.accessKey = result.accessKey;
          if (result.lastCoreVersion > 50)
            CoreInfo.CoreUpdate();
          else if (result.fixerLevel > Fixer.state.targetLevel)
            Fixer.CheckUpdates();
          EditorApplication.update += new EditorApplication.CallbackFunction(Welcome.SResultSuccess);
        }
        else
        {
          if (!result.failed)
            return;
          EditorApplication.update += new EditorApplication.CallbackFunction(Welcome.SResultFailed);
        }
      }
      catch (Exception ex)
      {
      }
    }

    private static void SResultSuccess()
    {
      EditorApplication.update -= new EditorApplication.CallbackFunction(Welcome.SResultSuccess);
      ProjectInfo.accessKey = Welcome.accessKey;
      if ((bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main && !Welcome.email.IsNullOrEmpty())
      {
        ProjectInfo.buyer = Welcome.name;
        ProjectInfo.email = Welcome.email;
        ProjectInfo.company = Welcome.company;
        ProjectInfo.teamSize = Welcome.teamSize;
        Welcome.SaveKey(Welcome.accessKey);
      }
      ProjectInfo.last_check = DateTime.Now;
    }

    private static void SResultFailed()
    {
      EditorApplication.update -= new EditorApplication.CallbackFunction(Welcome.SResultFailed);
      Welcome.SaveKey("");
      Banner.NewWindowOnLoad(typeof (Welcome));
    }

    internal override void OnBannerGUI()
    {
      if (!this.maximized)
        this.maximized = true;
      using (new GUIHelper.Lock(this.request != null || MailAgent.IsSending()))
      {
        GUILayout.Space(20f);
        GUILayout.Label("PLEASE FILL THE FORM", Styles.largeTitle, GUILayout.ExpandWidth(true));
        GUILayout.Label("This information will help to make this product better. Please take\na moment to provide this information.", Styles.multilineLabel, GUILayout.ExpandWidth(true));
        GUILayout.Space(10f);
        Welcome.name = this.InputField("YOUR NAME", Welcome.name, validator: new Func<string, bool>(((Banner) this).NameValidation));
        Welcome.company = this.InputField("YOUR COMPANY", Welcome.company, "I don't have it");
        Welcome.teamSize = this.PopupField("TEAM SIZE", Welcome.teamSize, "Single developer", "2-5 developers", "6-50 developers", "50+ developers");
        Welcome.email = this.InputField("YOUR EMAIL", Welcome.email, "name@domain.com", new Func<string, bool>(((Banner) this).EmailValidation));
        Welcome.invoice = this.InputField("ORDER #", Welcome.invoice, validator: new Func<string, bool>(((Banner) this).InvoiceValidation));
        using (new GUIHelper.Color(this.errorColor))
          GUILayout.Label(this.requestResult != null ? this.requestResult.message.NameFormat((string) null, (string) null, true) : "", this.style_Body, GUILayout.ExpandWidth(true));
        GUILayout.FlexibleSpace();
        using (new GUIHelper.Lock(!this.InvoiceValidation(Welcome.invoice) || !this.NameValidation(Welcome.name) || !this.EmailValidation(Welcome.email)))
        {
          if (this.Button("SUBMIT"))
          {
            AsyncExecutor asyncExecutor = new AsyncExecutor(new Action(this.Request), delay: 30f);
          }
        }
      }
      if (this.Button("CLOSE"))
      {
        ProjectInfo.last_check = DateTime.Now;
        this.Close();
      }
      GUILayout.Space(20f);
    }

    private IEnumerator MessageOnRequestSent()
    {
      this.Repaint();
      EditorUtility.DisplayDialog("Success", "Your request has been created. After we verify it, you will receive an access key by email. It can take about 1-12 hours.\\nThanks for your patience!", "Close");
      yield break;
    }

    private void Request()
    {
      this.request = new InvoiceVerify();
      this.request.Initialize(Welcome.invoice, 27990, Welcome.deviceID, Welcome.bundle, Welcome.name, Welcome.company, Welcome.teamSize, Welcome.email);
      this.request.Execute();
      this.requestResult = (InvoiceVerify.Result) this.request.GetResult();
      if (this.requestResult != null)
        EditorApplication.update += new EditorApplication.CallbackFunction(this.RequestResult);
      this.request = (InvoiceVerify) null;
    }

    private void RequestResult()
    {
      EditorApplication.update -= new EditorApplication.CallbackFunction(this.RequestResult);
      this.Repaint();
      if (this.requestResult.success)
      {
        ProjectInfo.last_check = DateTime.Now;
        if (!string.IsNullOrEmpty(this.requestResult.accessKey))
        {
          ProjectInfo.accessKey = this.requestResult.accessKey;
          ProjectInfo.buyer = Welcome.name;
          ProjectInfo.company = Welcome.company;
          ProjectInfo.email = Welcome.email;
          ProjectInfo.teamSize = Welcome.teamSize;
        }
        this.Close();
      }
      else
        ProjectInfo.accessKey = "";
      Welcome.SaveKey(ProjectInfo.accessKey);
    }

    private static void SaveKey(string key)
    {
      if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main || EditorApplication.isPlaying)
        return;
      SerializedObject serializedObject = new SerializedObject((UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main);
      serializedObject.FindProperty(nameof (key)).stringValue = key;
      serializedObject.ApplyModifiedPropertiesWithoutUndo();
      EditorSceneManager.MarkAllScenesDirty();
      EditorSceneManager.SaveOpenScenes();
    }

    internal override Vector2 GetSize() => new Vector2(400f, 400f);

    internal override void Initialize()
    {
    }
  }
}
