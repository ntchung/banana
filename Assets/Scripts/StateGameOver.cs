﻿using UnityEngine;
using System.Collections;

public class StateGameOver : GameState {

	public UIWidget Container;
	public UILabel StatisticLabel;
	
	public UIButton FacebookButton;
	public UIButton ShareButton;
	
	public UIWidget MonstersPanel;
	public UILabel[] ResultLabels;
	
	private string m_originalStatisticLabel;
	
	public static bool HasHighscoreBeaten = false;

	void Awake()
	{	
		UIEventListener.Get (FindChild ("ButtonMainMenu")).onClick += (obj) =>
		{
			StartCoroutine (OnMainMenuButtonClick ());
		};
		
		UIEventListener.Get (FindChild ("ButtonLeaderboard")).onClick += (obj) =>
		{
			StartCoroutine (OnLeaderboardButtonClick ());
		};
		
		UIEventListener.Get(FacebookButton.gameObject).onClick += (obj) =>
		{
			FacebookController.Instance.Operate(FacebookController.EOperation.LOG_IN);		
		};
		FacebookButton.gameObject.SetActive(false);
		
		UIEventListener.Get(ShareButton.gameObject).onClick += (obj) =>
		{
			FacebookController.Instance.Operate(FacebookController.EOperation.POST_TO_WALL);		
		};
		ShareButton.gameObject.SetActive(false);
		
		HasHighscoreBeaten = false;	
	}	
	
	private IEnumerator OnMainMenuButtonClick()
	{
		yield return StartCoroutine(Utils.WaitForRealSeconds(0.25f));
				
		StateManager.Instance.SetState(StateManager.Instance.MainMenu);
	}
	
	private IEnumerator OnLeaderboardButtonClick()
	{
		yield return StartCoroutine(Utils.WaitForRealSeconds(0.25f));
		
		StateManager.Instance.SetState(StateManager.Instance.MainMenu);
		StateManager.Instance.PendState(StateManager.Instance.Leaderboard);
	}
	
	public override void OnEnter()
	{	
		m_originalStatisticLabel = StatisticLabel.text;
		
		string label = StatisticLabel.text;
		label = label.Replace("{score}", PlayerStash.Instance.CurrentScore.ToString());
		
		int[] statistic = EnemiesManager.Instance.KillingStatistics;
		for( int i=0; i<statistic.Length; ++i )
		{
			ResultLabels[i].text = statistic[i].ToString();
		}
		EnemiesManager.Instance.Reset();
				
		MonstersPanel.gameObject.SetActive(true);
		
		PlayerStash.Instance.RecordHighScore();		
		if( HasHighscoreBeaten )
		{
			if( PlayerStash.Instance.HighScore > 1000 )
			{
				label = label.Replace("{comment}", "New highscore! Awesome!");
			}
			else
			{
				label = label.Replace("{comment}", "New highscore! Well done!");
			}
		}
		else
		{
			label = label.Replace("{comment}", "Try harder !");
		}
		   
		StatisticLabel.text = label;
		
		Vector3 pos = Container.transform.localPosition;
		pos.x = -1000;
		Container.transform.localPosition = pos;
		
		Container.GetComponent<SpringPosition>().enabled = true;
	}
	
	public override void OnUpdate()
	{
		FacebookButton.gameObject.SetActive(FacebookController.Instance.CanOperateLogIn());
		ShareButton.gameObject.SetActive(FacebookController.Instance.CanOperatePost());
		
		Vector3 pos = new Vector3(Container.transform.localPosition.x, 70, 0);
		MonstersPanel.transform.localPosition = pos;
	}
	
	public override void OnExit()
	{
		StatisticLabel.text = m_originalStatisticLabel;
		MonstersPanel.gameObject.SetActive(false);
	}
	
	public override void OnBackKey()
	{
		StateManager.Instance.SetState(StateManager.Instance.MainMenu);
	}
}
