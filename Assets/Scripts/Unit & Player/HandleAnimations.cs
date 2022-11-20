using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitControl
{
    public class HandleAnimations : MonoBehaviour
    {
        public Animator ani;
        public UnitStates states;

        void Start()
        {
            states = GetComponent<UnitStates>();
            SetupAnimator();
        }

        void Update()
        {
            ani.SetFloat("Movement", states.move ? 1 : 0, 0.1f, Time.deltaTime);
            states.movingSpeed = ani.GetFloat("Movement") * states.maxSpeed;
        }

        void SetupAnimator()
        {
            ani = GetComponent<Animator>();

            Animator[] a = GetComponentsInChildren<Animator>();
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != ani)
                {
                    ani.avatar = a[i].avatar;
                    Destroy(a[i]);
                    break;
                }
            }
        }
    }
}
