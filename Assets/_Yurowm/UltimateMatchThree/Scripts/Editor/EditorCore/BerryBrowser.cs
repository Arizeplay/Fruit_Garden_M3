// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.BerryBrowser
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  public class BerryBrowser : BerryPageViewer
  {
    private string addressField;
    public string startPage;
    private const int historySize = 32;
    private string openedPage;
    private static PrefVariable historyPosition = new PrefVariable(string.Format("{0}_BP_HistoryPosition", (object) 27990));
    private static PrefVariable scrollPosition = new PrefVariable("{0}_BP_ScrollPosition".FormatText((object) 27990));
    private static PrefVariable history = new PrefVariable(string.Format("{0}_BP_History", (object) 27990));
    private const string pageNotFoundPage = ":IMAGE:page_not_found\r\n{0}";
    protected static GUIStyle style_NavigationToolbar;
    protected static GUIStyle style_NavigationButton;
    private static GUIStyle style_NavigationInput;
    private Rect startRect = new Rect(0.0f, 0.0f, 400f, 20f);

    public override void Initialize()
    {
      this.onInitializeStyles = this.onInitializeStyles + new Action(this.InitializeStyles);
      this.onScroll = this.onScroll + (Action<float>) (position => BerryBrowser.scrollPosition.Float = position);
      base.Initialize();
      if (!this.openedPage.IsNullOrEmpty())
        this.startPage = this.openedPage;
      if (this.startPage.IsNullOrEmpty())
      {
        List<string> historyList = BerryBrowser.historyList;
        if (historyList.Count == 0)
        {
          this.startPage = BerryBrowser.homePage;
          historyList.Add(this.startPage);
          BerryBrowser.historyList = historyList;
          BerryBrowser.historyPosition.Int = 0;
        }
        else
        {
          if (BerryBrowser.historyPosition.IsEmpty())
            BerryBrowser.historyPosition.Int = historyList.Count - 1;
          this.startPage = historyList.Get<string>(BerryBrowser.historyPosition.Int);
        }
      }
      if (this.startPage.IsNullOrEmpty() || !this.IsEmpty())
        return;
      this.OpenPage(this.startPage, false);
    }

    private static string homePage => "{0}_main".FormatText((object) "ultimate");

    private string currentPage
    {
      get
      {
        List<string> historyList = BerryBrowser.historyList;
        if (BerryBrowser.historyPosition.IsEmpty())
          BerryBrowser.historyPosition.Int = historyList.Count - 1;
        return historyList.Get<string>(BerryBrowser.historyPosition.Int);
      }
    }

    private static List<string> historyList
    {
      get
      {
        if (BerryBrowser.history.IsEmpty())
          return new List<string>();
        return ((IEnumerable<string>) BerryBrowser.history.String.Split(';')).ToList<string>();
      }
      set => BerryBrowser.history.String = string.Join(";", value.ToArray());
    }

    private void InitializeStyles()
    {
      BerryBrowser.style_NavigationToolbar = new GUIStyle(EditorStyles.toolbar);
      BerryBrowser.style_NavigationToolbar.fixedHeight = 30f;
      BerryBrowser.style_NavigationToolbar.border.top = 3;
      BerryBrowser.style_NavigationToolbar.border.bottom = 3;
      BerryBrowser.style_NavigationToolbar.alignment = TextAnchor.MiddleCenter;
      BerryBrowser.style_NavigationButton = new GUIStyle(EditorStyles.toolbarButton);
      BerryBrowser.style_NavigationButton.fixedHeight = 30f;
      BerryBrowser.style_NavigationButton.border.top = 3;
      BerryBrowser.style_NavigationButton.border.bottom = 3;
      BerryBrowser.style_NavigationInput = new GUIStyle(EditorStyles.toolbarTextField);
      BerryBrowser.style_NavigationInput.fixedHeight = 25f;
      BerryBrowser.style_NavigationInput.border.top = 3;
      BerryBrowser.style_NavigationInput.border.bottom = 4;
      BerryBrowser.style_NavigationInput.fontSize = 20;
    }

    public IEnumerator LoadPageRoutine(string address, bool changeHistory = false)
    {
      this.locked = true;
      this.addressField = address;
      if (address.IsNullOrEmpty())
        address = BerryBrowser.homePage;
      FileInfo cacheFile = new FileInfo(Path.Combine(BerryBrowerCache.mainFolder.FullName, address + ".bppage"));
      string _content = (string) null;
      string _message = (string) null;
      if (cacheFile.Exists && cacheFile.Length > 0L && (DateTime.Now - cacheFile.LastWriteTime).TotalHours <= 6.0)
      {
        using (StreamReader streamReader = new StreamReader(cacheFile.FullName))
          _content = streamReader.ReadToEnd();
      }
      if (string.IsNullOrEmpty(_content))
      {
        if (address.Length > 0 && address.Length <= 64)
        {
          AsyncExecutor executor = new AsyncExecutor((Action) (() =>
          {
            LogicRequest logicRequest = new LogicRequest();
            logicRequest.Initialize("LoadBPPage", address, (string) null);
            logicRequest.Execute();
            LogicRequest.Result result = (LogicRequest.Result) logicRequest.GetResult();
            if (result == null)
              return;
            _message = result.error;
            _content = result.result;
          }), delay: 30f);
          while (!executor.IsComplete())
            yield return (object) 0;
          executor = (AsyncExecutor) null;
        }
        if (!string.IsNullOrEmpty(_content))
        {
          using (StreamWriter streamWriter = new StreamWriter(cacheFile.FullName))
            streamWriter.Write(_content);
        }
      }
      this.Clear();
      if (_content.IsNullOrEmpty())
        _content = string.Format(":IMAGE:page_not_found\r\n{0}", _message != null ? (object) _message : (object) "Unknown Error");
      this.ShowPage(_content);
      yield return (object) 0;
      this.ScrollTo(changeHistory ? 0.0f : BerryBrowser.scrollPosition.Float);
      this.openedPage = address;
      this.locked = false;
      this.onRepaintRequest();
    }

    public override void DrawPage()
    {
      if (!this.locked && string.IsNullOrEmpty(this.openedPage))
        this.OpenPage(this.openedPage, false);
      try
      {
        using (new GUIHelper.Horizontal(BerryBrowser.style_NavigationToolbar, new GUILayoutOption[1]
        {
          GUILayout.ExpandWidth(true)
        }))
          this.OnDrawNavigationPanel();
        this.OnDrawTopics();
        base.DrawPage();
      }
      catch (ArgumentException ex)
      {
        if (ex.Message.StartsWith("Getting control"))
          return;
        Debug.LogException((Exception) ex);
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }

    protected virtual void OnDrawNavigationPanel()
    {
      using (new GUIHelper.Lock(this.locked))
      {
        if (GUILayout.Button("<", BerryBrowser.style_NavigationButton, GUILayout.Width(40f)))
          this.PreviousPage();
        if (GUILayout.Button(">", BerryBrowser.style_NavigationButton, GUILayout.Width(40f)))
          this.NextPage();
        if (GUILayout.Button("Home", BerryBrowser.style_NavigationButton, GUILayout.Width(40f)))
          this.OpenPage(BerryBrowser.homePage);
      }
      GUILayout.Space(10f);
      GUI.SetNextControlName("Address field");
      GUILayout.Label("Address:", BerryBrowser.style_NavigationToolbar, GUILayout.Width(60f));
      this.addressField = EditorGUILayout.TextField(this.addressField, BerryBrowser.style_NavigationInput, GUILayout.ExpandWidth(true));
      using (new GUIHelper.Lock(this.locked))
      {
        if (GUI.GetNameOfFocusedControl() == "Address field" && UnityEngine.Event.current.keyCode == KeyCode.Return)
        {
          GUI.FocusControl("");
          this.onRepaintRequest();
          this.OpenPage(this.addressField);
        }
        if (!GUILayout.Button("Clear Cache", BerryBrowser.style_NavigationButton, GUILayout.Width(80f)))
          return;
        BerryBrowerCache.Clear();
        this.OpenPage(this.currentPage, false);
      }
    }

    protected virtual void OnDrawTopics()
    {
      if (this.topics.Count <= 0)
        return;
      if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
      {
        this.startRect = GUILayoutUtility.GetLastRect();
        this.startRect.y += this.startRect.height;
      }
      Vector2[] array = this.topics.Select<BerryPageViewer.BP_TOPIC, Vector2>((Func<BerryPageViewer.BP_TOPIC, Vector2>) (x => x.GetLinkSize())).ToArray<Vector2>();
      Vector2 position = this.startRect.position;
      for (int index = 0; index < this.topics.Count; ++index)
      {
        if ((double) position.x != (double) this.startRect.x && (double) position.x + (double) array[index].x > (double) this.startRect.xMax)
        {
          position.x = this.startRect.x;
          position.y += array[index].y;
        }
        this.topics[index].DrawLink(position);
        position.x += array[index].x;
      }
      position.y += array[0].y;
      EditorGUILayout.GetControlRect(GUILayout.Width(this.startRect.width - 20f), GUILayout.Height((float) ((double) position.y - (double) this.startRect.y - 5.0)));
    }

    private void PreviousPage()
    {
      List<string> historyList = BerryBrowser.historyList;
      int index = Mathf.Max(0, Mathf.Min(BerryBrowser.historyPosition.Int - 1, historyList.Count - 1));
      BerryBrowser.historyPosition.Int = index;
      this.OpenPage(historyList[index], false);
    }

    private void NextPage()
    {
      List<string> historyList = BerryBrowser.historyList;
      int index = Mathf.Max(0, Mathf.Min(BerryBrowser.historyPosition.Int + 1, historyList.Count - 1));
      BerryBrowser.historyPosition.Int = index;
      this.OpenPage(historyList[index], false);
    }

    public void OpenPage(string pagename, bool changeHistory = true)
    {
      if (changeHistory)
      {
        List<string> historyList = BerryBrowser.historyList;
        if (historyList.Count == 0 || historyList.Get<string>(BerryBrowser.historyPosition.Int) != pagename)
        {
          List<string> range = historyList.GetRange(0, BerryBrowser.historyPosition.Int + 1);
          range.Add(pagename);
          if (range.Count > 32)
            range = range.GetRange(range.Count - 32, range.Count - 1);
          BerryBrowser.historyList = range;
          BerryBrowser.historyPosition.Int = range.Count - 1;
        }
      }
      EditorCoroutine.start(this.LoadPageRoutine(pagename, changeHistory));
      GUI.FocusControl("");
    }
  }
}
