using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Collider weaponCollider;
    [SerializeField] int destructionCount = 0;
    [SerializeField] float rotateDuration = 0.2f;
    [SerializeField] float detectionTime = 0.2f;


    private Vector3 initialLocalRotation;
    private bool isRotating = false;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>(); 
        weaponCollider.enabled = false;
    }

    public void DetectHit()
    {
        if (isRotating == true)
            return;

        weaponCollider.enabled = true;
        initialLocalRotation = transform.localEulerAngles;

        StartCoroutine(RotateWeaponCoroutine());
    }

    private IEnumerator EnableWeaponCollider()
    {
        yield return new WaitForSeconds(detectionTime);
        weaponCollider.enabled = false;
        destructionCount = 0;
    }

    private IEnumerator RotateWeaponCoroutine()
    {
        isRotating = true;

        float targetZRotation = 40f;
        float elapsedTime = 0f;

        Vector3 startLocalRotation = transform.localEulerAngles;
        Vector3 targetLocalRotation = new Vector3(startLocalRotation.x, startLocalRotation.y, startLocalRotation.z + targetZRotation);

        while(elapsedTime < rotateDuration)
        {
            transform.localEulerAngles = Vector3.Lerp(startLocalRotation, targetLocalRotation, elapsedTime/ rotateDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = targetLocalRotation;

        StartCoroutine(EnableWeaponCollider());

        yield return new WaitForSeconds(0.05f);

        elapsedTime = 0f;

        Vector3 returnRotation = initialLocalRotation;

        while(elapsedTime < rotateDuration)
        {
            transform.localEulerAngles = Vector3.Lerp(targetLocalRotation, returnRotation, elapsedTime / rotateDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = returnRotation;

        isRotating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Prop>(out Prop prop))
        {
            if(destructionCount == 0)
            {
                prop.OnHit();
                destructionCount++;                
                return;
            }            
        }

        else if(other.TryGetComponent<Destructible>(out Destructible obj))
        {        
            if(destructionCount == 0)
            {
                obj.OnHit();
                destructionCount++;
                return;
            }                   
        }
    }
}
