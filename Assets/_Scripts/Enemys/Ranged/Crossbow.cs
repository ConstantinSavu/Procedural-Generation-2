using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crossbow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform arrow;
    [SerializeField] GameObject physicalArrow;
    [SerializeField] Vector3 globalArrowSpeed = new Vector3(10f, 10f, 10f);
    public Vector3 physicalArrowScale =  new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] Transform holder;
    [SerializeField] Animator animator;
    [SerializeField] public float damping = 1f;

    private bool updateRotation;
    [SerializeField] public bool UpdateRotationToTarget;
    [SerializeField] public bool ReadyToFire;
    [SerializeField] public bool ShowArrow;
    

    // Start is called before the first frame update

    // Update is called once per frame

    void Awake()
    {
        ShowArrow = true;
        ReadyToFire = false;
        UpdateRotationToTarget = false;
    }

    void Update()
    {

        if(UpdateRotationToTarget){
            AimCrossBowAtTarget();
        }
        
        arrow.gameObject.SetActive(ShowArrow);
        arrow.rotation = transform.rotation;
    }

    void AimCrossBowAtTarget(){
        
        Vector3 lookPos = target.position - arrow.position;
        lookPos.y = 0;
        if(lookPos != Vector3.zero){
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            arrow.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
            transform.rotation = arrow.rotation;
        }

    }

    public void ShootArrow(){
        ShowArrow = false;
        UpdateRotationToTarget = false;
        ReadyToFire = false;
        GameObject spawnedArrow = Instantiate(physicalArrow, arrow.position, arrow.rotation);
        spawnedArrow.transform.localScale = physicalArrowScale;
        Rigidbody arrowRigidBody = spawnedArrow.GetComponent<Rigidbody>();

        Vector3 difference = Vector3Abs(arrow.position - target.position);

        Vector3 arrowSpeed = Vector3.Scale(difference, globalArrowSpeed);

        Vector3 resultatSpeed = Vector3.Scale(spawnedArrow.transform.forward, arrowSpeed);

        arrowRigidBody.AddForce(resultatSpeed, ForceMode.VelocityChange);

    }

    private Vector3 Vector3Abs(Vector3 vector){
        return new Vector3(
            Mathf.Abs(vector.x),
            Mathf.Abs(vector.y),
            Mathf.Abs(vector.z)
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(arrow.position, arrow.position + arrow.forward * 100);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 100);
    }

    public void StartHoldCountDown(float delay){
        StopCoroutine(HoldCountDown(delay));
        StartCoroutine(HoldCountDown(delay));
    }

    private IEnumerator HoldCountDown(float delay){
        
        yield return new WaitForSeconds(delay);
        ReadyToFire = true;
    }
    
}
