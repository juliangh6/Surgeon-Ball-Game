﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubNubAPI;
using UnityEngine.UI;
using SimpleJSON;

public class LeaderBoard4script : MonoBehaviour
{
	public static PubNub pubnub;
	public Text Line1;
	public Text Line2;
	public Text Line3;
	public Text Line4;
	public Text Line5;
	public Text Score1;
	public Text Score2;
	public Text Score3;
	public Text Score4;
	public Text Score5;

	public Button SubmitButton;
	public InputField FieldUsername;
	//public InputField FieldScore;
	public Text LabelScore;
	//public Object[] tiles = {}
	// Use this for initialization
	void Start()
	{
		Button btn = SubmitButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);

		// Use this for initialization
		PNConfiguration pnConfiguration = new PNConfiguration();
		pnConfiguration.PublishKey = "pub-c-eb8b47f4-dc42-44b9-b3cd-843b1c7088a1";
		pnConfiguration.SubscribeKey = "sub-c-cee9b2c6-a626-11eb-b1ae-be4e92e38e16";

		pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
		pnConfiguration.UUID = Random.Range(0f, 999999f).ToString();

		pubnub = new PubNub(pnConfiguration);
		Debug.Log(pnConfiguration.UUID);


		MyClass myFireObject = new MyClass();
		myFireObject.test = "new user";
		string fireobject = JsonUtility.ToJson(myFireObject);
		pubnub.Fire()
			.Channel("submit_score")
			.Message(fireobject)
			.Async((result, status) => {
				if (status.Error)
				{
					Debug.Log(status.Error);
					Debug.Log(status.ErrorData.Info);
				}
				else
				{
					Debug.Log(string.Format("Fire Timetoken: {0}", result.Timetoken));
				}
			});

		pubnub.SusbcribeCallback += (sender, e) => {
			SusbcribeEventEventArgs mea = e as SusbcribeEventEventArgs;
			if (mea.Status != null)
			{
			}
			if (mea.MessageResult != null)
			{
				Dictionary<string, object> msg = mea.MessageResult.Payload as Dictionary<string, object>;

				string[] strArr = msg["username"] as string[];
				string[] strScores = msg["score"] as string[];

				int usernamevar = 1;
				foreach (string username in strArr)
				{
					string usernameobject = "Line" + usernamevar;
					GameObject.Find(usernameobject).GetComponent<Text>().text = usernamevar.ToString() + ". " + username.ToString();
					usernamevar++;
					Debug.Log(username);
				}

				int scorevar = 1;
				foreach (string score in strScores)
				{
					string scoreobject = "Score" + scorevar;
					GameObject.Find(scoreobject).GetComponent<Text>().text = "Score: " + score.ToString();
					scorevar++;
					Debug.Log(score);
				}
			}
			if (mea.PresenceEventResult != null)
			{
				Debug.Log("In Example, SusbcribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event);
			}
		};
		pubnub.Subscribe()
			.Channels(new List<string>() {
				"leaderboard_scores"
			})
			.WithPresence()
			.Execute();
	}

	void TaskOnClick()
	{
		var usernametext = FieldUsername.text;// this would be set somewhere else in the code
		var scoretext = LabelScore.text; //replace this with the PassScore value
		MyClass myObject = new MyClass();
		myObject.username = FieldUsername.text;
		//myObject.score = FieldScore.text; //Replace this with the PassScore value
		myObject.score = LabelScore.text;
		string json = JsonUtility.ToJson(myObject);

		pubnub.Publish()
			.Channel("submit_score")
			.Message(json)
			.Async((result, status) => {
				if (!status.Error)
				{
					Debug.Log(string.Format("Publish Timetoken: {0}", result.Timetoken));
				}
				else
				{
					Debug.Log(status.Error);
					Debug.Log(status.ErrorData.Info);
				}
			});
		//Output this to console when the Button is clicked
		Debug.Log("You have clicked the button!");
	}

}