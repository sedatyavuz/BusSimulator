using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerDestroyer : MonoBehaviour
{
    [SerializeField] private BusSc busSc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Passenger"))
        {
            Destroy(other.gameObject);
            busSc.currentPassengerUpdate(true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Passenger"))
        {
            Destroy(collision.gameObject);
            busSc.currentPassengerUpdate(true);
        }
    }
}
