using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Yurowm.GameCore;

[SlotTagRenderer('G', ConsoleColor.Green, priority: 100)]
public class SlotGeneratorExtended : ISlotModifier, INeedToBeSetup {
    public static bool IsTagVisible(SlotSettings slot) {
        return slot.HasContent("GeneratorExtended");
    }

    public const string weight_curve_parameter = "_weightCurve";

    [NonSerialized]
    public List<Case> cases = new List<Case>();

    private Dictionary<int, float> _totalSumDictionary;
    private Dictionary<Case, Dictionary<int, float>> _caseDictionary;
    
    public void OnSetup(Slot slot) {}

    public void OnSetupByContentInfo(Slot slot, SlotContent info) {
        cases = info.parameters
            .Select(p => SlotContent.Deserialize(XElement.Parse(p.Text)))
            .Select(s => new Case() {
                content = s,
                curve = s[weight_curve_parameter].Curve ?? AnimationCurve.Constant(0,1,0)
            }).ToList();
        StartCoroutine(MakeDictionaries());
        StartCoroutine(Routine());
    }

    readonly float delay = 0.1f;

    IEnumerator MakeDictionaries()
    {
        if (cases.Count == 0) yield break;

        _totalSumDictionary = new Dictionary<int, float>();
        _caseDictionary = new Dictionary<Case, Dictionary<int, float>>();
        
        var startMoveCount = SessionInfo.current.design.maxMoves;

        foreach (var c in cases)
        {
            var dict = new Dictionary<int, float>();
            for (var i = 0; i < startMoveCount; i++)
            {
                var time = (float) i / startMoveCount;
                var width = c.curve.Evaluate(time);
                dict.Add(i, width);
            }

            _caseDictionary.Add(c, dict);
            
            yield return null;
        }
        
        for (var i = 0; i < startMoveCount; i++)
        {
            var time = (float) i / startMoveCount;
            var totalSum = cases.Sum(c => c.curve.Evaluate(time));
            _totalSumDictionary.Add(i, totalSum);
            
            yield return null;
        }
    }
    
    IEnumerator Routine() {
        if (cases.Count == 0) yield break;
        
        while (true) {
            yield return new WaitForSeconds(delay);

            if (!SessionAssistant.main.enabled) continue;
		    if (slot.chip) continue;
		    if (slot.block) continue;
            if (gravityLockers.Count > 0) continue;
            if (SessionInfo.current.rule.GetMode() == PlayingMode.Wait)
                SessionInfo.current.rule.SetMode(PlayingMode.Gravity);
            if (SessionInfo.current.rule.GetMode() != PlayingMode.Gravity) continue;
            
            var currentMoveCount = SessionInfo.current.GetMovesCount();
            var startCount = SessionInfo.current.design.maxMoves;
            var move = Mathf.Clamp(startCount - currentMoveCount, 0, int.MaxValue);
            var totalSum = _totalSumDictionary.ContainsKey(move) ? _totalSumDictionary[move] : cases.Sum(c => c.curve.Evaluate(GetTime()));
            var sum = UnityEngine.Random.Range(0, totalSum);
            foreach (Case c in cases)
            {
                var width = _caseDictionary.ContainsKey(c) && _caseDictionary[c].ContainsKey(move) ? _caseDictionary[c][move] : c.curve.Evaluate(GetTime());
                sum -= width;
                if (sum > 0) continue;

                SlotContent slotContent = c.content.Clone();
                IChip chip = Content.Emit<IChip>(slotContent.name);
                chip.name = slotContent.name;

                IColored colored = chip as IColored;
                if (colored != null && (colored.color.IsColored() || slotContent.HasParameter("color"))) {
                    LevelParameter color = slotContent["color"];
                    if (color.ItemColor.IsPhysicalColor() && SessionInfo.current.colorMask.ContainsKey(color.ItemColor))
                        color.ItemColor = SessionInfo.current.colorMask[color.ItemColor];
                    else
                        color.ItemColor = SessionInfo.current.colorMask.Values.GetRandom();
                }

                chip.transform.position = slot.transform.position;

                slot.AttachContent(chip);
                chip.transform.SetParent(slot.transform);
                chip.slot = slot;

                if (chip is INeedToBeSetup)
                    (chip as INeedToBeSetup).OnSetupByContentInfo(slot, slotContent);
                break; 
            }
        }
    }
    
    private static float GetTime()
    {
        var currentMoveCount = SessionInfo.current.GetMovesCount();
        var startCount = SessionInfo.current.design.maxMoves;
        var time = 1f - (float) currentMoveCount / startCount;
        return time;
    }

    public override void Serialize(XElement xContent) {
        foreach (Case c in cases)
            xContent.Add(c.Serialize());
    }

    public override void Deserialize(XElement xContent, SlotContent slotContent) {
        int counter = 0;
        foreach (XElement x in xContent.Elements("case"))
            slotContent["case" + ++counter].Text = x.ToString();
    }

    public class Case {
        public SlotContent content;
        public AnimationCurve curve = AnimationCurve.Constant(0,1,0);
        public XElement Serialize() {
            content[weight_curve_parameter].Curve = curve;
            return content.Serialize("case");
        }
    }
}
