using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultishotPickup : MonoBehaviour
{
    // script for multishot pick up - created by Jeff Stevenson

    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if (!controller.hasMultishot)
            {
                controller.hasMultishot = true;
                gameObject.SetActive(false);

                controller.PlaySound(collectedClip);
            }
        }

    }
}
