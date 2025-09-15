using Photon.Pun;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drift : MonoBehaviourPun
{
    [Header("Drift Settings")]
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float minAngle = 10f;
    [SerializeField] private float driftingDelay = 0.2f;
    
    private float speed;
    private float driftAngle;
    private float driftFactor = 1;
    private float currentScore = 0;
    private int totalScore = 0;
    
    private DriftUI driftUI;
    private Rigidbody rb;

    private bool isDrifting = false;
    private Coroutine stopDriftingCoroutine = null;

    private bool isMine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isMine = photonView.IsMine;
    }

    private void Update()
    {
        if (!isMine) return;

        ManageDrift();
        if(driftUI != null) driftUI.UpdateUI(totalScore, driftFactor, currentScore, driftAngle);
    }

    private void ManageDrift()
    {
        speed = rb.velocity.magnitude;
        driftAngle = Vector3.Angle(rb.transform.forward, (rb.velocity + rb.transform.forward).normalized);
        
        if(driftAngle > 120f) driftAngle = 0f;

        if(driftAngle > minAngle && speed > minSpeed)
        {
            if(!isDrifting || stopDriftingCoroutine != null) StartDrift();
        }
        else
        {
            if(isDrifting && stopDriftingCoroutine == null) StopDrift();
        }
        
        if(isDrifting)
        {
            if(driftUI != null) driftUI.gameObject.SetActive(true);
            currentScore += Time.deltaTime * driftAngle * driftFactor;
            driftFactor += Time.deltaTime;
        }
    }

    async void StartDrift()
    {
        if(!isDrifting)
        {
            await Task.Delay(Mathf.RoundToInt(1000 * driftingDelay));
            driftFactor = 1;
        }

        if(stopDriftingCoroutine != null)
        {
            StopCoroutine(stopDriftingCoroutine);
            stopDriftingCoroutine = null;
        }

        isDrifting = true;
    }

    private void StopDrift()
    {
        stopDriftingCoroutine = StartCoroutine(StoppingDrift());
    }

    private IEnumerator StoppingDrift()
    {
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(driftingDelay * 4f);
        
        totalScore += Mathf.RoundToInt(currentScore);
        isDrifting = false;
        
        yield return new WaitForSeconds(0.5f);
        currentScore = 0f;
        if(driftUI != null) driftUI.gameObject.SetActive(false);

        stopDriftingCoroutine = null;
    }

    public bool GetIsDrifting() => isDrifting;
    public int GetTotalScore() => totalScore += Mathf.RoundToInt(currentScore);
    public void SetDriftUI(DriftUI gameObject) => driftUI = gameObject;
}