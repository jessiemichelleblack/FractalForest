using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MajorLazers : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    private GameObject laser;

    private Transform laserTransform;

    private Vector3 hitPoint;

    public Transform cameraRigTransform;

    public GameObject teleportReticlePrefab;

    private GameObject reticle;

    private Transform teleportReticleTransform;

    public Transform headTransform;

    public Vector3 teleportReticleOffset;

    public LayerMask teleportMask;
    public LayerMask defaultLayerMask;

    public Material fractalMaterial;
    public Material stump_mathroom;

    public float xBoundMin;
    public float xBoundMax;

    public float zBoundMin;
    public float zBoundMax;

    private bool shouldTeleport;

    public float delayInterval = 2.0f;
    public float triggerTime;

    public bool hold = true;

    private GameObject lastHitPlant;


    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        stump_mathroom = Resources.Load("stump_mathroom", typeof(Material)) as Material;
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    private void Teleport()
    {
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        Vector3 finalTeleportPoint = hitPoint + difference;

        if (finalTeleportPoint.x < xBoundMin) {
            finalTeleportPoint.x = xBoundMin;
        }
        else if(finalTeleportPoint.x > xBoundMax)
        {
            finalTeleportPoint.x = xBoundMax;
        }

        if(finalTeleportPoint.z < zBoundMin)
        {
            finalTeleportPoint.z = zBoundMin;
        }
        else if(finalTeleportPoint.z > zBoundMax)
        {
            finalTeleportPoint.z = zBoundMax;
        }

        cameraRigTransform.position = finalTeleportPoint;
    }

    // Update is called once per frame
     void Update()
    {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                shouldTeleport = true;
            }
        }
        else if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            RaycastHit hit;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, defaultLayerMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                // Git the plant object that the laser is hitting
                GameObject hitPlant = hit.transform.gameObject;

                if (hitPlant != lastHitPlant)
                {
                    lastHitPlant = hitPlant;

                    AudioSource hitPlantAudio = hitPlant.GetComponent<AudioSource>();

                    // Quick fix: Check if hit object has AudioSource
                    if (hitPlantAudio != null)
                    {
                        if (hitPlantAudio.mute)
                        {
                            // What we want: 
                            // Audio can not be started unless it has been 2 seconds after the user last stopped music
                            // This should avoid the continuous stop/start issue when holding the trigger on a mushroom object
                            // PROBLEM: Because this is a global script, this stop/start condition enacts when you start or stop any mushroom,
                            // not just a specific one

                            hitPlantAudio.mute = false; // unmute the audio
                            hitPlant.GetComponent<Renderer>().material = fractalMaterial;

                        }

                        // if the audio is currently playing mute the song, start the trigger time here
                        else
                        {
                            triggerTime = Time.time;
                            hitPlantAudio.mute = true; // muting the audio this is an easy work around rather than timing the start of each song everything
                            hitPlant.GetComponent<Renderer>().material = stump_mathroom; // This no work :(
                            hold = false;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {
            Teleport();
        }
    }
}
