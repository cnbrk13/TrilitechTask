using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Scripts
{

	using UnityEngine;
     
     public class PlayerFollower : MonoBehaviour
     {
     
         private Transform playerTransform;
         public float TransitionSpeed;
     
         public float XMin;
         public float XMax;
         public float YMin;
         public float YMax;

         private void Awake()
         {
             playerTransform = GameManager.Instance.Player.transform;
         }

         private void FixedUpdate()
         {
             if (playerTransform == null)
             {
                 return;
             }

             var clamped_x = Mathf.Clamp(playerTransform.position.x, XMin, XMax);
             var clamped_y = Mathf.Clamp(playerTransform.position.y, YMin, YMax);
     
             var target_position = new Vector3(clamped_x, clamped_y, transform.position.z);
     
             // Smoothly transition to the target position
             transform.position = Vector3.Lerp(transform.position, target_position, TransitionSpeed * Time.deltaTime);
         }
     }

}