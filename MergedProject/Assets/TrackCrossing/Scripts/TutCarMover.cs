using UnityEngine;
using System.Collections;

public class TutCarMover : MonoBehaviour {
	
	public Vector3 endPoint;
	public float speed;
	[Range(0,1)]
	public float percent;
	public bool randomPercent = true;
	
	public Transform car;
	
	public Vector3 offSet;
	public BoxCollider box;
	public bool useRedZone;
	public GameObject redZoneVisual;
	public AudioSource[] audioSources;
	
	public bool game;
	
	public Vector3 stretchDirection;
	
	private WarningSystem warningSystem;
	private float initialSpeed;
	private GameObject redZoneVisualSpawn;
	
	private float m_percent;
	
	void Start () {
		m_percent = percent;
		initialSpeed = speed;
		warningSystem = GameObject.FindWithTag("WarningSystem").GetComponent<WarningSystem>();
		
		if (useRedZone) {
			redZoneVisualSpawn = (GameObject)Instantiate(redZoneVisual);
			redZoneVisualSpawn.transform.localScale = new Vector3(box.size.z, box.size.y, box.size.x);
			redZoneVisualSpawn.transform.parent = car;
			redZoneVisualSpawn.transform.localPosition = box.center + offSet;
		}
		
		if (randomPercent)
			m_percent = Random.value;
	}
	
	void Update () {
		m_percent += Time.deltaTime * speed;
		
		if (speed != 0 && m_percent > 1f) {
			m_percent = 0;
		}
		
		if (m_percent >= 0) {
			car.position = Vector3.Lerp(transform.position, transform.position + endPoint, m_percent);
		} else { // This code is a total hack for one specific situation and will never be used in any other situation so it's whatever.  PS: By that I mean, DON'T USE in any other situation.  Should be easy to avoid.
			//car.position = Vector3.Lerp(transform.position + endPoint, transform.position, -m_percent);
			car.position += stretchDirection * Time.deltaTime;
		}
	}
	
	void OnTriggerEnter (Collider col) {
		if (col.gameObject.tag == "Player") {
			warningSystem.Warn(1);
			if (game) {
				GameObject.FindWithTag("CrossingRailer").SendMessage("GameOver", warningSystem.warnings[2].name);
			}
		}
	}
	
	void NukeSpeed () {
		speed = 0;
		foreach (AudioSource a in audioSources) {
			a.Pause();
		}
	}
	
	void ResumeSpeed () {
		speed = initialSpeed;
		foreach (AudioSource a in audioSources) {
			a.Play();
		}
	}
	
	public float Percent {
		get { return m_percent; }
		set { m_percent = value; }
	}
}