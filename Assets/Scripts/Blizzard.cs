﻿using UnityEngine;
using System.Collections;

public class Blizzard : MonoBehaviour {

	private static Blizzard g_instance = null;

	public static Blizzard Instance
	{
		get { return g_instance; }
	}

	public GameObject Effect;	
	private float m_duration = 0.0f;

	void Awake()
	{
		g_instance = this;
	}
	
	public float Duration
	{
		get { return m_duration; }
		set { m_duration = value; }
	}
	
	// Update is called once per frame
	void Update () {		
		if( m_duration <= 0.0f )
		{
			if( Effect.activeSelf )
			{
				ParticleSystem ps = Effect.GetComponent<ParticleSystem>();
				if( ps != null )
				{
					ps.Clear(true);
					ps.Stop(true);
				}
				
				Effect.SetActive(false);
			}
		}
		else if( m_duration > 0.0f )
		{
			m_duration -= Time.deltaTime;
			if( m_duration < 0.0f )
			{
				m_duration = 0.0f;
			}
			
			if( !Effect.activeSelf )
			{
				Effect.SetActive(true);	
				
				ParticleSystem ps = Effect.GetComponent<ParticleSystem>();
				if( ps != null )
				{			
					ps.Play(true);
				}
			}
		}
	}
}