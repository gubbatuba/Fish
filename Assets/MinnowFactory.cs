﻿
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BGE
{
    public class MinnowFactory : MonoBehaviour
    {

        public float radius;
        public int boidCount;
        public GameObject boidPrefab;

        public bool spawnInTopHemisphere;

        [Range(0, 1)]
        public float spread;

        public Flock flock;

        [Header("Debug")]
        public bool drawGizmos;


        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        MinnowFactory()
        {
            radius = 100;
            boidCount = 200;

            spread = 1.0f;
        }

        void Start()
        {
            flock = GetComponent<Flock>();
            Color color = Pallette.Random();

            for (int i = 0; i < boidCount; i++)
            {
                GameObject boid = GameObject.Instantiate<GameObject>(boidPrefab);
                flock.boids.Add(boid);
                //BGE.Utilities.RecursiveSetColor(boid, color);

                bool inside = false;
                do
                {
                    Vector3 unit = UnityEngine.Random.insideUnitSphere;
                    if (spawnInTopHemisphere)
                    {
                        unit.y = Mathf.Abs(unit.y);
                    }
                    boid.transform.position = transform.position + unit * UnityEngine.Random.Range(0, radius * spread);
                    inside = false;
                    foreach (Obstacle obstacle in BoidManager.Instance.obstacles)
                    {
                        if (Vector3.Distance(obstacle.transform.position, boid.transform.position) < obstacle.radius + boid.GetComponent<Boid>().minBoxLength)
                        {
                            inside = true;
                            break;
                        }
                    }
                }
                while (inside);
                boid.transform.parent = flock.transform;
                boid.GetComponent<Boid>().flock = flock;
                boid.GetComponent<Boid>().sphereConstrainEnabled = true;
                boid.GetComponent<Boid>().sphereRadius = radius;
                AudioSource audioSource = boid.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    if (Random.Range(0, 1) > 0.1f)
                    {
                        audioSource.enabled = true;
                        audioSource.loop = true;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.enabled = false;
                    }
                }
                if (i == boidCount / 2)
                {
                    if (drawGizmos)
                    {
                        boid.GetComponent<Boid>().drawGizmos = drawGizmos;
                        boid.GetComponent<Boid>().drawNeighbours = false;
                    }
                }
            }

        }
    }     
}