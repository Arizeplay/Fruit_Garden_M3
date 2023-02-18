// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.ContactForm
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Yurowm.Contact;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Development")]
  [BerryPanelTab("Support", "ContactTabIcon", 0)]
  public class ContactForm : MetaEditor
  {
    private const string message = "My name: {0}\nReply to: {1}\nMy invoice: {2} ({3})\n\nMessage:\n{4}\n\n------------\n\n{5}";
    private string invoice = "";
    private PrefVariable subject = new PrefVariable("ContactForm_Subject");
    private PrefVariable body = new PrefVariable("ContactForm_Body");
    private PrefVariable log = new PrefVariable("ContactForm_Log");
    private PrefVariable attachments = new PrefVariable("ContactForm_Attachments");

    public override void OnGUI()
    {
      bool enabled1 = MailAgent.IsSending();
      bool flag = true;
      using (new GUIHelper.Lock(enabled1))
      {
        using (new GUIHelper.Vertical(new GUILayoutOption[0]))
        {
          ProjectInfo.buyer = EditorGUILayout.TextField("My Name", ProjectInfo.buyer, GUILayout.ExpandWidth(true));
          if (!new Regex("[\\w]{2,}").IsMatch(ProjectInfo.buyer))
          {
            this.DrawError("Type your name");
            flag = false;
          }
          ProjectInfo.email = EditorGUILayout.TextField("Reply to (Email)", ProjectInfo.email, GUILayout.ExpandWidth(true));
          if (!new Regex("^([\\w\\.\\-\\+]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$").IsMatch(ProjectInfo.email))
          {
            this.DrawError("Type your email (format: yourname@domain.com)");
            flag = false;
          }
          EditorGUILayout.Space();
          this.subject.String = EditorGUILayout.TextField("Subject", this.subject.String, GUILayout.ExpandWidth(true));
          GUILayout.Label("Message", GUILayout.Width(300f));
          this.body.String = EditorGUILayout.TextArea(this.body.String, GUI.skin.textArea, GUILayout.ExpandWidth(true), GUILayout.Height(300f));
          if (this.body.String.Length == 0)
          {
            this.DrawError("Type a message");
            flag = false;
          }
          GUILayout.Label("Logs or another technical information", GUILayout.Width(300f));
          this.log.String = EditorGUILayout.TextArea(this.log.String, GUI.skin.textArea, GUILayout.ExpandWidth(true), GUILayout.Height(100f));
          EditorGUILayout.Space();
          List<string> attachments = new List<string>();
          if (!string.IsNullOrEmpty(this.attachments.String))
            attachments = ((IEnumerable<string>) this.attachments.String.Split(';')).ToList<string>();
          foreach (string fileName in attachments)
          {
            using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
            {
              if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(30f)))
              {
                attachments.Remove(fileName);
                break;
              }
              GUILayout.Label(new FileInfo(fileName).Name, EditorStyles.miniLabel, GUILayout.Width(200f));
            }
          }
          if (attachments.Count < 5)
          {
            using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
            {
              if (GUILayout.Button("Add attachment", EditorStyles.miniButton, GUILayout.Width(100f)))
              {
                string str = EditorUtility.OpenFilePanel("Select file", "", "");
                if (str.Length > 0)
                  attachments.Add(str);
              }
            }
          }
          this.attachments.String = string.Join(";", attachments.ToArray());
          using (new GUIHelper.Lock(enabled1 || !flag || EditorApplication.isCompiling))
          {
            EditorGUILayout.Space();
            using (new GUIHelper.Horizontal(new GUILayoutOption[0]))
            {
              if (enabled1)
                GUILayout.Label("Sending...", GUILayout.Width(90f));
              if (EditorApplication.isCompiling)
                GUILayout.Label("Compiling...", GUILayout.Width(90f));
              GUILayout.FlexibleSpace();
              if (MailAgent.IsSending())
              {
                bool enabled2 = GUI.enabled;
                GUI.enabled = true;
                if (GUILayout.Button("Break", GUILayout.Width(70f)))
                  MailAgent.Break();
                GUI.enabled = enabled2;
              }
              else if (GUILayout.Button("Send", GUILayout.Width(70f)))
              {
                EditorGUI.FocusTextInControl("");
                if (string.IsNullOrEmpty(this.subject.String))
                  this.subject.String = "No Subject";
                MailAgent.Send(ProjectInfo.buyer, this.subject.String, string.Format("My name: {0}\nReply to: {1}\nMy invoice: {2} ({3})\n\nMessage:\n{4}\n\n------------\n\n{5}", (object) ProjectInfo.buyer, (object) ProjectInfo.email, (object) this.invoice, (object) "Ultimate Match-Three", (object) this.body.String, (object) this.log.String), ProjectInfo.email, new MailAgent.ContactDelegate(this.OnSent), attachments);
              }
            }
          }
          foreach (string error in MailAgent.GetErrors())
            this.DrawError(error);
        }
      }
    }

    private void DrawError(string error)
    {
      UnityEngine.Color color = GUI.color;
      GUI.color = UnityEngine.Color.red;
      GUILayout.Label(error, EditorStyles.miniLabel, GUILayout.ExpandWidth(true));
      GUI.color = color;
    }

    private void OnSent() => EditorCoroutine.start(this.ClearOnSent());

    private IEnumerator ClearOnSent()
    {
      this.body.String = "";
      this.subject.String = "";
      this.attachments.String = "";
      this.log.String = "";
      MetaEditor<MonoBehaviour>.Repaint();
      EditorUtility.DisplayDialog("Contact", "Your email has been sent", "Ok");
      yield break;
    }

    public override bool Initialize()
    {
      this.invoice = ProjectInfo.accessKey;
      return true;
    }
  }
}
