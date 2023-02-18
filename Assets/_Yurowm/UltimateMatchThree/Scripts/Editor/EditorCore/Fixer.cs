// Decompiled with JetBrains decompiler
// Type: Yurowm.EditorCore.Fixer
// Assembly: EditorCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 7AAF915A-5EF6-46A3-BB18-471DA435BF5F
// Assembly location: D:\Desktop\EditorCore.dll

using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;
using Yurowm.Contact;
using Yurowm.GameCore;

namespace Yurowm.EditorCore
{
  [BerryPanelGroup("Development")]
  [BerryPanelTab("Fixer", "FixerTabIcon", 0)]
  [InitializeOnLoad]
  internal class Fixer : MetaEditor
  {
    private const string fixFileExtension = ".fix";
    private static DirectoryInfo cacheFolder;
    private static FileInfo fixerStateFile;
    private Fixer.IssuesTree issuesTree;
    private GUIHelper.LayoutSplitter splitter;
    private GUIHelper.Scroll scroll;
    internal static Fixer.State state;

    static Fixer() => EditorApplication.update += new EditorApplication.CallbackFunction(Fixer.Load);

    private static void Load()
    {
      if (!CoreSettings.IsCoreUpdateEnable)
        return;
      if (Fixer.cacheFolder == null)
      {
        Fixer.cacheFolder = new DirectoryInfo(Path.Combine(Application.temporaryCachePath, "FixerCache_" + (object) 27990));
        if (!Fixer.cacheFolder.Exists)
          Fixer.cacheFolder.Create();
      }
      if (Fixer.fixerStateFile == null)
      {
        Fixer.fixerStateFile = new FileInfo(EUtils.CombinePath(Application.dataPath, "Editor Default Resources", "FixerState.info"));
        if (!Fixer.fixerStateFile.Directory.Exists)
          Fixer.fixerStateFile.Directory.Create();
        if (!Fixer.fixerStateFile.Exists)
        {
          FileInfo fileInfo = new FileInfo(EUtils.CombinePath(new DirectoryInfo(Application.dataPath).Parent.FullName, "ProjectSettings", "FixerState.info"));
          if (fileInfo.Exists)
            fileInfo.MoveTo(Fixer.fixerStateFile.FullName);
          else
            Fixer.fixerStateFile.Create().Dispose();
        }
      }
      Fixer.state = new Fixer.State(Fixer.fixerStateFile);
      EditorApplication.update -= new EditorApplication.CallbackFunction(Fixer.Load);
      if (!Fixer.state.enable)
        return;
      EditorCoroutine.start(Fixer.Work());
    }

    public override bool Initialize()
    {
      if (Fixer.state == null)
        Fixer.Load();
      this.splitter = new GUIHelper.LayoutSplitter(OrientationLine.Horizontal, OrientationLine.Vertical, new float[1]
      {
        90f
      });
      this.splitter.drawCursor = (Action<Rect>) (r => GUI.Box(r, "", Styles.separator));
      this.scroll = new GUIHelper.Scroll(new GUILayoutOption[2]
      {
        GUILayout.ExpandHeight(true),
        GUILayout.ExpandWidth(true)
      });
      List<Fixer.Issue> list = ((IEnumerable<FileInfo>) Fixer.cacheFolder.GetFiles()).Where<FileInfo>((Func<FileInfo, bool>) (x => x.Extension == ".fix")).Select<FileInfo, Fixer.Issue>((Func<FileInfo, Fixer.Issue>) (f => new Fixer.Issue(File.ReadAllText(f.FullName)))).Where<Fixer.Issue>((Func<Fixer.Issue, bool>) (i => i.level > Fixer.state.level)).ToList<Fixer.Issue>();
      list.Sort((Comparison<Fixer.Issue>) ((a, b) => a.level.CompareTo(b.level)));
      this.issuesTree = new Fixer.IssuesTree(list);
      return true;
    }

