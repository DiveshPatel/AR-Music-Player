using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Networking;
using System.Collections;


public class AudioScript : MonoBehaviour
{
	public string URL = "https://file-examples.com/wp-content/uploads/2017/11/file_example_OOG_1MG.ogg";
	public AudioSource audioSource;

	void Start()
	{
		StartCoroutine(GetAudioClip());
	}

	IEnumerator GetAudioClip()
	{
		using (var uwr = UnityWebRequestMultimedia.GetAudioClip(URL, AudioType.OGGVORBIS))
		{
			yield return uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError)
			{
				Debug.LogError(uwr.error);
				yield break;
			}

			AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
			audioSource.clip = clip;
			audioSource.Play();
		}
	}
}