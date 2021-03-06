﻿using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct ObsecuredInt {

	public ObsecuredInt(int value)
	{
		Value = value;
	}

	public ObsecuredInt(string gut)
	{
		m_gut = gut;
	}

	public static implicit operator ObsecuredInt(int value)
	{
		return new ObsecuredInt (value);
	}
	
	public static implicit operator int(ObsecuredInt value)
	{
		return value.Value;
	}

	public static ObsecuredInt operator+(ObsecuredInt lhs, int add)
	{
		return new ObsecuredInt (lhs.Value + add);
	}
	
	public static ObsecuredInt operator-(ObsecuredInt lhs, int sub)
	{
		return new ObsecuredInt (lhs.Value - sub);
	}

	public static ObsecuredInt operator++(ObsecuredInt orig)
	{
		return new ObsecuredInt (orig.Value + 1);
	}

	public static ObsecuredInt operator--(ObsecuredInt orig)
	{
		return new ObsecuredInt (orig.Value - 1);
	}	

	public static bool operator <= (ObsecuredInt lhs, int rhs)
	{
		return lhs.Value <= rhs;
	}

	public static bool operator >= (ObsecuredInt lhs, int rhs)
	{
		return lhs.Value >= rhs;
	}

	public static bool operator < (ObsecuredInt lhs, int rhs)
	{
		return lhs.Value < rhs;
	}
	
	public static bool operator > (ObsecuredInt lhs, int rhs)
	{
		return lhs.Value > rhs;
	}

	public static bool operator == (ObsecuredInt lhs, int rhs)
	{
		return lhs.Value == rhs;
	}

	public static bool operator != (ObsecuredInt lhs, int rhs)
	{
		return lhs.Value != rhs;
	}
	
	public override int GetHashCode()
	{
		return m_gut.GetHashCode ();
	}
	
	public override bool Equals(object other)
	{
		return false;
	}
	
	public int Value
	{
		get
		{
			return Decode(m_gut);
		}

		set
		{
			m_gut = Encode(value);
		}
	}	
	
	public static int Decode(string input)
	{
		if( input == null || input.Length < 4 )
		{
			return 0;
		}
	
		byte[] combined = Convert.FromBase64String(input);
		int keyLength = (int)(combined[0] ^ 0x69);
		byte[] key = new byte[keyLength];
		System.Array.Copy(combined, 1, key, 0, keyLength);
		
		int dataLength = (int)(combined[1 + keyLength] ^ 0x13);
		byte[] data = new byte[dataLength];
		
		if( dataLength != combined.Length - (1 + keyLength + 1) )
		{
			return 0;
		}
		
		System.Array.Copy(combined, 1 + keyLength + 1, data, 0, dataLength);
		
		for( int i=0; i<data.Length; ++i )
		{
			for( int j=0; j<keyLength; ++j )
			{
				data[i] ^= key[(i + j) % keyLength];
			}
		}
		
		string origin = System.Text.ASCIIEncoding.ASCII.GetString(data);				
		int res;
		if( int.TryParse(origin, out res) )
		{
			return res;
		}
		
		return 0;
	}
	
	public static string Encode(int value)
	{
		int i, j;
		int keyLength = RandomRange(4, 8);
		byte[] key = new byte[keyLength];
		for (i=0; i<keyLength; ++i) {
			key[i] = (byte)RandomRange (0, 255);
		}
		
		byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(value.ToString ());
		for( i=0; i<data.Length; ++i )
		{
			for( j=0; j<keyLength; j++ )
			{
				data[i] ^= key[(i+j) % keyLength];
			}
		}
		
		byte[] combined = new byte[1 + keyLength + 1 + data.Length];
		combined[0] = (byte)(keyLength ^ 0x69);
		System.Array.Copy(key, 0, combined, 1, keyLength);
		combined[1 + keyLength] = (byte)(data.Length ^ 0x13);
		System.Array.Copy(data, 0, combined, 1 + keyLength + 1, data.Length);
		
		return Convert.ToBase64String(combined);
	}

	public string Gut
	{
		get { return m_gut; }
		set
		{
			Value = (new ObsecuredInt(value)).Value;
		}
	}
	
	private static int RandomRange(int min, int max)
	{		
		return min + (m_random.Next() % (max + 1 - min));
	}
	
	[SerializeField]
	private string m_gut;
	
	private static System.Random m_random = new System.Random((int)DateTime.Now.Millisecond);
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ObsecuredInt))]
public class ObsecuredIntPropertyDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		Rect temp = EditorGUI.PrefixLabel(position, label);	
		
		SerializedProperty prop = property.FindPropertyRelative("m_gut");
		prop.stringValue = ObsecuredInt.Encode(EditorGUI.IntField(temp, ObsecuredInt.Decode(prop.stringValue)));	
	}
}
#endif