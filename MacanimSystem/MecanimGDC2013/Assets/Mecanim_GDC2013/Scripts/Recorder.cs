using UnityEngine;
using System.Collections;

/// <summary>
/// Animator Recorder script.
/// Should work we any animator, be sure to register your states in the dictionnary in InitStateDictionnary()
/// </summary>
public class Recorder : MonoBehaviour {
	
	Animator m_Animator;
	
	public Texture Play;
	public Texture Next;
	public Texture Prev;
	public Texture Pause;
	
	const int FrameCount  = 500;	
	public bool isRecording;
	
	float m_TimeLinePixelSize;
	
	const float buttonBorderWidth = 4;	
	System.Collections.Generic.Dictionary<int,string> m_StateDictionnary = new System.Collections.Generic.Dictionary<int,string>();
	System.Collections.Generic.List<int> samples = new System.Collections.Generic.List<int>();
	
	void Start () 
	{
		m_Animator = GetComponent<Animator>();			
		StartRecord();		
		InitStateDictionnary();		
	}
	
	void OnGUI() 
	{		
		if(isRecording)
		{
	        if (GUILayout.Button(Pause))
			{
				StopRecord();	          
			}
		}
		else
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(Prev,GUILayout.ExpandWidth(false)))
			{
				m_Animator.playbackTime -= 0.03f;
			}
			if (GUILayout.Button(Play,GUILayout.ExpandWidth(false)))
			{
				StartRecord();
				return;
			}
			if (GUILayout.Button(Next,GUILayout.ExpandWidth(false)))
			{
				m_Animator.playbackTime += 0.03f;
			}
			GUILayout.EndHorizontal();
						
			
			m_TimeLinePixelSize = Screen.width -10;
			m_Animator.playbackTime = GUILayout.HorizontalSlider(m_Animator.playbackTime, m_Animator.recorderStartTime, m_Animator.recorderStopTime, GUILayout.Width(m_TimeLinePixelSize));			
			
			DisplayTimeLine();
			
		}		       
    }	
			
	private void StartRecord()
	{
		isRecording = true;
		samples.Clear();
		m_Animator.StopPlayback();
		m_Animator.StartRecording(FrameCount); // record a number of frame
	}
	
	private void StopRecord()
	{
		isRecording = false;
		m_Animator.StopRecording();
		m_Animator.StartPlayback();
		
	}
	
	
	
 
	
	void InitStateDictionnary()
	{
		m_StateDictionnary[Animator.StringToHash("Locomotion.Idle")] 		= "Idle"; 
		m_StateDictionnary[Animator.StringToHash("Locomotion.Run")] 		= "Run";
		m_StateDictionnary[Animator.StringToHash("Locomotion.TurnOnSpot")] 	= "TurnOnSpot";
		m_StateDictionnary[Animator.StringToHash("Base Layer.Slide")] 		= "Slide";
		m_StateDictionnary[Animator.StringToHash("Base Layer.Vault")] 		= "Vault";
		m_StateDictionnary[Animator.StringToHash("Base Layer.Dying")] 		= "Dying";
		m_StateDictionnary[Animator.StringToHash("Base Layer.Death")] 		= "Death";
		m_StateDictionnary[Animator.StringToHash("Base Layer.Reviving")] 	= "Reviving";
		
	}
	
		
	
	
	void Update()
	{
		if(isRecording)
		{				
			if(samples.Count == (FrameCount-1)) // has looped, removed 1st sample
			{							
				samples.RemoveAt(0);							
			}
			samples.Add(m_Animator.GetCurrentAnimatorStateInfo(0).nameHash);			
		}			
	}
	
	// timeline UI
	void DisplayTimeLine()
	{
		int startIndex = 0;
		int currentHash = samples[startIndex];
		
		GUILayout.BeginHorizontal();
		
		for(int i = 0 ; i < samples.Count ; i++)
		{
			if(samples[i] != currentHash)
			{									
				float pixelSize = m_TimeLinePixelSize * ((float)( i - startIndex)/(float)samples.Count) - buttonBorderWidth;				
				GUILayout.Box(m_StateDictionnary.ContainsKey(currentHash) ? m_StateDictionnary[currentHash] : "NotFound", GUILayout.Width(pixelSize));
				startIndex = i;
				currentHash = samples[startIndex];
			}
		}
				
		{
			float pixelSize = m_TimeLinePixelSize * ((float)( samples.Count - startIndex)/(float)samples.Count);						
			GUILayout.Box(m_StateDictionnary.ContainsKey(currentHash) ? m_StateDictionnary[currentHash] : "NotFound", GUILayout.Width(pixelSize));
		
		}
		
		GUILayout.EndHorizontal();
	}
}
