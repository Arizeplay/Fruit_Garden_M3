// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.BerryPageViewer
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  public class BerryPageViewer
  {
    private GUIHelper.Scroll scroll = new GUIHelper.Scroll(new GUILayoutOption[2]
    {
      GUILayout.ExpandHeight(true),
      GUILayout.ExpandWidth(true)
    });
    private List<BerryPageViewer.BPElement> content = new List<BerryPageViewer.BPElement>();
    internal List<BerryPageViewer.BP_TOPIC> topics = new List<BerryPageViewer.BP_TOPIC>();
    public bool locked;
    public Action<string> onOpenPageRequest = (Action<string>) (_param1 => { });
    public Action onRepaintRequest = (Action) (() => { });
    public Action<float> onScroll = (Action<float>) (_param1 => { });
    public Action onInitializeStyles = (Action) (() => { });
    private static Texture2D _bufferImage;
    private static bool style_IsInitialized;
    private static GUIStyle style_Background;
    private static GUIStyle style_PageTitle;
    private static GUIStyle style_PageTopic;
    private static GUIStyle style_PlainText;
    private static GUIStyle style_ErrorText;
    private static GUIStyle style_PageLink;
    private static GUIStyle style_PageLinkLabel;
    private static GUIStyle style_DescriptionText;
    private static GUIStyle style_CodeText;
    private string contentRequest;
    private static Dictionary<string, System.Type> elementTypes;

    private void InitializeStyles()
    {
      BerryPageViewer._bufferImage = new Texture2D(1, 1);
      BerryPageViewer.style_Background = new GUIStyle();
      BerryPageViewer.style_Background.normal.background = Texture2D.whiteTexture;
      BerryPageViewer.style_PageTitle = new GUIStyle(EditorStyles.label);
      BerryPageViewer.style_PageTitle.normal.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PageTitle.fontSize = 30;
      BerryPageViewer.style_PageTitle.alignment = TextAnchor.MiddleCenter;
      BerryPageViewer.style_PageTopic = new GUIStyle(EditorStyles.boldLabel);
      BerryPageViewer.style_PageTopic.normal.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PageTopic.fontSize = 20;
      BerryPageViewer.style_PageTopic.alignment = TextAnchor.MiddleLeft;
      BerryPageViewer.style_PlainText = new GUIStyle(EditorStyles.label);
      BerryPageViewer.style_PlainText.normal.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PlainText.focused.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PlainText.active.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PlainText.fontSize = 14;
      BerryPageViewer.style_PlainText.richText = true;
      BerryPageViewer.style_PlainText.wordWrap = true;
      BerryPageViewer.style_CodeText = new GUIStyle(EditorStyles.label);
      BerryPageViewer.style_CodeText.normal.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_CodeText.focused.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_CodeText.active.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_CodeText.fontSize = 12;
      BerryPageViewer.style_CodeText.font = Resources.Load<Font>("Fonts/CourierNew");
      BerryPageViewer.style_CodeText.richText = true;
      BerryPageViewer.style_CodeText.wordWrap = true;
      BerryPageViewer.style_ErrorText = new GUIStyle(EditorStyles.label);
      BerryPageViewer.style_ErrorText.normal.textColor = UnityEngine.Color.red;
      BerryPageViewer.style_ErrorText.focused.textColor = UnityEngine.Color.red;
      BerryPageViewer.style_ErrorText.active.textColor = UnityEngine.Color.red;
      BerryPageViewer.style_ErrorText.fontSize = 15;
      BerryPageViewer.style_ErrorText.richText = false;
      BerryPageViewer.style_ErrorText.wordWrap = true;
      BerryPageViewer.style_ErrorText.alignment = TextAnchor.MiddleCenter;
      BerryPageViewer.style_DescriptionText = new GUIStyle(BerryPageViewer.style_PlainText);
      BerryPageViewer.style_DescriptionText.alignment = TextAnchor.UpperCenter;
      BerryPageViewer.style_DescriptionText.fontSize = 10;
      BerryPageViewer.style_PageLink = new GUIStyle(GUI.skin.button);
      BerryPageViewer.style_PageLink.border = new RectOffset(10, 10, 8, 12);
      BerryPageViewer.style_PageLink.padding = new RectOffset(8, 8, 5, 5);
      BerryPageViewer.style_PageLink.fontStyle = FontStyle.Bold;
      BerryPageViewer.style_PageLink.normal.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PageLink.normal.background = EditorIcons.GetBuildInIcon("BPBrowserButton");
      BerryPageViewer.style_PageLink.active.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PageLink.active.background = EditorIcons.GetBuildInIcon("BPBrowserButtonFilled");
      BerryPageViewer.style_PageLinkLabel = new GUIStyle(GUI.skin.button);
      BerryPageViewer.style_PageLinkLabel.border = new RectOffset(0, 0, 0, 0);
      BerryPageViewer.style_PageLinkLabel.padding = new RectOffset(1, 1, 1, 1);
      BerryPageViewer.style_PageLinkLabel.fontStyle = FontStyle.Bold;
      BerryPageViewer.style_PageLinkLabel.fontSize = 14;
      BerryPageViewer.style_PageLinkLabel.alignment = TextAnchor.UpperLeft;
      BerryPageViewer.style_PageLinkLabel.wordWrap = true;
      BerryPageViewer.style_PageLinkLabel.normal.textColor = UnityEngine.Color.black;
      BerryPageViewer.style_PageLinkLabel.normal.background = Texture2D.blackTexture;
      BerryPageViewer.style_PageLinkLabel.hover = BerryPageViewer.style_PageLinkLabel.normal;
      BerryPageViewer.style_PageLinkLabel.active.textColor = new UnityEngine.Color(0.5f, 0.3f, 0.3f);
      BerryPageViewer.style_PageLinkLabel.active.background = Texture2D.blackTexture;
      BerryPageViewer.style_IsInitialized = true;
    }

    public void StyleUpdate()
    {
      Styles.Update();
      if (BerryPageViewer.style_IsInitialized && !((UnityEngine.Object) BerryPageViewer._bufferImage == (UnityEngine.Object) null))
        return;
      this.onInitializeStyles();
    }

    public virtual void Initialize()
    {
      BerryPageViewer.style_IsInitialized = false;
      this.onInitializeStyles += new Action(Styles.Initialize);
      this.onInitializeStyles += new Action(this.InitializeStyles);
      BerryBrowerCache.Initialize();
      if (BerryPageViewer.elementTypes != null)
        return;
      System.Type baseElement = typeof (BerryPageViewer.BPElement);
      BerryPageViewer.elementTypes = ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, System.Type>((Func<Assembly, IEnumerable<System.Type>>) (x => (IEnumerable<System.Type>) x.GetTypes())).Where<System.Type>((Func<System.Type, bool>) (x => baseElement.IsAssignableFrom(x))).ToDictionary<System.Type, string, System.Type>((Func<System.Type, string>) (x => x.Name), (Func<System.Type, System.Type>) (x => x));
    }

    private List<BerryPageViewer.BPElement> ParseContent(string content)
    {
      List<BerryPageViewer.BPElement> content1 = new List<BerryPageViewer.BPElement>();
      if (content.IsNullOrEmpty())
        return content1;
      string[] array = ((IEnumerable<string>) content.Split(new string[1]
      {
        "\r\n"
      }, StringSplitOptions.RemoveEmptyEntries)).ToArray<string>();
      Regex regex = new Regex("^\\:(?<type>\\w+)\\:(?<params>.*)$");
      BerryPageViewer.BPElement bpElement = (BerryPageViewer.BPElement) null;
      string[] strArray1 = (string[]) null;
      string str = (string) null;
      foreach (string input in array)
      {
        Match match = regex.Match(input);
        if (match.Success)
        {
          if (bpElement != null)
          {
            bpElement.Parse(str ?? "", strArray1);
            content1.Add(bpElement);
            bpElement = (BerryPageViewer.BPElement) null;
          }
          str = (string) null;
          string key = "BP_" + match.Groups["type"].Value;
          if (BerryPageViewer.elementTypes.ContainsKey(key))
          {
            bpElement = (BerryPageViewer.BPElement) Activator.CreateInstance(BerryPageViewer.elementTypes[key]);
            bpElement.viewer = this;
            string[] strArray2;
            if (!string.IsNullOrEmpty(match.Groups["params"].Value))
              strArray2 = ((IEnumerable<string>) match.Groups["params"].Value.Split(';')).ToArray<string>();
            else
              strArray2 = (string[]) null;
            strArray1 = strArray2;
          }
        }
        else
          str = str + (str == null ? "" : "\n") + input;
      }
      if (bpElement != null)
      {
        bpElement.Parse(str ?? "", strArray1);
        content1.Add(bpElement);
      }
      return content1;
    }

    public void OnGUI()
    {
      this.StyleUpdate();
      if (this.contentRequest != null)
      {
        this.content = this.ParseContent(this.contentRequest);
        this.topics = this.content.Where<BerryPageViewer.BPElement>((Func<BerryPageViewer.BPElement, bool>) (x => x is BerryPageViewer.BP_TOPIC)).Select<BerryPageViewer.BPElement, BerryPageViewer.BP_TOPIC>((Func<BerryPageViewer.BPElement, BerryPageViewer.BP_TOPIC>) (x => (BerryPageViewer.BP_TOPIC) x)).ToList<BerryPageViewer.BP_TOPIC>();
      }
      this.DrawPage();
    }

    public virtual void DrawPage()
    {
      using (this.scroll.Start())
      {
        using (new GUIHelper.Vertical(BerryPageViewer.style_Background, new GUILayoutOption[1]
        {
          GUILayout.ExpandHeight(true)
        }))
        {
          if (!this.locked)
          {
            foreach (BerryPageViewer.BPElement bpElement in this.content)
              bpElement.Draw();
          }
          GUILayout.Space(100f);
          GUILayout.FlexibleSpace();
        }
        if (this.locked || (double) this.scroll.position.y <= 0.0)
          return;
        this.onScroll(this.scroll.position.y);
      }
    }

    public void ShowPage(string raw) => this.contentRequest = raw;

    public void Clear()
    {
      this.content.Clear();
      this.topics.Clear();
    }

    public void ScrollTo(float position) => this.scroll.ScrollTo(position);

    public bool IsEmpty() => this.content.Count == 0;

    public abstract class BPElement
    {
      public BerryPageViewer viewer;
      protected const float width = 500f;

      public abstract void Draw();

      public abstract void Parse(string content, params string[] parameters);
    }

    private class BP_TITLE : BerryPageViewer.BPElement
    {
      private string title;

      public override void Draw() => GUILayout.Label(this.title, BerryPageViewer.style_PageTitle);

      public override void Parse(string content, params string[] parameters) => this.title = content;
    }

    internal class BP_TOPIC : BerryPageViewer.BPElement
    {
      private GUIContent body;
      private Vector2 linkSize;
      private static BerryPageViewer.BP_TOPIC scrollTo;

      public void DrawLink(Vector2 position)
      {
        if (!GUI.Button(new Rect(position.x, position.y, this.linkSize.x, this.linkSize.y), this.body, EditorStyles.toolbarButton))
          return;
        BerryPageViewer.BP_TOPIC.scrollTo = this;
      }

      public override void Draw()
      {
        GUILayout.Space(20f);
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(BerryPageViewer.style_PageTopic.CalcHeight(this.body, 500f)));
        controlRect.x += (float) (((double) controlRect.width - 500.0) / 2.0);
        controlRect.width = 500f;
        GUI.Label(controlRect, this.body, BerryPageViewer.style_PageTopic);
        if ((double) controlRect.y == 0.0 || BerryPageViewer.BP_TOPIC.scrollTo != this)
          return;
        this.viewer.ScrollTo(controlRect.y);
        BerryPageViewer.BP_TOPIC.scrollTo = (BerryPageViewer.BP_TOPIC) null;
      }

      public override void Parse(string content, params string[] parameters)
      {
        this.body = new GUIContent(content);
        this.linkSize = EditorStyles.toolbarButton.CalcSize(this.body);
      }

      internal string GetText() => this.body.text;

      internal Vector2 GetLinkSize() => this.linkSize;
    }

    private class BP_BODY : BerryPageViewer.BPElement
    {
      private GUIContent body;

      public override void Draw()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(BerryPageViewer.style_PlainText.CalcHeight(this.body, 500f)));
        controlRect.x += (float) (((double) controlRect.width - 500.0) / 2.0);
        controlRect.width = 500f;
        EditorGUI.SelectableLabel(controlRect, this.body.text, BerryPageViewer.style_PlainText);
      }

      public override void Parse(string content, params string[] parameters)
      {
        content = "   " + content;
        content = content.Replace("\n", "\n   ");
        this.body = new GUIContent(content);
      }
    }

    private class BP_CSHARP : BerryPageViewer.BPElement
    {
      private GUIContent body;

      public override void Draw()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(BerryPageViewer.style_CodeText.CalcHeight(this.body, 500f)));
        controlRect.x += (float) (((double) controlRect.width - 500.0) / 2.0);
        controlRect.width = 500f;
        EditorGUI.SelectableLabel(controlRect, this.body.text, BerryPageViewer.style_CodeText);
      }

      public override void Parse(string content, params string[] parameters) => this.body = new GUIContent(content);
    }

    private class BP_IMAGE : BerryPageViewer.BPElement
    {
      private string description;
      private string name;
      private bool cached;
      private const string url = "http://jellymobile.net/jellymobile.net/yurowm/bpbrowser/images/{0}.jpg";

      public override void Draw()
      {
        if (this.name.IsNullOrEmpty())
          return;
        GUILayout.Space(10f);
        if (this.name == null || !BerryBrowerCache.images.ContainsKey(this.name))
          GUILayout.Label("ERROR: Image isn't loaded", BerryPageViewer.style_ErrorText);
        else if ((UnityEngine.Object) BerryBrowerCache.images[this.name] == (UnityEngine.Object) null)
        {
          if (this.cached)
          {
            this.cached = false;
            EditorCoroutine.start(this.ImageDownloader());
          }
          GUILayout.Label("downloading...", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
          Rect controlRect = EditorGUILayout.GetControlRect(false, GUILayout.Height((float) BerryBrowerCache.images[this.name].height), GUILayout.ExpandWidth(true));
          if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
          {
            controlRect.x += (float) (((double) controlRect.width - (double) BerryBrowerCache.images[this.name].width) / 2.0);
            controlRect.width = (float) BerryBrowerCache.images[this.name].width;
            GUI.DrawTexture(controlRect, BerryBrowerCache.images[this.name]);
            this.cached = true;
          }
        }
        if (!string.IsNullOrEmpty(this.description))
          GUILayout.Label(this.description, BerryPageViewer.style_DescriptionText);
        GUILayout.Space(10f);
      }

      public override void Parse(string content, params string[] parameters)
      {
        this.description = content;
        if (parameters == null || parameters.Length == 0)
          return;
        this.name = parameters[0];
        if (BerryBrowerCache.images.ContainsKey(this.name))
          return;
        BerryBrowerCache.images.Add(this.name, (Texture) null);
        EditorCoroutine.start(this.ImageDownloader());
      }

      private IEnumerator ImageDownloader()
      {
        FileInfo cacheFile = new FileInfo(Path.Combine(BerryBrowerCache.mainFolder.FullName, this.name + ".jpg"));
        if (!cacheFile.Exists || cacheFile.Length == 0L || (cacheFile.LastWriteTime - DateTime.Now).TotalDays > 3.0)
        {
          WWW www = new WWW("http://jellymobile.net/jellymobile.net/yurowm/bpbrowser/images/{0}.jpg".FormatText((object) this.name));
          while (!www.isDone && www.error.IsNullOrEmpty())
            yield return (object) null;
          if (!www.error.IsNullOrEmpty())
          {
            if (!BerryBrowerCache.images.ContainsKey(this.name))
            {
              yield break;
            }
            else
            {
              BerryBrowerCache.images.Remove(this.name);
              yield break;
            }
          }
          else
          {
            File.WriteAllBytes(cacheFile.FullName, www.bytes);
            www = (WWW) null;
          }
        }
        byte[] data = File.ReadAllBytes(cacheFile.FullName);
        if (data != null && BerryBrowerCache.images.ContainsKey(this.name))
        {
          Texture2D texture = new Texture2D(1, 1);
          Upgradable.LoadImage(texture, data);
          BerryBrowerCache.images[this.name] = (Texture) texture;
        }
        this.viewer.onRepaintRequest();
      }

      private void AddImage(byte[] raw)
      {
        if (raw != null && BerryBrowerCache.images.ContainsKey(this.name))
        {
          Texture2D texture = new Texture2D(1, 1);
          Upgradable.LoadImage(texture, raw);
          BerryBrowerCache.images[this.name] = (Texture) texture;
        }
        this.viewer.onRepaintRequest();
      }
    }

    private class BP_TOPAGE : BerryPageViewer.BPElement
    {
      protected string pagename;
      protected GUIContent label;
      private float _width;

      public virtual GUIStyle style => BerryPageViewer.style_PageLink;

      public virtual Rect GetRect()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(this.style.CalcHeight(this.label, 500f)));
        controlRect.x += (float) (((double) controlRect.width - (double) this._width) / 2.0);
        controlRect.width = this._width;
        return controlRect;
      }

      public override void Draw()
      {
        if (!GUI.Button(this.GetRect(), this.label, this.style))
          return;
        this.viewer.onOpenPageRequest(this.pagename);
      }

      public override void Parse(string content, params string[] parameters)
      {
        if (!parameters.IsNullOrEmpty())
          this.pagename = parameters[0];
        if (content.IsNullOrEmpty())
          content = this.pagename;
        if (content.IsNullOrEmpty())
          content = "...";
        this.label = new GUIContent(content);
        this._width = this.style.CalcSize(this.label).x;
      }
    }

    private class BP_TOPAGELABEL : BerryPageViewer.BP_TOPAGE
    {
      public override GUIStyle style => BerryPageViewer.style_PageLinkLabel;

      public override Rect GetRect()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(this.style.CalcHeight(this.label, 500f)));
        controlRect.x += (float) (((double) controlRect.width - 500.0) / 2.0);
        controlRect.width = 500f;
        return controlRect;
      }

      public override void Draw()
      {
        using (new GUIHelper.Lock(this.pagename.IsNullOrEmpty()))
          base.Draw();
      }
    }

    private class BP_TOEDITOR : BerryPageViewer.BPElement
    {
      private string editor;
      private GUIContent label;
      private float _width;
      private static Texture2D icon = EditorIcons.GetBuildInIcon("BPLinkIcon");

      public override void Draw()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(BerryPageViewer.style_PageLink.CalcHeight(this.label, 500f)));
        controlRect.x += (float) (((double) controlRect.width - (double) this._width) / 2.0);
        controlRect.width = this._width;
        if (!GUI.Button(controlRect, this.label, BerryPageViewer.style_PageLink))
          return;
        BerryPanel berryPanel = BerryPanel.CreateBerryPanel();
        berryPanel.ShowTab();
        berryPanel.Show(this.editor);
      }

      public override void Parse(string content, params string[] parameters)
      {
        if (!parameters.IsNullOrEmpty())
          this.editor = parameters[0];
        if (content.IsNullOrEmpty())
          content = this.editor;
        if (content.IsNullOrEmpty())
          content = "...";
        this.label = new GUIContent(content, (Texture) BerryPageViewer.BP_TOEDITOR.icon);
        this._width = BerryPageViewer.style_PageLink.CalcSize(this.label).x;
      }
    }

    private class BP_TOWEB : BerryPageViewer.BPElement
    {
      private string link;
      private GUIContent label;
      private float _width;
      private static Texture2D icon = EditorIcons.GetBuildInIcon("BPWebLinkIcon");

      public override void Draw()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(BerryPageViewer.style_PageLink.CalcHeight(this.label, 500f)));
        controlRect.x += (float) (((double) controlRect.width - (double) this._width) / 2.0);
        controlRect.width = this._width;
        if (!GUI.Button(controlRect, this.label, BerryPageViewer.style_PageLink))
          return;
        Application.OpenURL(this.link);
      }

      public override void Parse(string content, params string[] parameters)
      {
        if (!parameters.IsNullOrEmpty())
          this.link = parameters[0];
        if (content.IsNullOrEmpty())
          content = this.link;
        if (content.IsNullOrEmpty())
          content = "...";
        this.label = new GUIContent(content, (Texture) BerryPageViewer.BP_TOWEB.icon);
        this._width = BerryPageViewer.style_PageLink.CalcSize(this.label).x;
      }
    }

    private class BP_DOWNLOAD : BerryPageViewer.BPElement
    {
      private string link;
      private GUIContent label;
      private float _width;
      private static Texture2D icon = EditorIcons.GetBuildInIcon("BPDownloadIcon");
      private Regex fileNameExtructor = new Regex("^.*\\/(?<name>.*)\\.(?<extension>[A-Za-z0-9]*)(\\?.*)?");
      private PrefVariable defaultDirectoryPath = new PrefVariable("BP_DOWNLOAD_defaultDirectoryPath");
      private bool downloading;

      public override void Draw()
      {
        Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinWidth(500f), GUILayout.Height(BerryPageViewer.style_PageLink.CalcHeight(this.label, 500f)));
        controlRect.x += (float) (((double) controlRect.width - (double) this._width) / 2.0);
        controlRect.width = this._width;
        using (new GUIHelper.Lock(this.downloading))
        {
          if (!GUI.Button(controlRect, this.label, BerryPageViewer.style_PageLink))
            return;
          EditorCoroutine.start(this.Download());
        }
      }

      public override void Parse(string content, params string[] parameters)
      {
        if (parameters.Length != 0)
          this.link = parameters[0];
        if (string.IsNullOrEmpty(content))
          content = this.link;
        this.label = new GUIContent(content, (Texture) BerryPageViewer.BP_DOWNLOAD.icon);
        this._width = BerryPageViewer.style_PageLink.CalcSize(this.label).x;
      }

      private IEnumerator Download()
      {
        this.downloading = true;
        string path = EditorUtility.OpenFolderPanel("Choose directory for downloading", this.defaultDirectoryPath.String, "");
        if (!string.IsNullOrEmpty(path))
        {
          this.defaultDirectoryPath.String = path;
          DirectoryInfo targetDirectory = new DirectoryInfo(path);
          if (!targetDirectory.Exists)
            targetDirectory.Create();
          LogicRequestInstruction request = new LogicRequestInstruction("LinkRequest", this.link, (string) null);
          while (request.keepWaiting)
            yield return (object) 0;
          string url = (string) null;
          if (!string.IsNullOrEmpty(request.error))
            Debug.LogError((object) request.error);
          else
            url = request.result;
          if (!string.IsNullOrEmpty(url))
          {
            EditorUtility.DisplayCancelableProgressBar("Downloading", "Progress", 0.0f);
            WWW www = new WWW(url);
            while (!www.isDone && !EditorUtility.DisplayCancelableProgressBar("Downloading", string.Format("Progress {0:F1}%", (object) (float) ((double) www.progress * 100.0)), www.progress))
              yield return (object) 0;
            EditorUtility.ClearProgressBar();
            if (www.isDone)
            {
              Match match = this.fileNameExtructor.Match(url);
              string link;
              string extension;
              if (match.Success)
              {
                link = match.Groups["name"].Value;
                extension = match.Groups["extension"].Value;
              }
              else
              {
                link = this.link;
                extension = "";
              }
              int index = 0;
              FileInfo fileInfo;
              do
              {
                fileInfo = new FileInfo(Path.Combine(targetDirectory.FullName, this.GenerateFileName(link, extension, index)));
                ++index;
              }
              while (fileInfo.Exists);
              using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Create))
                fileStream.Write(www.bytes, 0, www.bytes.Length);
            }
            www = (WWW) null;
          }
          targetDirectory = (DirectoryInfo) null;
          request = (LogicRequestInstruction) null;
          url = (string) null;
        }
        this.downloading = false;
      }

      private string GenerateFileName(string name, string extension, int index) => name + (index > 0 ? "(" + index.ToString() + ")" : "") + (string.IsNullOrEmpty(extension) ? "" : "." + extension);
    }
  }
}
