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

    public Stopwatch userStartedSongTimer = new Stopwatch();

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
        stump_mathroom = Resources.Load("Toon Forest free set/Mesh/Materials/stump_mathroom", typeof(Material)) as Material;
    
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
                AudioSource hitPlantAudio = hitPlant.GetComponent<AudioSource>();

                // Keep the audio from sporadically restarting
                // Quick fix: Check if hit object has AudioSource
                if (hitPlantAudio != null)
                {
                    if (hitPlantAudio.mute)
                    {
                        hitPlantAudio.mute = false; // unmute the audio
                        hitPlant.GetComponent<Renderer>().material = fractalMaterial;
                        userStartedSongTimer.Start();
                    }
                    else
                    {   
                        hitPlantAudio.mute = true; // muter the audio this is an easy work around to avoid timing everything
                         hitPlant.GetComponent<Renderer>().material = stump_mathroom; // Need to make this
                    }
                }
                else
                {
                    return;
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
