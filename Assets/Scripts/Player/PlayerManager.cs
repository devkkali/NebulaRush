using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
        public static PlayerManager instance { get; set; }

        [SerializeField] public GameObject player;

        private void Start()
        {
                instance = this;
        }
}