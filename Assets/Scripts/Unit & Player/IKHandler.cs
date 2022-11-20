using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitControl
{
    public class IKHandler : MonoBehaviour
    {
        public Transform leftHandTarget;
        public float leftHandWeight;
        new Animator animation;

        void Start()
        {
            animation = GetComponent<Animator>();
        }

        private void OnAnimatorIK()
        {
            if (leftHandTarget)
            {
                animation.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                animation.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animation.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);
                animation.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

            }
        }
    }
}
