using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
	[SerializeField] private float speed = 0.3f;
	private TMP_Text _tmpLable;
	private Text _label;

	float _target = 0;
	float _current = 0;
	
	void  Awake()
	{
		_tmpLable = GetComponent<TMP_Text>();
		_label = GetComponent<Text> ();
	} 

	private void OnEnable() 
	{
		_current = SessionInfo.current.GetScore();
	}

	private void Update()
	{
		_target = SessionInfo.current.GetScore();
		_current = Mathf.MoveTowards (_current, _target, Time.unscaledDeltaTime * SessionInfo.current.design.thirdStarScore * speed);
		
		SetText(Mathf.RoundToInt(_current).ToString());
	}

	private void SetText(string text)
	{
		if (_tmpLable)
		{
			_tmpLable.text = text;
		}
		else if (_label)
		{
			_label.text = text;
		}
	}
}