    public override void OnGUI()
    {
      if (!CoreSettings.IsCoreUpdateEnable)
      {
        EditorGUILayout.HelpBox("The Core updates is disabled. Fixer and DLC features isn't work anymore.", MessageType.Info);
      }
      else
      {
        GUILayout.Label(string.Format("{0} v{1}:{2}", (object) "Ultimate Match-Three", (object) "1.00", (object) 2), Styles.miniLabel);
        using (new GUIHelper.Change(new Action(Fixer.state.Save)))
          Fixer.state.enable = EditorGUILayout.Toggle("Enable", Fixer.state.enable);
        if (!(bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
          EditorGUILayout.HelpBox("Please open main scene (Game.unity)", MessageType.Error);
        else if (Fixer.state.enable)
        {
          GUILayout.Label(string.Format("Fixer Level: {0}", (object) Fixer.state.level));
          if (GUILayout.Button("Fix from file...", GUILayout.Width(180f)))
            EditorCoroutine.start(this.FixFromFile());
          if (Fixer.state.targetLevel - Fixer.state.level > 0)
          {
            if (GUILayout.Button("Fix All Issues Automaticaly", GUILayout.Width(180f)))
              Fixer.Apply();
            if (this.issuesTree == null)
              return;
            using (this.splitter.Start())
            {
              if (this.splitter.Area(Styles.area))
                this.issuesTree.OnGUI(GUILayout.ExpandHeight(true));
              if (!this.splitter.Area(Styles.area))
                return;
              if (this.issuesTree.selected != null)
              {
                using (this.scroll.Start())
                {
                  if (this.issuesTree.selected.level > Fixer.state.level + 1)
                    EditorGUILayout.HelpBox("Please, don't fix this issue until the previous issue isn't fixed!", MessageType.Warning);
                  this.issuesTree.selected.OnGUI();
                  if (this.issuesTree.selected.level != Fixer.state.level + 1)
                    return;
                  using (new GUIHelper.Color(UnityEngine.Color.yellow))
                  {
                    if (GUILayout.Button("Fix Automatically", GUILayout.Width(150f)))
                    {
                      Fixer.Issue selected = this.issuesTree.selected;
                      Fixer.Issue.Result result = selected.Apply();
                      if (!result.success)
                      {
                        foreach (object error in result.errors)
                          Debug.LogError(error);
                        EditorUtility.DisplayDialog("Error", "Issue #{0} can't be fixed. See the console for details.".FormatText((object) selected.level), "Ok");
                      }
                      else
                      {
                        Fixer.state.level = selected.level;
                        Fixer.state.Save();
                        EditorUtility.DisplayDialog("Success", "Issue #{0} is fixed successfully.".FormatText((object) selected.level), "Ok");
                        this.Initialize();
                      }
                    }
                    if (!GUILayout.Button("Mark As Fixed", GUILayout.Width(150f)))
                      return;
                    Fixer.Issue selected1 = this.issuesTree.selected;
                    if (!EditorUtility.DisplayDialog("Mark As Fixed", "Are you sure, that you want to mark Issue #{0} as fixed?\r\n    Make sure, that you read this instruction completely and made all described manipulations.\r\n    After you press \"Mark It\" this instruction will not be avaliable.".FormatText((object) selected1.level), "Mark It", "Cancel"))
                      return;
                    Fixer.state.level = selected1.level;
                    Fixer.state.Save();
                    this.Initialize();
                  }
                }
              }
              else
                GUILayout.FlexibleSpace();
            }
          }
          else
            EditorGUILayout.HelpBox("No new issues found. The Fixer checks updates each hour automatically.", MessageType.Info);
        }
        else
          EditorGUILayout.HelpBox("The Fixer is disabled. Please, turn it on, if you want fix all known bugs in this project", MessageType.Warning);
      }
    }

    private IEnumerator FixFromFile()
    {
      if (!Application.isPlaying)
      {
        string str = EditorUtility.OpenFilePanel("Open Fix File", "", "fix");
        if (!str.IsNullOrEmpty())
        {
          FileInfo fileInfo = new FileInfo(str);
          if (fileInfo.Exists)
          {
            Fixer.Issue issue = new Fixer.Issue(File.ReadAllText(fileInfo.FullName));
            if (!issue.error && EditorUtility.DisplayDialog("Issue", "Do you really want to apply this fix?\n\n" + issue.annotation, "Yes", "No"))
            {
              EditorUtility.DisplayProgressBar("Fixing...", "Wait awhile", 0.0f);
              IEnumerator routine = issue.ApplyRoutine();
              float progress = 0.0f;
              while (routine.MoveNext())
              {
                progress += 0.1f;
                progress = Mathf.Repeat(progress, 1f);
                EditorUtility.DisplayProgressBar("Fixing...", "Wait a while", progress);
                yield return (object) 0;
              }
              if (issue.result != null)
              {
                Fixer.Issue.Result result = issue.result;
                if (result.success)
                {
                  EditorUtility.DisplayDialog("Success", "The issue is fixed successfully!", "Ok");
                  AssetDatabase.Refresh();
                }
                else
                {
                  EditorUtility.DisplayDialog("Failed", "The issue is not fixed!" + (result.errors.Count > 0 ? "\n" + result.errors[0] : ""), "Ok");
                  foreach (object error in result.errors)
                    Debug.LogError(error);
                }
              }
              EditorUtility.ClearProgressBar();
            }
          }
        }
      }
    }

    internal static void CheckUpdates() => EditorCoroutine.start(Fixer.NewFixesRequest());

    private static IEnumerator NewFixesRequest()
    {
      if (Fixer.state.enable && (bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
      {
        XDocument xdocument = new XDocument();
        XElement content = new XElement((XName) "request");
        xdocument.Add((object) content);
        content.Add((object) new XAttribute((XName) "level", (object) Fixer.state.targetLevel));
        content.Add((object) new XAttribute((XName) "project", (object) 27990));
        LogicRequestInstruction request = new LogicRequestInstruction("RequestFix", xdocument.ToString(), (string) null);
        while (request.keepWaiting)
          yield return (object) 0;
        List<Fixer.Issue> source = new List<Fixer.Issue>();
        if (request.error == null)
        {
          foreach (XElement element in XDocument.Parse(request.result).Root.Elements((XName) "issue"))
          {
            Fixer.Issue issue = new Fixer.Issue(element.Value);
            if (issue.error)
            {
              MailAgent.Send(ProjectInfo.buyer, "Fixer Problem", element.Value, string.IsNullOrEmpty(ProjectInfo.email) ? "yurowm@gmail.com" : ProjectInfo.email, (MailAgent.ContactDelegate) null);
              yield break;
            }
            else
              source.Add(issue);
          }
        }
        else
          Debug.LogError((object) request.error);
        foreach (Fixer.Issue issue in source)
          File.WriteAllText(new FileInfo(Path.Combine(Fixer.cacheFolder.FullName, issue.id + ".fix")).FullName, issue.raw);
        if (source.Count > 0)
        {
          Fixer.state.targetLevel = source.Max<Fixer.Issue>((Func<Fixer.Issue, int>) (x => x.level));
          Fixer.state.Save();
          Banner.NewWindow(typeof (NewFixesBanner));
        }
      }
    }

    internal static void Apply()
    {
      if (Application.isPlaying)
        return;
      List<Fixer.Issue> source1 = Fixer.state.LoadIssues();
      List<Fixer.Issue> source2 = new List<Fixer.Issue>();
      int level = Fixer.state.level;
      while (true)
      {
        level++;
        Fixer.Issue issue = source1.FirstOrDefault<Fixer.Issue>((Func<Fixer.Issue, bool>) (x => x.level == level));
        if (issue != null)
          source2.Add(issue);
        else
          break;
      }
      Fixer.state.tasks = source2.Select<Fixer.Issue, string>((Func<Fixer.Issue, string>) (x => x.id)).ToList<string>();
      Fixer.state.Save();
      EditorCoroutine.start(Fixer.Work());
    }

    private static IEnumerator Work()
    {
      if ((bool) (UnityEngine.Object) MonoBehaviourAssistant<Yurowm.GameCore.Content>.main)
      {
        int fixedCount = 0;
        while (Fixer.state.tasks.Count > 0)
        {
          string currentTaskID = Fixer.state.tasks[0];
          FileInfo fileInfo = ((IEnumerable<FileInfo>) Fixer.cacheFolder.GetFiles()).FirstOrDefault<FileInfo>((Func<FileInfo, bool>) (x => x.Name == currentTaskID + ".fix"));
          if (fileInfo.Exists)
          {
            Fixer.Issue issue = new Fixer.Issue(File.ReadAllText(fileInfo.FullName));
            if (!issue.error)
            {
              if (fixedCount > 0 && issue.IsRebuildRequired())
              {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
                yield break;
              }
              else
              {
                IEnumerator process = issue.ApplyRoutine();
                float progress = 0.0f;
                while (process.MoveNext())
                {
                  progress = Mathf.Repeat(progress + 0.05f, 1f);
                  EditorUtility.DisplayProgressBar("Fixing", "Issue #" + (object) issue.level, progress);
                  yield return (object) 0;
                }
                EditorUtility.ClearProgressBar();
                Fixer.Issue.Result result = issue.result;
                if (!result.success)
                {
                  Debug.Log((object) string.Format("Issue #{0} can't be fixed.", (object) issue.level));
                  using (List<string>.Enumerator enumerator = result.errors.GetEnumerator())
                  {
                    while (enumerator.MoveNext())
                      Debug.LogError((object) enumerator.Current);
                    break;
                  }
                }
                else
                {
                  ++fixedCount;
                  Fixer.state.level = issue.level;
                  Fixer.state.tasks.RemoveAt(0);
                  Fixer.state.Save();
                  EditorUtility.ClearProgressBar();
                  yield return (object) 0;
                  Debug.Log((object) string.Format("Issue #{0} is fixed successfully.", (object) issue.level, (object) Fixer.state.level));
                  EditorSceneManager.SaveOpenScenes();
                  issue = (Fixer.Issue) null;
                  process = (IEnumerator) null;
                }
              }
            }
            else
              break;
          }
          else
            break;
        }
        if (fixedCount > 0)
          AssetDatabase.Refresh();
      }
    }

    internal class State
    {
      public int level;
      public int targetLevel;
      private FileInfo file;
      public List<string> tasks = new List<string>();
      public bool enable = true;

      public State(FileInfo file)
      {
        this.file = file;
        this.Load();
      }

      private void Load()
      {
        try
        {
          XElement root = XDocument.Parse(File.ReadAllText(this.file.FullName)).Root;
          this.level = int.Parse(root.Attribute((XName) "level").Value);
          this.targetLevel = int.Parse(root.Attribute((XName) "targetLevel").Value);
          this.enable = root.Attribute((XName) "enable").Value == "1";
          this.tasks = root.Elements((XName) "task").Select<XElement, string>((Func<XElement, string>) (x => x.Attribute((XName) "id").Value)).ToList<string>();
        }
        catch (Exception ex)
        {
          this.level = 0;
          this.targetLevel = 0;
        }
        if (this.level >= 0)
          return;
        this.level = 0;
      }

      public void Save()
      {
        XDocument xdocument = new XDocument();
        XElement content1 = new XElement((XName) "state");
        xdocument.Add((object) content1);
        content1.Add((object) new XAttribute((XName) "level", (object) this.level));
        content1.Add((object) new XAttribute((XName) "targetLevel", (object) this.targetLevel));
        content1.Add((object) new XAttribute((XName) "enable", (object) (this.enable ? 1 : 0)));
        foreach (string task in this.tasks)
        {
          XElement content2 = new XElement((XName) "task");
          content1.Add((object) content2);
          content2.Add((object) new XAttribute((XName) "id", (object) task));
        }
        File.WriteAllText(this.file.FullName, xdocument.ToString());
      }

      internal List<Fixer.Issue> LoadIssues()
      {
        List<Fixer.Issue> issueList = new List<Fixer.Issue>();
        foreach (FileInfo file in Fixer.cacheFolder.GetFiles())
        {
          if (!(file.Extension != ".fix"))
          {
            Fixer.Issue issue = new Fixer.Issue(File.ReadAllText(file.FullName));
            if (issue.level <= Fixer.state.level)
              file.Delete();
            else
              issueList.Add(issue);
          }
        }
        return issueList;
      }
    }

    internal class Issue
    {
      public string id;
      public int level;
      public bool error = true;
      public readonly string raw;
      private List<Fixer.Issue.IFix> fixes = new List<Fixer.Issue.IFix>();
      public string annotation = "";
      internal Fixer.Issue.Result result;

      public Issue(string xml)
      {
        this.raw = xml;
        this.Load(xml);
      }

      private void Load(string raw)
      {
        try
        {
          XElement root = XDocument.Parse(raw).Root;
          if (int.Parse(root.Attribute((XName) "projectID").Value) != 27990)
            return;
          this.level = int.Parse(root.Attribute((XName) "level").Value);
          this.id = root.Attribute((XName) "id").Value;
          if (root.Attribute((XName) "annotation") != null)
            this.annotation = root.Attribute((XName) "annotation").Value;
          if (string.IsNullOrEmpty(this.annotation))
            this.annotation = "Without annotation";
          this.fixes.Clear();
          foreach (XElement element in root.Elements())
            this.fixes.Add(Fixer.Issue.IFix.Get(element) ?? throw new NullReferenceException("Unknown fix tag. Please update the core"));
          this.error = false;
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
          this.error = true;
        }
      }

      internal Fixer.Issue.Result Apply()
      {
        Fixer.Issue.Result result1 = new Fixer.Issue.Result();
        try
        {
          result1.success = true;
          foreach (Fixer.Issue.IFix fix in this.fixes)
          {
            Fixer.Issue.Result result2 = fix.Initialize();
            if (!result2.success)
              result1.success = false;
            result1.errors.AddRange((IEnumerable<string>) result2.errors);
          }
          if (result1.success)
          {
            foreach (Fixer.Issue.IFix fix in this.fixes)
            {
              Fixer.Issue.Result result3 = fix.Fix();
              if (!result3.success)
                result1.success = false;
              result1.errors.AddRange((IEnumerable<string>) result3.errors);
            }
          }
          return result1;
        }
        catch (Exception ex)
        {
          result1.success = false;
          result1.errors.Add(ex.ToString());
          return result1;
        }
      }

      internal IEnumerator ApplyRoutine()
      {
        Fixer.Issue.Result result = new Fixer.Issue.Result();
        result.success = true;
        foreach (Fixer.Issue.IFix fix in this.fixes)
        {
          try
          {
            Fixer.Issue.Result result1 = fix.Initialize();
            if (!result1.success)
              result.success = false;
            result.errors.AddRange((IEnumerable<string>) result1.errors);
          }
          catch (Exception ex)
          {
            result.success = false;
            result.errors.Add(ex.ToString());
          }
          yield return (object) 0;
        }
        if (result.success)
        {
          foreach (Fixer.Issue.IFix fix in this.fixes)
          {
            try
            {
              Fixer.Issue.Result result2 = fix.Fix();
              if (!result2.success)
                result.success = false;
              result.errors.AddRange((IEnumerable<string>) result2.errors);
            }
            catch (Exception ex)
            {
              result.success = false;
              result.errors.Add(ex.ToString());
            }
            yield return (object) 0;
          }
        }
        this.result = result;
      }

      internal bool IsRebuildRequired()
      {
        foreach (Fixer.Issue.IFix fix in this.fixes)
        {
          if (fix.IsRebuildRequired())
            return true;
        }
        return false;
      }

      internal void OnGUI()
      {
        GUILayout.Label("Issue #" + (object) this.level, Styles.title);
        GUILayout.Label(this.annotation, EditorStyles.wordWrappedLabel);
        int num = 0;
        foreach (Fixer.Issue.IFix fix in this.fixes)
        {
          GUILayout.Label((++num).ToString() + ". " + fix.GetTitle(), EditorStyles.boldLabel);
          fix.OnGUI();
          EditorGUILayout.Space();
        }
      }

      internal class Result
      {
        public bool success;
        public List<string> errors = new List<string>();
      }

      private abstract class IFix
      {
        private static Dictionary<string, Fixer.Issue.IFix> parsers;
        private bool readyToFix;

        public Fixer.Issue.Result Initialize()
        {
          try
          {
            Fixer.Issue.Result result = this.Check();
            this.readyToFix = result.success;
            return result;
          }
          catch (Exception ex)
          {
            Fixer.Issue.Result result = new Fixer.Issue.Result();
            result.success = false;
            this.readyToFix = false;
            result.errors.Add(ex.ToString());
            return result;
          }
        }

        public Fixer.Issue.Result Fix()
        {
          if (!this.readyToFix)
            return new Fixer.Issue.Result()
            {
              success = false,
              errors = {
                "The fix is not ready for applying"
              }
            };
          try
          {
            return this.Apply();
          }
          catch (Exception ex)
          {
            return new Fixer.Issue.Result()
            {
              success = false,
              errors = {
                ex.ToString()
              }
            };
          }
        }

        protected abstract Fixer.Issue.Result Check();

        protected abstract Fixer.Issue.Result Apply();

        public abstract string GetTag();

        public static Fixer.Issue.IFix Get(XElement xml)
        {
          if (Fixer.Issue.IFix.parsers == null)
            Fixer.Issue.IFix.LoadParsers();
          string localName = xml.Name.LocalName;
          return !Fixer.Issue.IFix.parsers.ContainsKey(localName) ? (Fixer.Issue.IFix) null : Fixer.Issue.IFix.parsers[localName].Parse(xml);
        }

        private static void LoadParsers()
        {
          Fixer.Issue.IFix.parsers = new Dictionary<string, Fixer.Issue.IFix>();
          System.Type refType = typeof (Fixer.Issue.IFix);
          foreach (Fixer.Issue.IFix ifix in ((IEnumerable<System.Type>) typeof (Fixer).Assembly.GetTypes()).Where<System.Type>((Func<System.Type, bool>) (x => refType.IsAssignableFrom(x) && !x.IsAbstract)).Select<System.Type, object>((Func<System.Type, object>) (x => Activator.CreateInstance(x))).ToList<object>())
            Fixer.Issue.IFix.parsers.Set<string, Fixer.Issue.IFix>(ifix.GetTag(), ifix);
        }

        protected abstract Fixer.Issue.IFix Parse(XElement xml);

        internal abstract bool IsRebuildRequired();

        internal abstract string GetTitle();

        internal abstract void OnGUI();
      }

      private class ScriptFix : Fixer.Issue.IFix
      {
        private string scriptName;
        private string original;
        private string replacement;
        private string originalL;
        private string replacementL;
        private FileInfo scriptFile;
        private Regex trimRegex = new Regex("^\\s*(?<code>\\S[\\S\\s]*\\S)\\s*$");
        private static readonly Dictionary<string, string> regexCodeCollapse = new Dictionary<string, string>()
        {
          {
            "\\s*\\s*",
            "\\s*"
          },
          {
            "\\s+\\s+",
            "\\s+"
          },
          {
            "\\s*\\s+",
            "\\s+"
          },
          {
            "\\s+\\s*",
            "\\s+"
          }
        };

        public override string GetTag() => "script";

        protected override Fixer.Issue.IFix Parse(XElement xml)
        {
          Fixer.Issue.ScriptFix scriptFix = new Fixer.Issue.ScriptFix()
          {
            scriptName = xml.Attribute((XName) "name").Value,
            original = xml.Element((XName) "original").Value,
            replacement = xml.Element((XName) "replacement").Value
          };
          scriptFix.replacementL = scriptFix.replacement;
          scriptFix.originalL = scriptFix.original;
          return (Fixer.Issue.IFix) scriptFix;
        }

        protected override Fixer.Issue.Result Apply()
        {
          string str1 = File.ReadAllText(this.scriptFile.FullName);
          Match match1 = new Regex(this.original, RegexOptions.Multiline).Match(Fixer.Issue.ScriptFix.CodeWithoutComments(str1));
          if (match1.Success)
          {
            Match match2 = this.trimRegex.Match(match1.Value);
            int num = match2.Success ? match2.Groups["code"].Index : 0;
            int count = match2.Success ? match2.Groups["code"].Length : match1.Length;
            string str2 = str1.Remove(match1.Index + num, count);
            Match match3 = this.trimRegex.Match(this.replacement);
            str1 = str2.Insert(match1.Index + num, match3.Success ? match3.Groups["code"].Value : this.replacement);
          }
          File.WriteAllText(this.scriptFile.FullName, str1);
          return new Fixer.Issue.Result() { success = true };
        }

        protected override Fixer.Issue.Result Check()
        {
          Fixer.Issue.Result result = new Fixer.Issue.Result();
          List<FileInfo> fileInfoList = Fixer.Issue.ScriptFix.Search(Application.dataPath, this.scriptName);
          if (fileInfoList.Count == 0)
          {
            result.success = false;
            result.errors.Add(this.scriptName + " script is not found");
            return result;
          }
          if (fileInfoList.Count > 1)
          {
            result.success = false;
            result.errors.Add("There is several " + this.scriptName + " files.");
            return result;
          }
          this.scriptFile = fileInfoList[0];
          this.original = Fixer.Issue.ScriptFix.CodeToRegularExpression(Fixer.Issue.ScriptFix.CodeWithoutComments(this.original));
          if (new Regex(this.original, RegexOptions.Multiline).Matches(Fixer.Issue.ScriptFix.CodeWithoutComments(File.ReadAllText(this.scriptFile.FullName))).Count == 0)
          {
            result.success = false;
            result.errors.Add("No matches found in " + this.scriptName + " script.");
            return result;
          }
          result.success = true;
          return result;
        }

        private static List<FileInfo> Search(string directoryPath, string fileName)
        {
          List<FileInfo> fileInfoList = new List<FileInfo>();
          foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles())
          {
            if (file.Name == fileName)
              fileInfoList.Add(file);
          }
          foreach (DirectoryInfo directory in new DirectoryInfo(directoryPath).GetDirectories())
            fileInfoList.AddRange((IEnumerable<FileInfo>) Fixer.Issue.ScriptFix.Search(directory.FullName, fileName));
          return fileInfoList;
        }

        private static string CodeToRegularExpression(string code)
        {
          char ch1 = '✏';
          code = code.Trim();
          foreach (char ch2 in "\\/[]^.{}?+*|()$,;")
            code = code.Replace(ch2.ToString(), ch1.ToString() + "\\" + ch2.ToString() + ch1.ToString());
          code = code.Replace(ch1.ToString(), "\\s*");
          code = new Regex("\\s+").Replace(code, "\\s+");
          bool flag;
          do
          {
            flag = true;
            foreach (KeyValuePair<string, string> keyValuePair in Fixer.Issue.ScriptFix.regexCodeCollapse)
            {
              string str = code.Replace(keyValuePair.Key, keyValuePair.Value);
              if (str != code)
              {
                flag = false;
                code = str;
              }
            }
          }
          while (!flag);
          return code;
        }

        private static string CodeWithoutComments(string code)
        {
          foreach (Match match in new Regex("(?://.*$|/\\*.*\\*/)", RegexOptions.Multiline).Matches(code))
            code = code.Replace(match.Value, new string(' ', match.Value.Length));
          return code;
        }

        internal override bool IsRebuildRequired() => false;

        internal override string GetTitle() => "Code fix in {0} file".FormatText((object) this.scriptName);

        internal override void OnGUI()
        {
          using (new GUIHelper.Change((Action) (() => GUI.FocusControl(""))))
          {
            GUILayout.Label("Find {0} file in your project. You can use search field in the Project view. Or any kind of file manager you have.\nOpen it in any text editor application.".FormatText((object) this.scriptName), EditorStyles.wordWrappedLabel);
            GUILayout.Label("You need to find next code in the opened file.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.TextArea(this.originalL, Styles.monospaceLabel);
            GUILayout.Label("And to replace it on the next code.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.TextArea(this.replacementL, Styles.monospaceLabel);
          }
        }
      }

      private class ExecutableFix : Fixer.Issue.IFix
      {
        private Fixer.Issue.ExecutableFix.Core core;
        private bool rebuildRequired;
        private string fixer = "";
        private string checker = "";
        private string description;

        public override string GetTag() => "executable";

        protected override Fixer.Issue.Result Apply() => new Fixer.Issue.Result()
        {
          success = this.core.Fix(),
          errors = this.core.errors
        };

        protected override Fixer.Issue.Result Check() => new Fixer.Issue.Result()
        {
          success = this.core.Check(),
          errors = this.core.errors
        };

        protected override Fixer.Issue.IFix Parse(XElement xml)
        {
          Fixer.Issue.ExecutableFix executableFix = new Fixer.Issue.ExecutableFix();
          executableFix.fixer = xml.Element((XName) "fixer").Value;
          executableFix.checker = xml.Element((XName) "checker").Value;
          XElement xelement = xml.Element((XName) "description");
          if (xelement != null)
            executableFix.description = xelement.Value;
          executableFix.core = new Fixer.Issue.ExecutableFix.Core(executableFix.checker, executableFix.fixer, ((IEnumerable<string>) xml.Element((XName) "namespaces").Value.Split(',')).ToList<string>());
          executableFix.rebuildRequired = xml.Attribute((XName) "rebuildRequired").Value == "1";
          return (Fixer.Issue.IFix) executableFix;
        }

        internal override bool IsRebuildRequired() => this.rebuildRequired;

        internal override string GetTitle() => "Execute script";

        internal override void OnGUI()
        {
          using (new GUIHelper.Change((Action) (() => GUI.FocusControl(""))))
          {
            GUILayout.Label("This short script allows to resolve described issue. You need to research it and to make the same manipulations manually. Or you can execute the script by clicking the \"Execute\" button.", EditorStyles.wordWrappedLabel);
            GUILayout.Label("\"Checker\" script returns Boolean value, and if it's false, issue can't be fixed for some reasons.", EditorStyles.wordWrappedLabel);
            if (!this.description.IsNullOrEmpty())
              GUILayout.Label("What the script do:\n" + this.description, EditorStyles.wordWrappedLabel);
            GUILayout.Label("Namespaces:", EditorStyles.wordWrappedLabel);
            EditorGUILayout.TextArea(string.Join("\n", this.core.namespaces.ToArray()), Styles.monospaceLabel);
            GUILayout.Label("Checker:", EditorStyles.wordWrappedLabel);
            EditorGUILayout.TextArea("result = true;\n" + this.checker + "\nreturn result;", Styles.monospaceLabel);
            GUILayout.Label("Fixer:", EditorStyles.wordWrappedLabel);
            EditorGUILayout.TextArea(this.fixer, Styles.monospaceLabel);
            if (!GUILayout.Button("Execute", GUILayout.Width(150f)))
              return;
            if (this.core.Check() && this.core.Fix())
              EditorUtility.DisplayDialog("Result", "Success", "Ok");
            else
              EditorUtility.DisplayDialog("Result", "Failed", "Ok");
          }
        }

        private class Core
        {
          private object instance;
          private int id;
          private static int lastID = 0;
          public List<string> errors = new List<string>();
          private string checker;
          private string fixer;
          internal readonly List<string> namespaces = new List<string>()
          {
            "UnityEngine",
            "System.Collections",
            "System.Linq"
          };
          private static readonly string[] assemblyBlackList = new string[2]
          {
            "eval-",
            "mscorlib"
          };
          private Fixer.Issue.ExecutableFix.Core.State _state;

          public Fixer.Issue.ExecutableFix.Core.State state => this._state;

          public Core(string checker, string fixer, List<string> namespaces)
          {
            this.checker = checker;
            this.fixer = fixer;
            this.namespaces.AddRange((IEnumerable<string>) namespaces);
            this.namespaces = this.namespaces.Distinct<string>().ToList<string>();
            this.namespaces.RemoveAll((Predicate<string>) (n => n.IsNullOrEmpty()));
            this.id = ++Fixer.Issue.ExecutableFix.Core.lastID;
          }

          public bool Check() => this.Compile() && (bool) this.instance.GetType().GetMethod(nameof (Check)).Invoke(this.instance, (object[]) null);

          public bool Fix()
          {
            if (!this.Compile())
              return false;
            this.instance.GetType().GetMethod(nameof (Fix)).Invoke(this.instance, (object[]) null);
            return true;
          }

          private bool Compile()
          {
            if (this._state == Fixer.Issue.ExecutableFix.Core.State.NotCompiled)
            {
              this.checker = this.checker.Trim();
              this.fixer = this.fixer.Trim();
              string str = string.Join("", this.namespaces.Select<string, string>((Func<string, string>) (x => "using " + x + ";\n")).ToArray<string>()) + "\r\n                                namespace LocalFixer" + (object) this.id + " {\r\n                                    public class Code {    \r\n                                        public bool Check() {\r\n                                            bool result = true;\r\n                                            " + this.checker + "\r\n                                            return result;\r\n                                        }\r\n\r\n                                        public void Fix() {\r\n                                            " + this.fixer + "\r\n                                        }\r\n                                    }\r\n                                }";
              CompilerSettingsFixer compilerSettingsFixer = new CompilerSettingsFixer();
              compilerSettingsFixer.LoadDefaultReferences = false;
              StringBuilder sb = new StringBuilder();
              EvaluatorFixer evaluatorFixer = new EvaluatorFixer(new CompilerContextFixer(compilerSettingsFixer, (ReportPrinter) new StreamReportPrinterFixer((TextWriter) new StringWriter(sb))));
              foreach (Assembly assembly1 in AppDomain.CurrentDomain.GetAssemblies())
              {
                Assembly assembly = assembly1;
                if (!((IEnumerable<string>) Fixer.Issue.ExecutableFix.Core.assemblyBlackList).Contains<string>((Func<string, bool>) (a => assembly.FullName.StartsWith(a))))
                  evaluatorFixer.ReferenceAssembly(assembly);
              }
              object obj = (object) null;
              try
              {
                evaluatorFixer.Compile(str);
                bool flag = false;
                evaluatorFixer.Evaluate("new LocalFixer" + (object) this.id + ".Code()", out obj, out flag);
                if (flag)
                {
                  this.instance = obj;
                  this._state = Fixer.Issue.ExecutableFix.Core.State.Compiled;
                }
                else
                {
                  this.errors.Add(sb.ToString());
                  this._state = Fixer.Issue.ExecutableFix.Core.State.Crushed;
                }
              }
              catch (Exception ex)
              {
                this.errors.Add(ex.Message + "\n" + ex.StackTrace);
                this._state = Fixer.Issue.ExecutableFix.Core.State.Crushed;
              }
            }
            return this._state == Fixer.Issue.ExecutableFix.Core.State.Compiled;
          }

          public enum State
          {
            NotCompiled,
            Compiled,
            Crushed,
          }
        }
      }

      private class FileRemoveFix : Fixer.Issue.IFix
      {
        private string fileName;
        private FileInfo file;
        private Regex rebuildRegex = new Regex("^.*\\.(js|cs|shader|dll)$");

        public override string GetTag() => "remove";

        protected override Fixer.Issue.IFix Parse(XElement xml) => (Fixer.Issue.IFix) new Fixer.Issue.FileRemoveFix()
        {
          fileName = xml.Attribute((XName) "name").Value
        };

        protected override Fixer.Issue.Result Apply()
        {
          Fixer.Issue.Result result = new Fixer.Issue.Result();
          try
          {
            this.file.Delete();
            result.success = true;
          }
          catch (Exception ex)
          {
            result.success = false;
          }
          return result;
        }

        protected override Fixer.Issue.Result Check()
        {
          Fixer.Issue.Result result = new Fixer.Issue.Result();
          List<FileInfo> list = EUtils.SearchFiles(Application.dataPath).Where<FileInfo>((Func<FileInfo, bool>) (x => x.Name == this.fileName)).ToList<FileInfo>();
          if (list.Count == 0)
          {
            result.success = false;
            result.errors.Add(this.fileName + " file is not found");
            return result;
          }
          this.file = list[0];
          result.success = true;
          return result;
        }

        internal override bool IsRebuildRequired() => this.rebuildRegex.IsMatch(this.fileName);

        internal override string GetTitle() => "Remove file {0}".FormatText((object) this.fileName);

        internal override void OnGUI() => GUILayout.Label("Find {0} file in your project. You can use search field in the Project view. Or any kind of file manager you have.\nRemove this file.".FormatText((object) this.fileName), EditorStyles.wordWrappedLabel);
      }

      private class ReplaceScriptFix : Fixer.Issue.IFix
      {
        private string scriptName;
        private string text;
        private FileInfo scriptFile;

        public override string GetTag() => "replace";

        protected override Fixer.Issue.IFix Parse(XElement xml) => (Fixer.Issue.IFix) new Fixer.Issue.ReplaceScriptFix()
        {
          scriptName = xml.Attribute((XName) "name").Value,
          text = xml.Element((XName) "text").Value
        };

        protected override Fixer.Issue.Result Apply()
        {
          FileInfo fileInfo = new FileInfo(Path.Combine(this.scriptFile.Directory.FullName, this.scriptFile.Name + "_backup"));
          if (fileInfo.Exists && (DateTime.Now - fileInfo.CreationTime).TotalHours < 24.0)
          {
            fileInfo.Delete();
            File.Move(this.scriptFile.FullName, fileInfo.FullName);
          }
          else
            this.scriptFile.Delete();
          File.WriteAllText(this.scriptFile.FullName, this.text);
          return new Fixer.Issue.Result() { success = true };
        }

        protected override Fixer.Issue.Result Check()
        {
          Fixer.Issue.Result result = new Fixer.Issue.Result();
          List<FileInfo> fileInfoList = Fixer.Issue.ReplaceScriptFix.Search(Application.dataPath, this.scriptName);
          if (fileInfoList.Count == 0)
          {
            result.success = false;
            result.errors.Add(this.scriptName + " script is not found");
            return result;
          }
          if (fileInfoList.Count > 1)
          {
            result.success = false;
            result.errors.Add("There is several " + this.scriptName + " files.");
            return result;
          }
          this.scriptFile = fileInfoList[0];
          result.success = true;
          return result;
        }

        private static List<FileInfo> Search(string directoryPath, string fileName)
        {
          List<FileInfo> fileInfoList = new List<FileInfo>();
          foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles())
          {
            if (file.Name == fileName)
              fileInfoList.Add(file);
          }
          foreach (DirectoryInfo directory in new DirectoryInfo(directoryPath).GetDirectories())
            fileInfoList.AddRange((IEnumerable<FileInfo>) Fixer.Issue.ReplaceScriptFix.Search(directory.FullName, fileName));
          return fileInfoList;
        }

        internal override bool IsRebuildRequired() => false;

        internal override string GetTitle() => "Replace {0} script".FormatText((object) this.scriptName);

        internal override void OnGUI()
        {
          using (new GUIHelper.Change((Action) (() => GUI.FocusControl(""))))
          {
            GUILayout.Label("Take this file:", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Save File", GUILayout.Width(150f)))
            {
              string str = EditorUtility.SaveFilePanel("Save file to replace", "", this.scriptName, "");
              if (!str.IsNullOrEmpty())
                File.WriteAllText(str, this.text);
            }
            GUILayout.Label("Find the original file ({0}) in the project, and replace it to the new one.".FormatText((object) this.scriptName), EditorStyles.wordWrappedLabel);
            GUILayout.Label("Don't forget to make backup of the original file. If you will run this fix automaticaly, the backup will be created in the same folder.", EditorStyles.wordWrappedLabel);
          }
        }
      }
    }

    internal class IssuesTree : GUIHelper.NonHierarchyList<Fixer.Issue>
    {
      private static Texture2D issueIcon;
      public Fixer.Issue selected;

      public IssuesTree(List<Fixer.Issue> collection)
        : base(collection, new TreeViewState())
      {
        this.onSelectedItemChanged = this.onSelectedItemChanged + (Action<List<Fixer.Issue>>) (issues =>
        {
          if (issues.Count != 1)
            return;
          this.selected = issues[0];
        });
      }

      public override Fixer.Issue CreateItem() => (Fixer.Issue) null;

      public override void DrawItem(
        Rect rect,
        GUIHelper.HierarchyList<Fixer.Issue, TreeFolder>.ItemInfo info)
      {
        if ((UnityEngine.Object) Fixer.IssuesTree.issueIcon == (UnityEngine.Object) null)
          Fixer.IssuesTree.issueIcon = EditorIcons.GetBuildInIcon("IssueItemIcon");
        GUI.DrawTexture(new Rect(rect.x, rect.y, 16f, rect.height), (Texture) Fixer.IssuesTree.issueIcon);
        GUI.Label(new Rect(rect.x + 16f, rect.y, rect.width - 16f, rect.height), "Issue #" + (object) info.content.level);
      }

      public override int GetUniqueID(Fixer.Issue element) => element.level;

      public override bool ObjectToItem(
        UnityEngine.Object o,
        out GUIHelper.HierarchyList<Fixer.Issue, TreeFolder>.IInfo result)
      {
        result = (GUIHelper.HierarchyList<Fixer.Issue, TreeFolder>.IInfo) null;
        return false;
      }
    }
  }
}
