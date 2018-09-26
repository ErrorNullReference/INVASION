using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighScoreMgr : MonoBehaviour
{
	static HighScoreMgr instance;
	public static int Score { get { return instance.score; } }

	const string HighScorePath = "HighScore.txt";
	int score;
	string path;

	void Start ()
	{
		instance = this;

		DontDestroyOnLoad (this);
		
		Client.OnGameEnd += UpdateScore;

		path = Path.Combine (Application.streamingAssetsPath, HighScorePath);

		LoadScore ();
	}

	void OnDestroy ()
	{
		Client.OnGameEnd -= SaveScore;
	}

	void UpdateScore ()
	{
		score = PlayersMgr.Players [Client.MyID].Player.TotalPoints;
		SaveScore ();
	}

	void SaveScore ()
	{
		File.WriteAllText (path, score.ToString ());
	}

	void LoadScore ()
	{
		if (File.Exists (path))
			score = int.Parse (File.ReadAllText (path));
	}
}
