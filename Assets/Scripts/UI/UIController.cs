using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
	public static UIController Instance {  get; private set; }

    public TMP_Text Coin;
    public TMP_Text Crystal;
	public int uid;

	private void Awake()
	{
		if (Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{ 
		// @@@@@@ 나중에 KindomScene에서 읽어오기
		var curUser = UserDatabase.Get(uid);
		Coin.text = curUser.coin.ToString();
		Crystal.text = curUser.crystal.ToString();
	}

	public void ConsumeCoin(int cost)
	{
		int curCoin = Convert.ToInt32(Coin.text);
		//UserDatabase.ConsumeCoin(uid, cost); // @@@@@@@@ 나중에 추가하기
		Coin.text = (curCoin - cost).ToString();
	}

}
